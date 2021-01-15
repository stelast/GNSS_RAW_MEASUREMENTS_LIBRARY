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
    public interface TridiagonalSimilarDecomposition_F64<T> : TridiagonalSimilarDecomposition<T>
        where T:Matrix
    {
        /**
         * Extracts the diagonal and off diagonal elements of the decomposed tridiagonal matrix.
         * Since it is symmetric only one off diagonal array is returned.
         *
         * @param diag Diagonal elements. Modified.
         * @param off off diagonal elements. Modified.
         */
        void getDiagonal(double[] diag, double[] off);
    }
}