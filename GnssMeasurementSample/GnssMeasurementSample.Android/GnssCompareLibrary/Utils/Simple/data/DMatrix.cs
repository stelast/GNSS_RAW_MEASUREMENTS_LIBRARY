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
	/// Interface for all 64F real matrices.
	/// 
	/// @author Peter Abeles
	/// </summary>
	public interface DMatrix : Matrix
	{

		/// <summary>
		/// Returns the value of value of the specified matrix element.
		/// </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <returns> The specified element's value. </returns>
		double get(int row, int col);

		/// <summary>
		/// Same as <seealso cref="get"/> but does not perform bounds check on input parameters.  This results in about a 25%
		/// speed increase but potentially sacrifices stability and makes it more difficult to track down simple errors.
		/// It is not recommended that this function be used, except in highly optimized code where the bounds are
		/// implicitly being checked.
		/// </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <returns> The specified element's value. </returns>
		double unsafe_get(int row, int col);

		/// <summary>
		/// Sets the value of the specified matrix element.
		/// </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <param name="val"> The element's new value. </param>
		void set(int row, int col, double val);

		/// <summary>
		/// Same as <seealso cref="setTo"/> but does not perform bounds check on input parameters.  This results in about a 25%
		/// speed increase but potentially sacrifices stability and makes it more difficult to track down simple errors.
		/// It is not recommended that this function be used, except in highly optimized code where the bounds are
		/// implicitly being checked.
		/// </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <param name="val"> The element's new value. </param>
		void unsafe_set(int row, int col, double val);

		/// <summary>
		/// Returns the number of elements in this matrix, which is the number of rows
		/// times the number of columns.
		/// </summary>
		/// <returns> Number of elements in this matrix. </returns>
		int NumElements { get; }
	}
}