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
    public class LinearSolverQrHouseCol_ZDRM : LinearSolverAbstract_ZDRM
    {

        private readonly QRDecompositionHouseholderColumn_ZDRM decomposer;

        private readonly ZMatrixRMaj a = new ZMatrixRMaj(1, 1);
        private readonly ZMatrixRMaj temp = new ZMatrixRMaj(1, 1);

        protected int maxRows = -1;
        protected int maxCols = -1;

        private double[][] QR; // a column major QR matrix
        private readonly ZMatrixRMaj R = new ZMatrixRMaj(1, 1);
        private double[] gammas;

        /**
         * Creates a linear solver that uses QR decomposition.
         */
        public LinearSolverQrHouseCol_ZDRM()
        {
            decomposer = new QRDecompositionHouseholderColumn_ZDRM();
        }

        public void setMaxSize(int maxRows, int maxCols)
        {
            this.maxRows = maxRows;
            this.maxCols = maxCols;
        }

        /**
         * Performs QR decomposition on A
         *
         * @param A not modified.
         */
        override public bool setA(ZMatrixRMaj A)
        {
            if (A.numRows < A.numCols)
                throw new ArgumentException("Can't solve for wide systems.  More variables than equations.");
            if (A.numRows > maxRows || A.numCols > maxCols)
                setMaxSize(A.numRows, A.numCols);

            R.reshape(A.numCols, A.numCols);
            a.reshape(A.numRows, 1);
            temp.reshape(A.numRows, 1);

            _setA(A);
            if (!decomposer.decompose(A))
                return false;

            gammas = decomposer.getGammas();
            QR = decomposer.getQR();
            decomposer.getR(R, true);
            return true;
        }

        override public /**/double quality()
        {
            return SpecializedOps_ZDRM.qualityTriangular(R);
        }

        /**
         * Solves for X using the QR decomposition.
         *
         * @param B A matrix that is n by m.  Not modified.
         * @param X An n by m matrix where the solution is written to.  Modified.
         */
        override public void solve(ZMatrixRMaj B, ZMatrixRMaj X)
        {
            UtilEjml.checkReshapeSolve(numRows, numCols, B, X);

            int BnumCols = B.numCols;

            // solve each column one by one
            for (int colB = 0; colB < BnumCols; colB++)
            {

                // make a copy of this column in the vector
                for (int i = 0; i < numRows; i++)
                {
                    int indexB = (i * BnumCols + colB) * 2;
                    a.data[i * 2] = B.data[indexB];
                    a.data[i * 2 + 1] = B.data[indexB + 1];
                }

                // Solve Qa=b
                // a = Q'b
                // a = Q_{n-1}...Q_2*Q_1*b
                //
                // Q_n*b = (I-gamma*u*u^T)*b = b - u*(gamma*U^T*b)
                for (int n = 0; n < numCols; n++)
                {
                    double[] u = QR[n];

                    double realVV = u[n * 2];
                    double imagVV = u[n * 2 + 1];

                    u[n * 2] = 1;
                    u[n * 2 + 1] = 0;

                    QrHelperFunctions_ZDRM.rank1UpdateMultR(a, u, 0, gammas[n], 0, n, numRows, temp.data);

                    u[n * 2] = realVV;
                    u[n * 2 + 1] = imagVV;
                }

                // solve for Rx = b using the standard upper triangular solver
                TriangularSolver_ZDRM.solveU(R.data, a.data, numCols);

                // save the results
                for (int i = 0; i < numCols; i++)
                {
                    int indexB = (i * BnumCols + colB) * 2;
                    X.data[indexB] = a.data[i * 2];
                    X.data[indexB + 1] = a.data[i * 2 + 1];
                }
            }
        }

        override public bool modifiesA()
        {
            return false;
        }

        override public bool modifiesB()
        {
            return false;
        }

        public QRDecompositionHouseholderColumn_ZDRM getDecomposition()
        {
            return decomposer;
        }
    }
}