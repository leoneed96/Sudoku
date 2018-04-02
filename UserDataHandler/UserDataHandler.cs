using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace SudokuMaker.UserDataHandler
{
    public class UserDataWorker
    {
        public UserData currentData;
        public readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Sudoku");
        public readonly string SaveGamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Sudoku", "SaveGame");
        public readonly string StatisticsFileName = "stat.dat";

        public UserDataWorker()
        {
            MakeGameFoler();
        }
        public void SaveGame(UserData data, string fileName)
        {
            var bf = new BinaryFormatter();
            // to make only one .sudo in the end
            using (FileStream fs = new FileStream($"{fileName.Replace(".sudo", "")}.sudo", FileMode.OpenOrCreate))
            {
                bf.Serialize(fs, data); // 
            }
        }

        public UserData LoadGame(string fileName)
        {
            var bf = new BinaryFormatter();
            using (FileStream fs = new FileStream($"{fileName}", FileMode.OpenOrCreate))
            {
                UserData data = (UserData)bf.Deserialize(fs);
                return data;
            }
        }

        public void MakeGameFoler()
        {
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
                if (!Directory.Exists(SaveGamePath))
                    Directory.CreateDirectory(SaveGamePath);
            }
        }

        public UserStatisticsData LoadStatistics()
        {
            if(!File.Exists(Path.Combine(AppDataPath, StatisticsFileName)))
            {
                using(FileStream fs = File.Create(Path.Combine(AppDataPath, StatisticsFileName)))
                {
                    fs.Close();
                }
                var newStat = new UserStatisticsData();
                SaveStatistic(newStat);
                return newStat;
            }
            else
            {
                var bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(Path.Combine(AppDataPath, StatisticsFileName), FileMode.OpenOrCreate))
                {
                    UserStatisticsData data = (UserStatisticsData)bf.Deserialize(fs);
                    return data;
                }
            }
        }

        public void SaveStatistic(UserStatisticsData data)
        {
            var bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(AppDataPath, StatisticsFileName), FileMode.OpenOrCreate))
            {
                bf.Serialize(fs, data);
            }
        }

    }
}
