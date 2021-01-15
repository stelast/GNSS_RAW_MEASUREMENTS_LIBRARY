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
    public interface CholeskyDecomposition_F64<T> : CholeskyDecomposition<T>
        where T: Matrix
    {
        /**
         * Computes the matrix's determinant using the decomposition.
         *
         * @return The determinant.
         */
        Complex_F64 computeDeterminant();
    }
}