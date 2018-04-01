using SudokuMaker.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SudokuMaker.Util;
using System.Windows.Data;
using Microsoft.Win32;
using SudokuMaker.UserDataHandler;
using System.Windows;

namespace SudokuMaker.ViewModel
{
    public class SudokuViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Cell> Cells { get; set; }
        public ObservableCollection<Cell> UserCells { get; set; }
        public Game Game { get; set; }
        public UserStatisticsData UserStatistics { get; set; }
        private System.Timers.Timer timer;
        private UserDataWorker userData;

        private object m_lock;
        public SudokuViewModel()
        {
            m_lock = new object();
            Cells = new ObservableCollection<Cell>();
            UserCells = new ObservableCollection<Cell>();
            PreInitCells(Cells);
            PreInitCells(UserCells);
            InitCellsCollections(Cells);
            FillCells();
            BindingOperations.EnableCollectionSynchronization(UserCells, m_lock);

          
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            userData = new UserDataWorker();
            UserStatistics = userData.LoadStatistics();
            Game = new Game();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Game.SpendTime = DateTime.Now - Game.StartTime;
        }
        #region Commands
        private RelayCommand newGameCommand;
        private RelayCommand loadGameCommand;
        private RelayCommand saveGameCommand;
        private RelayCommand stopGameCommand;
        private RelayCommand checkCellsCommand;
        private RelayCommand checkCommand;
        private RelayCommand exitCommand;
        private RelayCommand showStatsCommand;

