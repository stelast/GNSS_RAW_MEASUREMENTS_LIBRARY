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
    public interface EigenDecomposition<T> : DecompositionInterface<T>
        where T:Matrix
    {
        /**
         * Returns the number of eigenvalues/eigenvectors.  This is the matrix's dimension.
         *
         * @return number of eigenvalues/eigenvectors.
         */
        int getNumberOfEigenvalues();


        /**
         * <p>
         * Used to retrieve real valued eigenvectors.  If an eigenvector is associated with a complex eigenvalue
         * then null is returned instead.
         * </p>
         *
         * @param index Index of the eigenvalue eigenvector pair.
         * @return If the associated eigenvalue is real then an eigenvector is returned, null otherwise.
         */
        T getEigenVector(int index);
    }
}