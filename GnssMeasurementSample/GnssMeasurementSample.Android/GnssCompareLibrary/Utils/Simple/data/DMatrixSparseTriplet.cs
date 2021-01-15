using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data
{
	/// <summary>
	/// A sparse matrix format that is designed to act as an intermediate step for other matrix types. Constructing
	/// <seealso cref="DMatrixSparseCSC"/> from scratch is difficult, but if a triplet is first defined then it is much easier.
	/// Inside this class elements are stored in an unsorted list. Adding an element to the list with <seealso cref="addItem(int, int, double)"/>
	/// is an O(1) operation but reading a specific element is O(N) operation, making it impractical for operations like
	/// matrix multiplications.
	/// 
	/// @author Peter Abeles
	/// </summary>
	public class DMatrixSparseTriplet : DMatrixSparse
	{
		/// <summary>
		/// Storage for row and column coordinate for non-zero elements
		/// </summary>
		public IGrowArray nz_rowcol = new IGrowArray();
		/// <summary>
		/// Storage for value of a non-zero element
		/// </summary>
		public DGrowArray nz_value = new DGrowArray();

		/// <summary>
		/// Number of non-zero elements in this matrix
		/// </summary>
		public int nz_length;

		/// <summary>
		/// Number of rows in the matrix
		/// </summary>
		public int numRows;
		/// <summary>
		/// Number of columns in the matrix
		/// </summary>
		public int numCols;

		public DMatrixSparseTriplet()
		{
		}

		/// <param name="numRows"> Number of rows in the matrix </param>
		/// <param name="numCols"> Number of columns in the matrix </param>
		/// <param name="initLength"> Initial maximum length of data array. </param>
		public DMatrixSparseTriplet(int numRows, int numCols, int initLength)
		{
			nz_rowcol.reshape(initLength * 2);
			nz_value.reshape(initLength);
			this.numRows = numRows;
			this.numCols = numCols;
		}

		public DMatrixSparseTriplet(DMatrixSparseTriplet orig)
		{
			To = orig;
		}

		public virtual void reset()
		{
			nz_length = 0;
			numRows = 0;
			numCols = 0;
		}

		public virtual void reshape(int numRows, int numCols)
		{
			this.numRows = numRows;
			this.numCols = numCols;
			this.nz_length = 0;
		}

		public virtual void reshape(int numRows, int numCols, int arrayLength)
		{
			reshape(numRows, numCols);
			nz_rowcol.reshape(arrayLength * 2);
			nz_value.reshape(arrayLength);
		}

		/// <summary>
		/// <para>Adds a triplet of (row,vol,value) to the end of the list. This is the preferred way to add elements
		/// into this array type as it has a runtime complexity of O(1).</para>
		/// 
		////* * One potential problem with @using this function instead of
		///{
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @link #set(int, int, double)
		///# set(int, int, double)
		///}
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: is that it does * not check to see if a(row,col) has already been assigned a value.If a(row,col) is defined multiple times * how this is handled is not defined. * * @param row Row the element belongs in * @param col Column the element belongs in * @param value The value of the element */ public void addItem(int row, int col, double value)
		///@is that it does * not check to see if a(row, col) has already been assigned a value.If a(row, col) @is defined multiple times* how this @is handled @is not defined. ** row Row the element belongs @in* col Column the element belongs @in* value The value of the element* / 
		public void addItem(int row, int col, double value)
		{
			if (nz_length == nz_value.data.Count())
			{
				int amount = nz_length + 10;
				nz_value.growInternal(amount);
				nz_rowcol.growInternal(amount * 2);
			}
			nz_value.data[nz_length] = value;
			nz_rowcol.data[nz_length * 2] = row;
			nz_rowcol.data[nz_length * 2 + 1] = col;
			nz_length += 1;
		}

		/// <summary>
		/// Adds a triplet of (row,vol,value) to the end of the list and performs a bounds check to make
		/// sure it is a legal value.
		/// </summary>
		/// <param name="row"> Row the element belongs in </param>
		/// <param name="col"> Column the element belongs in </param>
		/// <param name="value"> The value of the element </param>
		/// <seealso cref= #addItem(int, int, double) </seealso>
		public void addItemCheck(int row, int col, double value)
		{
			if (row < 0 || col < 0 || row >= numRows || col >= numCols)
			{
				throw new System.ArgumentException("Out of bounds. (" + row + "," + col + ") " + numRows + " " + numCols);
			}
			if (nz_length == nz_value.data.Count())
			{
				int amount = nz_length + 10;
				nz_value.growInternal(amount);
				nz_rowcol.growInternal(amount * 2);
			}
			nz_value.data[nz_length] = value;
			nz_rowcol.data[nz_length * 2] = row;
			nz_rowcol.data[nz_length * 2 + 1] = col;
			nz_length += 1;
		}

		/// <summary>
		/// Sets the element's value at (row,col). It first checks to see if the element already has a value and if it
		/// does that value is changed. As a result this operation is O(N), where N is the number of elements in the matrix.
		/// </summary>
		/// <param name="row"> Matrix element's row index. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <param name="value"> value of element. </param>
		/// <seealso cref= #addItem(int, int, double) For a faster but less "safe" alternative </seealso>
		public void set(int row, int col, double value)
		{
			if (row < 0 || row >= numRows || col < 0 || col >= numCols)
			{
				throw new System.ArgumentException("Outside of matrix bounds");
			}

			unsafe_set(row, col, value);
		}

		/// <summary>
		/// Same as <seealso cref="set(int, int, double)"/> but does not check to see if row and column are within bounds.
		/// </summary>
		/// <param name="row"> Matrix element's row index. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <param name="value"> value of element. </param>
		public void unsafe_set(int row, int col, double value)
		{
			int index = nz_index(row, col);
			if (index < 0)
			{
				addItem(row, col, value);
			}
			else
			{
				nz_value.data[index] = value;
			}
		}

		public virtual int NumElements
		{
			get
			{
				return nz_length;
			}
		}

		/// <summary>
		/// Searches the list to see if the element at (row,col) has been assigned. The worst case runtime for this
		/// operation is O(N), where N is the number of elements in the matrix.
		/// </summary>
		/// <param name="row"> Matrix element's row index. </param>
		/// <param name="col"> Matrix element's column index. </param>
		/// <returns> Value at (row,col) </returns>
		public double get(int row, int col)
		{
			if (row < 0 || row >= numRows || col < 0 || col >= numCols)
			{
				throw new System.ArgumentException("Outside of matrix bounds");
			}

			return unsafe_get(row, col);
		}

		/// 
		
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: * Searches the list to see if the element at(row,col) has been assigned.The worst case runtime for this * operation is O(N), where N is the number of elements in the matrix. * * @param row Matrix element's row index. * @param col Matrix element's column index. * @param fallBackValue Value to return, if the element is not assigned * return Value at(row,col) or the fallBackValue, if the element is not assigned. */ @Override public double get(int row, int col, double fallBackValue) { if(row < 0 || row >= numRows || col < 0 || col >= numCols) throw new IllegalArgumentException("Outside of matrix bounds"); return unsafe_get(row, col, fallBackValue); } @Override public double unsafe_get(int row, int col) { int index = nz_index(row, col); if(index < 0) return 0; else return nz_value.data[index]; } @Override public double unsafe_get(int row, int col, double fallBackValue) { int index = nz_index(row, col); if(index < 0) return fallBackValue; else return nz_value.data[index]; } public int nz_index(int row, int col) { int end = nz_length*2; for(int i = 0; i < end; i += 2) { int r = nz_rowcol.data[i]; int c = nz_rowcol.data[i + 1]; if(r == row && c == col) return i/2; } return -1; } public int getLength() { return nz_length; } @Override public int getNumRows() { return numRows; } @Override public int getNumCols() { return numCols; } @Override public <T extends Matrix> T copy() { return(T)new DMatrixSparseTriplet(this); } @Override public <T extends Matrix> T createLike() { return(T)new DMatrixSparseTriplet(numRows, numCols, nz_length); } @Override public <T extends Matrix> T create(int numRows, int numCols) { return(T)new DMatrixSparseTriplet(numRows, numCols, 1); } @Override public void setTo(Matrix original) { DMatrixSparseTriplet orig = (DMatrixSparseTriplet)original; reshape(orig.numRows, orig.numCols); this.nz_rowcol.setTo(orig.nz_rowcol); this.nz_value.setTo(orig.nz_value); this.nz_length = orig.nz_length; } @Override public void shrinkArrays() { if(nz_length < nz_value.length) { double[] vtmp = new double[nz_length]; int[] rctmp = new int[nz_length*2]; System.arraycopy(this.nz_value.data, 0, vtmp, 0, vtmp.length); System.arraycopy(this.nz_rowcol.data, 0, rctmp, 0, rctmp.length); nz_value.data = vtmp; nz_rowcol.data = rctmp; } } @Override public void remove(int row, int col) { int where = nz_index(row, col);
		///* Searches the list to see if the element at(row, col) has been assigned.The worst case runtime for this * operation @is O(N), where N @is the number of elements @in the matrix. * * row Matrix element's row index. * col Matrix element's column index. * fallBackValue Value to return, if the element @is not assigned* Value at(row, col) or the fallBackValue, if the element @is not assigned. */ 
		public double get(int row, int col, double fallBackValue) { if (row < 0 || row >= numRows || col < 0 || col >= numCols) throw new System.ArgumentException("Outside of matrix bounds"); 
			return unsafe_get(row, col, fallBackValue); 
		}
		public double unsafe_get(int row, int col) { int index = nz_index(row, col); if (index < 0) return 0; else return nz_value.data[index]; }
		public double unsafe_get(int row, int col, double fallBackValue) { int index = nz_index(row, col); if (index < 0) return fallBackValue; else return nz_value.data[index]; }
		public int nz_index(int row, int col)
		{
			int end = nz_length * 2; for (int i = 0; i < end; i += 2) { int r = nz_rowcol.data[i]; int c = nz_rowcol.data[i + 1]; if (r == row && c == col) return i / 2; }
			return -1;
		}
		public virtual int Length
		{
			get
			{
				return nz_length;
			}
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
		public void setTo(Matrix original) { DMatrixSparseTriplet orig = (DMatrixSparseTriplet)original; reshape(orig.numRows, orig.numCols);
			this.nz_rowcol.To = orig.nz_rowcol;
			this.nz_value.To = orig.nz_value;
			this.nz_length = orig.nz_length; 
		}
		public void shrinkArrays()
		{
			if (nz_length < nz_value.length()) { 
				double[] vtmp = new double[nz_length]; 
				int[] rctmp = new int[nz_length * 2]; 
				Array.Copy(this.nz_value.data, 0, vtmp, 0, vtmp.Count()); Array.Copy(this.nz_rowcol.data, 0, rctmp, 0, rctmp.Count()); 
				nz_value.data = vtmp; nz_rowcol.data = rctmp; 
			}
		}
		public void remove(int row, int col)
		{
			int where = nz_index(row, col);

			if (where >= 0)
			{

				nz_length -= 1;
				for (int i = where; i < nz_length; i++)
				{
					nz_value.data[i] = nz_value.data[i + 1];
				}
				int end = nz_length * 2;
				for (int i = where * 2; i < end; i += 2)
				{
					nz_rowcol.data[i] = nz_rowcol.data[i + 2];
					nz_rowcol.data[i + 1] = nz_rowcol.data[i + 3];
				}
			}
		}

		public bool isAssigned(int row, int col)
		{
			return nz_index(row, col) >= 0;
		}

		public void zero()
		{
			nz_length = 0;
		}

		public void print()
		{
			print(MatrixIO.DEFAULT_FLOAT_FORMAT);
		}

		public void print(string format)
		{
			Console.WriteLine("Type = " + this.GetType().Name + " , rows = " + numRows + " , cols = " + numCols + " , nz_length = " + nz_length);
			for (int row = 0; row < numRows; row++)
			{
				for (int col = 0; col < numCols; col++)
				{
					int index = nz_index(row, col);
					if (index >= 0)
					{
						//System.out.printf(format, nz_value.data[index]);
					}
					else
					{
						Console.Write("   *  ");
					}
					if (col != numCols - 1)
					{
						Console.Write(" ");
					}
				}
				Console.WriteLine();
			}
		}

		public void printNonZero()
		{
			Console.WriteLine("Type = " + this.GetType().Name + " , rows = " + numRows + " , cols = " + numCols + " , nz_length = " + nz_length);

			for (int i = 0; i < nz_length; i++)
			{
				int row = nz_rowcol.data[i * 2];
				int col = nz_rowcol.data[i * 2 + 1];
				double value = nz_value.data[i];
				Console.Write("{0:D} {1:D} {2:F}\n", row, col, value);
			}
		}

		public virtual MatrixType Type
		{
			get
			{
				return MatrixType.DTRIPLET();
			}
		}

        private class MyIterator : IEnumerator<CoordinateRealValue>
		{
			int index = 0;
			public CoordinateRealValue Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
			{
			}

            public bool MoveNext()
			{
				//index++;
				//return (index < _people.Length);
				throw new NotImplementedException();
            }

            public void Reset()
            {
				index = 0;

			}
        }

        public IEnumerator<CoordinateRealValue> createCoordinateIterator()
		{
			return new MyIterator();
			/*
			return new IEnumerator<CoordinateRealValue>() {
				readonly CoordinateRealValue coordinate = new CoordinateRealValue();
				int index = 0;

				@Override
				public boolean hasNext()
				{
					return index < nz_length;
				}

				@Override
				public CoordinateRealValue next()
				{
					coordinate.row = nz_rowcol.data[index * 2];
					coordinate.col = nz_rowcol.data[index * 2 + 1];
					coordinate.value = nz_value.data[index];
					index++;
					return coordinate;
				}
			};
			*/
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

        public virtual int NonZeroCount
		{
			get
			{
				return nz_length;
			}
		}

        public int NonZeroLength => throw new NotImplementedException();

        public Matrix To { set => throw new NotImplementedException(); }
    }


}