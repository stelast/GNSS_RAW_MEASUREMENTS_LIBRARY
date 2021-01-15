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
    public class QRDecompositionHouseholderTran_DDRM : QRDecomposition<DMatrixRMaj>
    {
        /**
         * Where the Q and R matrices are stored.  For speed reasons
         * this is transposed
         */
        protected DMatrixRMaj QR;

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
        protected double tau;

        // did it encounter an error?
        protected bool error;

        virtual public void setExpectedMaxSize(int numRows, int numCols)
        {
            this.numCols = numCols;
            this.numRows = numRows;
            minLength = Math.Min(numCols, numRows);
            int maxLength = Math.Max(numCols, numRows);

            if (QR == null)
            {
                QR = new DMatrixRMaj(numCols, numRows);
                v = new double[maxLength];
                gammas = new double[minLength];
            }
            else
            {
                QR.reshape(numCols, numRows, false);
            }

            if (v.Count() < maxLength)
            {
                v = new double[maxLength];
            }
            if (gammas.Count() < minLength)
            {
                gammas = new double[minLength];
            }
        }

        /**
         * Inner matrix that stores the decomposition
         */
        public DMatrixRMaj getQR()
        {
            return QR;
        }

        /**
         * A = Q*A
         *
         * @param A Matrix that is being multiplied by Q.  Is modified.
         */
        public void applyQ(DMatrixRMaj A)
        {
            if (A.numRows != numRows)
                throw new ArgumentException("A must have at least " + numRows + " rows.");

            for (int j = minLength - 1; j >= 0; j--)
            {
                int diagIndex = j * numRows + j;
                double before = QR.data[diagIndex];
                QR.data[diagIndex] = 1;
                QrHelperFunctions_DDRM.rank1UpdateMultR(A, QR.data, j * numRows, gammas[j], 0, j, numRows, v);
                QR.data[diagIndex] = before;
            }
        }

        /**
         * A = Q<sup>T</sup>*A
         *
         * @param A Matrix that is being multiplied by Q<sup>T</sup>.  Is modified.
         */
        public void applyTranQ(DMatrixRMaj A)
        {
            for (int j = 0; j < minLength; j++)
            {
                int diagIndex = j * numRows + j;
                double before = QR.data[diagIndex];
                QR.data[diagIndex] = 1;
                QrHelperFunctions_DDRM.rank1UpdateMultR(A, QR.data, j * numRows, gammas[j], 0, j, numRows, v);
                QR.data[diagIndex] = before;
            }
        }
        /**
         * <p>
         * Computes the householder vector "u" for the first column of submatrix j.  Note this is
         * a specialized householder for this problem.  There is some protection against
         * overflow and underflow.
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
        protected void householder( int j)
        {
            int startQR = j * numRows;
            int endQR = startQR + numRows;
            startQR += j;

             double max = QrHelperFunctions_DDRM.findMax(QR.data, startQR, numRows - j);

            if (max == 0.0)
            {
                gamma = 0;
                error = true;
            }
            else
            {
                // computes tau and normalizes u by max
                tau = QrHelperFunctions_DDRM.computeTauAndDivide(startQR, endQR, QR.data, max);

                // divide u by u_0
                double u_0 = QR.data[startQR] + tau;
                QrHelperFunctions_DDRM.divideElements(startQR + 1, endQR, QR.data, u_0);

                gamma = u_0 / tau;
                tau *= max;

                QR.data[startQR] = -tau;
            }

            gammas[j] = gamma;
        }

        /**
         * <p>
         * Takes the results from the householder computation and updates the 'A' matrix.<br>
         * <br>
         * A = (I - &gamma;*u*u<sup>T</sup>)A
         * </p>
         *
         * @param w The submatrix.
         */
        protected void updateA( int w)
        {
            //        int rowW = w*numRows;
            //        int rowJ = rowW + numRows;
            //
            //        for( int j = w+1; j < numCols; j++ , rowJ += numRows) {
            //            double val = QR.data[rowJ + w];
            //
            //            // val = gamma*u^T * A
            //            for( int k = w+1; k < numRows; k++ ) {
            //                val += QR.data[rowW + k]*QR.data[rowJ + k];
            //            }
            //            val *= gamma;
            //
            //            // A - val*u
            //            QR.data[rowJ + w] -= val;
            //            for( int i = w+1; i < numRows; i++ ) {
            //                QR.data[rowJ + i] -= QR.data[rowW + i]*val;
            //            }
            //        }

             double[] data = QR.data;
             int rowW = w * numRows + w + 1;
            int rowJ = rowW + numRows;
             int rowJEnd = rowJ + (numCols - w - 1) * numRows;
             int indexWEnd = rowW + numRows - w - 1;

            for (; rowJEnd != rowJ; rowJ += numRows)
            {
                // assume the first element in u is 1
                double val = data[rowJ - 1];

                int indexW = rowW;
                int indexJ = rowJ;

                while (indexW != indexWEnd)
                {
                    val += data[indexW++] * data[indexJ++];
                }
                val *= gamma;

                data[rowJ - 1] -= val;
                indexW = rowW;
                indexJ = rowJ;
                while (indexW != indexWEnd)
                {
                    data[indexJ++] -= data[indexW++] * val;
                }
            }
        }

        public double[] getGammas()
        {
            return gammas;
        }

        public DMatrixRMaj getQ(DMatrixRMaj Q, bool compact)
        {
            if (compact)
            {
                Q = UtilDecompositons_DDRM.ensureIdentity(Q, numRows, minLength);
            }
            else
            {
                Q = UtilDecompositons_DDRM.ensureIdentity(Q, numRows, numRows);
            }

            // Unlike applyQ() this takes advantage of zeros in the identity matrix
            // by not multiplying across all rows.
            for (int j = minLength - 1; j >= 0; j--)
            {
                int diagIndex = j * numRows + j;
                double before = QR.data[diagIndex];
                QR.data[diagIndex] = 1;
                QrHelperFunctions_DDRM.rank1UpdateMultR(Q, QR.data, j * numRows, gammas[j], j, j, numRows, v);
                QR.data[diagIndex] = before;
            }

            return Q;
        }

        public DMatrixRMaj getR(DMatrixRMaj R, bool compact)
        {
            if (compact)
            {
                R = UtilDecompositons_DDRM.checkZerosLT(R, minLength, numCols);
            }
            else
            {
                R = UtilDecompositons_DDRM.checkZerosLT(R, numRows, numCols);
            }

            for (int i = 0; i < R.numRows; i++)
            {
                for (int j = i; j < R.numCols; j++)
                {
                    R.unsafe_set(i, j, QR.unsafe_get(j, i));
                }
            }


            return R;
        }

        virtual public bool decompose(DMatrixRMaj A)
        {
            setExpectedMaxSize(A.numRows, A.numCols);

            CommonOps_DDRM.transpose(A, QR);

            error = false;

            for (int j = 0; j < minLength; j++)
            {
                householder(j);
                updateA(j);
            }

            return !error;
        }

        public bool inputModified()
        {
            return false;
        }
    }
}