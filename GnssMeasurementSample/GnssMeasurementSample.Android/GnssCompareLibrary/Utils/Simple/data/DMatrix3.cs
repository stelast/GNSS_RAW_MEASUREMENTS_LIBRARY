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
    //@Generated("org.ejml.data.GenerateMatrixFixedNxN")
    public class DMatrix3 : DMatrixFixed
    {
        public double a1, a2, a3;

        public DMatrix3() { }

        public DMatrix3(double a1, double a2, double a3)
        {
            this.a1 = a1;
            this.a2 = a2;
            this.a3 = a3;
        }

        public DMatrix3(DMatrix3 o)
        {
            this.a1 = o.a1;
            this.a2 = o.a2;
            this.a3 = o.a3;
        }
        public virtual void zero()
        {
            a1 = 0.0;
            a2 = 0.0;
            a3 = 0.0;
        }

        public void setTo(double a1, double a2, double a3)
        {
            this.a1 = a1;
            this.a2 = a2;
            this.a3 = a3;
        }

        public void setTo(int offset, double[] array)
        {
            this.a1 = array[offset + 0];
            this.a2 = array[offset + 1];
            this.a3 = array[offset + 2];
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

        public virtual int NumRows
        {
            get
            {
                return 3;
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
                return 3;
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


        public virtual MatrixType Type
        {
            get
            {
                return MatrixType.UNSPECIFIED();
            }
        }
        public virtual Matrix To
        {
            set
            {
                DMatrix m = (DMatrix)value;

                if (m.NumCols == 1 && m.NumRows == 3)
                {
                    a1 = m.get(0, 0);
                    a2 = m.get(1, 0);
                    a3 = m.get(2, 0);
                }
                else if (m.NumRows == 1 && m.NumCols == 3)
                {
                    a1 = m.get(0, 0);
                    a2 = m.get(0, 1);
                    a3 = m.get(0, 2);
                }
                else
                {
                    throw new ArgumentException("Incompatible shape");
                }
            }
        } 
        
        public T copy<T>() where T : Matrix
        {
            //return (T)(new DMatrix3(this));
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