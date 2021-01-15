using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.linsol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class LinearSolverChol_DDRB : LinearSolver_DDRB_to_DDRM
    {
        public LinearSolverChol_DDRB() : base(new CholeskyOuterSolver_DDRB())
        {
        }

        public LinearSolverChol_DDRB(LinearSolverDense<DMatrixRBlock> alg) : base(alg)
        {
        }

        /**
         * Only converts the B matrix and passes that onto solve.  Te result is then copied into
         * the input 'X' matrix.
         *
         * @param B A matrix &real; <sup>m &times; p</sup>.  Not modified.
         * @param X A matrix &real; <sup>n &times; p</sup>, where the solution is written to.  Modified.
         */
        public override void solve(DMatrixRMaj B, DMatrixRMaj X)
        {
            blockB.reshape(B.numRows, B.numCols, false);
            MatrixOps_DDRB.convert(B, blockB);

            // since overwrite B is true X does not need to be passed in
            alg.solve(blockB, null);

            MatrixOps_DDRB.convert(blockB, X);
        }
    }
}