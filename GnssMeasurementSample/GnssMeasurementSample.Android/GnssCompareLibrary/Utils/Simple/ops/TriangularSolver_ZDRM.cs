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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class TriangularSolver_ZDRM
    {
        /**
         * <p>
         * This is a forward substitution solver for non-singular upper triangular matrices.
         * <br>
         * b = U<sup>-1</sup>b<br>
         * <br>
         * where b is a vector, U is an n by n matrix.<br>
         * </p>
         *
         * @param U An n by n non-singular upper triangular matrix. Not modified.
         * @param b A vector of length n. Modified.
         * @param n The size of the matrices.
        */
        public static void solveU(double[] U, double[] b, int n)
        {
            //        for( int i =n-1; i>=0; i-- ) {
            //            double sum = b[i];
            //            for( int j = i+1; j <n; j++ ) {
            //                sum -= U[i*n+j]* b[j];
            //            }
            //            b[i] = sum/U[i*n+i];
            //        }
            int stride = n * 2;
            for (int i = n - 1; i >= 0; i--)
            {
                double sumReal = b[i * 2];
                double sumImg = b[i * 2 + 1];
                int indexU = i * stride + i * 2 + 2;
                for (int j = i + 1; j < n; j++)
                {
                    double realB = b[j * 2];
                    double imgB = b[j * 2 + 1];

                    double realU2 = U[indexU++];
                    double imgU2 = U[indexU++];

                    sumReal -= realB * realU2 - imgB * imgU2;
                    sumImg -= realB * imgU2 + imgB * realU2;
                }

                // b = sum/U
                double realU = U[i * stride + i * 2];
                double imgU = U[i * stride + i * 2 + 1];

                double normU = realU * realU + imgU * imgU;
                b[i * 2] = (sumReal * realU + sumImg * imgU) / normU;
                b[i * 2 + 1] = (sumImg * realU - sumReal * imgU) / normU;
            }
        }

        /**
         * <p>
         * Solves for non-singular lower triangular matrices with real valued diagonal elements
         * using forward substitution.
         * <br>
         * b = L<sup>-1</sup>b<br>
         * <br>
         * where b is a vector, L is an n by n matrix.<br>
         * </p>
         *
         * @param L An n by n non-singular lower triangular matrix. Not modified.
         * @param b A vector of length n. Modified.
         * @param n The size of the matrices.
         */
        public static void solveL_diagReal(double[] L, double[] b, int n)
        {
            //        for( int i = 0; i < n; i++ ) {
            //            double sum = b[i];
            //            for( int k=0; k<i; k++ ) {
            //                sum -= L[i*n+k]* b[k];
            //            }
            //            b[i] = sum / L[i*n+i];
            //        }
            int stride = n * 2;

            for (int i = 0; i < n; i++)
            {
                double realSum = b[i * 2];
                double imagSum = b[i * 2 + 1];

                int indexL = i * stride;
                int indexB = 0;
                for (int k = 0; k < i; k++)
                {
                    double realL2 = L[indexL++];
                    double imagL = L[indexL++];

                    double realB = b[indexB++];
                    double imagB = b[indexB++];

                    realSum -= realL2 * realB - imagL * imagB;
                    imagSum -= realL2 * imagB + imagL * realB;
                }

                double realL = L[indexL];

                b[i * 2] = realSum / realL;
                b[i * 2 + 1] = imagSum / realL;
            }
        }

        /**
         * <p>
         * This is a forward substitution solver for non-singular lower triangular matrices with
         * real valued diagonal elements.
         * <br>
         * b = (L<sup>CT</sup>)<sup>-1</sup>b<br>
         * <br>
         * where b is a vector, L is an n by n matrix.<br>
         * </p>
         * <p>
         * L is a lower triangular matrix, but it comes up with a solution as if it was
         * an upper triangular matrix that was computed by conjugate transposing L.
         * </p>
         *
         * @param L An n by n non-singular lower triangular matrix. Not modified.
         * @param b A vector of length n. Modified.
         * @param n The size of the matrices.
         */
        public static void solveConjTranL_diagReal(double[] L, double[] b, int n)
        {
            //        for( int i =n-1; i>=0; i-- ) {
            //            double sum = b[i];
            //            for( int k = i+1; k <n; k++ ) {
            //                sum -= L[k*n+i]* b[k];
            //            }
            //            b[i] = sum/L[i*n+i];
            //        }

            for (int i = n - 1; i >= 0; i--)
            {
                double realSum = b[i * 2];
                double imagSum = b[i * 2 + 1];

                int indexB = (i + 1) * 2;
                for (int k = i + 1; k < n; k++)
                {
                    int indexL = (k * n + i) * 2;

                    double realL2 = L[indexL];
                    double imagL = L[indexL + 1];

                    double realB = b[indexB++];
                    double imagB = b[indexB++];

                    realSum -= realL2 * realB + imagL * imagB;
                    imagSum -= realL2 * imagB - imagL * realB;
                }

                double realL = L[(i * n + i) * 2];

                b[i * 2] = realSum / realL;
                b[i * 2 + 1] = imagSum / realL;
            }
        }
    }
}