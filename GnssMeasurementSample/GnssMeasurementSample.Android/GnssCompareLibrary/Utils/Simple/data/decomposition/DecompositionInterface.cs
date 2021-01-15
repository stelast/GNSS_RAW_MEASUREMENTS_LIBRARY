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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition
{
    public interface DecompositionInterface<T>
        where T:Matrix
    {
        /**
         * Computes the decomposition of the input matrix.  Depending on the implementation
         * the input matrix might be stored internally or modified.  If it is modified then
         * the function {@link #inputModified()} will return true and the matrix should not be
         * modified until the decomposition is no longer needed.
         *
         * @param orig The matrix which is being decomposed.  Modification is implementation dependent.
         * @return Returns if it was able to decompose the matrix.
         */
        bool decompose(T orig);

        /**
         * Is the input matrix to {@link #decompose(org.ejml.data.Matrix)} is modified during
         * the decomposition process.
         *
         * @return true if the input matrix to decompose() is modified.
         */
        bool inputModified();
    }
}