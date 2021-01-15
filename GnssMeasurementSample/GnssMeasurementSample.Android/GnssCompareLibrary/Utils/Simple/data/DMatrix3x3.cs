﻿using Android.App;
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
    /**
     * Fixed sized 3 by DMatrix3x3 matrix.  The matrix is stored as class variables for very fast read/write.  aXY is the
     * value of row = X and column = Y.
     *
     * <p>DO NOT MODIFY. Automatically generated code created by GenerateMatrixFixedNxN</p>
     *
     * @author Peter Abeles
     */
    //@Generated("org.ejml.data.GenerateMatrixFixedNxN")
    public class DMatrix3x3 : DMatrixFixed
    {

        public double a11, a12, a13;
        public double a21, a22, a23;
        public double a31, a32, a33;


        public DMatrix3x3() { }

        public DMatrix3x3(double a11, double a12, double a13,
                           double a21, double a22, double a23,
                           double a31, double a32, double a33)
        {
            this.a11 = a11; this.a12 = a12; this.a13 = a13;
            this.a21 = a21; this.a22 = a22; this.a23 = a23;
            this.a31 = a31; this.a32 = a32; this.a33 = a33;
        }

        public DMatrix3x3(DMatrix3x3 o)
        {
            this.a11 = o.a11; this.a12 = o.a12; this.a13 = o.a13;
            this.a21 = o.a21; this.a22 = o.a22; this.a23 = o.a23;
            this.a31 = o.a31; this.a32 = o.a32; this.a33 = o.a33;
        }

        public void zero()
        {
            a11 = 0.0; a12 = 0.0; a13 = 0.0;
            a21 = 0.0; a22 = 0.0; a23 = 0.0;
            a31 = 0.0; a32 = 0.0; a33 = 0.0;
        }

        public void setTo(double a11, double a12, double a13,
                           double a21, double a22, double a23,
                           double a31, double a32, double a33)
        {
            this.a11 = a11; this.a12 = a12; this.a13 = a13;
            this.a21 = a21; this.a22 = a22; this.a23 = a23;
            this.a31 = a31; this.a32 = a32; this.a33 = a33;
        }

        public void setTo(int offset, double[] a)
        {
            this.a11 = a[offset + 0]; this.a12 = a[offset + 1]; this.a13 = a[offset + 2];
            this.a21 = a[offset + 3]; this.a22 = a[offset + 4]; this.a23 = a[offset + 5];
            this.a31 = a[offset + 6]; this.a32 = a[offset + 7]; this.a33 = a[offset + 8];
        }

        public double get(int row, int col)
        {
            return unsafe_get(row, col);
        }

        public double unsafe_get(int row, int col)
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
                else if (col == 2)
                {
                    return a13;
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
                else if (col == 2)
                {
                    return a23;
                }
            }
            else if (row == 2)
            {
                if (col == 0)
                {
                    return a31;
                }
                else if (col == 1)
                {
                    return a32;
                }
                else if (col == 2)
                {
                    return a33;
                }
            }
            throw new ArgumentException("Row and/or column out of range. " + row + " " + col);
        }

        public void set(int row, int col, double val)
        {
            unsafe_set(row, col, val);
        }

        public void unsafe_set(int row, int col, double val)
        {
            if (row == 0)
            {
                if (col == 0)
                {
                    a11 = val; return;
                }
                else if (col == 1)
                {
                    a12 = val; return;
                }
                else if (col == 2)
                {
                    a13 = val; return;
                }
            }
            else if (row == 1)
            {
                if (col == 0)
                {
                    a21 = val; return;
                }
                else if (col == 1)
                {
                    a22 = val; return;
                }
                else if (col == 2)
                {
                    a23 = val; return;
                }
            }
            else if (row == 2)
            {
                if (col == 0)
                {
                    a31 = val; return;
                }
                else if (col == 1)
                {
                    a32 = val; return;
                }
                else if (col == 2)
                {
                    a33 = val; return;
                }
            }
            throw new ArgumentException("Row and/or column out of range. " + row + " " + col);
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
                return 3;
            }
        }
        public virtual int NumElements
        {
            get
            {
                return 9;
            }
        }

        public virtual Matrix To
        {
            set
            {
                if (value.NumCols != 3 || value.NumRows != 3)
                    throw new ArgumentException("Rows and/or columns do not match");
                DMatrix m = (DMatrix)value;

                a11 = m.get(0, 0);
                a12 = m.get(0, 1);
                a13 = m.get(0, 2);
                a21 = m.get(1, 0);
                a22 = m.get(1, 1);
                a23 = m.get(1, 2);
                a31 = m.get(2, 0);
                a32 = m.get(2, 1);
                a33 = m.get(2, 2);
            }
        }

        public virtual MatrixType Type
        {
            get
            {
                return MatrixType.UNSPECIFIED();
            }
        }

        public Matrix copy<T>()
        {
            return (Matrix)new DMatrix3x3(this);
        }

        public void print()
        {
            MatrixIO.printFancy(this, MatrixIO.DEFAULT_LENGTH);
        }

        public void print(String format)
        {
            MatrixIO.print(this, format);
        }

        public DMatrix3x3 createLike<T>()
        {
            return (DMatrix3x3)new DMatrix3x3();
        }

        public MatrixType getType() { return MatrixType.UNSPECIFIED(); }

        T Matrix.copy<T>()
        {
            throw new NotImplementedException();
        }

        T Matrix.createLike<T>()
        {
            throw new NotImplementedException();
        }

        public T create<T>(int numRows, int numCols) where T : Matrix
        {
            throw new NotImplementedException();
        }
    }

}