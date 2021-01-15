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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.linsol
{
    public interface LinearSolverDense<T> : LinearSolver<T,T> 
        where T: Matrix
    {
        /**
         * Computes the inverse of of the 'A' matrix passed into {@link #setA(Matrix)}
         * and writes the results to the provided matrix.  If 'A_inv' needs to be different from 'A'
         * is implementation dependent.
         *
         * @param A_inv Where the inverted matrix saved. Modified.
         */
        void invert(T A_inv);
    }
}