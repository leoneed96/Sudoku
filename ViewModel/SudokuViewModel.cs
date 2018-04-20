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
using SudokuMaker.Properties;
using System.Windows.Controls;

namespace SudokuMaker.ViewModel
{
    public class SudokuViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Ячейки, заполняемые полностью и служащие опорой для формирования урезанного списка ячеек для
        /// игрока. Так же, позже, возможно, для подсказки.
        /// </summary> 
        public ObservableCollection<Cell> Cells { get; set; }
        /// <summary>
        /// Отображается игроку
        /// </summary>
        public ObservableCollection<Cell> UserCells { get; set; }
        /// <summary>
        /// Текущий и по идее единственный экземпляр класса Game.
        /// TODO: Может переделать в singleton?
        /// </summary>
        public Game Game { get; set; }
        /// <summary>
        /// Текущий десериазированный/по-новой инициализированный класс, отвечающий за статистику игрока
        /// </summary>
        public UserStatisticsData UserStatistics { get; set; }
        /// <summary>
        /// Таймер, служащий для обновления Game.SpendTime
        /// TODO: переделать в Task?
        /// </summary>
        private System.Timers.Timer timer;
        /// <summary>
        /// Экземпляр класса, отвещающего за управление данными статистики (сохраниние, загрузка, TODO: reset)
        /// </summary>
        private UserDataWorker userData;
        /// <summary>
        /// Объкт для синхронизации доступа к мультипоточным ресурсам
        /// </summary>
        private object m_lock;
        /// <summary>
        /// Конструктор. Полностью инициализирует закрытые <see cref="Cells"/> ячейки и выполняет
        /// пре-инициализацю для <see cref="UserCells"/>
        /// </summary>
        public SudokuViewModel()
        {
            m_lock = new object();
            Cells = new ObservableCollection<Cell>();
            UserCells = new ObservableCollection<Cell>();
            PreInitCells(Cells);
            PreInitCells(UserCells);
            InitCellsCollections(Cells);
            FillCells();
            /// <summary>
            /// Необходимо для возможности доступа к являющимся предметом Binding <see cref="UserCells"/>
            /// из другого потока (в данном случае для инициализации и нежелания блокировать UI)
            /// </summary>
            BindingOperations.EnableCollectionSynchronization(UserCells, m_lock);

          
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            userData = new UserDataWorker();
            UserStatistics = userData.LoadStatistics();
            Game = new Game();
        }

