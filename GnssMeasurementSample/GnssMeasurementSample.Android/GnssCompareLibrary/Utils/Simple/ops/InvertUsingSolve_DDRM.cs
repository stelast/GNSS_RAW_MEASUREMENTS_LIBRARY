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
    public class InvertUsingSolve_DDRM
    {

        public static void invert(LinearSolverDense<DMatrixRMaj> solver, DMatrix1Row A, DMatrixRMaj A_inv, DMatrixRMaj storage)
        {

            if (A.numRows != A_inv.numRows || A.numCols != A_inv.numCols)
            {
                throw new ArgumentException("A and A_inv must have the same dimensions");
            }

            CommonOps_DDRM.setIdentity(storage);

            solver.solve(storage, A_inv);
        }

        public static void invert(LinearSolverDense<DMatrixRMaj> solver, DMatrix1Row A, DMatrixRMaj A_inv)
        {

            if (A.numRows != A_inv.numRows || A.numCols != A_inv.numCols)
            {
                throw new ArgumentException("A and A_inv must have the same dimensions");
            }

            CommonOps_DDRM.setIdentity(A_inv);

            solver.solve(A_inv, A_inv);
        }
    }
}