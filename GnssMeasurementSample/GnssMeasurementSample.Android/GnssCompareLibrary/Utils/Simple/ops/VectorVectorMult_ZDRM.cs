﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class VectorVectorMult_ZDRM
    {
        /**
         * <p>
         * Computes the inner product of the two vectors.  In geometry this is known as the dot product.<br>
         * <br>
         * &sum;<sub>k=1:n</sub> x<sub>k</sub> * y<sub>k</sub><br>
         * where x and y are vectors with n elements.
         * </p>
         *
         * <p>
         * These functions are often used inside of highly optimized code and therefor sanity checks are
         * kept to a minimum.  It is not recommended that any of these functions be used directly.
         * </p>
         *
         * @param x A vector with n elements. Not modified.
         * @param y A vector with n elements. Not modified.
         * @return The inner product of the two vectors.
         */
        public static Complex_F64 innerProd(ZMatrixRMaj x, ZMatrixRMaj y,  Complex_F64 output )
        {
            if (output == null)
                output = new Complex_F64();
            else
            {
                output.real = output.imaginary = 0;
            }

            int m = x.DataLength;

            for (int i = 0; i < m; i += 2)
            {
                double realX = x.data[i];
                double imagX = x.data[i + 1];

                double realY = y.data[i];
                double imagY = y.data[i + 1];

                output.real += realX * realY - imagX * imagY;
                output.imaginary += realX * imagY + imagX * realY;
            }

            return output;
        }

        /**
         * <p>
         * Computes the inner product between a vector and the conjugate of another one.
         * <br>
         * <br>
         * &sum;<sub>k=1:n</sub> x<sub>k</sub> * conj(y<sub>k</sub>)<br>
         * where x and y are vectors with n elements.
         * </p>
         *
         * <p>
         * These functions are often used inside of highly optimized code and therefor sanity checks are
         * kept to a minimum.  It is not recommended that any of these functions be used directly.
         * </p>
         *
         * @param x A vector with n elements. Not modified.
         * @param y A vector with n elements. Not modified.
         * @return The inner product of the two vectors.
         */
        public static Complex_F64 innerProdH(ZMatrixRMaj x, ZMatrixRMaj y,  Complex_F64 output )
        {
            if (output == null)
                output = new Complex_F64();
            else
            {
                output.real = output.imaginary = 0;
            }

            int m = x.DataLength;

            for (int i = 0; i < m; i += 2)
            {
                double realX = x.data[i];
                double imagX = x.data[i + 1];

                double realY = y.data[i];
                double imagY = -y.data[i + 1];

                output.real += realX * realY - imagX * imagY;
                output.imaginary += realX * imagY + imagX * realY;
            }

            return output;
        }

        /**
         * <p>
         * Sets A &isin; &real; <sup>m &times; n</sup> equal to an outer product multiplication of the two
         * vectors.  This is also known as a rank-1 operation.<br>
         * <br>
         * A = x * y<sup>T</sup>
         * where x &isin; &real; <sup>m</sup> and y &isin; &real; <sup>n</sup> are vectors.
         * </p>
         * <p>
         * Which is equivalent to: A<sub>ij</sub> = x<sub>i</sub>*y<sub>j</sub>
         * </p>
         *
         * @param x A vector with m elements. Not modified.
         * @param y A vector with n elements. Not modified.
         * @param A A Matrix with m by n elements. Modified.
         */
        public static void outerProd(ZMatrixRMaj x, ZMatrixRMaj y, ZMatrixRMaj A)
        {
            int m = A.numRows;
            int n = A.numCols;

            int index = 0;
            for (int i = 0; i < m; i++)
            {
                double realX = x.data[i * 2];
                double imagX = x.data[i * 2 + 1];

                int indexY = 0;
                for (int j = 0; j < n; j++)
                {
                    double realY = y.data[indexY++];
                    double imagY = y.data[indexY++];

                    A.data[index++] = realX * realY - imagX * imagY;
                    A.data[index++] = realX * imagY + imagX * realY;
                }
            }
        }

        /**
         * <p>
         * Sets A &isin; &real; <sup>m &times; n</sup> equal to an outer product multiplication of the two
         * vectors.  This is also known as a rank-1 operation.<br>
         * <br>
         * A = x * y<sup>H</sup>
         * where x &isin; &real; <sup>m</sup> and y &isin; &real; <sup>n</sup> are vectors.
         * </p>
         * <p>
         * Which is equivalent to: A<sub>ij</sub> = x<sub>i</sub>*y<sub>j</sub>
         * </p>
         *
         * @param x A vector with m elements. Not modified.
         * @param y A vector with n elements. Not modified.
         * @param A A Matrix with m by n elements. Modified.
         */
        public static void outerProdH(ZMatrixRMaj x, ZMatrixRMaj y, ZMatrixRMaj A)
        {
            int m = A.numRows;
            int n = A.numCols;

            int index = 0;
            for (int i = 0; i < m; i++)
            {
                double realX = x.data[i * 2];
                double imagX = x.data[i * 2 + 1];

                int indexY = 0;
                for (int j = 0; j < n; j++)
                {
                    double realY = y.data[indexY++];
                    double imagY = -y.data[indexY++];

                    A.data[index++] = realX * realY - imagX * imagY;
                    A.data[index++] = realX * imagY + imagX * realY;
                }
            }
        }
    }
}