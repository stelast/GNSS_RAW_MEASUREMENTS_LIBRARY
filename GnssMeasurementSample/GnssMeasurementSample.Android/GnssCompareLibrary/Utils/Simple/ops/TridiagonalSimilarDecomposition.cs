using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public interface TridiagonalSimilarDecomposition<T> : DecompositionInterface<T>
        where T:Matrix
    {

        /**
         * Extracts the tridiagonal matrix found in the decomposition.
         *
         * @param T If not null then the results will be stored here.  Otherwise a new matrix will be created.
         * @return The extracted T matrix.
         */
        MatrixType getT(MatrixType T);

        /**
         * An orthogonal matrix that has the following property: T = Q<sup>H</sup>AQ
         *
         * @param Q If not null then the results will be stored here.  Otherwise a new matrix will be created.
         * @param transposed If true then the transpose (real) or conjugate transpose (complex) of Q is returned.
         * @return The extracted Q matrix.
         */
        MatrixType getQ(MatrixType Q, bool transposed);
    }
}