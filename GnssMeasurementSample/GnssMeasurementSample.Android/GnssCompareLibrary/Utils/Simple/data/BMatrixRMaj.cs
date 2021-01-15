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
	/// Dense matrix composed of boolean values
	/// 
	/// @author Peter Abeles
	/// </summary>
	[Serializable]
	public class BMatrixRMaj : ReshapeMatrix
	{
		/// <summary>
		/// 1D row-major array for storing theboolean matrix
		/// </summary>
		public bool[] data;
		/// <summary>
		/// Number of rows in the matrix.
		/// </summary>
		public int numRows;
		/// <summary>
		/// Number of columns in the matrix.
		/// </summary>
		public int numCols;

		public BMatrixRMaj(int numRows, int numCols)
		{
			data = new bool[numRows * numCols];
			this.numRows = numRows;
			this.numCols = numCols;
		}

		public virtual int NumElements
		{
			get
			{
				return numRows * numCols;
			}
		}

		public virtual int getIndex(int row, int col)
		{
			return row * numCols + col;
		}

		/// <summary>
		/// Sets every element in the matrix to the specified value </summary>
		/// <param name="value"> new value of every element </param>
		public virtual void fill(bool value)
		{
			Arrays.Fill(data, 0, NumElements, value);
		}

		public virtual bool get(int index)
		{
			return data[index];
		}

		public virtual bool get(int row, int col)
		{
			if (!isInBounds(row, col))
			{
				throw new System.ArgumentException("Out of matrix bounds. " + row + " " + col);
			}
			return data[row * numCols + col];
		}

		public virtual void set(int row, int col, bool value)
		{
			if (!isInBounds(row, col))
			{
				throw new System.ArgumentException("Out of matrix bounds. " + row + " " + col);
			}
			data[row * numCols + col] = value;
		}

		public virtual bool unsafe_get(int row, int col)
		{
			return data[row * numCols + col];
		}

		public virtual void unsafe_set(int row, int col, bool value)
		{
			data[row * numCols + col] = value;
		}

		/// <summary>
		/// Determines if the specified element is inside the bounds of the Matrix.
		/// </summary>
		/// <param name="row"> The element's row. </param>
		/// <param name="col"> The element's column. </param>
		/// <returns> True if it is inside the matrices bound, false otherwise. </returns>
		public virtual bool isInBounds(int row, int col)
		{
			return (col >= 0 && col < numCols && row >= 0 && row < numRows);
		}

		/// <summary>
		/// Returns the total number of elements which are true. </summary>
		/// <returns> number of elements which are set to true </returns>
		public virtual int sum()
		{
			int total = 0;
			int N = NumElements;
			for (int i = 0; i < N; i++)
			{
				if (data[i])
				{
					total += 1;
				}
			}
			return total;
		}

		public virtual void reshape(int numRows, int numCols)
		{
			int N = numRows * numCols;
			if (data.Length < N)
			{
				data = new bool[N];
			}
			this.numRows = numRows;
			this.numCols = numCols;
		}

		public virtual int NumRows
		{
			get
			{
				return numRows;
			}
		}

		public virtual int NumCols
		{
			get
			{
				return numCols;
			}
		}

		public virtual void zero()
		{
			Arrays.Fill(data, 0, NumElements, false);
		}


		public virtual BMatrixRMaj copy()
		{
			BMatrixRMaj ret = new BMatrixRMaj(numRows, numCols);
			ret.To = this;
			return ret;
		}

		public virtual Matrix To
		{
			set
			{
				BMatrixRMaj orig = (BMatrixRMaj)value;

				reshape(value.NumRows, value.NumCols);
				Array.Copy(orig.data, 0, data, 0, orig.NumElements);
			}
		}

		public virtual void print()
		{
			Console.WriteLine("Type = binary , numRows = " + numRows + " , numCols = " + numCols);
			for (int row = 0; row < numRows; row++)
			{
				for (int col = 0; col < numCols; col++)
				{
					if (get(row, col))
					{
						Console.Write("+");
					}
					else
					{
						Console.Write("-");
					}
				}
				Console.WriteLine();
			}
		}

		public virtual void print(string format)
		{
			print();
		}

		public virtual BMatrixRMaj createLike()
		{
			return new BMatrixRMaj(numRows, numCols);
		}

		public virtual BMatrixRMaj create(int numRows, int numCols)
		{
			return new BMatrixRMaj(numRows, numCols);
		}

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

        public virtual MatrixType Type
		{
			get
			{
				return MatrixType.UNSPECIFIED();
			}
		}
	}
}