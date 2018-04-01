using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuMaker.UserDataHandler;

namespace SudokuMaker.ViewModel
{
    public class StatisticsViewModel
    {
        private UserStatistics selectedDiff;
        public StatisticsViewModel(List<UserStatistics> stat)
        {
            Statistics = stat;
        }
        public List<UserStatistics> Statistics { get; set; }
        /// <summary>
        /// Текущий выбранный уровень сложности для просмотра статистики.
        /// </summary>
        public UserStatistics SelectedDiff
        {
            get
            {
                return selectedDiff ?? 
                    Statistics.Where(x => x.Difficulty == Model.Difficulty.VeryEasy).FirstOrDefault();
            }
            set
            {
                selectedDiff = value;
            }
        }

    }
}
