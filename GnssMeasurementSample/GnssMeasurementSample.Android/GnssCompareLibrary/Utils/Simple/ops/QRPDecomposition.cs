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
    public interface QRPDecomposition<T> : QRDecomposition<T>
        where T:Matrix
    {
        /**
         * Returns the rank as determined by the algorithm.  This is dependent upon a fixed threshold
         * and might not be appropriate for some applications.
         *
         * @return Matrix's rank
         */
        int getRank();

        /**
         * Ordering of each column after pivoting.   The current column i was original at column pivot[i].
         *
         * @return Order of columns.
         */
        int[] getColPivots();

        /**
         * Creates the column pivot matrix.
         *
         * @param P Optional storage for pivot matrix.  If null a new matrix will be created.
         * @return The pivot matrix.
         */
        T getColPivotMatrix(T P);
    }
}