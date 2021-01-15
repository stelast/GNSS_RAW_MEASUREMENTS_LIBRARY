using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations
{
    public class GalileoConstellation : Constellation
    {
        private readonly static char satType = 'E';
        protected static readonly string NAME = "Galileo E1";
        private static readonly string TAG = "GalileoE1Constellation";
        private static GnssConstellationType constellationId = GnssConstellationType.Galileo;
        private static readonly double E1a_FREQUENCY = 1.57542e9;
        private static readonly double FREQUENCY_MATCH_RANGE = 0.1e9;
        private static readonly double MASK_ELEVATION = 15; // degrees
        private static readonly double MASK_CN0 = 10; // dB-Hz


        private bool fullBiasNanosInitialized = false;
        private long FullBiasNanos;

        private Coordinates<Matrix> rxPos;

        protected double tRxGalileoTOW;
        private double tRxGalileoE1_2nd;
        protected double weekNumber;

        public double getWeekNumber()
        {
            return weekNumber;
        }

        public double gettRxGalileoTOW()
        {
            return tRxGalileoTOW;
        }

        /**
         * Time of the measurement
         */
        private Time timeRefMsec;

        protected int visibleButNotUsed = 0;

        private static readonly int MAXTOWUNCNS = 50;                            
        // [nanoseconds]

        /**
         * List holding used satellites
         */
        protected List<SatelliteParameters> observedSatellites = new List<SatelliteParameters>();

        /**
         * List holding unused satellites
         */
        protected List<SatelliteParameters> unusedSatellites = new List<SatelliteParameters>();


        //    private long timeRx;

        private NavigationProducer rinexNavGalileo = null;

        /**
         * Corrections which are to be applied to received pseudoranges
         */
        private List<Correction> corrections = new List<Correction>();

        public GalileoConstellation()
        {
            // URL template from where the Galileo ephemerides should be downloaded
            //string GNSS_BEV_GALILEO_RINEX = "ftp://gnss.bev.gv.at/pub/nrt/${ddd}/${yy}/bute${ddd}s.${yy}l.Z";
            string IGS_GALILEO_RINEX = "ftp://igs.bkg.bund.de/IGS/BRDC/${yyyy}/${ddd}/BRDC00WRD_R_${yyyy}${ddd}0000_01D_EN.rnx.gz";
            //string EUREF_GALILEO_RINEX = "ftp://igs.bkg.bund.de/EUREF/BRDC/${yyyy}/${ddd}/BRDC00WRD_R_${yyyy}${ddd}0000_01D_EN.rnx.gz";


            // Declare a RinexNavigation type object
            //if (rinexNavGalileo == null)
            //    rinexNavGalileo = new RinexNavigationGalileo(IGS_GALILEO_RINEX);
        }

        public static bool approximateEqual(double a, double b, double eps)
        {
            return System.Math.Abs(a - b) < eps;
        }


        public override void addCorrections(List<Correction> corrections)
        {
            lock(this) {
                this.corrections = corrections;
            }
        }

        public override void calculateSatPosition(Location initialLocation, Coordinates<Matrix> position)
        {
            // Make a list to hold the satellites that are to be excluded based on elevation/CN0 masking criteria
            List<SatelliteParameters> excludedSatellites = new List<SatelliteParameters>();

            lock(this) {
                rxPos = Coordinates<Matrix>.globalXYZInstance(position.getX(), position.getY(), position.getZ());
                foreach (SatelliteParameters observedSatellite in observedSatellites)
                {

                    /*
                      Computation of the Galileo satellite coordinates in ECEF frame
                    */

                    // Determine the current Galileo week number (info: is the same as GPS week number)
                    // todo: confirm difference to github fork
                    int galileoWeek = (int)weekNumber;

                    // Time of signal reception in Galileo Seconds of the Week (SoW)
                    double galileoSow = (tRxGalileoTOW) * 1e-9;
                    Time tGalileo = new Time(galileoWeek, galileoSow);

                    // Convert the time of reception from GPS SoW to UNIX time (milliseconds)
                    long timeRx = tGalileo.getMsec();


                    /*Compute the Galileo satellite coordinates

                     INPUT:
                     @param timeRx         = time of measurement reception - UNIX        [milliseconds]
                     @param pseudorange    = pseudorange measuremnent                          [meters]
                     @param satID          = satellite ID
                     @param satType        = satellite type indicating the constellation (E: Galileo)

                     */
                    SatellitePosition rnp = ((RinexNavigationGalileo)rinexNavGalileo).getGalileoSatPosition(
                            timeRx,
                            observedSatellite.getPseudorange(),
                            observedSatellite.getSatId(),
                            satType,
                            0.0,
                            initialLocation);

                    if (rnp == null)
                    {
                        excludedSatellites.Add(observedSatellite);
                        GnssCoreService.notifyUser("Failed getting ephemeris data!", Snackbar.LengthShort, RNP_NULL_MESSAGE);
                        continue;
                    }

                    //observedSatellite.setSatellitePosition(rnp);


                    /* Compute the azimuth and elevation w.r.t the user's approximate location

                     INPUT:
                     @param rxPos                = user's approximate ECEF coordinates       [cartesian]
                     @param satellitePosition    = satellite ECEF coordinates                [cartesian]

                     */
                    observedSatellite.setRxTopo(
                            new TopocentricCoordinates<Matrix>(
                                    rxPos,
                                    observedSatellite.getSatellitePosition()));


                    // Add to the exclusion list the satellites that do not pass the masking criteria
                    if (observedSatellite.getRxTopo().getElevation() < MASK_ELEVATION)
                    {
                        excludedSatellites.Add(observedSatellite);
                        continue;
                    }

                    // Initialize the variable to hold the results of the entire pseudorange correction models
                    double accumulatedCorrection = 0;

                    /* Compute the accumulated corrections for the pseudorange measurements
                     * Currently the accumulated corrections contain the following effects:
                     *                  - Ionosphere
                     *                  - Troposphere
                     *                  - Shapiro delay (i.e, relativistic path range correction)

                     INPUT:
                     @param timeRx               = time of measurement reception - UNIX   [milliseconds]
                     @param rxPos                = user's approximate ECEF coordinates       [cartesian]
                     @param satellitePosition    = satellite ECEF coordinates                [cartesian]
                     @param rinexNavGalileo      = RinexNavigationGalileo type object
                     */
                    //foreach (Correction correction in corrections)
                    //{

                    //    correction.calculateCorrection(
                    //            new Time(timeRx),
                    //            rxPos,
                    //            observedSatellite.getSatellitePosition(),
                    //            rinexNavGalileo,
                    //            initialLocation);

                    //    accumulatedCorrection += correction.getCorrection();
                    //}

                    observedSatellite.setAccumulatedCorrection(accumulatedCorrection);
                }

                // Remove from the list all the satellites that did not pass the masking criteria
                visibleButNotUsed += excludedSatellites.Count();
                //observedSatellites.removeAll(excludedSatellites);
                unusedSatellites.AddRange(excludedSatellites);
            }
        }

        public override GnssConstellationType getConstellationId()
        {
            lock(this) {
                return constellationId;
            }
        }

        public override string getName()
        {
            return NAME;
        }

        public override Coordinates<Matrix> getRxPos()
        {
            lock(this) {
                return rxPos;
            }
        }

        public override SatelliteParameters getSatellite(int index)
        {
            lock(this) {
                return observedSatellites[index];
            }
        }

        public override List<SatelliteParameters> getSatellites()
        {
            lock (this) {
                return observedSatellites;
            }
        }

        public override double getSatelliteSignalStrength(int index)
        {
            lock(this) {
                return observedSatellites[index].getSignalStrength();
            }
        }

        public override Time getTime()
        {
            lock(this) {
                return timeRefMsec;
            }
        }

        public override List<SatelliteParameters> getUnusedSatellites()
        {
            return unusedSatellites;
        }

        public override int getUsedConstellationSize()
        {
            lock (this) {
                return observedSatellites.Count();
            }
        }

        public override int getVisibleConstellationSize()
        {
            lock (this)
            {
                return getUsedConstellationSize() + visibleButNotUsed;
            }
        }

        public override void setRxPos(Coordinates<Matrix> rxPos)
        {
            lock(this) {
                this.rxPos = rxPos;
            }
        }

        public override void updateMeasurements(GnssMeasurementsEvent evento)
        {
            lock(this) {

                visibleButNotUsed = 0;
                observedSatellites.Clear();
                unusedSatellites.Clear();

                GnssClock gnssClock = evento.Clock;
                long TimeNanos = gnssClock.TimeNanos;
                timeRefMsec = new Time(JavaSystem.CurrentTimeMillis());
                double BiasNanos = gnssClock.BiasNanos;
                double galileoTime, pseudorangeTOW, pseudorangeE1_2nd, tTxGalileo;

                // Use only the first instance of the FullBiasNanos (as done in gps-measurement-tools)
                if (!fullBiasNanosInitialized) {
                    FullBiasNanos = gnssClock.FullBiasNanos;
                    fullBiasNanosInitialized = true;
                }

                // Start computing the pseudoranges using the raw data from the phone's GNSS receiver
                foreach (GnssMeasurement measurement in evento.Measurements) {

                    if (measurement.ConstellationType != constellationId)
                        continue;

                    if (measurement.HasCarrierFrequencyHz)
                        if (!approximateEqual(measurement.CarrierFrequencyHz, E1a_FREQUENCY, FREQUENCY_MATCH_RANGE))
                            continue;

                    long ReceivedSvTimeNanos = measurement.ReceivedSvTimeNanos;
                    double TimeOffsetNanos = measurement.TimeOffsetNanos;

                    // Galileo Time generation (GSA White Paper - page 20)
                    galileoTime = TimeNanos - (FullBiasNanos + BiasNanos);

                    // Compute the time of signal reception for when  GNSS_MEASUREMENT_STATE_TOW_KNOWN or GNSS_MEASUREMENT_STATE_TOW_DECODED are true
                    tRxGalileoTOW = galileoTime % Constants.NUMBER_NANO_SECONDS_PER_WEEK;

                    // Measurement time in full Galileo time without taking into account weekNumberNanos(the number of
                    // nanoseconds that have occurred from the beginning of GPS time to the current
                    // week number)
                    weekNumber = System.Math.Floor((-1.0 * FullBiasNanos) / Constants.NUMBER_NANO_SECONDS_PER_WEEK);

                    // Compute the signal reception for when GNSS_MEASUREMENT_STATE_GAL_E1C_2ND_CODE_LOCK is true
                    tRxGalileoE1_2nd = galileoTime % Constants.NumberNanoSeconds100Milli;

                    tTxGalileo = ReceivedSvTimeNanos + TimeOffsetNanos;

                    // Valid only if GNSS_MEASUREMENT_STATE_TOW_KNOWN or GNSS_MEASUREMENT_STATE_TOW_DECODED are true
                    pseudorangeTOW = (tRxGalileoTOW - tTxGalileo) * 1e-9 * Constants.SPEED_OF_LIGHT;

                    // Valid only if GNSS_MEASUREMENT_STATE_GAL_E1C_2ND_CODE_LOCK
                    pseudorangeE1_2nd = ((galileoTime - tTxGalileo) % Constants.NumberNanoSeconds100Milli) * 1e-9 * Constants.SPEED_OF_LIGHT;


                    /*

                    According to https://developer.android.com/ and GSA White Paper (pg.20)
                    the GnssMeasurements States required for GALILEO valid pseudoranges are:

                    STATE_TOW_KNOWN                   = 16384                            (1 << 11)
                    STATE_TOW_DECODED                 =     8                            (1 <<  3)
                    STATE_GAL_E1C_2ND_CODE_LOCK       =  2048                            (1 << 11)

                    */

                    // Get the measurement state
                    GnssState measState = measurement.State;

                    // Bitwise AND to identify the states
                    bool towKnown = false;
                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        towKnown = (measState & GnssState.TowKnown) != 0;
                    }

                    bool towDecoded = (measState & GnssState.TowDecoded) != 0;

                    bool codeLockE1BC = (measState & GnssState.GalE1bcCodeLock) != 0;
                    bool codeLockE1C = (measState & GnssState.GalE1c2ndCodeLock) != 0;

                    // Variables for debugging
                    double prTOW = pseudorangeTOW;
                    double prE1_2nd = pseudorangeE1_2nd;
                    double diffPR = prTOW - prE1_2nd;
                    int svID = measurement.Svid;

                    if (towDecoded || towKnown)
                    {

                        SatelliteParameters satelliteParameters = new SatelliteParameters(
                                measurement.Svid,
                                new Pseudorange(pseudorangeTOW, 0.0));

                        satelliteParameters.setUniqueSatId("E" + satelliteParameters.getSatId() + "_E1");

                        satelliteParameters.setSignalStrength(measurement.Cn0DbHz);

                        satelliteParameters.setConstellationType(measurement.ConstellationType);

                        if (measurement.HasCarrierFrequencyHz)
                            satelliteParameters.setCarrierFrequency(measurement.CarrierFrequencyHz);

                        observedSatellites.Add(satelliteParameters);
                        Log.Debug(TAG, "updateConstellations(" + measurement.Svid + "): " + weekNumber + ", " + tRxGalileoTOW + ", " + pseudorangeTOW);
                        Log.Debug(TAG, "updateConstellations: Passed with measurement state: " + measState);


                    }
                    else if (codeLockE1C)
                    {
                        SatelliteParameters satelliteParameters = new SatelliteParameters(
                                measurement.Svid,
                                new Pseudorange(pseudorangeE1_2nd, 0.0)
                        );

                        satelliteParameters.setUniqueSatId("E" + satelliteParameters.getSatId() + "_E1");
                        satelliteParameters.setSignalStrength(measurement.Cn0DbHz);
                        satelliteParameters.setConstellationType(measurement.ConstellationType);


                        if (measurement.HasCarrierFrequencyHz)
                            satelliteParameters.setCarrierFrequency(measurement.CarrierFrequencyHz);
                        observedSatellites.Add(satelliteParameters);
                        Log.Debug(TAG, "updateConstellations(" + measurement.Svid + "): " + weekNumber + ", " + tRxGalileoTOW + ", " + pseudorangeE1_2nd);
                        Log.Debug(TAG, "updateConstellations: Passed with measurement state: " + measState);
                    }
                    else
                    {
                        SatelliteParameters satelliteParameters = new SatelliteParameters(
                                measurement.Svid,
                                null
                        );

                        satelliteParameters.setUniqueSatId("E" + satelliteParameters.getSatId() + "_E1");
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
    }
}