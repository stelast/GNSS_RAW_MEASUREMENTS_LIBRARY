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
    public class QrHelperFunctions_DDRM
    {//CONCURRENT_OMIT_BEGIN
        public static double findMax(double[] u, int startU, int length)
        {
            double max = -1;

            int index = startU;
            int stopIndex = startU + length;
            for (; index < stopIndex; index++)
            {
                double val = u[index];
                val = (val < 0.0) ? -val : val;
                if (val > max)
                    max = val;
            }

            return max;
        }

        public static void divideElements(int j, int numRows,
                                           double[] u, double u_0)
        {
            //        double div_u = 1.0/u_0;
            //
            //        if( Double.isInfinite(div_u)) {
            for (int i = j; i < numRows; i++)
            {
                u[i] /= u_0;
            }
            //        } else {
            //            for( int i = j; i < numRows; i++ ) {
            //                u[i] *= div_u;
            //            }
            //        }
        }

        public static void divideElements(int j, int numRows, double[] u, int startU, double u_0)
        {
            //        double div_u = 1.0/u_0;
            //
            //        if( Double.isInfinite(div_u)) {
            for (int i = j; i < numRows; i++)
            {
                u[i + startU] /= u_0;
            }
            //        } else {
            //            for( int i = j; i < numRows; i++ ) {
            //                u[i+startU] *= div_u;
            //            }
            //        }
        }

        public static void divideElements_Brow(int j, int numRows, double[] u,
                                                double[] b, int startB,
                                                double u_0)
        {
            //        double div_u = 1.0/u_0;
            //
            //        if( Double.isInfinite(div_u)) {
            for (int i = j; i < numRows; i++)
            {
                u[i] = b[i + startB] /= u_0;
            }
            //        } else {
            //            for( int i = j; i < numRows; i++ ) {
            //                u[i] = b[i+startB] *= div_u;
            //            }
            //        }
        }

        public static void divideElements_Bcol(int j, int numRows, int numCols,
                                                double[] u,
                                                double[] b, int startB,
                                                double u_0)
        {
            //        double div_u = 1.0/u_0;
            //
            //        if( Double.isInfinite(div_u)) {
            int indexB = j * numCols + startB;
            for (int i = j; i < numRows; i++, indexB += numCols)
            {
                b[indexB] = u[i] /= u_0;
            }
            //        } else {
            //            int indexB = j*numCols+startB;
            //            for( int i = j; i < numRows; i++ , indexB += numCols ) {
            //                b[indexB] = u[i] *= div_u;
            //            }
            //        }
        }

        public static double computeTauAndDivide(int j, int numRows, double[] u, int startU, double max)
        {
            // compute the norm2 of the matrix, with each element
            // normalized by the max value to avoid overflow problems
            double tau = 0;
            //        double div_max = 1.0/max;
            //        if( Double.isInfinite(div_max)) {
            // more accurate
            for (int i = j; i < numRows; i++)
            {
                double d = u[startU + i] /= max;
                tau += d * d;
            }
            //        } else {
            //            // faster
            //            for( int i = j; i < numRows; i++ ) {
            //                double d = u[startU+i] *= div_max;
            //                tau += d*d;
            //            }
            //        }
            tau = Math.Sqrt(tau);

            if (u[startU + j] < 0)
                tau = -tau;

            return tau;
        }

        /**
         * Normalizes elements in 'u' by dividing by max and computes the norm2 of the normalized
         * array u.  Adjust the sign of the returned value depending on the size of the first
         * element in 'u'. Normalization is done to avoid overflow.
         *
         * <pre>
         * for i=j:numRows
         *   u[i] = u[i] / max
         *   tau = tau + u[i]*u[i]
         * end
         * tau = sqrt(tau)
         * if( u[j] &lt; 0 )
         *    tau = -tau;
         * </pre>
         *
         * @param j Element in 'u' that it starts at.
         * @param numRows Element in 'u' that it stops at.
         * @param u Array
         * @param max Max value in 'u' that is used to normalize it.
         * @return norm2 of 'u'
         */
        public static double computeTauAndDivide(int j, int numRows,
                                                  double[] u, double max)
        {
            double tau = 0;
            //        double div_max = 1.0/max;
            //        if( Double.isInfinite(div_max)) {
            for (int i = j; i < numRows; i++)
            {
                double d = u[i] /= max;
                tau += d * d;
            }
            //        } else {
            //            for( int i = j; i < numRows; i++ ) {
            //                double d = u[i] *= div_max;
            //                tau += d*d;
            //            }
            //        }
            tau = Math.Sqrt(tau);

            if (u[j] < 0)
                tau = -tau;

            return tau;
        }
        //CONCURRENT_OMIT_END

        /**
         * <p>
         * Performs a rank-1 update operation on the submatrix specified by w with the multiply on the right.<br>
         * <br>
         * A = (I - &gamma;*u*u<sup>T</sup>)*A<br>
         * </p>
         * <p>
         * The order that matrix multiplies are performed has been carefully selected
         * to minimize the number of operations.
         * </p>
         *
         * <p>
         * Before this can become a truly generic operation the submatrix specification needs
         * to be made more generic.
         * </p>
         */
        public static void rank1UpdateMultR(DMatrixRMaj A, double[] u, double gamma,
                                             int colA0,
                                             int w0, int w1,
                                             double[] _temp)
        {
            //        for( int i = colA0; i < A.numCols; i++ ) {
            //            double val = 0;
            //
            //            for( int k = w0; k < w1; k++ ) {
            //                val += u[k]*A.data[k*A.numCols +i];
            //            }
            //            _temp[i] = gamma*val;
            //        }

            // reordered to reduce cpu cache issues
            for (int i = colA0; i < A.numCols; i++)
            {
                _temp[i] = u[w0] * A.data[w0 * A.numCols + i];
            }

            for (int k = w0 + 1; k < w1; k++)
            {
                int indexA = k * A.numCols + colA0;
                double valU = u[k];
                for (int i = colA0; i < A.numCols; i++)
                {
                    _temp[i] += valU * A.data[indexA++];
                }
            }

            for (int i = colA0; i < A.numCols; i++)
            {
                _temp[i] *= gamma;
            }

            // end of reorder

            //CONCURRENT_BELOW EjmlConcurrency.loopFor(w0, w1, i->{
            for (int i = w0; i < w1; i++)
            {
                double valU = u[i];

                int indexA = i * A.numCols + colA0;
                for (int j = colA0; j < A.numCols; j++)
                {
                    A.data[indexA++] -= valU * _temp[j];
                }
            }
            //CONCURRENT_ABOVE });
        }

        // Useful for concurrent implementations where you don't want to modify u[0] to set it to 1.0
        public static void rank1UpdateMultR_u0(DMatrixRMaj A, double[] u, double u_0,
                                                double gamma,
                                                int colA0,
                                                int w0, int w1,
                                                double[] _temp)
        {
            //        for( int i = colA0; i < A.numCols; i++ ) {
            //            double val = 0;
            //
            //            for( int k = w0; k < w1; k++ ) {
            //                val += u[k]*A.data[k*A.numCols +i];
            //            }
            //            _temp[i] = gamma*val;
            //        }

            // reordered to reduce cpu cache issues
            for (int i = colA0; i < A.numCols; i++)
            {
                _temp[i] = u_0 * A.data[w0 * A.numCols + i];
            }

            for (int k = w0 + 1; k < w1; k++)
            {
                int indexA = k * A.numCols + colA0;
                double valU = u[k];
                for (int i = colA0; i < A.numCols; i++)
                {
                    _temp[i] += valU * A.data[indexA++];
                }
            }

            for (int i = colA0; i < A.numCols; i++)
            {
                _temp[i] *= gamma;
            }

            // end of reorder
            {
                int indexA = w0 * A.numCols + colA0;
                for (int j = colA0; j < A.numCols; j++)
                {
                    A.data[indexA++] -= u_0 * _temp[j];
                }
            }

            //CONCURRENT_BELOW EjmlConcurrency.loopFor(w0+1, w1, i->{
            for (int i = w0 + 1; i < w1; i++)
            {
                double valU = u[i];

                int indexA = i * A.numCols + colA0;
                for (int j = colA0; j < A.numCols; j++)
                {
                    A.data[indexA++] -= valU * _temp[j];
                }
            }
            //CONCURRENT_ABOVE });
        }

        public static void rank1UpdateMultR(DMatrixRMaj A,
                                             double[] u, int offsetU,
                                             double gamma,
                                             int colA0,
                                             int w0, int w1,
                                             double[] _temp)
        {
            //        for( int i = colA0; i < A.numCols; i++ ) {
            //            double val = 0;
            //
            //            for( int k = w0; k < w1; k++ ) {
            //                val += u[k+offsetU]*A.data[k*A.numCols +i];
            //            }
            //            _temp[i] = gamma*val;
            //        }

            // reordered to reduce cpu cache issues
            for (int i = colA0; i < A.numCols; i++)
            {
                _temp[i] = u[w0 + offsetU] * A.data[w0 * A.numCols + i];
            }

            for (int k = w0 + 1; k < w1; k++)
            {
                int indexA = k * A.numCols + colA0;
                double valU = u[k + offsetU];
                for (int i = colA0; i < A.numCols; i++)
                {
                    _temp[i] += valU * A.data[indexA++];
                }
            }
            for (int i = colA0; i < A.numCols; i++)
            {
                _temp[i] *= gamma;
            }

            // end of reorder

            //CONCURRENT_BELOW EjmlConcurrency.loopFor(w0, w1, i->{
            for (int i = w0; i < w1; i++)
            {
                double valU = u[i + offsetU];

                int indexA = i * A.numCols + colA0;
                for (int j = colA0; j < A.numCols; j++)
                {
                    A.data[indexA++] -= valU * _temp[j];
                }
            }
            //CONCURRENT_ABOVE });
        }

        /**
         * <p>
         * Performs a rank-1 update operation on the submatrix specified by w with the multiply on the left.<br>
         * <br>
         * A = A(I - &gamma;*u*u<sup>T</sup>)<br>
         * </p>
         * <p>
         * The order that matrix multiplies are performed has been carefully selected
         * to minimize the number of operations.
         * </p>
         *
         * <p>
         * Before this can become a truly generic operation the submatrix specification needs
         * to be made more generic.
         * </p>
         */
        public static void rank1UpdateMultL(DMatrixRMaj A, double[] u,
                                             double gamma,
                                             int colA0,
                                             int w0, int w1)
        {
            //CONCURRENT_BELOW EjmlConcurrency.loopFor(colA0, A.numRows, i->{
            for (int i = colA0; i < A.numRows; i++)
            {
                int startIndex = i * A.numCols + w0;
                double sum = 0;
                int rowIndex = startIndex;
                for (int j = w0; j < w1; j++)
                {
                    sum += A.data[rowIndex++] * u[j];
                }
                sum = -gamma * sum;

                rowIndex = startIndex;
                for (int j = w0; j < w1; j++)
                {
                    A.data[rowIndex++] += sum * u[j];
                }
            }
            //CONCURRENT_ABOVE });
        }
    }
}