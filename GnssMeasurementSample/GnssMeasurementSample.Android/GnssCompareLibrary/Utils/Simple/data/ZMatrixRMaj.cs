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
	/// Dense matrix for complex numbers.  Internally it stores its data in a single row-major array with the real
	/// and imaginary components interlaces, in that order.  The total number of elements in the array will be
	/// numRows*numColumns*2.
	/// 
	/// @author Peter Abeles
	/// </summary>
	public class ZMatrixRMaj : ZMatrixD1
	{

		/// <summary>
		/// <para>
		/// Creates a matrix with the values and shape defined by the 2D array 'data'.
		/// It is assumed that 'data' has a row-major formatting:<br>
		/// <br>
		/// data[ row ][ column ]
		/// </para>
		/// </summary>
		/// <param name="data"> 2D array representation of the matrix. Not modified. </param>
		public ZMatrixRMaj(double[][] data)
		{
			this.numRows = data.Length;
			this.numCols = data[0].Length / 2;

			UtilEjml.checkTooLargeComplex(numRows, numCols);

			this.data = new double[numRows * numCols * 2];

			for (int i = 0; i < numRows; i++)
			{
				double[] row = data[i];
				if (row.Length != numCols * 2)
				{
					throw new System.ArgumentException("Unexpected row size in input data at row " + i);
				}

				Array.Copy(row, 0, this.data, i * numCols * 2, row.Length);
			}
		}

		public ZMatrixRMaj(int numRows, int numCols, bool rowMajor, params double[] data)
		{
			if (data.Length != numRows * numCols * 2)
			{
				throw new Exception("Unexpected length for data");
			}

			this.data = new double[numRows * numCols * 2];

			this.numRows = numRows;
			this.numCols = numCols;

			setTo(numRows, numCols, rowMajor, data);
		}

		/// <summary>
		/// Creates a new <seealso cref="ZMatrixRMaj"/> which is a copy of the passed in matrix.
		/// </summary>
		/// <param name="original"> Matrix which is to be copied </param>
		public ZMatrixRMaj(ZMatrixRMaj original) : this(original.numRows, original.numCols)
		{
			setTo(original);
		}

		/// <summary>
		/// Creates a new matrix with the specified number of rows and columns
		/// </summary>
		/// <param name="numRows"> number of rows </param>
		/// <param name="numCols"> number of columns </param>
		public ZMatrixRMaj(int numRows, int numCols)
		{
			UtilEjml.checkTooLargeComplex(numRows, numCols);
			this.numRows = numRows;
			this.numCols = numCols;
			this.data = new double[numRows * numCols * 2];
		}

		public override int getIndex(int row, int col)
		{
			return row * numCols * 2 + col * 2;

		}

		public override void reshape(int numRows, int numCols)
		{
			UtilEjml.checkTooLargeComplex(numRows, numCols);
			int newLength = numRows * numCols * 2;

			if (newLength > data.Count())
			{
				data = new double[newLength];
			}

			this.numRows = numRows;
			this.numCols = numCols;
		}

		public override void get(int row, int col, Complex_F64 output)
		{
			int index = row * numCols * 2 + col * 2;
			output.real = data[index];
			output.imaginary = data[index + 1];
		}

		public override void set(int row, int col, double real, double imaginary)
		{
			int index = row * numCols * 2 + col * 2;
			data[index] = real;
			data[index + 1] = imaginary;
		}

		public virtual double getReal(int element)
		{
			return data[element * 2];
		}

		public virtual double getImag(int element)
		{
			return data[element * 2 + 1];
		}

		public override double getReal(int row, int col)
		{
			return data[(row * numCols + col) * 2];
		}

		public override void setReal(int row, int col, double val)
		{
			data[(row * numCols + col) * 2] = val;
		}

		public override double getImag(int row, int col)
		{
			return data[(row * numCols + col) * 2 + 1];
		}

		public override void setImag(int row, int col, double val)
		{
			data[(row * numCols + col) * 2 + 1] = val;
		}

		public override int DataLength
		{
			get
			{
				return numRows * numCols * 2;
			}
		}

		public virtual void setTo(ZMatrixRMaj original)
		{
			reshape(original.numRows, original.numCols);
			int columnSize = numCols * 2;
			for (int y = 0; y < numRows; y++)
			{
				int index = y * numCols * 2;
				Array.Copy(original.data, index, data, index, columnSize);
			}
		}

		public virtual ZMatrixRMaj copy()
		{
			return new ZMatrixRMaj(this);
		}

		public virtual void setTo(Matrix original)
		{
			reshape(original.NumRows, original.NumCols);

			ZMatrix n = (ZMatrix)original;

			Complex_F64 c = new Complex_F64();
			for (int i = 0; i < numRows; i++)
			{
				for (int j = 0; j < numCols; j++)
				{
					n.get(i, j, c);
					set(i, j, c.real, c.imaginary);
				}
			}
		}

		public void print()
		{
			MatrixIO.printFancy((DMatrix)this, MatrixIO.DEFAULT_LENGTH);
		}

		public void print(string format)
		{
			MatrixIO.print(format);
		}

		/// <summary>
		/// Number of array elements in the matrix's row.
		/// </summary>
		public virtual int RowStride
		{
			get
			{
				return numCols * 2;
			}
		}

		/// <summary>
		/// Sets this matrix equal to the matrix encoded in the array.
		/// </summary>
		/// <param name="numRows"> The number of rows. </param>
		/// <param name="numCols"> The number of columns. </param>
		/// <param name="rowMajor"> If the array is encoded in a row-major or a column-major format. </param>
		/// <param name="data"> The formatted 1D array. Not modified. </param>
		public virtual void setTo(int numRows, int numCols, bool rowMajor, params double[] data)
		{
			reshape(numRows, numCols);
			int length = numRows * numCols * 2;

			if (length > data.Length)
			{
				throw new Exception("Passed in array not long enough");
			}

			if (rowMajor)
			{
				Array.Copy(data, 0, this.data, 0, length);
			}
			else
			{
				int index = 0;
				int stride = numRows * 2;
				for (int i = 0; i < numRows; i++)
				{
					for (int j = 0; j < numCols; j++)
					{
						this.data[index++] = data[j * stride + i * 2];
						this.data[index++] = data[j * stride + i * 2 + 1];
					}
				}
			}
		}

		/// <summary>
		/// Sets all the elements in the matrix to zero
		/// </summary>
		public override void zero()
		{
			Arrays.Fill(data, 0, numCols * numRows * 2, 0.0);
		}

		public virtual ZMatrixRMaj createLike()
		{
			return new ZMatrixRMaj(numRows, numCols);
		}

		public virtual ZMatrixRMaj create(int numRows, int numCols)
		{
			return new ZMatrixRMaj(numRows, numCols);
		}
        public override MatrixType Type
		{
			get
			{
				return MatrixType.ZDRM();
			}
		}

        public override Matrix To { set => throw new NotImplementedException(); }
    }

}