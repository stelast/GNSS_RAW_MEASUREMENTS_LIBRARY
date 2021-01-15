using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.linsol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class CholeskyOuterSolver_DDRB : LinearSolverDense<DMatrixRBlock>
    {
        // cholesky decomposition
        private CholeskyOuterForm_DDRB decomposer = new CholeskyOuterForm_DDRB(true);

        // size of a block take from input matrix
        private int blockLength;

        // temporary data structure used in some calculation.
        //private GrowArray<DGrowArray> workspace = new GrowArray<>(DGrowArray::new);
        private GrowArray<DGrowArray> workspace = null;

        public Decomposition getDescomposition<Decomposition, T>()
            where Decomposition : DecompositionInterface<T>
            where T : Matrix
        {
            throw new NotImplementedException();
        }

        public void invert(DMatrixRBlock A_inv)
        {
            DMatrixRBlock T = decomposer.getT(null);
            if (A_inv.numRows != T.numRows || A_inv.numCols != T.numCols)
                throw new ArgumentException("Unexpected number or rows and/or columns");

            // zero the upper triangular portion of A_inv
            MatrixOps_DDRB.zeroTriangle(true, A_inv);

            DSubmatrixD1 L = new DSubmatrixD1(T);
            DSubmatrixD1 B = new DSubmatrixD1(A_inv);

            // invert L from cholesky decomposition and write the solution into the lower
            // triangular portion of A_inv
            // B = inv(L)
            TriangularSolver_DDRB.invert(blockLength, false, L, B, workspace);

            // B = L^-T * B
            // todo could speed up by taking advantage of B being lower triangular
            // todo take advantage of symmetry
            TriangularSolver_DDRB.solveL(blockLength, L, B, true);
        }

        public bool modifiesA()
        {
            return decomposer.inputModified();
        }

        public bool modifiesB()
        {
            return true;
        }

        public double quality()
        {
            return SpecializedOps_DDRM.qualityTriangular(decomposer.getT(null));
        }

        public bool setA(DMatrixRBlock A)
        {
            // Extract a lower triangular solution
            if (!decomposer.decompose(A))
                return false;

            blockLength = A.blockLength;

            return true;
        }

        virtual public void solve(DMatrixRBlock B, DMatrixRBlock X)
        {
            if (B.blockLength != blockLength)
                throw new ArgumentException("Unexpected blocklength in B.");

            DSubmatrixD1 L = new DSubmatrixD1(decomposer.getT(null));

            if (X == null)
            {
                //X = B.create<DMatrixRBlock>(L.col1, B.numCols); 
                X = new DMatrixRBlock(L.col1, B.numCols);
            }
            else
            {
                X.reshape(L.col1, B.numCols, blockLength, false);
            }

            //  L * L^T*X = B

            // Solve for Y:  L*Y = B
            TriangularSolver_DDRB.solve(blockLength, false, L, new DSubmatrixD1(B), false);

            // L^T * X = Y
            TriangularSolver_DDRB.solve(blockLength, false, L, new DSubmatrixD1(B), true);

            if (X != null)
            {
                // copy the solution from B into X
                MatrixOps_DDRB.extractAligned(B, X);
            }
        }
    }
}