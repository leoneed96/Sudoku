
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuMaker.ViewModel
{
    public class InfoModel
    {
        public InfoModel()
        {

        }

        public InfoModel(string windowHeaderText, string articleHeaderText, string mainText)
        {
            WindowHeader = windowHeaderText;
            TextHeader = articleHeaderText;
            MainText = mainText;
        }
        public string WindowHeader { get; set; }
        public string TextHeader { get; set; }
        public string MainText { get; set; }
        //TODO: как-нибудь сначала оформить данные справа, потом описать
    }
}