        /// <summary>
        /// Обработчик события истекания времени для таймера.
        /// TODO: переделать в Task?
        /// </summary>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Game.SpendTime = DateTime.Now - Game.StartTime;
        }
        /// <summary>
        /// Команды для связи View и ViewModel
        /// </summary>
        #region Commands
        private RelayCommand newGameCommand;
        private RelayCommand loadGameCommand;
        private RelayCommand saveGameCommand;
        private RelayCommand stopGameCommand;
        private RelayCommand checkCellsCommand;
        private RelayCommand checkCommand;
        private RelayCommand exitCommand;
        private RelayCommand showStatsCommand;
        private RelayCommand rulesShowCommand;
		private RelayCommand helpCommand;
		private RelayCommand triggerHelpModeCommand;

		/// <summary>
		/// Команда, привязанная к нажатию button "New Game"
		/// </summary>
		public RelayCommand NewGameCommand
        {
            get
            {
                return newGameCommand ??
                    (newGameCommand = new RelayCommand(obj =>
                    {
                        // Привязано в View, по умолчанию уровень сложности не выбран
                        if((int)Game.Difficulty == 0)
                        {
                            MessageBox.Show("Choose game difficulty first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        ///<summary>
                        /// Task, который сгенерирует набор ячеек для пользователя или полностью обновит
                        /// <see cref="Cells"/> в случае новой игры после первоначального запуска 
                        ///</summary>
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
                        // повторный запуск
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

        /// <summary>
        /// Команда, привязанная к нажатию кнопки Stop
        /// </summary>
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

        /// <summary>
        /// Команда загрузки новой игры. Нуждается в доработке функционала заргрузки/сохраненения.
        /// В данный момент работает/может работать некорректно.
        /// </summary>
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
                                {
                                    if(Game.IsStarted)
                                        UserStatistics.AddFinish(Game.Difficulty, Game.SpendTime);
                                    userData.currentData = userData.LoadGame(dialog.FileName);
                                }
                            }
                            UserCells.Clear();
                            Cells.Clear();
                            InitLoadedGame(userData.currentData.Game);
                            InitLoadedCells(userData.currentData.UserCells);
                            Cells = userData.currentData.Cells;
                            if (!timer.Enabled)
                                timer.Start();
                        }
                    ));
            }
        }

        /// <summary>
        /// Команда сохранения текущей игры
        /// В данный момент выкинет exception, ругаясь на невозможность сериализовать структуру Thickness. 
        /// Нуждается в доработке функционала заргрузки/сохраненения.
        /// </summary>
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
                                userData.SaveGame(new UserData
                                {
                                    Cells = Cells,
                                    Game = Game,
                                    UserCells = UserCells
                                }, dialog.FileName);
                        }
                    }
                    ));
            }
        }

        /// <summary>
        /// Команда проверки текущего состояния <see cref="UserCells"/>
        /// В случае правильного заполнения поля следает необходимые действия, пусть и не так, как хотелось бы.
        /// </summary>
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
                            Game.IsFinished = true;
                            //MessageBox.Show("You've finished the game!", "Congratulations!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ));
            }
        }

        /// <summary>
        /// Команда, привязанная к кнопке "Check". Отвечает за проверку соответствия 
        /// значений, введенных пользователем по сравнению с <see cref="Cells"/>
        /// </summary>
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
                        if (!UserCells.Where(x => x.Number != null && !x.IsCorrect).Any())
                        {
                            /// TODO: наверное, можно заставить анимацию сделать это менее ущербно
                            Task hackAnimation = new Task(() => 
                            {
                                Game.ShouldPlayCorrectAnimation = true;
                                System.Threading.Thread.Sleep(700);
                                Game.ShouldPlayCorrectAnimation = false;
                            });
                            hackAnimation.Start();
                        }
                    }
                    ));
            }
        }

        /// <summary>
        /// Команда, привязанная к закрытию окна с игрой.
        /// TODO: проверку на желание сохранить иргу, если она еще не сохранена.
        /// </summary>
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

        /// <summary>
        /// Команда, привязанная к вызову пункта меню,отвечающего за открытие окна статистики
        /// TODO: менее уродская реализация вызова Statistics?
        /// </summary>
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

        public RelayCommand RulesShowCommand
        {
            get
            {
                return rulesShowCommand ??
                    (
                    rulesShowCommand = new RelayCommand(obj =>
                    {
                        AppLib.ShowInfoWindow("Sudoku rules", "Rules", Resources.SudokuRulesString);
                    }
                    ));
            }
        }
		public RelayCommand HelpCommand
		{
			get
			{
				return helpCommand ??
					(
					helpCommand = new RelayCommand(obj =>
					{
						if (!Game.IsHelpMode)
							return;
						
						//AppLib.ShowInfoWindow("Sudoku rules", "Rules", Resources.SudokuRulesString);
					}
					));
			}
		}
		public RelayCommand TriggerHelpModeCommand
		{
			get
			{
				return triggerHelpModeCommand ??
					(
					triggerHelpModeCommand = new RelayCommand(obj =>
					{
						Game.IsHelpMode = !Game.IsHelpMode;
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
        /// <summary>
        /// Пре-инициализация переданных ячеек - заполнение позиций.
        /// </summary>
        /// <param name="cells"></param>
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
        /// <summary>
        /// Инициализация коллекций строки/столбца/квадрата для каждой ячейки из коллекции <paramref name="Cells"/>
        /// </summary>
        /// <param name="Cells"></param>
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
        /// <summary>
        /// Полное заполнение полей Number для <see cref="Cells"/> в соответствии с правилами Sudoku.
        /// TODO: нуждается в пересмотре и рефакторинге
        /// </summary>
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

        /// <summary>
        /// Инициализирует отображаемые пользователю <see cref="UserCells"/> в соответсвии с <see cref="Game.Difficulty"/>
        /// </summary>
        private void InitUserCells()
        {
            lock (m_lock)
            {
                //System.Threading.Thread.Sleep(1000);
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

        /// <summary>
        /// Повторяющийся кусок для окончания Task-а инициализации ячеек для игрока.
        /// </summary>
        private void AfterStartGame()
        {
            Game.StartTime = DateTime.Now;
            Game.IsStarted = true;
            Game.IsFinished = false;
            UserStatistics.AddStart(Game.Difficulty);
            timer.Start();
        }

        private void InitLoadedGame(Game game)
        {
            Game.Difficulty = game.Difficulty;
            Game.IsStarted = true;
            Game.StartTime = DateTime.Now - game.SpendTime;
            Game.IsFinished = false;
        }

        public void InitLoadedCells(ObservableCollection<Cell> userCells)
        {
            if (UserCells.Count == 0)
                PreInitCells(UserCells);
            for (int i = 0; i < userCells.Count; i++)
            {
                UserCells[i] = userCells[i];
            }
        }
    }
}
