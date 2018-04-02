using SudokuMaker.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuMaker.Model
{
    [Serializable]
    public class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// зачем разграничение <see cref="number"/> и <see cref="strNumber"/> - не вспомню, но, наверное
        /// , в этом была great thought
        /// </summary>
        private int? number;
        /// <summary>
        /// <see cref="number"/>
        /// </summary>
        private string strNumber;
        /// <summary>
        /// Является ли текущее значение ячейки корректным
        /// </summary>
        private bool isCorrect;
        /// <summary>
        /// Позиция текущей ячейки
        /// </summary>
        private Position position;
        [NonSerialized]
        private SolidColorBrush color;
        [NonSerialized]
        private Thickness borderThickness;

        public List<Cell> Squares { get; set; }
        public List<Cell> Row { get; set; }
        public List<Cell> Col { get; set; }
        public Cell(Cell cell)
        {
            position = cell.Position;
            Number = cell.Number;
            IsCorrect = true;
            InitThickness();
        }
        public Cell()
        {

        }
        public Cell(Position pos)
        {
            position = pos;
            IsCorrect = true;
            InitThickness();
        }
        

        public Thickness BorderThickness
        {
            get
            {
                if (this.borderThickness.Top == 0)
                    InitThickness();
                return borderThickness;
            }
            set
            {
                borderThickness = value;
            }
        }

        public Position Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
                OnPropertyChanged("Position");
            }
        }

        public SolidColorBrush Color
        {
            get
            {
                if (color == null)
                    IsCorrect = isCorrect; // this'll init Color property cos it is not serializable
                return color;
            }
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }
        }

        public bool IsCorrect
        {
            get
            {
                return isCorrect;
            }
            set
            {
                if (value)
                    Color = AppLib.CorrectColor;
                else
                    Color = AppLib.IncorrectColor;
                isCorrect = value;
                OnPropertyChanged("IsCorrect");
            }
        }

        public string StrNumber
        {
            get
            {
                return strNumber ?? "";
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value) || value == String.Empty)
                {
                    Number = null;
                    strNumber = "";
                }
                int ret = 0;
                if (Int32.TryParse(value, out ret) && ret > 0 && ret < 10)
                {
                    strNumber = value;
                    Number = ret;
                }
            }
        }

        public int? Number 
        { 
            get 
            { 
                return number; 
            }
            set
            {
                number = value;
                strNumber = value.ToString();
                //Keyboard.ClearFocus();
                if (Row != null && Col != null && Squares != null)
                {
                    if (Row.Count(x => x.Number == value && x.Number != null) > 0
                    || Col.Count(x => x.Number == value && x.Number != null) > 0
                    || Squares.Count(x => x.Number == value && x.Number != null) > 0)
                        IsCorrect = false;
                    else IsCorrect = true;
                }
            } 
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void InitThickness()
        {
            var thick = new Thickness(0.5);

            if (AppLib.ThickPositions.Contains(this.Position.X))
                thick.Left = 2;
            if (AppLib.ThickPositions.Contains(this.Position.Y))
                thick.Top = 2;
            if (this.Position.X == 9)
                thick.Right = 2;
            if (this.Position.Y == 9)
                thick.Bottom = 2;
            this.borderThickness = thick;
        }
    }
    // maybe ill use it
    //public class CellComparer : IEqualityComparer<Cell>
    //{
    //    public bool Equals(Cell x, Cell y)
    //    {
    //        return (x.XPos == y.XPos) && (x.YPos == y.YPos);
    //    }

    //    public int GetHashCode(Cell obj)
    //    {
    //        // may be cells like x:6 y:2 & y:6 x:2 with the same value
    //        return (obj.YPos + obj.XPos + obj.Number).GetHashCode();
    //    }
    //}
}
[Serializable]
public struct Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}
