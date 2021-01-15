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
    public interface QRPDecomposition_F64<T> : QRPDecomposition<T>
        where T:Matrix
    {
        /**
         * <p>
         * Specifies the threshold used to flag a column as being singular.  The specified threshold is relative
         * and will very depending on the system.  The default value is UtilEJML.EPS.
         * </p>
         *
         * @param threshold Singular threshold.
         */
        void setSingularThreshold(double threshold);
    }
}