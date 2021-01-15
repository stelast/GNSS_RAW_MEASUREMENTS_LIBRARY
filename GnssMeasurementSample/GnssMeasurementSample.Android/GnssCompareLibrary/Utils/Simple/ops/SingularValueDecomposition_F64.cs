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
    public interface SingularValueDecomposition_F64<T> : SingularValueDecomposition<T>
        where T:Matrix
    {
        /**
         * Returns the singular values.  This is the diagonal elements of the W matrix in the decomposition.
         * <b>Ordering of singular values is not guaranteed.</b>.
         * 
         * @return Singular values. Note this array can be longer than the number of singular values.
         * Extra elements have no meaning.
         */
        double[] getSingularValues();
    }
}