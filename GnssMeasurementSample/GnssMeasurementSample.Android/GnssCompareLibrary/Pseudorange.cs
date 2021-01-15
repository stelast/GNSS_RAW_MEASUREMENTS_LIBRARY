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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary
{
    public class Pseudorange
    {

        /**
         * Pseudorange
         */
        private double pseudorange;

        /**
         * Pseudorange rate
         */
        private double pseudorangeRate;

        public Pseudorange(double pseudorange,
                           double pseudorangeRate)
        {
            this.pseudorange = pseudorange;
            this.pseudorangeRate = pseudorangeRate;

        }

        /**
         * Variance of the measurement. Updated by the Constellation object, based on
         * the satellite's topocentric coordinates
         */
        private double measurementVariance = 0;

        public double getPseudorange()
        {
            return pseudorange;
        }

        public double getPseudorangeRate()
        {
            return pseudorangeRate;
        }


        public double getMeasurementVariance()
        {
            return measurementVariance;
        }

        public void setMeasurementVariance(double measurementVariance)
        {
            this.measurementVariance = measurementVariance;
        }
    }
}