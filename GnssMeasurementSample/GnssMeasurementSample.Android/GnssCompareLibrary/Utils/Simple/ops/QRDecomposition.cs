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
    public interface QRDecomposition<T> : DecompositionInterface<T>
        where T:Matrix
    {
        /**
         * <p>
         * Returns the Q matrix from the decomposition.  Should only be called after
         * {@link #decompose(org.ejml.data.Matrix)} has been called.
         * </p>
         *
         * @param Q (Input) Storage for Q. Reshaped to correct size automatically. If null a new matrix is created.
         * @param compact If true an m by n matrix is created, otherwise n by n.
         * @return The Q matrix.
         */
        T getQ(T Q, bool compact);

        /**
         * <p>
         * Returns the R matrix from the decomposition.  Should only be
         * called after {@link #decompose(org.ejml.data.Matrix)} has been.
         * </p>
         * <p>
         * If setZeros is true then an n &times; m matrix is required and all the elements are set.
         * If setZeros is false then the matrix must be at least m &times; m and only the upper triangular
         * elements are set.
         * </p>
         *
         * @param R (Input) Storage for R. Reshaped to correct size automatically. If null a new matrix is created.
         * @param compact If true only the upper triangular elements are set
         * @return The R matrix.
         */
        T getR(T R, bool compact);
    }
}