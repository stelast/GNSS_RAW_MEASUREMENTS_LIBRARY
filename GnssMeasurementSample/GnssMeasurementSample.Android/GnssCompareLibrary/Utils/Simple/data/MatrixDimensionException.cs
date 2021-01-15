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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data
{
    /**
     * If two matrices did not have compatible dimensions for the operation this exception
     * is thrown.
     *
     * @author Peter Abeles
     */
    public class MatrixDimensionException : SystemException
    {
    public MatrixDimensionException() { }

    public MatrixDimensionException(String message) : base(message)
    {
    }
}
}