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
    public class LinearSolverQrBlock64_DDRM : LinearSolver_DDRB_to_DDRM
    {
        public LinearSolverQrBlock64_DDRM() : base(new QrHouseHolderSolver_DDRB())
        {
        }

        public LinearSolverQrBlock64_DDRM(LinearSolverDense<DMatrixRBlock> alg) : base(alg)
        {
        }
    }
}