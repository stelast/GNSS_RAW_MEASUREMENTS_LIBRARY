using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public interface CholeskyDecomposition<A> : DecompositionInterface<A>
        where A:Matrix
    {
        /**
         * If true the decomposition was for a lower triangular matrix.
         * If false it was for an upper triangular matrix.
         *
         * @return True if lower, false if upper.
         */
        bool isLower();

        /**
         * <p>
         * Returns the triangular matrix from the decomposition.
         * </p>
         *
         * <p>
         * If an input is provided that matrix is used to write the results to.
         * Otherwise a new matrix is created and the results written to it.
         * </p>
         *
         * @param T If not null then the decomposed matrix is written here.
         * @return A lower or upper triangular matrix.
         */
        A getT(A T);
    }
}