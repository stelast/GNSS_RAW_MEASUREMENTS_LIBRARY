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
    public class DMatrix5x5 : DMatrixFixed
    {

        public double a11, a12, a13, a14, a15;
        public double a21, a22, a23, a24, a25;
        public double a31, a32, a33, a34, a35;
        public double a41, a42, a43, a44, a45;
        public double a51, a52, a53, a54, a55;

        public DMatrix5x5() { }

        public DMatrix5x5(double a11, double a12, double a13, double a14, double a15,
                           double a21, double a22, double a23, double a24, double a25,
                           double a31, double a32, double a33, double a34, double a35,
                           double a41, double a42, double a43, double a44, double a45,
                           double a51, double a52, double a53, double a54, double a55)
        {
            this.a11 = a11; this.a12 = a12; this.a13 = a13; this.a14 = a14; this.a15 = a15;
            this.a21 = a21; this.a22 = a22; this.a23 = a23; this.a24 = a24; this.a25 = a25;
            this.a31 = a31; this.a32 = a32; this.a33 = a33; this.a34 = a34; this.a35 = a35;
            this.a41 = a41; this.a42 = a42; this.a43 = a43; this.a44 = a44; this.a45 = a45;
            this.a51 = a51; this.a52 = a52; this.a53 = a53; this.a54 = a54; this.a55 = a55;
        }

        public void setTo(double a11, double a12, double a13, double a14, double a15,
                           double a21, double a22, double a23, double a24, double a25,
                           double a31, double a32, double a33, double a34, double a35,
                           double a41, double a42, double a43, double a44, double a45,
                           double a51, double a52, double a53, double a54, double a55)
        {
            this.a11 = a11; this.a12 = a12; this.a13 = a13; this.a14 = a14; this.a15 = a15;
            this.a21 = a21; this.a22 = a22; this.a23 = a23; this.a24 = a24; this.a25 = a25;
            this.a31 = a31; this.a32 = a32; this.a33 = a33; this.a34 = a34; this.a35 = a35;
            this.a41 = a41; this.a42 = a42; this.a43 = a43; this.a44 = a44; this.a45 = a45;
            this.a51 = a51; this.a52 = a52; this.a53 = a53; this.a54 = a54; this.a55 = a55;
        }

        public void setTo(int offset, double[] a)
        {
            this.a11 = a[offset + 0]; this.a12 = a[offset + 1]; this.a13 = a[offset + 2]; this.a14 = a[offset + 3]; this.a15 = a[offset + 4];
            this.a21 = a[offset + 5]; this.a22 = a[offset + 6]; this.a23 = a[offset + 7]; this.a24 = a[offset + 8]; this.a25 = a[offset + 9];
            this.a31 = a[offset + 10]; this.a32 = a[offset + 11]; this.a33 = a[offset + 12]; this.a34 = a[offset + 13]; this.a35 = a[offset + 14];
            this.a41 = a[offset + 15]; this.a42 = a[offset + 16]; this.a43 = a[offset + 17]; this.a44 = a[offset + 18]; this.a45 = a[offset + 19];
            this.a51 = a[offset + 20]; this.a52 = a[offset + 21]; this.a53 = a[offset + 22]; this.a54 = a[offset + 23]; this.a55 = a[offset + 24];
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
                else if (col == 2)
                {
                    return a13;
                }
                else if (col == 3)
                {
                    return a14;
                }
                else if (col == 4)
                {
                    return a15;
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
                else if (col == 3)
                {
                    return a24;
                }
                else if (col == 4)
                {
                    return a25;
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
                else if (col == 3)
                {
                    return a34;
                }
                else if (col == 4)
                {
                    return a35;
                }
            }
            else if (row == 3)
            {
                if (col == 0)
                {
                    return a41;
                }
                else if (col == 1)
                {
                    return a42;
                }
                else if (col == 2)
                {
                    return a43;
                }
                else if (col == 3)
                {
                    return a44;
                }
                else if (col == 4)
                {
                    return a45;
                }
            }
            else if (row == 4)
            {
                if (col == 0)
                {
                    return a51;
                }
                else if (col == 1)
                {
                    return a52;
                }
                else if (col == 2)
                {
                    return a53;
                }
                else if (col == 3)
                {
                    return a54;
                }
                else if (col == 4)
                {
                    return a55;
                }
            }
            throw new System.ArgumentException("Out of range.  " + row + " " + col);
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
                else if (col == 3)
                {
                    a14 = val; return;
                }
                else if (col == 4)
                {
                    a15 = val; return;
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
                else if (col == 3)
                {
                    a24 = val; return;
                }
                else if (col == 4)
                {
                    a25 = val; return;
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
                else if (col == 3)
                {
                    a34 = val; return;
                }
                else if (col == 4)
                {
                    a35 = val; return;
                }
            }
            else if (row == 3)
            {
                if (col == 0)
                {
                    a41 = val; return;
                }
                else if (col == 1)
                {
                    a42 = val; return;
                }
                else if (col == 2)
                {
                    a43 = val; return;
                }
                else if (col == 3)
                {
                    a44 = val; return;
                }
                else if (col == 4)
                {
                    a45 = val; return;
                }
            }
            else if (row == 4)
            {
                if (col == 0)
                {
                    a51 = val; return;
                }
                else if (col == 1)
                {
                    a52 = val; return;
                }
                else if (col == 2)
                {
                    a53 = val; return;
                }
                else if (col == 3)
                {
                    a54 = val; return;
                }
                else if (col == 4)
                {
                    a55 = val; return;
                }
            }
            throw new System.ArgumentException("Out of range.  " + row + " " + col);
        }
        public int NumElements => throw new NotImplementedException();

        public int NumRows => throw new NotImplementedException();

        public int NumCols => throw new NotImplementedException();

        public virtual Matrix To
        {
            set
            {
                if (value.NumCols != 5 || value.NumRows != 5)
                    throw new ArgumentException("Rows and/or columns do not match");
                DMatrix m = (DMatrix)value;

                a11 = m.get(0, 0);
                a12 = m.get(0, 1);
                a13 = m.get(0, 2);
                a14 = m.get(0, 3);
                a15 = m.get(0, 4);
                a21 = m.get(1, 0);
                a22 = m.get(1, 1);
                a23 = m.get(1, 2);
                a24 = m.get(1, 3);
                a25 = m.get(1, 4);
                a31 = m.get(2, 0);
                a32 = m.get(2, 1);
                a33 = m.get(2, 2);
                a34 = m.get(2, 3);
                a35 = m.get(2, 4);
                a41 = m.get(3, 0);
                a42 = m.get(3, 1);
                a43 = m.get(3, 2);
                a44 = m.get(3, 3);
                a45 = m.get(3, 4);
                a51 = m.get(4, 0);
                a52 = m.get(4, 1);
                a53 = m.get(4, 2);
                a54 = m.get(4, 3);
                a55 = m.get(4, 4);
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
            a11 = 0.0; a12 = 0.0; a13 = 0.0; a14 = 0.0; a15 = 0.0;
            a21 = 0.0; a22 = 0.0; a23 = 0.0; a24 = 0.0; a25 = 0.0;
            a31 = 0.0; a32 = 0.0; a33 = 0.0; a34 = 0.0; a35 = 0.0;
            a41 = 0.0; a42 = 0.0; a43 = 0.0; a44 = 0.0; a45 = 0.0;
            a51 = 0.0; a52 = 0.0; a53 = 0.0; a54 = 0.0; a55 = 0.0;
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