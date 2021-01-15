using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data
{
    public class LinearSolverChol_ZDRM : LinearSolverAbstract_ZDRM
    {
        CholeskyDecompositionCommon_ZDRM decomposer;
        int n;
        double[] vv = new double[0];
        double[] t;

        public LinearSolverChol_ZDRM(CholeskyDecompositionCommon_ZDRM decomposer)
        {
            this.decomposer = decomposer;
        }

        override
        public bool setA(ZMatrixRMaj A)
        {
            if (A.numRows != A.numCols)
                throw new ArgumentException("Matrix must be square");

            _setA(A);
            if (decomposer.decompose(A))
            {
                n = A.numCols;
                if (vv.Count() < n * 2)
                    vv = new double[n * 2];
                t = decomposer._getT().data;
                return true;
            }
            else
            {
                return false;
            }
        }

        
        public override /**/double quality()
        {
            return SpecializedOps_ZDRM.qualityTriangular(decomposer._getT());
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
        public override void solve(ZMatrixRMaj B, ZMatrixRMaj X)
        {
            UtilEjml.checkReshapeSolve(numRows, numCols, B, X);

            int numCols2 = B.numCols;

            double[] dataB = B.data;
            double[] dataX = X.data;

            if (decomposer.isLower())
            {
                for (int j = 0; j < numCols2; j++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        vv[i * 2] = dataB[(i * numCols2 + j) * 2];
                        vv[i * 2 + 1] = dataB[(i * numCols2 + j) * 2 + 1];
                    }
                    solveInternalL();
                    for (int i = 0; i < n; i++)
                    {
                        dataX[(i * numCols2 + j) * 2] = vv[i * 2];
                        dataX[(i * numCols2 + j) * 2 + 1] = vv[i * 2 + 1];
                    }
                }
            }
            else
            {
                throw new SystemException("Implement");
            }
        }

        /**
         * Used internally to find the solution to a single column vector.
         */
        private void solveInternalL()
        {
            // This takes advantage of the diagonal elements always being real numbers

            // solve L*y=b storing y in x
            TriangularSolver_ZDRM.solveL_diagReal(t, vv, n);

            // solve L^T*x=y
            TriangularSolver_ZDRM.solveConjTranL_diagReal(t, vv, n);
        }

        /**
         * Sets the matrix 'inv' equal to the inverse of the matrix that was decomposed.
         *
         * @param inv Where the value of the inverse will be stored.  Modified.
         */
        public override void invert(ZMatrixRMaj inv)
        {
            if (inv.numRows != n || inv.numCols != n)
            {
                throw new SystemException("Unexpected matrix dimension");
            }
            if (inv.data == t)
            {
                throw new ArgumentException("Passing in the same matrix that was decomposed.");
            }

            if (decomposer.isLower())
            {
                setToInverseL(inv.data);
            }
            else
            {
                throw new SystemException("Implement");
            }
        }

        /**
         * Sets the matrix to the inverse using a lower triangular matrix.
         */
        public void setToInverseL(double[] a)
        {

            // the more direct method which takes full advantage of the sparsity of the data structures proved to
            // be difficult to get right due to the conjugates and reordering.
            // See comparable real number code for an example.
            for (int col = 0; col < n; col++)
            {
                Arrays.Fill(vv, 0);
                vv[col * 2] = 1;
                TriangularSolver_ZDRM.solveL_diagReal(t, vv, n);
                TriangularSolver_ZDRM.solveConjTranL_diagReal(t, vv, n);
                for (int i = 0; i < n; i++)
                {
                    a[(i * numCols + col) * 2] = vv[i * 2];
                    a[(i * numCols + col) * 2 + 1] = vv[i * 2 + 1];
                }
            }
            // NOTE: If you want to make inverse faster take advantage of the sparsity
        }

        
        public override bool modifiesA()
        {
            return decomposer.inputModified();
        }

        
        public override bool modifiesB()
        {
            return false;
        }

        public CholeskyDecompositionCommon_ZDRM getDescomposition()
        {
            return decomposer;
        }
        

    }
}