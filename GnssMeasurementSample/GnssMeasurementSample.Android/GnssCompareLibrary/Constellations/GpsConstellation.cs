using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex;
using Java.Lang;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations
{
    public class GpsConstellation : Constellation
    {

        private readonly static char satType = 'G';
        private static readonly string NAME = "GPS L1";
        private static readonly string TAG = "GpsConstellation";
        private static double L1_FREQUENCY = 1.57542e9;
        private static double FREQUENCY_MATCH_RANGE = 0.1e9;

        private bool fullBiasNanosInitialized = false;
        private long FullBiasNanos;

        private Coordinates<Matrix> rxPos;
        protected double tRxGPS;
        protected double weekNumberNanos;
        private List<SatelliteParameters> unusedSatellites = new List<SatelliteParameters>();

        public double getWeekNumber()
        {
            return weekNumberNanos;
        }

        public double gettRxGPS()
        {
            return tRxGPS;
        }

        private static readonly GnssConstellationType constellationId = GnssConstellationType.Gps;
        private static double MASK_ELEVATION = 20; // degrees
        private static double MASK_CN0 = 10; // dB-Hz

        /**
         * Time of the measurement
         */
        private Time timeRefMsec;

        protected int visibleButNotUsed = 0;

        // Condition for the pseudoranges that takes into account a maximum uncertainty for the TOW
        // (as done in gps-measurement-tools MATLAB code)
        private static readonly int MAXTOWUNCNS = 50;                                     // [nanoseconds]

        private NavigationProducer rinexNavGps = null;

        /**
         * List holding observed satellites
         */
        protected List<SatelliteParameters> observedSatellites = new List<SatelliteParameters>();

        /**
         * Corrections which are to be applied to received pseudoranges
         */
        private List<Correction> corrections;

        public GpsConstellation()
        {
            // URL template from where the GPS ephemerides should be downloaded
            string IGN_NAVIGATION_HOURLY_ZIM2 = "ftp://igs.ensg.ign.fr/pub/igs/data/hourly/${yyyy}/${ddd}/zim2${ddd}${h}.${yy}n.Z";
            string NASA_NAVIGATION_HOURLY = "ftp://cddis.gsfc.nasa.gov/pub/gps/data/hourly/${yyyy}/${ddd}/hour${ddd}0.${yy}n.Z";
            string GARNER_NAVIGATION_AUTO_HTTP = "http://garner.ucsd.edu/pub/rinex/${yyyy}/${ddd}/auto${ddd}0.${yy}n.Z";
            string BKG_HOURLY_SUPER_SEVER = "ftp://igs.bkg.bund.de/IGS/BRDC/${yyyy}/${ddd}/brdc${ddd}0.${yy}n.Z";

            // Declare a RinexNavigation type object
            if (rinexNavGps == null)
            {
                var rn = new RinexNavigationGps(BKG_HOURLY_SUPER_SEVER);
                rinexNavGps = rn;
            }
        }


        override public void addCorrections(List<Correction> corrections)
        {
            lock (this) {
                this.corrections = corrections;
            }
        }


        override public Time getTime()
        {
            lock (this) {
                return timeRefMsec;
            }
        }


        override public string getName()
        {
            lock (this) {
                return NAME;
            }
        }

        public static bool approximateEqual(double a, double b, double eps)
        {
            return System.Math.Abs(a - b) < eps;
        }


        override public void updateMeasurements(GnssMeasurementsEvent evento) {

            lock (this) {
                visibleButNotUsed = 0;
                observedSatellites.Clear();
                unusedSatellites.Clear();
                GnssClock gnssClock = evento.Clock;
                long TimeNanos = gnssClock.TimeNanos;
                timeRefMsec = new Time(JavaSystem.CurrentTimeMillis());
                double BiasNanos = gnssClock.BiasNanos;
                double gpsTime, pseudorange;

                // Use only the first instance of the FullBiasNanos (as done in gps-measurement-tools)
                if (!fullBiasNanosInitialized) {
                    FullBiasNanos = gnssClock.FullBiasNanos;
                    fullBiasNanosInitialized = true;
                }


                // Start computing the pseudoranges using the raw data from the phone's GNSS receiver
                foreach (GnssMeasurement measurement in evento.Measurements) {

                    if (measurement.ConstellationType != constellationId) // si la senial no es de la gps, continue
                        continue;

                    if (measurement.HasCarrierFrequencyHz)
                        if (!approximateEqual(measurement.CarrierFrequencyHz, L1_FREQUENCY, FREQUENCY_MATCH_RANGE))
                            continue;

                        // excluding satellites which don't have the L5 component
                        //                if(measurement.getSvid() == 2 || measurement.getSvid() == 4
                        //                        || measurement.getSvid() == 5 || measurement.getSvid() == 7
                        //                        || measurement.getSvid() == 11 || measurement.getSvid() == 12
                        //                        || measurement.getSvid() == 13 || measurement.getSvid() == 14
                        //                        || measurement.getSvid() == 15 || measurement.getSvid() == 16
                        //                        || measurement.getSvid() == 17 || measurement.getSvid() == 18
                        //                        || measurement.getSvid() == 19 || measurement.getSvid() == 20
                        //                        || measurement.getSvid() == 21 || measurement.getSvid() == 22
                        //                        || measurement.getSvid() == 23 || measurement.getSvid() == 28
                        //                        || measurement.getSvid() == 29 || measurement.getSvid() == 31)
                        //                    continue;


                        long ReceivedSvTimeNanos = measurement.ReceivedSvTimeNanos;
                        double TimeOffsetNanos = measurement.TimeOffsetNanos;


                        // GPS Time generation (GSA White Paper - page 20)
                        gpsTime = TimeNanos - (FullBiasNanos + BiasNanos); // TODO intersystem bias?

                        // Measurement time in full GPS time without taking into account weekNumberNanos(the number of
                        // nanoseconds that have occurred from the beginning of GPS time to the current
                        // week number)
                        tRxGPS =
                                gpsTime + TimeOffsetNanos;


                        weekNumberNanos =
                                System.Math.Floor((-1.0 * FullBiasNanos) / Constants.NUMBER_NANO_SECONDS_PER_WEEK)
                                        * Constants.NUMBER_NANO_SECONDS_PER_WEEK;

                        // GPS pseudorange computation
                        pseudorange =
                                (tRxGPS - weekNumberNanos - ReceivedSvTimeNanos) / 1.0E9
                                        * Constants.SPEED_OF_LIGHT;

                        // TODO Check that the measurement have a valid state such that valid pseudoranges are used in the PVT algorithm

                        /*

                        According to https://developer.android.com/ the GnssMeasurements States required
                        for GPS valid pseudoranges are:

                        int STATE_CODE_LOCK         = 1      (1 << 0)
                        int int STATE_TOW_DECODED   = 8      (1 << 3)

                        */

                        GnssState measState = measurement.State;

                        // Bitwise AND to identify the states
                        bool codeLock = (measState & GnssState.CodeLock) != GnssState.Unknown;
                        bool towDecoded = (measState & GnssState.TowDecoded) != 0;
                        bool towKnown = false;
                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
                        {
                            towKnown = (measState & GnssState.TowKnown) != 0;
                        }
                        //                bool towUncertainty = measurement.getReceivedSvTimeUncertaintyNanos() <  MAXTOWUNCNS;


                        if (codeLock && (towDecoded || towKnown) && pseudorange < 1e9)
                        { // && towUncertainty
                            SatelliteParameters satelliteParameters = new SatelliteParameters(
                                    measurement.Svid,
                                    new Pseudorange(pseudorange, 0.0));

                            satelliteParameters.setUniqueSatId("G" + satelliteParameters.getSatId() + "_L1");

                            satelliteParameters.setSignalStrength(measurement.Cn0DbHz);

                            satelliteParameters.setConstellationType(measurement.ConstellationType);

                            if (measurement.HasCarrierFrequencyHz)
                                satelliteParameters.setCarrierFrequency(measurement.CarrierFrequencyHz);

                            observedSatellites.Add(satelliteParameters);

                            //Log.d(TAG, "updateConstellations(" + measurement.Svid + "): " + weekNumberNanos + ", " + tRxGPS + ", " + pseudorange);
                            //Log.d(TAG, "updateConstellations: Passed with measurement state: " + measState);
                        }
                        else
                        {
                            SatelliteParameters satelliteParameters = new SatelliteParameters(
                                measurement.Svid,
                                null);

                            satelliteParameters.setUniqueSatId("G" + satelliteParameters.getSatId() + "_L1");

                            satelliteParameters.setSignalStrength(measurement.Cn0DbHz);

                            satelliteParameters.setConstellationType(measurement.ConstellationType);

                            if (measurement.HasCarrierFrequencyHz)
                                satelliteParameters.setCarrierFrequency(measurement.CarrierFrequencyHz);

                            unusedSatellites.Add(satelliteParameters);
                            visibleButNotUsed++;
                        }
                    }
                }
            }


        override public double getSatelliteSignalStrength(int index)
        {
            lock (this) {
                return observedSatellites[index].getSignalStrength();
            }
        }


        override public GnssConstellationType getConstellationId()
        {
            lock (this) {
                return constellationId;
            }
        }



        override public void calculateSatPosition(Location initialLocation, Coordinates<Matrix> position)
        {

            //// Make a list to hold the satellites that are to be excluded based on elevation/CN0 masking criteria
            List<SatelliteParameters> excludedSatellites = new List<SatelliteParameters>();

            lock (this) {

                rxPos = Coordinates<Matrix>.globalXYZInstance(position.getX(), position.getY(), position.getZ());

                foreach (SatelliteParameters observedSatellite in observedSatellites) {
                    // Computation of the GPS satellite coordinates in ECEF frame

                    // Determine the current GPS week number
                    int gpsWeek = (int)(weekNumberNanos / Constants.NUMBER_NANO_SECONDS_PER_WEEK); 

                    // Time of signal reception in GPS Seconds of the Week (SoW)
                    double gpsSow = (tRxGPS - weekNumberNanos) * 1e-9;
                    Time tGPS = new Time(gpsWeek, gpsSow);

                    // Convert the time of reception from GPS SoW to UNIX time (milliseconds)
                    long timeRx = tGPS.getMsec();

                    SatellitePosition rnp = ((RinexNavigationGps)rinexNavGps).getSatPositionAndVelocities(
                        timeRx,
                        observedSatellite.getPseudorange(),
                        observedSatellite.getSatId(),
                        satType,
                        0.0,
                        initialLocation);

                    if (rnp == null)
                    {
                        excludedSatellites.Add(observedSatellite);
                        //GnssCoreService.notifyUser("Failed getting ephemeris data!", Snackbar.LengthShort, RNP_NULL_MESSAGE);
                        continue;
                    }

                    observedSatellite.setSatellitePosition(rnp);

                    observedSatellite.setRxTopo(
                            new TopocentricCoordinates<Matrix>(
                                    rxPos,
                                    observedSatellite.getSatellitePosition()));

            //        // Add to the exclusion list the satellites that do not pass the masking criteria
                    if (observedSatellite.getRxTopo().getElevation() < MASK_ELEVATION)
                    {
                        excludedSatellites.Add(observedSatellite);
                    }

                    double accumulatedCorrection = 0;

                    foreach (Correction correction in corrections)
                    {

                        correction.calculateCorrection(
                                new Java.Sql.Time(timeRx),
                                rxPos,
                                observedSatellite.getSatellitePosition(),
                                rinexNavGps,
                                initialLocation);

                        accumulatedCorrection += correction.getCorrection();
                    }

                    observedSatellite.setAccumulatedCorrection(accumulatedCorrection);
                }

                // Remove from the list all the satellites that did not pass the masking criteria
                visibleButNotUsed += excludedSatellites.Count();
                foreach(SatelliteParameters sp in excludedSatellites)
                {
                    observedSatellites.Remove(sp);
                }
                unusedSatellites.AddRange(excludedSatellites);
            }
        }


        public static void registerClass()
        {
            //register(
            //        NAME,
            //        GpsConstellation.class);
        }



        override public Coordinates<Matrix> getRxPos()
        {
            lock (this) {
                return rxPos;
            }
        }


        override public void setRxPos(Coordinates<Matrix> rxPos)
        {
            lock (this) {
                this.rxPos = rxPos;
            }
        }


        override public SatelliteParameters getSatellite(int index)
        {
            lock (this) {
                return observedSatellites[index];
            }
        }


        override public List<SatelliteParameters> getSatellites()
        {
            lock (this) {
                return observedSatellites;
            }
        }


        override public List<SatelliteParameters> getUnusedSatellites()
        {
            return unusedSatellites;
        }


        override public int getVisibleConstellationSize()
        {
            lock (this) {
                return getUsedConstellationSize() + visibleButNotUsed;
            }
        }


        override public int getUsedConstellationSize()
        {
            lock (this) {
                return observedSatellites.Count();
            }
        }

    }
}