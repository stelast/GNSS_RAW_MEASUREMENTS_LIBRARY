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
    public class LinearSolverLu_DDRM : LinearSolverLuBase_DDRM
    {

        bool doImprove = false;

        public LinearSolverLu_DDRM(LUDecompositionBase_DDRM decomp) : base(decomp)
        {
        }

        public LinearSolverLu_DDRM(LUDecompositionBase_DDRM decomp, bool doImprove) : base(decomp)
        {
            this.doImprove = doImprove;
        }

        public override void solve(DMatrixRMaj B, DMatrixRMaj X)
        {
            UtilEjml.checkReshapeSolve(numRows, numCols, B, X);

            int numCols2 = B.numCols;

            double[] dataB = B.data;
            double[] dataX = X.data;

            double[] vv = decomp._getVV();

            //        for( int j = 0; j < numCols; j++ ) {
            //            for( int i = 0; i < this.numCols; i++ ) vv[i] = dataB[i*numCols+j];
            //            decomp._solveVectorInternal(vv);
            //            for( int i = 0; i < this.numCols; i++ ) dataX[i*numCols+j] = vv[i];
            //        }
            for (int j = 0; j < numCols2; j++)
            {
                int index = j;
                for (int i = 0; i < this.numCols; i++, index += numCols2) vv[i] = dataB[index];
                decomp._solveVectorInternal(vv);
                index = j;
                for (int i = 0; i < this.numCols; i++, index += numCols2) dataX[index] = vv[i];
            }

            if (doImprove)
            {
                improveSol(B, X);
            }
        }

    }
}