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
    public class LinearSolverChol_DDRM : LinearSolverAbstract_DDRM
    {
        CholeskyDecompositionCommon_DDRM decomposer;
        double[] vv;
        double[] t;

        public LinearSolverChol_DDRM(CholeskyDecompositionCommon_DDRM decomposer)
        {
            this.decomposer = decomposer;
        }
        public static void solveLower(DMatrixRMaj L, DMatrixRMaj B, DMatrixRMaj X, double[] vv)
        {
            int numCols = B.numCols;
            int N = L.numCols;
            for (int j = 0; j < numCols; j++)
            {
                for (int i = 0; i < N; i++) vv[i] = B.data[i * numCols + j];
                // solve L*y=b storing y in x
                TriangularSolver_DDRM.solveL(L.data, vv, N);

                // solve L^T*x=y
                TriangularSolver_DDRM.solveTranL(L.data, vv, N);
                for (int i = 0; i < N; i++) X.data[i * numCols + j] = vv[i];
            }
        }

        /**
         * Sets the matrix to the inverse using a lower triangular matrix.
         */
        public void setToInverseL(double[] a)
        {
            int n = numCols;
            // TODO reorder these operations to avoid cache misses

            // inverts the lower triangular system and saves the result
            // in the upper triangle to minimize cache misses
            for (int i = 0; i < n; i++)
            {
                double el_ii = t[i * n + i];
                for (int j = 0; j <= i; j++)
                {
                    double sum = (i == j) ? 1.0 : 0;
                    for (int k = i - 1; k >= j; k--)
                    {
                        sum -= t[i * n + k] * a[j * n + k];
                    }
                    a[j * n + i] = sum / el_ii;
                }
            }
            // solve the system and handle the previous solution being in the upper triangle
            // takes advantage of symmetry
            for (int i = n - 1; i >= 0; i--)
            {
                double el_ii = t[i * n + i];

                for (int j = 0; j <= i; j++)
                {
                    double sum = (i < j) ? 0 : a[j * n + i];
                    for (int k = i + 1; k < n; k++)
                    {
                        sum -= t[k * n + i] * a[j * n + k];
                    }
                    a[i * n + j] = a[j * n + i] = sum / el_ii;
                }
            }
        }

        override public bool setA(DMatrixRMaj A)
        {
            if (A.numRows != A.numCols)
                throw new ArgumentException("Matrix must be square");

            _setA(A);

            if (decomposer.decompose(A))
            {
                vv = decomposer._getVV();
                t = decomposer.getT().data;
                return true;
            }
            else
            {
                return false;
            }
        }

        override public /**/double quality()
        {
            return SpecializedOps_DDRM.qualityTriangular(decomposer.getT());
        }

        /**
         * <p>
         * Using the decomposition, finds the value of 'X' in the linear equation below:<br>
         *
         * A*x = b<br>
         *
         * where A has dimension of n by n, x and b are n by m dimension.
         * </p>
         * <p>
         * *Note* that 'b' and 'x' can be the same matrix instance.
         * </p>
         *
         * @param B A matrix that is n by m.  Not modified.
         * @param X An n by m matrix where the solution is writen to.  Modified.
         */
        override public void solve(DMatrixRMaj B, DMatrixRMaj X)
        {
            UtilEjml.checkReshapeSolve(numRows, numCols, B, X);

            if (A == null)
                throw new SystemException("Must call setA() first");

            if (decomposer.isLower())
            {
                solveLower(A, B, X, vv);
            }
            else
            {
                throw new SystemException("Implement");
            }
        }

    }
}