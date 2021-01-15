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
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Corrections
{
    public class IonoCorrection : Correction
    {
        private double correctionValue;

        private readonly static String NAME = "Klobuchar Iono Correction";
        public override void calculateCorrection(Time currentTime, Coordinates<Matrix> approximatedPose, SatellitePosition satelliteCoordinates, NavigationProducer navigationProducer, Location initialLocation)
        {
            DateTime foo = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            IonoGps iono = navigationProducer.getIono(unixTime, initialLocation);

            if (iono.getBeta(0) == 0)
            {

                correctionValue = 0.0;

            }
            else
            {


                // Compute the elevation and azimuth angles for each satellite
                TopocentricCoordinates<Matrix> topo = new TopocentricCoordinates<Matrix>();
                topo.computeTopocentric(approximatedPose, satelliteCoordinates);

                // Assign the elevation and azimuth information to new variables
                double elevation = topo.getElevation();
                double azimuth = topo.getAzimuth();

                double ionoCorr = 0;

                if (iono == null)
                    return;
                //		    double a0 = navigation.getIono(currentTime.getMsec(),0);
                //		    double a1 = navigation.getIono(currentTime.getMsec(),1);
                //		    double a2 = navigation.getIono(currentTime.getMsec(),2);
                //		    double a3 = navigation.getIono(currentTime.getMsec(),3);
                //		    double b0 = navigation.getIono(currentTime.getMsec(),4);
                //		    double b1 = navigation.getIono(currentTime.getMsec(),5);
                //		    double b2 = navigation.getIono(currentTime.getMsec(),6);
                //		    double b3 = navigation.getIono(currentTime.getMsec(),7);

                elevation = Math.Abs(elevation);

                // Parameter conversion to semicircles
                double lon = approximatedPose.getGeodeticLongitude() / 180; // geod.get(0)
                double lat = approximatedPose.getGeodeticLatitude() / 180; //geod.get(1)
                azimuth = azimuth / 180;
                elevation = elevation / 180;

                // Klobuchar algorithm

                // Compute the slant factor
                double f = 1 + 16 * Math.Pow((0.53 - elevation), 3);

                // Compute the earth-centred angle
                double psi = 0.0137 / (elevation + 0.11) - 0.022;

                // Compute the latitude of the Ionospheric Pierce Point (IPP)
                double phi = lat + psi * Math.Cos(azimuth * Math.PI);

                if (phi > 0.416)
                {
                    phi = 0.416;

                }
                if (phi < -0.416)
                {
                    phi = -0.416;
                }

                // Compute the longitude of the IPP
                double lambda = lon + (psi * Math.Sin(azimuth * Math.PI))
                        / Math.Cos(phi * Math.PI);

                // Find the geomagnetic latitude of the IPP
                double ro = phi + 0.064 * Math.Cos((lambda - 1.617) * Math.PI);

                // Find the local time at the IPP
                double t = lambda * 43200 + unixTime;

                while (t >= 86400)
                    t = t - 86400;

                while (t < 0)
                    t = t + 86400;

                // Compute the period of ionospheric delay
                double p = iono.getBeta(0) + iono.getBeta(1) * ro + iono.getBeta(2) * Math.Pow(ro, 2) + iono.getBeta(3) * Math.Pow(ro, 3);

                if (p < 72000)
                    p = 72000;

                // Compute the amplitude of ionospheric delay
                double a = iono.getAlpha(0) + iono.getAlpha(1) * ro + iono.getAlpha(2) * Math.Pow(ro, 2) + iono.getAlpha(3) * Math.Pow(ro, 3);

                if (a < 0)
                    a = 0;

                // Compute the phase of ionospheric delay
                double x = (2 * Math.PI * (t - 50400)) / p;

                // Compute the ionospheric correction
                if (Math.Abs(x) < 1.57)
                {
                    ionoCorr = Constants.SPEED_OF_LIGHT
                            * f
                            * (5e-9 + a
                            * (1 - (Math.Pow(x, 2)) / 2 + (Math.Pow(x, 4)) / 24));
                }
                else
                {
                    ionoCorr = Constants.SPEED_OF_LIGHT * f * 5e-9;
                }

                correctionValue = ionoCorr;
            }
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
            //register(NAME, IonoCorrection.class);
    }
}
}