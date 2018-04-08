using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SudokuMaker.Model
{
    [Serializable]
    public class Game : INotifyPropertyChanged
    {
        public Game()
        {
            isStarted = false;
        }

        private DateTime startTime;
        private TimeSpan spendTime;
        private Difficulty difficulty;
        private bool isStarted;
        private bool isFinished;
        private bool shouldPlayCorrectAnimation;

        public bool ShouldPlayCorrectAnimation
        {
            get
            {
                return shouldPlayCorrectAnimation;
            }
            set
            {
                shouldPlayCorrectAnimation = value;
                OnPropertyChanged("ShouldPlayCorrectAnimation");
            }
        }

        public bool IsStarted
        {
            get
            {
                return isStarted;
            }
            set
            {
                isStarted = value;
                OnPropertyChanged("IsStarted");
            }
        }

        /// <summary>
        /// TODO: что-нибудь эдакое, что бы ловко убирало это обратно в false
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return isFinished;
            }
            set
            {
                isFinished = value;
                OnPropertyChanged("IsFinished");
            }
        }

        public Difficulty Difficulty
        {
            get
            {
                return difficulty;
            }
            set
            {
                difficulty = value;
                OnPropertyChanged("Difficulty");
            }
        }

        public TimeSpan SpendTime
        {
            get
            {
                return spendTime;
            }
            set
            {
                spendTime = value;
                OnPropertyChanged("SpendTime");
            }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    // percent of unfilled space
    public enum Difficulty
    {
        VeryEasy = 20,
        Easy = 30,
        Medium = 45,
        Hard = 55,
        Impressive = 70
    }
}
