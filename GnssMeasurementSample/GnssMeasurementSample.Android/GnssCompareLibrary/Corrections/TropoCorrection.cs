using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Corrections
{
    public class TropoCorrection : Correction
    {
        private readonly static String NAME = "Tropospheric correction";

        private double correctionValue;
        public override void calculateCorrection(Time currentTime, Coordinates<Matrix> approximatedPose, SatellitePosition satelliteCoordinates, NavigationProducer navigationProducer, Location initialLocation)
        {
            // Compute also the geodetic version of the user position (latitude, longitude, height)
            approximatedPose.computeGeodetic();

            // Get the user's height
            double height = approximatedPose.getGeodeticHeight();

            // Compute the elevation and azimuth angles for each satellite
            TopocentricCoordinates<Matrix> topo = new TopocentricCoordinates<Matrix>();
            topo.computeTopocentric(approximatedPose, satelliteCoordinates);

            // Assign the elevation information to a new variable
            double elevation = topo.getElevation();

            double tropoCorr = 0;

            if (height > 5000)
                return;

            elevation = Math.Abs(elevation) / 180.0 * Math.PI;
            if (elevation == 0)
            {
                elevation = elevation + 0.01;
            }

            // Numerical constants and tables for Saastamoinen algorithm
            // (troposphere correction)
            double hr = 50.0;
            int[] ha = { 0, 500, 1000, 1500, 2000, 2500, 3000, 4000, 5000 };
            double[] ba = { 1.156, 1.079, 1.006, 0.938, 0.874, 0.813, 0.757, 0.654, 0.563 };

            // Saastamoinen algorithm
            double P = Constants.STANDARD_PRESSURE * Math.Pow((1 - 0.0000226 * height), 5.225);
            double T = Constants.STANDARD_TEMPERATURE - 0.0065 * height;
            double H = hr * Math.Exp(-0.0006396 * height);

            // If height is below zero, keep the maximum correction value
            double B = ba[0];
            // Otherwise, interpolate the tables
            if (height >= 0)
            {
                int i = 1;
                while (height > ha[i])
                {
                    i++;
                }
                double m = (ba[i] - ba[i - 1]) / (ha[i] - ha[i - 1]);
                B = ba[i - 1] + m * (height - ha[i - 1]);
            }

            double e = 0.01
                    * H
                    * Math.Exp(-37.2465 + 0.213166 * T - 0.000256908
                    * Math.Pow(T, 2));

            tropoCorr = ((0.002277 / Math.Sin(elevation))
                    * (P - (B / Math.Pow(Math.Tan(elevation), 2))) + (0.002277 / Math.Sin(elevation))
                    * (1255 / T + 0.05) * e);

            correctionValue = tropoCorr;
        }

        public override double getCorrection()
        {
            return correctionValue;
        }

        public override string getName()
        {
            return NAME;
        }
        public static void registerClass()
        {
            //register(NAME, TropoCorrection.get);
        }
    }
}