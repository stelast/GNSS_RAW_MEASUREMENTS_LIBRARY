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
	/// Fixed sized vector with 2 elements.  Can represent a 2 x 1 or 1 x 2 matrix, context dependent.
	/// 
	/// <para>DO NOT MODIFY. Automatically generated code created by GenerateMatrixFixedN</para>
	/// 
	/// @author Peter Abeles
	/// </summary>
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @Generated("org.ejml.data.GenerateMatrixFixedN") public class DMatrix2 implements DMatrixFixed
	[Serializable]
	public class DMatrix2 : DMatrixFixed
	{
		public double a1, a2;

		public DMatrix2()
		{
		}

		public DMatrix2(double a1, double a2)
		{
			this.a1 = a1;
			this.a2 = a2;
		}

		public DMatrix2(DMatrix2 o)
		{
			this.a1 = o.a1;
			this.a2 = o.a2;
		}

		public virtual void zero()
		{
			a1 = 0.0;
			a2 = 0.0;
		}

		public virtual void setTo(double a1, double a2)
		{
			this.a1 = a1;
			this.a2 = a2;
		}

		public virtual void setTo(int offset, double[] array)
		{
			this.a1 = array[offset + 0];
			this.a2 = array[offset + 1];
		}

		public virtual double get(int row, int col)
		{
			return unsafe_get(row, col);
		}
		public virtual double unsafe_get(int row, int col)
		{
			if (row != 0 && col != 0)
			{
				throw new System.ArgumentException("Row or column must be zero since this is a vector");
			}

			int w = Math.Max(row, col);

			if (w == 0)
			{
				return a1;
			}
			else if (w == 1)
			{
				return a2;
			}
			else
			{
				throw new System.ArgumentException("Out of range.  " + w);
			}
		}

		public virtual void set(int row, int col, double val)
		{
			unsafe_set(row, col, val);
		}

		public virtual void unsafe_set(int row, int col, double val)
		{
			if (row != 0 && col != 0)
			{
				throw new System.ArgumentException("Row or column must be zero since this is a vector");
			}

			int w = Math.Max(row, col);

			if (w == 0)
			{
				a1 = val;
			}
			else if (w == 1)
			{
				a2 = val;
			}
			else
			{
				throw new System.ArgumentException("Out of range.  " + w);
			}
		}

		public virtual Matrix To
		{
			set
			{
				DMatrix m = (DMatrix)value;

				if (m.NumCols == 1 && m.NumRows == 2)
				{
					a1 = m.get(0, 0);
					a2 = m.get(1, 0);
				}
				else if (m.NumRows == 1 && m.NumCols == 2)
				{
					a1 = m.get(0, 0);
					a2 = m.get(0, 1);
				}
				else
				{
					throw new System.ArgumentException("Incompatible shape");
				}
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
				return 1;
			}
		}
		public virtual int NumElements
		{
			get
			{
				return 2;
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