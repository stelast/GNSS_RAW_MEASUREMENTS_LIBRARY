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
    public class LinearSolverLu_ZDRM : LinearSolverAbstract_ZDRM
    {
        LUDecompositionBase_ZDRM decomp;
        public LinearSolverLu_ZDRM(LUDecompositionBase_ZDRM decomp) 
        {
            this.decomp = decomp;
        }

        override public void solve(ZMatrixRMaj B, ZMatrixRMaj X)
        {
            UtilEjml.checkReshapeSolve(numRows, numCols, B, X);

            int bnumCols = B.numCols;
            int bstride = B.RowStride;

            double[] dataB = B.data;
            double[] dataX = X.data;

            double[] vv = decomp._getVV();

            //        for( int j = 0; j < numCols; j++ ) {
            //            for( int i = 0; i < this.numCols; i++ ) vv[i] = dataB[i*numCols+j];
            //            decomp._solveVectorInternal(vv);
            //            for( int i = 0; i < this.numCols; i++ ) dataX[i*numCols+j] = vv[i];
            //        }
            for (int j = 0; j < bnumCols; j++)
            {
                int index = j * 2;
                for (int i = 0; i < numRows; i++, index += bstride)
                {
                    vv[i * 2] = dataB[index];
                    vv[i * 2 + 1] = dataB[index + 1];
                }
                decomp._solveVectorInternal(vv);
                index = j * 2;
                for (int i = 0; i < numRows; i++, index += bstride)
                {
                    dataX[index] = vv[i * 2];
                    dataX[index + 1] = vv[i * 2 + 1];
                }
            }
        }

    }
}