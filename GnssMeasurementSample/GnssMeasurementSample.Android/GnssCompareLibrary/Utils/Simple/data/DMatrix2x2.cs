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
	/// Fixed sized 2 by DMatrix2x2 matrix.  The matrix is stored as class variables for very fast read/write.  aXY is the
	/// value of row = X and column = Y.
	/// 
	/// <para>DO NOT MODIFY. Automatically generated code created by GenerateMatrixFixedNxN</para>
	/// 
	/// @author Peter Abeles
	/// </summary>
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @Generated("org.ejml.data.GenerateMatrixFixedNxN") public class DMatrix2x2 implements DMatrixFixed
	[Serializable]
	public class DMatrix2x2 : DMatrixFixed
	{

		public double a11, a12;
		public double a21, a22;

		public DMatrix2x2()
		{
		}

		public DMatrix2x2(double a11, double a12, double a21, double a22)
		{
			this.a11 = a11;
			this.a12 = a12;
			this.a21 = a21;
			this.a22 = a22;
		}

		public DMatrix2x2(DMatrix2x2 o)
		{
			this.a11 = o.a11;
			this.a12 = o.a12;
			this.a21 = o.a21;
			this.a22 = o.a22;
		}

		public virtual void zero()
		{
			a11 = 0.0;
			a12 = 0.0;
			a21 = 0.0;
			a22 = 0.0;
		}

		public virtual void setTo(double a11, double a12, double a21, double a22)
		{
			this.a11 = a11;
			this.a12 = a12;
			this.a21 = a21;
			this.a22 = a22;
		}

		public virtual void setTo(int offset, double[] a)
		{
			this.a11 = a[offset + 0];
			this.a12 = a[offset + 1];
			this.a21 = a[offset + 2];
			this.a22 = a[offset + 3];
		}

		public virtual double get(int row, int col)
		{
			return unsafe_get(row, col);
		}

		public virtual double unsafe_get(int row, int col)
		{
			if (row == 0)
			{
				if (col == 0)
				{
					return a11;
				}
				else if (col == 1)
				{
					return a12;
				}
			}
			else if (row == 1)
			{
				if (col == 0)
				{
					return a21;
				}
				else if (col == 1)
				{
					return a22;
				}
			}
			throw new System.ArgumentException("Row and/or column out of range. " + row + " " + col);
		}

		public virtual void set(int row, int col, double val)
		{
			unsafe_set(row, col, val);
		}

		public virtual void unsafe_set(int row, int col, double val)
		{
			if (row == 0)
			{
				if (col == 0)
				{
					a11 = val;
					return;
				}
				else if (col == 1)
				{
					a12 = val;
					return;
				}
			}
			else if (row == 1)
			{
				if (col == 0)
				{
					a21 = val;
					return;
				}
				else if (col == 1)
				{
					a22 = val;
					return;
				}
			}
			throw new System.ArgumentException("Row and/or column out of range. " + row + " " + col);
		}

		public virtual Matrix To
		{
			set
			{
				if (value.NumCols != 2 || value.NumRows != 2)
				{
					throw new System.ArgumentException("Rows and/or columns do not match");
				}
				DMatrix m = (DMatrix)value;

				a11 = m.get(0, 0);
				a12 = m.get(0, 1);
				a21 = m.get(1, 0);
				a22 = m.get(1, 1);
			}
		}

		public virtual int NumRows
		{
			get
			{
				return 2;
			}
		}
		public virtual int NumCols
		{
			get
			{
				return 2;
			}
		}
		public virtual int NumElements
		{
			get
			{
				return 4;
			}
		}

		public virtual void print()
		{
			MatrixIO.printFancy(this, MatrixIO.DEFAULT_LENGTH);
		}

		public virtual void print(string format)
		{
			MatrixIO.print(this, format);
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