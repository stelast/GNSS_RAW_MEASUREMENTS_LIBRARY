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
	/// Interface for all complex 64 bit floating point rectangular matrices.
	/// 
	/// @author Peter Abeles
	/// </summary>
	public interface ZMatrix : Matrix
	{

		/// <summary>
		/// Returns the complex value of the matrix's element </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <param name="output"> Storage for the complex number </param>
		void get(int row, int col, Complex_F64 output);

		/// <summary>
		/// Set's the complex value of the matrix's element
		/// </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <param name="real"> The real component </param>
		/// <param name="imaginary"> The imaginary component </param>
		void set(int row, int col, double real, double imaginary);

		/// <summary>
		/// Returns the real component of the matrix's element.
		/// </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <returns> The specified element's value. </returns>
		double getReal(int row, int col);


		/// <summary>
		/// Sets the real component of the matrix's element.
		/// </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <param name="val">  The element's new value. </param>
		void setReal(int row, int col, double val);

		/// <summary>
		/// Returns the imaginary component of the matrix's element.
		/// </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <returns> The specified element's value. </returns>
		double getImag(int row, int col);


		/// <summary>
		/// Sets the imaginary component of the matrix's element.
		/// </summary>
		/// <param name="row"> Matrix element's row index.. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <param name="val">  The element's new value. </param>
		void setImag(int row, int col, double val);

		/// <summary>
		/// Returns the number of elements in the internal data array
		/// </summary>
		/// <returns> Number of elements in the data array. </returns>
		public int DataLength { get; }

	}
}