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
    public class DMatrix6 : DMatrixFixed
    {
        public double a1, a2, a3, a4, a5, a6;

        public DMatrix6() { }

        public DMatrix6(double a1, double a2, double a3, double a4, double a5, double a6)
        {
            this.a1 = a1;
            this.a2 = a2;
            this.a3 = a3;
            this.a4 = a4;
            this.a5 = a5;
            this.a6 = a6;
        }

        public DMatrix6(DMatrix6 o)
        {
            this.a1 = o.a1;
            this.a2 = o.a2;
            this.a3 = o.a3;
            this.a4 = o.a4;
            this.a5 = o.a5;
            this.a6 = o.a6;
        }

        public void setTo(double a1, double a2, double a3, double a4, double a5, double a6)
        {
            this.a1 = a1;
            this.a2 = a2;
            this.a3 = a3;
            this.a4 = a4;
            this.a5 = a5;
            this.a6 = a6;
        }

        public void setTo(int offset, double[] array)
        {
            this.a1 = array[offset + 0];
            this.a2 = array[offset + 1];
            this.a3 = array[offset + 2];
            this.a4 = array[offset + 3];
            this.a5 = array[offset + 4];
            this.a6 = array[offset + 5];
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
            else if (w == 2)
            {
                return a3;
            }
            else if (w == 3)
            {
                return a4;
            }
            else if (w == 4)
            {
                return a5;
            }
            else if (w == 5)
            {
                return a6;
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
            else if (w == 2)
            {
                a3 = val;
            }
            else if (w == 3)
            {
                a4 = val;
            }
            else if (w == 4)
            {
                a5 = val;
            }
            else if (w == 5)
            {
                a6 = val;
            }
            throw new System.ArgumentException("Out of range.  " + w);
        }
        public int NumElements => throw new NotImplementedException();

        public int NumRows => throw new NotImplementedException();

        public int NumCols => throw new NotImplementedException();

        public virtual Matrix To
        {
            set
            {
                DMatrix m = (DMatrix)value;

                if (m.NumCols == 1 && m.NumRows == 6)
                {
                    a1 = m.get(0, 0);
                    a2 = m.get(1, 0);
                    a3 = m.get(2, 0);
                    a4 = m.get(3, 0);
                    a5 = m.get(4, 0);
                    a6 = m.get(5, 0);
                }
                else if (m.NumRows == 1 && m.NumCols == 6)
                {
                    a1 = m.get(0, 0);
                    a2 = m.get(0, 1);
                    a3 = m.get(0, 2);
                    a4 = m.get(0, 3);
                    a5 = m.get(0, 4);
                    a6 = m.get(0, 5);
                }
                else
                {
                    throw new ArgumentException("Incompatible shape");
                }
            }
        }

        public virtual MatrixType Type
        {
            get
            {
                return MatrixType.UNSPECIFIED();
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

        public void zero()
        {
            a1 = 0.0;
            a2 = 0.0;
            a3 = 0.0;
            a4 = 0.0;
            a5 = 0.0;
            a6 = 0.0;
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
    }
}