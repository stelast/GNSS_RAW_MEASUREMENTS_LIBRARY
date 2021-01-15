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
    public class InvertUsingSolve_ZDRM
    {
        public static void invert(LinearSolverDense<ZMatrixRMaj> solver, ZMatrixRMaj A, ZMatrixRMaj A_inv, ZMatrixRMaj storage)
        {

            A_inv.reshape(A.numRows, A.numCols);

            CommonOps_ZDRM.setIdentity(storage);

            solver.solve(storage, A_inv);
        }

        public static void invert(LinearSolverDense<ZMatrixRMaj> solver, ZMatrixRMaj A, ZMatrixRMaj A_inv)
        {

            A_inv.reshape(A.numRows, A.numCols);

            CommonOps_ZDRM.setIdentity(A_inv);

            solver.solve(A_inv, A_inv);
        }
    }
}