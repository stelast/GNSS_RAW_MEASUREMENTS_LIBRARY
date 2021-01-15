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
	/// Base interface for all rectangular matrices
	/// 
	/// @author Peter Abeles
	/// </summary>
	public interface Matrix
	{

		/// <summary>
		/// Returns the number of rows in this matrix.
		/// </summary>
		/// <returns> Number of rows. </returns>
		int NumRows { get; }

		/// <summary>
		/// Returns the number of columns in this matrix.
		/// </summary>
		/// <returns> Number of columns. </returns>
		int NumCols { get; }

		/// <summary>
		/// Sets all values inside the matrix to zero
		/// </summary>
		void zero();

		/// <summary>
		/// Creates an exact copy of the matrix
		/// </summary>
		T copy<T>() where T:Matrix;

		/// <summary>
		/// Creates a new matrix with the same shape as this matrix
		/// </summary>
		T createLike<T>() where T : Matrix;

		/// <summary>
		/// Creates a new matrix of the same type with the specified shape
		/// </summary>
		T create<T>(int numRows, int numCols) where T : Matrix;

		/// <summary>
		/// Sets this matrix to be identical to the 'original' matrix passed in.
		/// </summary>
		Matrix To { set; }

		/// <summary>
		/// Prints the matrix to standard out using standard formatting. This is the same as calling print("%e")
		/// </summary>
		void print();

		/// <summary>
		/// Prints the matrix to standard out with the specified formatting.
		/// </summary>
		/// <seealso cref= java.util.Formatter </seealso>
		/// <param name="format"> printf style formatting for a float. E.g. "%f" </param>
		void print(string format);

		/// <summary>
		/// Returns the type of matrix
		/// </summary>
		abstract MatrixType Type { get; }
	}

}