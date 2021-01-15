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
	/// High level interface for sparse matrices double types.
	/// 
	/// @author Peter Abeles
	/// </summary>
	public interface DMatrixSparse : DMatrix, MatrixSparse
	{

		/// <summary>
		/// Returns the value of value of the specified matrix element.
		/// </summary>
		/// <param name="row">           Matrix element's row index.. </param>
		/// <param name="col">           Matrix element's column index. </param>
		/// <param name="fallBackValue"> Value to return, if the matrix element is not assigned </param>
		/// <returns> The specified element's value. </returns>
		double get(int row, int col, double fallBackValue);

		/// <summary>
		/// Same as <seealso cref="get"/> but does not perform bounds check on input parameters.  This results in about a 25%
		/// speed increase but potentially sacrifices stability and makes it more difficult to track down simple errors.
		/// It is not recommended that this function be used, except in highly optimized code where the bounds are
		/// implicitly being checked.
		/// </summary>
		/// <param name="row">           Matrix element's row index.. </param>
		/// <param name="col">           Matrix element's column index. </param>
		/// <param name="fallBackValue"> Value to return, if the matrix element is not assigned </param>
		/// <returns> The specified element's value or the fallBackValue, if the element is not assigned. </returns>
		double unsafe_get(int row, int col, double fallBackValue);

		/// <summary>
		/// Creates an iterator which will go through each non-zero value in the sparse matrix. Order is not defined
		/// and is implementation specific
		/// </summary>
		IEnumerator<CoordinateRealValue> createCoordinateIterator();

		/// <summary>
		/// Value of an element in a sparse matrix
		/// </summary>
	}

	public class CoordinateRealValue
	{
		/// <summary>
		/// The coordinate </summary>
		public int row, col;
		/// <summary>
		/// The value of the coordinate </summary>
		public double value;
	}
}