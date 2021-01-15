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
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public interface LUDecomposition_F64<T>: LUDecomposition<T>
        where T: Matrix
    {
        /**
         * Computes the matrix's determinant using the LU decomposition.
         *
         * @return The determinant.
         */
        Complex_F64 computeDeterminant();
    }
}