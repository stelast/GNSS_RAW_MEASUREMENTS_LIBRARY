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
    public class CovarianceRandomDraw_DDRM
    {
        private DMatrixRMaj A;
        private Java.Util.Random rand;
        private DMatrixRMaj r;

        /**
         * Creates a random distribution with the specified mean and covariance.  The references
         * to the variables are not saved, their value are copied.
         *
         * @param rand Used to create the random numbers for the draw. Reference is saved.
         * @param cov The covariance of the distribution.  Not modified.
         */
        public CovarianceRandomDraw_DDRM(Random rand, DMatrixRMaj cov)
        {
            r = new DMatrixRMaj(cov.numRows, 1);
            CholeskyDecompositionInner_DDRM cholesky = new CholeskyDecompositionInner_DDRM(true);

            if (cholesky.inputModified())
                cov = cov.copy();
            if (!cholesky.decompose(cov))
                throw new SystemException("Decomposition failed!");

            A = cholesky.getT();
            //this.rand = rand;
            this.rand = new Java.Util.Random();
        }

        /**
         * Makes a draw on the distribution.  The results are added to parameter 'x'
         */
        public void next(DMatrixRMaj x)
        {
            for (int i = 0; i < r.numRows; i++)
            {
                r.set(i, 0, (double)rand.NextGaussian());
            }

            CommonOps_DDRM.multAdd(A, r, x);
        }

        /**
         * Computes the likelihood of the random draw
         *
         * @return The likelihood.
         */
        public double computeLikelihoodP()
        {
            double ret = 1.0;

            for (int i = 0; i < r.numRows; i++)
            {
                double a = r.get(i, 0);

                ret *= Math.Exp(-a * a / 2.0);
            }

            return ret;
        }
    }
}