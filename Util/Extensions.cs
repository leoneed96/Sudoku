using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuMaker.Util
{
	public static class Extensions
	{
		public static void Trigger(this bool val)
		{
			val = !val;
		}
	}
}
