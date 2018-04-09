﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuMaker.Util
{
    public static class AppLib
    {
        public static readonly int SudokuCellsCount = 81;
        public static List<int> ThickPositions = new List<int> { 1, 4, 7 };
        private static SolidColorBrush correctColor = new SolidColorBrush(Colors.Black); 
        private static SolidColorBrush incorrectColor = new SolidColorBrush(Colors.Red);

        private static StatisticsWindow statWindow;
        private static InfoWindow infoWindow;

        public static StatisticsWindow StatWindow
        {
            get
            {
                if (statWindow != null)
                    return statWindow;
                statWindow = new StatisticsWindow();
                return statWindow;
            }
            set
            {
                statWindow = value;
            }
        }

        public static InfoWindow InfoWindow
        {
            get
            {
                if (infoWindow != null)
                    return infoWindow;
                infoWindow = new InfoWindow();
                return infoWindow;
            }
            set
            {
                infoWindow = value;
            }
        }

        public static void ShowStats(List<UserDataHandler.UserStatistics> viewModel)
        {
            StatWindow.DataContext = new SudokuMaker.ViewModel.StatisticsViewModel(viewModel);
            StatWindow.ShowDialog();
            StatWindow = null;
        }

        public static void ShowInfoWindow(string windowHeaderText, string articleHeaderText, string mainText)
        {
            InfoWindow.DataContext = new SudokuMaker.ViewModel.InfoModel(windowHeaderText, articleHeaderText, mainText);
            InfoWindow.ShowDialog();
            InfoWindow = null;
        }

        public static SolidColorBrush CorrectColor
        {
            get
            {
                return correctColor; 
            }
        }

        public static SolidColorBrush IncorrectColor
        {
            get
            {
                return incorrectColor;
            }
        }
    }
}
