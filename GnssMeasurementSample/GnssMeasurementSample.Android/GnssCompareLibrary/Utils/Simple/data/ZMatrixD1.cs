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
	/// A generic abstract class for matrices whose data is stored in a single 1D array of doubles.  The
	/// format of the elements in this array is not specified.  For example row major, column major,
	/// and block row major are all common formats.
	/// 
	/// @author Peter Abeles
	/// </summary>
	public abstract class ZMatrixD1 : ZMatrix, ReshapeMatrix
	{
		/// <summary>
		/// Where the raw data for the matrix is stored.  The format is type dependent.
		/// </summary>
		public double[] data = UtilEjml.ZERO_LENGTH_F64;

		/// <summary>
		/// Number of rows in the matrix.
		/// </summary>
		public int numRows;
		/// <summary>
		/// Number of columns in the matrix.
		/// </summary>
		public int numCols;

		/// <summary>
		/// Used to get a reference to the internal data.
		/// </summary>
		/// <returns> Reference to the matrix's data. </returns>
		public virtual double[] Data
		{
			get
			{
				return data;
			}
			set
			{
				this.data = value;
			}
		}


		/// <summary>
		/// Returns the internal array index for the specified row and column.
		/// </summary>
		/// <param name="row"> Row index. </param>
		/// <param name="col"> Column index. </param>
		/// <returns> Internal array index. </returns>
		public abstract int getIndex(int row, int col);

		/// <summary>
		/// Sets the value of this matrix to be the same as the value of the provided matrix.  Both
		/// matrices must have the same shape:<br>
		/// <br>
		/// a<sub>ij</sub> = b<sub>ij</sub><br>
		/// <br>
		/// </summary>
		/// <param name="b"> The matrix that this matrix is to be set equal to. </param>
		public virtual void setTo(ZMatrixD1 b)
		{
			if (numRows != b.numRows || numCols != b.numCols)
			{
				throw new MatrixDimensionException("The two matrices do not have compatible shapes.");
			}

			int dataLength = b.DataLength;

			Array.Copy(b.data, 0, this.data, 0, dataLength);
		}
		public virtual int DataLength { get; }

		/// <summary>
		/// Sets the number of rows.
		/// </summary>
		/// <param name="numRows"> Number of rows </param>
		public virtual int NumCols
		{
			get
			{
				return this.numCols;
			}
			set
			{
				this.numCols = value;
			}
		}


		/// <summary>
		/// Sets the number of rows.
		/// </summary>
		/// <param name="numRows"> Number of rows </param>
		public virtual int NumRows
		{
			get
			{
				return this.numRows;
			}
			set
			{
				this.numRows = value;
			}
		}


		public virtual int NumElements
		{
			get
			{
				return numRows * numCols;
			}
		}
		public void print()
		{
			MatrixIO.printFancy(this, MatrixIO.DEFAULT_LENGTH);
		}

		public void print(string format)
		{
			MatrixIO.print(format);
		}

		public abstract Matrix To { set; }
        public abstract MatrixType Type { get; }

		public abstract void reshape(int numRows, int numCols);
		public abstract void zero();

		public abstract void get(int row, int col, Complex_F64 output);
        public abstract void set(int row, int col, double real, double imaginary);
        public abstract double getReal(int row, int col);
        public abstract void setReal(int row, int col, double val);
        public abstract double getImag(int row, int col);
        public abstract void setImag(int row, int col, double val);

        public T copy<T>() where T : Matrix
        {
            throw new NotImplementedException();
        }

        public T createLike<T>() where T : Matrix
        {
            throw new NotImplementedException();
        }

        public T create<T>(int numRows, int numCols) where T : Matrix
        {
            throw new NotImplementedException();
        }
    }
}