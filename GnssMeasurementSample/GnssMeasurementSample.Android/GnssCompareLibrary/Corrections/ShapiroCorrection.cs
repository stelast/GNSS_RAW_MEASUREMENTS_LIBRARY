using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Corrections
{
    public class ShapiroCorrection : Correction
    {
        private readonly static String NAME = "Relativistic path range correction";

        private double correctionValue;

        public ShapiroCorrection() : base()
        {
        }
        public override void calculateCorrection(Time currentTime, Coordinates<Matrix> approximatedPose, SatellitePosition satelliteCoordinates, NavigationProducer navigationProducer, Location initialLocation)
        {
            // Compute the difference vector between the receiver and the satellite
            SimpleMatrix<Matrix> diff = approximatedPose.minusXYZ(satelliteCoordinates);

            // Compute the geometric distance between the receiver and the satellite

            double geomDist = Math.Sqrt(Math.Pow(diff.get(0), 2) + Math.Pow(diff.get(1), 2) + Math.Pow(diff.get(2), 2));

            // Compute the geocentric distance of the receiver
            double geoDistRx = Math.Sqrt(Math.Pow(approximatedPose.getX(), 2) + Math.Pow(approximatedPose.getY(), 2) + Math.Pow(approximatedPose.getZ(), 2));

            // Compute the geocentric distance of the satellite
            double geoDistSv = Math.Sqrt(Math.Pow(satelliteCoordinates.getX(), 2) + Math.Pow(satelliteCoordinates.getY(), 2) + Math.Pow(satelliteCoordinates.getZ(), 2));


            // Compute the shapiro correction
            correctionValue = ((2.0 * Constants.EARTH_GRAVITATIONAL_CONSTANT) / Math.Pow(Constants.SPEED_OF_LIGHT, 2)) * Math.Log((geoDistSv + geoDistRx + geomDist) / (geoDistSv + geoDistRx - geomDist));

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
            //register(NAME, ShapiroCorrection.class);
        }
    }
}