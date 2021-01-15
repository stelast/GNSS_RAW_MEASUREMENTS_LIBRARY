using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data
{
	/// <summary>
	/// Matrix which can be reshaped
	/// 
	/// @author Peter Abeles
	/// </summary>
	public interface ReshapeMatrix : Matrix
	{
		/// <summary>
		/// Equivalent to invoking reshape(numRows,numCols,false);
		/// </summary>
		/// <param name="numRows"> The new number of rows in the matrix. </param>
		/// <param name="numCols"> The new number of columns in the matrix. </param>
		void reshape(int numRows, int numCols);
	}
}