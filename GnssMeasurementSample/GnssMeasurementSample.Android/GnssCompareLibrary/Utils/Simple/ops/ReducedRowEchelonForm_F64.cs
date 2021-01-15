using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public interface ReducedRowEchelonForm_F64<T> : ReducedRowEchelonForm<T>
        where T: Matrix
    {
        /**
         * Specifies tolerance for determining if the system is singular and it should stop processing.
         * A reasonable value is: tol = EPS/max(||tol||).
         *
         * @param tol Tolerance for singular matrix. A reasonable value is: tol = EPS/max(||tol||). Or just set to zero.
         */
        void setTolerance(double tol);
    }
}