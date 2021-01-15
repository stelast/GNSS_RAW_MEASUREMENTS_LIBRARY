using Android.App;
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
    public class QRDecompositionHouseholderColumn_ZDRM : QRDecomposition<ZMatrixRMaj>
    {

        /**
         * Where the Q and R matrices are stored.  R is stored in the
         * upper triangular portion and Q on the lower bit.  Lower columns
         * are where u is stored.  Q_k = (I - gamma_k*u_k*u_k^T).
         */
        protected double[][] dataQR; // [ column][ row ]

        // used internally to store temporary data
        protected double[] v;

        // dimension of the decomposed matrices
        protected int numCols; // this is 'n'
        protected int numRows; // this is 'm'
        protected int minLength;

        // the computed gamma for Q_k matrix
        protected double[] gammas;
        // local variables
        protected double gamma;
        protected Complex_F64 tau = new Complex_F64();

        // did it encounter an error?
        protected bool error;

        public void setExpectedMaxSize(int numRows, int numCols)
        {
            this.numCols = numCols;
            this.numRows = numRows;
            minLength = Math.Min(numCols, numRows);
            int maxLength = Math.Max(numCols, numRows);

            if (dataQR == null || dataQR.Count() < numCols || dataQR[0].Count() < numRows * 2)
            {
                dataQR = new double[numCols][];
                for (int ii=0;ii<numCols;ii++)
                {
                    dataQR[ii] = new double[numRows * 2];
                }
                v = new double[maxLength * 2];
                gammas = new double[minLength];
            }

            if (v.Count() < maxLength * 2)
            {
                v = new double[maxLength * 2];
            }
            if (gammas.Count() < minLength)
            {
                gammas = new double[minLength];
            }
        }

        public double[] getGammas()
        {
            return gammas;
        }

        /**
         * Returns the combined QR matrix in a 2D array format that is column major.
         *
         * @return The QR matrix in a 2D matrix column major format. [ column ][ row ]
         */
        public double[][] getQR()
        {
            return dataQR;
        }

        public bool decompose(ZMatrixRMaj A)
        {
            setExpectedMaxSize(A.numRows, A.numCols);

            convertToColumnMajor(A);

            error = false;

            for (int j = 0; j < minLength; j++)
            {
                householder(j);
                updateA(j);
            }

            return !error;
        }

        /**
         * <p>
         * Takes the results from the householder computation and updates the 'A' matrix.<br>
         * <br>
         * A = (I - &gamma;*u*u<sup>H</sup>)A
         * </p>
         *
         * @param w The submatrix.
         */
        protected void updateA(int w)
        {
            double[] u = dataQR[w];

            for (int j = w + 1; j < numCols; j++)
            {

                double[] colQ = dataQR[j];
                // first element in u is assumed to be 1.0 + 0*i
                double realSum = colQ[w * 2];
                double imagSum = colQ[w * 2 + 1];

                for (int k = w + 1; k < numRows; k++)
                {
                    double realU = u[k * 2];
                    double imagU = -u[k * 2 + 1];

                    double realQ = colQ[k * 2];
                    double imagQ = colQ[k * 2 + 1];

                    realSum += realU * realQ - imagU * imagQ;
                    imagSum += imagU * realQ + realU * imagQ;
                }
                realSum *= gamma;
                imagSum *= gamma;

                colQ[w * 2] -= realSum;
                colQ[w * 2 + 1] -= imagSum;

                for (int i = w + 1; i < numRows; i++)
                {
                    double realU = u[i * 2];
                    double imagU = u[i * 2 + 1];

                    colQ[i * 2] -= realU * realSum - imagU * imagSum;
                    colQ[i * 2 + 1] -= imagU * realSum + realU * imagSum;
                }
            }
        }

        /**
         * <p>
         * Computes the householder vector "u" for the first column of submatrix j.  Note this is
         * a specialized householder for this problem.  There is some protection against
         * overfloaw and underflow.
         * </p>
         * <p>
         * Q = I - &gamma;uu<sup>T</sup>
         * </p>
         * <p>
         * This function finds the values of 'u' and '&gamma;'.
         * </p>
         *
         * @param j Which submatrix to work off of.
         */
        protected void householder(int j)
        {
            double[] u = dataQR[j];

            // find the largest value in this column
            // this is used to normalize the column and mitigate overflow/underflow
            double max = QrHelperFunctions_ZDRM.findMax(u, j, numRows - j);

            if (max == 0.0)
            {
                gamma = 0;
                error = true;
            }
            else
            {
                // computes tau and gamma, and normalizes u by max
                gamma = QrHelperFunctions_ZDRM.computeTauGammaAndDivide(j, numRows, u, max, tau);

                // divide u by u_0
                //            double u_0 = u[j] + tau;
                double real_u_0 = u[j * 2] + tau.real;
                double imag_u_0 = u[j * 2 + 1] + tau.imaginary;
                QrHelperFunctions_ZDRM.divideElements(j + 1, numRows, u, 0, real_u_0, imag_u_0);

                tau.real *= max;
                tau.imaginary *= max;

                u[j * 2] = -tau.real;
                u[j * 2 + 1] = -tau.imaginary;
            }

            gammas[j] = gamma;
        }

        /**
         * Converts the standard row-major matrix into a column-major vector
         * that is advantageous for this problem.
         *
         * @param A original matrix that is to be decomposed.
         */
        protected void convertToColumnMajor(ZMatrixRMaj A)
        {
            for (int x = 0; x < numCols; x++)
            {
                double[] colQ = dataQR[x];
                int indexCol = 0;
                for (int y = 0; y < numRows; y++)
                {
                    int index = (y * numCols + x) * 2;
                    colQ[indexCol++] = A.data[index];
                    colQ[indexCol++] = A.data[index + 1];
                }
            }
        }

        public ZMatrixRMaj getQ(ZMatrixRMaj Q, bool compact)
        {
            if (compact)
                Q = UtilDecompositons_ZDRM.checkIdentity(Q, numRows, minLength);
            else
                Q = UtilDecompositons_ZDRM.checkIdentity(Q, numRows, numRows);

            for (int j = minLength - 1; j >= 0; j--)
            {
                double[] u = dataQR[j];

                double vvReal = u[j * 2];
                double vvImag = u[j * 2 + 1];

                u[j * 2] = 1;
                u[j * 2 + 1] = 0;
                double gammaReal = gammas[j];

                QrHelperFunctions_ZDRM.rank1UpdateMultR(Q, u, 0, gammaReal, j, j, numRows, v);

                u[j * 2] = vvReal;
                u[j * 2 + 1] = vvImag;
            }

            return Q;
        }

        public ZMatrixRMaj getR(ZMatrixRMaj R, bool compact)
        {
            if (compact)
                R = UtilDecompositons_ZDRM.checkZerosLT(R, minLength, numCols);
            else
                R = UtilDecompositons_ZDRM.checkZerosLT(R, numRows, numCols);

            for (int j = 0; j < numCols; j++)
            {
                double[] colR = dataQR[j];
                int l = Math.Min(j, numRows - 1);
                for (int i = 0; i <= l; i++)
                {
                    R.set(i, j, colR[i * 2], colR[i * 2 + 1]);
                }
            }

            return R;
        }

        public bool inputModified()
        {
            return false;
        }
    }
}