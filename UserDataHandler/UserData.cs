using SudokuMaker.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuMaker.UserDataHandler
{
    [Serializable]
    public class UserData
    {
        public ObservableCollection<Cell> Cells { get; set; }
        public ObservableCollection<Cell> UserCells { get; set; }
        public Game Game { get; set; }
    }

    [Serializable]
    public class UserStatisticsData
    {
        public UserStatisticsData()
        {
            Statistics = new List<UserStatistics>();
            foreach (var item in Enum.GetValues(typeof(Difficulty)))
            {
                Statistics.Add(new UserStatistics()
                {
                    Difficulty = (Difficulty)item,
                    GamesCount = 0,
                    SuccessGamesCount = 0,
                    Times = new List<TimeSpan?>()
                });
            }
        }
        public List<UserStatistics> Statistics { get; set; }

        public void AddStart(Difficulty diff)
        {
            var curr = Statistics.Where(x => x.Difficulty == diff).FirstOrDefault();
            if (curr == null) ;
            //handle
            curr.GamesCount++;
        }

        public void AddFinish(Difficulty diff, TimeSpan ts)
        {
            var curr = Statistics.Where(x => x.Difficulty == diff).FirstOrDefault();
            if (curr == null) ;
            //handle
            curr.SuccessGamesCount++;
            curr.Times.Add(ts);
        }
    }

    [Serializable]
    public class UserStatistics
    {
        public Difficulty Difficulty { get; set; }
        public int GamesCount { get; set; }
        public int SuccessGamesCount { get; set; }
        public TimeSpan? MiddleTime
        {
            get
            {
                if (Times == null || Times.Count == 0)
                    return null;
                double sum = 0;
                Times.ForEach(x => sum += x.Value.TotalSeconds);
                return TimeSpan.FromSeconds(sum / Times.Count);
            }
        }
        public TimeSpan? BestTime
        {
            get
            {
                if (Times.Count == 0)
                    return null;
                return Times.Min();
            }
        }
        public List<TimeSpan?> Times { get; set; }
    }


}
