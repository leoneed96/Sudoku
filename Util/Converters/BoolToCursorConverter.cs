using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;

namespace SudokuMaker.Util.Converters
{
    [ValueConversion(typeof(bool), typeof(Cursor))]
    public class BoolToCursorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            //if (targetType != typeof(Cursor))
              //  throw new InvalidOperationException("The target must be a Cursor");

			var val = (bool)value;
			if (val)
				return Cursors.UpArrow;
			return Cursors.Arrow;
		}

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
