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
    public class LinearSolver_DDRB_to_DDRM : LinearSolverDense<DMatrixRMaj>
    {
        protected LinearSolverDense<DMatrixRBlock> alg = new CholeskyOuterSolver_DDRB();

        // block matrix copy of the system A matrix.
        protected DMatrixRBlock blockA = new DMatrixRBlock(1, 1);
        // block matrix copy of B matrix passed into solve
        protected DMatrixRBlock blockB = new DMatrixRBlock(1, 1);
        // block matrix copy of X matrix passed into solve
        protected DMatrixRBlock blockX = new DMatrixRBlock(1, 1);

        public LinearSolver_DDRB_to_DDRM(LinearSolverDense<DMatrixRBlock> alg)
        {
            this.alg = alg;
        }

        public Decomposition getDescomposition<Decomposition, T>()
            where Decomposition : DecompositionInterface<T>
            where T : Matrix
        {
            throw new NotImplementedException();
        }

        public void invert(DMatrixRMaj A_inv)
        {
            blockB.reshape(A_inv.numRows, A_inv.numCols, false);

            alg.invert(blockB);

            MatrixOps_DDRB.convert(blockB, A_inv);
        }

        public bool modifiesA()
        {
            return false;
        }

        public bool modifiesB()
        {
            return false;
        }

        public double quality()
        {
            return alg.quality();
        }

        public bool setA(DMatrixRMaj A)
        {
            blockA.reshape(A.numRows, A.numCols, false);
            MatrixOps_DDRB.convert(A, blockA);

            return alg.setA(blockA);
        }

        virtual public void solve(DMatrixRMaj B, DMatrixRMaj X)
        {
            X.reshape(blockA.numCols, B.numCols);
            blockB.reshape(B.numRows, B.numCols, false);
            blockX.reshape(X.numRows, X.numCols, false);
            MatrixOps_DDRB.convert(B, blockB);

            alg.solve(blockB, blockX);

            MatrixOps_DDRB.convert(blockX, X);
        }
    }
}