        public RelayCommand NewGameCommand
        {
            get
            {
                return newGameCommand ??
                    (newGameCommand = new RelayCommand(obj =>
                    {
                        if((int)Game.Difficulty == 0)
                        {
                            MessageBox.Show("Choose game difficulty first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        Task initCellsTask;
                        // первый запуск
                        if (UserCells.Count == 0)
                        {
                            initCellsTask = new Task( () => 
                            {
                                InitUserCells();
                                AfterStartGame();
                            });
                        }
                        // сгенерировать новый набор
                        else
                        {
                            initCellsTask = new Task(() =>
                            {
                                Cells.Clear();
                                PreInitCells(Cells);
                                InitCellsCollections(Cells);
                                FillCells();
                                InitUserCells();
                                AfterStartGame();
                            });
                        }
                        initCellsTask.Start();
                    }));
            }
        }

        public RelayCommand StopGameCommand
        {
            get
            {
                return stopGameCommand ??
                    (
                        stopGameCommand = new RelayCommand(obj =>
                        {
                            var isLoad = MessageBox.Show("Current game will be lost. Would you like to continue?", "Stop game",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (isLoad == MessageBoxResult.No)
                                return;
                            Game.IsStarted = false;
                            Game.SpendTime = TimeSpan.FromSeconds(0);
                            timer.Stop();
                        }
                    ));
            }
        }

        public RelayCommand LoadGameCommand
        {
            get
            {
                return loadGameCommand ??
                    (
                        loadGameCommand = new RelayCommand(obj =>
                        {
                            if (Game.IsStarted)
                            {
                                var isLoad = MessageBox.Show("After loading saves game the current game will be lost. Would you like to continue?", "Load game",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (isLoad == MessageBoxResult.No)
                                    return;
                            }

                            OpenFileDialog dialog = new OpenFileDialog();
                            dialog.Filter = "Sudoku save game files (*.sudo)|*.sudo";
                            dialog.AddExtension = false;
                            dialog.InitialDirectory = userData.SaveGamePath;
                            if(dialog.ShowDialog() == true)
                            {
                                if (dialog.FileName != null)
                                    userData.LoadGame(dialog.FileName);
                            }
                        }
                    ));
            }
        }

        public RelayCommand SaveGameCommand
        {
            get
            {
                return saveGameCommand ??
                    (
                    saveGameCommand = new RelayCommand(obj =>
                    {
                        SaveFileDialog dialog = new SaveFileDialog();
                        dialog.AddExtension = false;
                        dialog.InitialDirectory = userData.SaveGamePath;
                        if(dialog.ShowDialog() == true)
                        {
                            if(dialog.FileName != null)
                                userData.SaveGame(new UserData { Cells = Cells, Game = Game }, dialog.FileName);
                        }
                    }
                    ));
            }
        }

        public RelayCommand CheckCellsCommand
        {
            get
            {
                return checkCellsCommand ??
                    (
                    checkCellsCommand = new RelayCommand(obj =>
                    {
                        if (UserCells.Where(x => x.Number == null).Any())
                            return;
                        if(!UserCells.Where(x => x.IsCorrect == false).Any())
                        {
                            Game.IsStarted = false;
                            UserStatistics.AddFinish(Game.Difficulty, Game.SpendTime);
                            userData.SaveStatistic(UserStatistics);
                            timer.Stop();
                            MessageBox.Show("You've finished the game!", "Congratulations!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ));
            }
        }

        public RelayCommand CheckCommand
        {
            get
            {
                return checkCommand ??
                    (
                    checkCommand = new RelayCommand(obj =>
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                var curr = UserCells.Where(x => x.Position.X == i+1 && x.Position.Y == j+1).FirstOrDefault();
                                if (curr.Number == null)
                                    continue;
                                if (UserCells.Where(x => x.Position.X == i+1 && x.Position.Y == j+1).FirstOrDefault().Number !=
                                    Cells.Where(x => x.Position.X == i+1 && x.Position.Y == j+1).FirstOrDefault().Number)
                                    curr.IsCorrect = false;
                            }
                        }
                    }
                    ));
            }
        }

        public RelayCommand ExitCommand
        {
            get
            {
                return exitCommand ??
                    (
                    exitCommand = new RelayCommand(obj =>
                    {
                        userData.SaveStatistic(UserStatistics);
                    }
                    ));
            }
        }

        public RelayCommand ShowStatsCommand
        {
            get
            {
                return showStatsCommand ??
                    (
                    showStatsCommand = new RelayCommand(obj =>
                    {
                        AppLib.ShowStats(UserStatistics.Statistics);
                    }
                    ));
            }
        }
        #endregion
        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
        #region Fillment methods
        private void PreInitCells(ObservableCollection<Cell> cells)
        {
            lock (m_lock)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        cells.Add(new Cell(new Position(j + 1, i + 1)));
                    }
                }
            }
        }
        private void InitCellsCollections(ObservableCollection<Cell> Cells)
        {
            lock (m_lock)
            {
                foreach (var item in Cells)
                {
                    item.Row = Cells.Where(x => x.Position.X == item.Position.X).ToList();
                    item.Row.Remove(item);

                    item.Col = Cells.Where(x => x.Position.Y == item.Position.Y).ToList();
                    item.Col.Remove(item);

                    List<int> xRange;
                    List<int> yRange;

                    // TODO not so stupid algorithm
                    if (item.Position.X < 4)
                        xRange = new List<int> { 1, 2, 3 };
                    else if (item.Position.X > 3 && item.Position.X < 7)
                        xRange = new List<int> { 4, 5, 6 };
                    else
                        xRange = new List<int> { 7, 8, 9 };

                    if (item.Position.Y < 4)
                        yRange = new List<int> { 1, 2, 3 };
                    else if (item.Position.Y > 3 && item.Position.Y < 7)
                        yRange = new List<int> { 4, 5, 6 };
                    else
                        yRange = new List<int> { 7, 8, 9 };

                    item.Squares = Cells.Where(x => (xRange.Contains(x.Position.X) && yRange.Contains(x.Position.Y))).ToList();
                    // remove the current cell
                    item.Squares.Remove(item);
                }
            }
        }
        private void FillCells()
        {
            lock (m_lock)
            {
                int badCount = 0;
                Random rnd = new Random();
                for (int i = 0; i < 9; i++)
                {
                    var toUse = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    for (int j = 0; j < 9; j++)
                    {
                        var cell = Cells.Where(x => (x.Position.X == j + 1) && (x.Position.Y == i + 1)).FirstOrDefault();
                        var aviable = toUse.Where(x =>
                            !(cell.Squares.Count(y => (y.Number ?? 0) == x) > 0) &&
                            !(cell.Row.Count(y => (y.Number ?? 0) == x) > 0) &&
                            !(cell.Col.Count(y => (y.Number ?? 0) == x) > 0)
                            ).ToList();
                        if (aviable.Count == 0)
                        {
                            j--;
                            badCount++;
                            if (badCount == 10)
                            {

                                badCount = 0;
                                // clear current unfilled row and the row below. then fill row below again
                                var toDel = Cells.Where(x => (x.Position.Y == i + 1) || (x.Position.Y == i));
                                foreach (var item in toDel)
                                    item.Number = null;
                                i -= 2;
                                break;
                            }
                            continue;
                        }
                        var num = aviable[rnd.Next(0, aviable.Count)];
                        toUse.Remove(num);
                        cell.Number = num;
                    }
                }
            }
        }

        private void InitUserCells()
        {
            lock (m_lock)
            {
                System.Threading.Thread.Sleep(3000);
                UserCells.Clear();
                foreach (var cell in Cells)
                    UserCells.Add(new Cell(cell));
                InitCellsCollections(UserCells);
                // убрать этот процент
                int unfilledSpace = (int)Game.Difficulty;
                // TODO: может нах так извращаться?
                int numsToRemove = (int)Math.Round(((double)unfilledSpace / 100) * (double)AppLib.SudokuCellsCount, 0);
                Random rnd = new Random();
                Position pos = new Position(0, 0);
                List<Position> used = new List<Position>();
                int removed = 0; ;
                while (removed != numsToRemove)
                {
                    pos.X = rnd.Next(1, 10);
                    pos.Y = rnd.Next(1, 10);
                    var toRemove = UserCells.Where(x => x.Position.X == pos.X && x.Position.Y == pos.Y && !used.Contains(pos)).FirstOrDefault();
                    if (toRemove?.Number != null)
                    {
                        toRemove.Number = null;
                        removed++;
                        used.Add(pos);
                    }
                }
            }

        }
        #endregion

        private void AfterStartGame()
        {
            Game.StartTime = DateTime.Now;
            Game.IsStarted = true;
            UserStatistics.AddStart(Game.Difficulty);
            timer.Start();
        }
    }
}
