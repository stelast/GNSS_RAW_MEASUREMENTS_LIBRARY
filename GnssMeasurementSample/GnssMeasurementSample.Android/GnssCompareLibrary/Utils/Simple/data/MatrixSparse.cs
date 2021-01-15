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
	/// High level interface for all sparse matrices
	/// 
	/// @author Peter Abeles
	/// </summary>
	public interface MatrixSparse : ReshapeMatrix
	{
		/// <summary>
		/// Prints to standard out the non-zero elements only.
		/// </summary>
		void printNonZero();

		/// <summary>
		/// Reshapes the matrix so that it can store a matrix with the specified dimensions and the number of
		/// non-zero elements.  The reshaped matrix will be empty.
		/// </summary>
		/// <param name="numRows"> number of rows </param>
		/// <param name="numCols"> number of columns </param>
		/// <param name="arrayLength"> Array length for storing non-zero elements. </param>
		void reshape(int numRows, int numCols, int arrayLength);

		/// <summary>
		/// Changes the number of rows and columns in the matrix. The graph structure is flushed and the matrix will
		/// be empty/all zeros.  Similar to <seealso cref="reshape(int, int, int)"/>, but the storage for non-zero elements is
		/// not changed
		/// </summary>
		/// <param name="numRows"> number of rows </param>
		/// <param name="numCols"> number of columns </param>
		void reshape(int numRows, int numCols);

		/// <summary>
		/// Reduces the size of internal data structures to their minimal size.  No information is lost but the arrays will
		/// change
		/// </summary>
		void shrinkArrays();

		/// <summary>
		/// If the specified element is non-zero it is removed from the structure </summary>
		/// <param name="row"> the row </param>
		/// <param name="col"> the column </param>
		void remove(int row, int col);

		/// <summary>
		/// Is the specified element explicitly assigned a value </summary>
		/// <param name="row"> the row </param>
		/// <param name="col"> the column </param>
		/// <returns> true if it has been assigned a value or false if not </returns>
		bool isAssigned(int row, int col);

		/// <summary>
		/// Returns number of non-zero values
		/// </summary>
		int NonZeroCount { get; }

		/// <summary>
		/// Sets all elements to zero by removing the sparse graph
		/// </summary>
		void zero();

		/// <summary>
		/// Returns the number of non-zero elements.
		/// </summary>
		int NonZeroLength { get; }
	}
}