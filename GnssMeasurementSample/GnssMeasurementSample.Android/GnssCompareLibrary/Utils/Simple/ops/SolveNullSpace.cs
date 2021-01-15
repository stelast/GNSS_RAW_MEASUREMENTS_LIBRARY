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
    public interface SolveNullSpace<T>
        where T:Matrix
    {
        /**
         * Finds the nullspace inside of input
         *
         * @param input (Input) input matrix. Maybe modified
         * @param numberOfSingular Number of singular values in the input
         * @param nullspace (Output) storage for null space
         * @return true if successful or false if it failed
         */
        bool process(T input, int numberOfSingular, T nullspace);

        /**
         * Returns true if the input matrix is modified
         */
        bool inputModified();
    }
}