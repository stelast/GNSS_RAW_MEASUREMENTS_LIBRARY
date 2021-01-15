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
    public interface EigenDecomposition_F64<T> : EigenDecomposition<T>
        where T : Matrix
    {
        /**
         * <p>
         * Returns an eigenvalue as a complex number.  For symmetric matrices the returned eigenvalue will always be a real
         * number, which means the imaginary component will be equal to zero.
         * </p>
         *
         * <p>
         * NOTE: The order of the eigenvalues is dependent upon the decomposition algorithm used.  This means that they may
         * or may not be ordered by magnitude.  For example the QR algorithm will returns results that are partially
         * ordered by magnitude, but this behavior should not be relied upon.
         * </p>
         * 
         * @param index Index of the eigenvalue eigenvector pair.
         * @return An eigenvalue.
         */
        Complex_F64 getEigenvalue(int index);
    }
}