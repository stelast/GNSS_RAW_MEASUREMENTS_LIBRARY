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
	/// <para>
	/// DMatrixRMaj is a row matrix with real elements that are 64-bit floats.  A matrix
	/// is the fundamental data structure in linear algebra.  Unlike a sparse matrix, there is no
	/// compression in a row matrix and every element is stored in memory.  This allows for fast
	/// reads and writes to the matrix.
	/// </para>
	/// 
	/// <para>
	/// The matrix is stored internally in a row-major 1D array format:<br>
	/// <br>
	/// data[ y*numCols + x ] = data[y][x]<br>
	/// <br>
	/// For example:<br>
	/// data =
	/// </para>
	/// <pre>
	/// a[0]  a[1]   a[2]   a[3]
	/// a[4]  a[5]   a[6]   a[7]
	/// a[8]  a[9]   a[10]  a[11]
	/// a[12] a[13]  a[14]  a[15]
	/// </pre>
	/// @author Peter Abeles
	/// </summary>
	public class DMatrixRMaj : DMatrix1Row
	{

		/// <summary>
		/// <para>
		/// Creates a new matrix which has the same value as the matrix encoded in the
		/// provided array.  The input matrix's format can either be row-major or
		/// column-major.
		/// </para>
		/// 
		/// <para>
		/// Note that 'data' is a variable argument type, so either 1D arrays or a set of numbers can be
		/// passed in:<br>
		/// DenseMatrix a = new DenseMatrix(2,2,true,new double[]{1,2,3,4});<br>
		/// DenseMatrix b = new DenseMatrix(2,2,true,1,2,3,4);<br>
		/// <br>
		/// Both are equivalent.
		/// </para>
		/// </summary>
		/// <param name="numRows"> The number of rows. </param>
		/// <param name="numCols"> The number of columns. </param>
		/// <param name="rowMajor"> If the array is encoded in a row-major or a column-major format. </param>
		/// <param name="data"> The formatted 1D array. Not modified. </param>
		public DMatrixRMaj(int numRows, int numCols, bool rowMajor, params double[] data)
		{
			UtilEjml.checkTooLarge(numRows, numCols);
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final int length = numRows * numCols;
			int length = numRows * numCols;
			this.data = new double[length];

			this.numRows = numRows;
			this.numCols = numCols;

			set(numRows, numCols, rowMajor, data);
		}

		/// <summary>
		/// <para>
		/// Creates a matrix with the values and shape defined by the 2D array 'data'.
		/// It is assumed that 'data' has a row-major formatting:<br>
		///  <br>
		/// data[ row ][ column ]
		/// </para> </summary>
		/// <param name="data"> 2D array representation of the matrix. Not modified. </param>
		public DMatrixRMaj(double[][] data) : this(1, 1)
		{
			set(data);
		}

		/// <summary>
		/// Creates a column vector the same length as this array </summary>
		/// <param name="data"> elements in vector. copied </param>
		//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
		//ORIGINAL LINE: public DMatrixRMaj(double data[])
		public DMatrixRMaj(double[] data)
		{
			this.data = (double[])data.Clone();
			this.numRows = this.data.Count();
			this.numCols = 1;
		}

		/// <summary>
		/// Creates a new Matrix with the specified shape whose elements initially
		/// have the value of zero.
		/// </summary>
		/// <param name="numRows"> The number of rows in the matrix. </param>
		/// <param name="numCols"> The number of columns in the matrix. </param>
		//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
		//ORIGINAL LINE: public DMatrixRMaj(int numRows, int numCols)
		public DMatrixRMaj(int numRows, int numCols)
		{
			UtilEjml.checkTooLarge(numRows, numCols);
			data = new double[numRows * numCols];

			this.numRows = numRows;
			this.numCols = numCols;
		}

		/// <summary>
		/// Creates a new matrix which is equivalent to the provided matrix.  Note that
		/// the length of the data will be determined by the shape of the matrix.
		/// </summary>
		/// <param name="orig"> The matrix which is to be copied.  This is not modified or saved. </param>
		//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
		//ORIGINAL LINE: public DMatrixRMaj(DMatrixRMaj orig)
		public DMatrixRMaj(DMatrixRMaj orig) : this(orig.numRows, orig.numCols)
		{
			Array.Copy(orig.data, 0, this.data, 0, orig.NumElements);
		}

		/// <summary>
		/// This declares an array that can store a matrix up to the specified length.  This is use full
		/// when a matrix's size will be growing and it is desirable to avoid reallocating memory.
		/// </summary>
		/// <param name="length"> The size of the matrice's data array. </param>
		//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
		//ORIGINAL LINE: public DMatrixRMaj(int length)
		public DMatrixRMaj(int length)
		{
			data = new double[length];
		}

		/// <summary>
		/// Default constructor in which nothing is configured.  THIS IS ONLY PUBLICLY ACCESSIBLE SO THAT THIS
		/// CLASS CAN BE A JAVA BEAN. DON'T USE IT UNLESS YOU REALLY KNOW WHAT YOU'RE DOING!
		/// </summary>
		//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
		//ORIGINAL LINE: public DMatrixRMaj()
		public DMatrixRMaj()
		{
		}

		/// <summary>
		/// Creates a new DMatrixRMaj which contains the same information as the provided Matrix64F.
		/// </summary>
		/// <param name="mat"> Matrix whose values will be copied.  Not modified. </param>
		//JAVA TO C# CONVERTER WARNING: The following constructor is declared outside of its associated class:
		//ORIGINAL LINE: public DMatrixRMaj(DMatrix mat)
		public DMatrixRMaj(DMatrix mat) : this(mat.NumRows, mat.NumCols)
		{
			for (int i = 0; i < numRows; i++)
			{
				for (int j = 0; j < numCols; j++)
				{
					set(i, j, mat.get(i, j));
				}
			}
		}

		/// <summary>
		/// Creates a new DMatrixRMaj around the provided data.  The data must encode
		/// a row-major matrix.  Any modification to the returned matrix will modify the
		/// provided data.
		/// </summary>
		/// <param name="numRows"> Number of rows in the matrix. </param>
		/// <param name="numCols"> Number of columns in the matrix. </param>
		/// <param name="data"> Data that is being wrapped. Referenced Saved. </param>
		/// <returns> A matrix which references the provided data internally. </returns>
		public static DMatrixRMaj wrap(int numRows, int numCols, double[] data)
		{
			UtilEjml.checkTooLarge(numRows, numCols);
			DMatrixRMaj s = new DMatrixRMaj();
			s.data = data;
			s.numRows = numRows;
			s.numCols = numCols;

			return s;
		}


		/// <summary>
		/// <para>
		/// Assigns the element in the Matrix to the specified value.  Performs a bounds check to make sure
		/// the requested element is part of the matrix. <br>
		/// <br>
		/// a<sub>ij</sub> = value<br>
		/// </para>
		/// </summary>
		/// <param name="row"> The row of the element. </param>
		/// <param name="col"> The column of the element. </param>
		/// <param name="value"> The element's new value. </param>
		public void set(int row, int col, double value)
		{
			if (col < 0 || col >= numCols || row < 0 || row >= numRows)
			{
				throw new System.ArgumentException("Specified element is out of bounds: (" + row + " , " + col + ")");
			}

			data[row * numCols + col] = value;
		}

		public void unsafe_set(int row, int col, double value)
		{
			data[row * numCols + col] = value;
		}

		/// <summary>
		/// <para>
		/// Adds 'value' to the specified element in the matrix.<br>
		/// <br>
		/// a<sub>ij</sub> = a<sub>ij</sub> + value<br>
		/// </para>
		/// </summary>
		/// <param name="row"> The row of the element. </param>
		/// <param name="col"> The column of the element. </param>
		/// <param name="value"> The value that is added to the element </param>
		// todo move to commonops
		public void add(int row, int col, double value)
		{
			if (col < 0 || col >= numCols || row < 0 || row >= numRows)
			{
				throw new System.ArgumentException("Specified element is out of bounds");
			}

			data[row * numCols + col] += value;
		}

		/// <summary>
		/// Returns the value of the specified matrix element.  Performs a bounds check to make sure
		/// the requested element is part of the matrix.
		/// </summary>
		/// <param name="row"> The row of the element. </param>
		/// <param name="col"> The column of the element. </param>
		/// <returns> The value of the element. </returns>
		public double get(int row, int col)
		{
			if (col < 0 || col >= numCols || row < 0 || row >= numRows)
			{
				throw new System.ArgumentException("Specified element is out of bounds: " + row + " " + col);
			}

			return data[row * numCols + col];
		}

		public double unsafe_get(int row, int col)
		{
			return data[row * numCols + col];
		}

		public override int getIndex(int row, int col)
		{
			return row * numCols + col;
		}

		/// <summary>
		/// Determines if the specified element is inside the bounds of the Matrix.
		/// </summary>
		/// <param name="row"> The element's row. </param>
		/// <param name="col"> The element's column. </param>
		/// <returns> True if it is inside the matrices bound, false otherwise. </returns>
		public bool isInBounds(int row, int col)
		{
			return (col >= 0 && col < numCols && row >= 0 && row < numRows);
		}

		/// <summary>
		/// Returns the number of elements in this matrix, which is equal to
		/// the number of rows times the number of columns.
		/// </summary>
		/// <returns> The number of elements in the matrix. </returns>
		public int NumElements
		{
			get
			{
				return numRows * numCols;
			}
		}

		/// <summary>
		/// Sets this matrix equal to the matrix encoded in the array.
		/// </summary>
		/// <param name="numRows"> The number of rows. </param>
		/// <param name="numCols"> The number of columns. </param>
		/// <param name="rowMajor"> If the array is encoded in a row-major or a column-major format. </param>
		/// <param name="data"> The formatted 1D array. Not modified. </param>
		public virtual void set(int numRows, int numCols, bool rowMajor, params double[] data)
		{
			reshape(numRows, numCols);
			int length = numRows * numCols;

			if (length > this.data.Count())
			{
				throw new System.ArgumentException("The length of this matrix's data array is too small.");
			}

			if (rowMajor)
			{
				Array.Copy(data, 0, this.data, 0, length);
			}
			else
			{
				int index = 0;
				for (int i = 0; i < numRows; i++)
				{
					for (int j = 0; j < numCols; j++)
					{
						this.data[index++] = data[j * numRows + i];
					}
				}
			}
		}

		/// <summary>
		/// Sets all elements equal to zero.
		/// </summary>
		public virtual void zero()
		{
			Arrays.Fill(data, 0, NumElements, 0.0);
		}

		/// <summary>
		/// Sets all elements equal to the specified value.
		/// </summary>
		public virtual void fill(double value)
		{
			Arrays.Fill(data, 0, NumElements, value);
		}

		/// <summary>
		/// Creates and returns a matrix which is idential to this one.
		/// </summary>
		/// <returns> A new identical matrix. </returns>
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @SuppressWarnings({"unchecked"}) @Override public DMatrixRMaj copy()
		public virtual DMatrixRMaj copy()
		{
			return new DMatrixRMaj(this);
		}

		public virtual void setTo(Matrix original)
		{
			DMatrix m = (DMatrix)original;

			reshape(original.NumRows, original.NumCols);

			if (original is DMatrixRMaj)
			{
				// do a faster copy if its of type DMatrixRMaj
				Array.Copy(((DMatrixRMaj)m).data, 0, data, 0, numRows * numCols);
			}
			else
			{
				int index = 0;
				for (int i = 0; i < numRows; i++)
				{
					for (int j = 0; j < numCols; j++)
					{
						data[index++] = m.get(i, j);
					}
				}
			}
		}

		/// <summary>
		/// <para>
		/// Converts the array into a string format for display purposes.
		/// The conversion is done using <seealso cref="MatrixIO.print(java.io.PrintStream, DMatrix)"/>.
		/// </para>
		/// </summary>
		/// <returns> String representation of the matrix. </returns>
		public override string ToString()
		{
			//MemoryStream stream = new MemoryStream();
			//MatrixIO.print(new PrintStream(stream), this);

			//return stream.ToString();
			return "";
		}

		public T createLike<T>()
		{
			throw new NotImplementedException();
		}

		public T create<T>(int numRows, int numCols)
		{
			throw new NotImplementedException();
		}
		override public MatrixType Type
		{
			get
			{
				return MatrixType.DDRM();
			}
		}

		/// <summary>
		/// Assigns this matrix using a 2D array representation </summary>
		/// <param name="input"> 2D array which this matrix will be set to </param>
		public virtual void set(double[][] input)
		{
			DConvertArrays.convert(input, this);
		}

        public override void reshape(int numRows, int numCols, bool saveValues)
		{
			UtilEjml.checkTooLarge(numRows, numCols);
			if (data.Count() < numRows * numCols)
			{
				double[] d = new double[numRows * numCols];

				if (saveValues)
				{
					Array.Copy(data, 0, d, 0, NumElements);
				}

				this.data = d;
			}
			this.numRows = numRows;
			this.numCols = numCols;
		}
    }

}