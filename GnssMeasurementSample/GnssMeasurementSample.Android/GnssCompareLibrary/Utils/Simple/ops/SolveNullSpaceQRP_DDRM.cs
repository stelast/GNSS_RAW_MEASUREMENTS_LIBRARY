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
    public class SolveNullSpaceQRP_DDRM : SolveNullSpace<DMatrixRMaj>
    {
        CustomizedQRP decomposition = new CustomizedQRP();

        // Storage for Q matrix
        DMatrixRMaj Q = new DMatrixRMaj(1, 1);
        public DMatrixRMaj getQ()
        {
            return Q;
        }

        public bool inputModified()
        {
            return decomposition.inputModified();
        }

        public bool process(DMatrixRMaj A, int numSingularValues, DMatrixRMaj nullspace)
        {
            decomposition.decompose(A);

            if (A.numRows > A.numCols)
            {
                Q.reshape(A.numCols, Math.Min(A.numRows, A.numCols));
                decomposition.getQ(Q, true);
            }
            else
            {
                Q.reshape(A.numCols, A.numCols);
                decomposition.getQ(Q, false);
            }

            nullspace.reshape(Q.numRows, numSingularValues);
            CommonOps_DDRM.extract(Q, 0, Q.numRows, Q.numCols - numSingularValues, Q.numCols, nullspace, 0, 0);

            return true;
        }
        private class CustomizedQRP : QRColPivDecompositionHouseholderColumn_DDRM
        {
        }
    }
}