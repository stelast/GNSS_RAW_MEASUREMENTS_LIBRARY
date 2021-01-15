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
    public interface ReducedRowEchelonForm<T> where T:Matrix
    {
        /**
         * Puts the augmented matrix into RREF.  The coefficient matrix is stored in
         * columns less than coefficientColumns.
         *
         *
         * @param A Input: Augmented matrix.  Output: RREF.  Modified.
         * @param coefficientColumns Number of coefficients in the system matrix.
         */
        void reduce(T A, int coefficientColumns);
    }
}