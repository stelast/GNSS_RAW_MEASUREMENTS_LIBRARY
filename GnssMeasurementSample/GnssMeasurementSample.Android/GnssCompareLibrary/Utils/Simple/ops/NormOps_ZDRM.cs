using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class NormOps_ZDRM
    {
        /**
         * <p>
         * Computes the Frobenius matrix norm:<br>
         * <br>
         * normF = Sqrt{  &sum;<sub>i=1:m</sub> &sum;<sub>j=1:n</sub> { a<sub>ij</sub><sup>2</sup>}   }
         * </p>
         * <p>
         * This is equivalent to the element wise p=2 norm.
         * </p>
         *
         * @param a The matrix whose norm is computed.  Not modified.
         * @return The norm's value.
         */
        public static double normF(ZMatrixRMaj a)
        {
            double total = 0;

            double scale = CommonOps_ZDRM.elementMaxAbs(a);

            if (scale == 0.0)
                return 0.0;

            int size = a.DataLength;

            for (int i = 0; i < size; i += 2)
            {
                double real = a.data[i] / scale;
                double imag = a.data[i + 1] / scale;

                total += real * real + imag * imag;
            }

            return scale * (double)Math.Sqrt(total);
        }
    }
}