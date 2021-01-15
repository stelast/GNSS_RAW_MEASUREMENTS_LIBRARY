using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Corrections;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex
{
    public class RinexNavigationParserGalileo : EphemerisSystemGalileo, NavigationProducer
    {
        private File fileNav;
        private FileInputStream streamNav;
        private InputStreamReader inStreamNav;
        private BufferedReader buffStreamNav;

        private FileOutputStream cacheOutputStream;
        private OutputStreamWriter cacheStreamWriter;

        //public static String newline = System.getProperty("line.separator");

        private readonly String TAG = "RinexNavigationParserGps";

        private List<EphGps> eph = new List<EphGps>(); /* GPS broadcast ephemerides */
        //private double[] iono = new double[8]; /* Ionosphere model parameters */
        private IonoGps iono = null; /* Ionosphere model parameters */
        //	private double A0; /* Delta-UTC parameters: A0 */
        //	private double A1; /* Delta-UTC parameters: A1 */
        //	private double T; /* Delta-UTC parameters: T */
        //	private double W; /* Delta-UTC parameters: W */
        //	private int leaps; /* Leap seconds */


        // RINEX Read constructors
        public RinexNavigationParserGalileo(File fileNav)
        {
            this.fileNav = fileNav;
        }
        /*
        public RinexNavigationParserGps(EphemerisResponse ephResponse)
        {
            foreach (GnssEphemeris eph in ephResponse.ephList)
            {
                if (eph is GpsEphemeris)
                {
                    this.eph.Add(new EphGps((GpsEphemeris)eph));
                }
            }
            this.iono = new IonoGps(ephResponse.ionoProto);
        }
        */
        public SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
        {
            long unixTime = obs.getRefTime().getMsec();
            double range = obs.getSatByIDType(satID, satType).getPseudorange(0);

            if (range == 0)
                return null;

            EphGps eph = findEph(unixTime, satID, satType);
            if (eph.Equals(EphGps.UnhealthyEph))
                return SatellitePosition.UnhealthySat;

            if (eph != null)
            {

                //			char satType = eph.getSatType();

                SatellitePosition sp = computePositionGps(obs, satID, satType, eph, receiverClockError);
                //			SatellitePosition sp = computePositionGps(unixTime, satType, satID, eph, range, receiverClockError);
                //if(receiverPosition!=null) earthRotationCorrection(receiverPosition, sp);
                return sp;// new SatellitePosition(eph, unixTime, satID, range);
            }
            return null;
        }

        public IonoGps getIono(long unixTime, Location initialLocation)
        {
            throw new NotImplementedException();
        }

        public IonoGalileo getIonoNeQuick(long unixTime, Location initialLocation)
        {
            return null;
        }

        public bool isTimestampInEpocsRange(long unixTime)
        {
            return eph.Count() > 0 /*&&
                eph.get(0).getRefTime().getMsec() <= unixTime *//*&&
		unixTime <= eph.get(eph.size()-1).getRefTime().getMsec() missing interval +epochInterval*/;
        }

        public SatellitePosition getSatPositionAndVelocities(long unixTime, double range, int satID, char satType, double receiverClockError)
        {
            //long unixTime = obs.getRefTime().getMsec();
            //double range = obs.getSatByIDType(satID, satType).getPseudorange(0);

            if (range == 0)
                return null;

            EphGps eph = findEph(unixTime, satID, satType);

            if (eph == null)
            {
                Log.Error(TAG, "getSatPositionAndVelocities: Ephemeris failed to load...");
                return null;
            }

            if (eph.Equals(EphGps.UnhealthyEph))
                return SatellitePosition.UnhealthySat;

            //			char satType = eph.getSatType();

            SatellitePosition sp = computeSatPositionAndVelocities(unixTime, range, satID, satType, eph, receiverClockError);
            //			SatellitePosition sp = computePositionGps(unixTime, satType, satID, eph, range, receiverClockError);
            //if(receiverPosition!=null) earthRotationCorrection(receiverPosition, sp);
            return sp;// new SatellitePosition(eph, unixTime, satID, range);

        }

        public void init()
        {
            throw new NotImplementedException();
        }

        public void release(bool waitForThread, long timeoutMs)
        {
            throw new NotImplementedException();
        }

        /**
         * @param unixTime
         * @param satID
         * @return Reference ephemeris set for given time and satellite
         */
        public EphGps findEph(long unixTime, int satID, char satType)
        {

            long dt = 0;
            long dtMin = 0;
            long dtMax = 0;
            long delta = 0;
            EphGps refEph = null;

            //long gpsTime = (new Time(unixTime)).getGpsTime();

            for (int i = 0; i < eph.Count(); i++)
            {
                // Find ephemeris sets for given satellite
                if (eph[i].getSatID() == satID && eph[i].getSatType() == satType)
                {
                    // Consider BeiDou time (BDT) for BeiDou satellites (14 sec difference wrt GPS time)
                    if (satType == 'C')
                    {
                        delta = 14000;
                        unixTime = unixTime - delta;
                    }
                    // Compare current time and ephemeris reference time
                    dt = Math.Abs(eph[i].getRefTime().getMsec() - unixTime /*getGpsTime() - gpsTime*/) / 1000;
                    // If it's the first round, set the minimum time difference and
                    // select the first ephemeris set candidate; if the current ephemeris set
                    // is closer in time than the previous candidate, select new candidate
                    if (refEph == null || dt < dtMin)
                    {
                        dtMin = dt;
                        refEph = eph[i];
                    }
                }
            }

            if (refEph == null)
                return null;

            if (refEph.getSvHealth() != 0)
            {
                return EphGps.UnhealthyEph;
            }

            //maximum allowed interval from ephemeris reference time
            long fitInterval = refEph.getFitInt();

            if (fitInterval != 0)
            {
                dtMax = fitInterval * 3600 / 2;
            }
            else
            {
                switch (refEph.getSatType())
                {
                    case 'R':
                        dtMax = 950;
                        break;
                    case 'J':
                        dtMax = 3600;
                        break;
                    default:
                        dtMax = 7200;
                        break;
                }
            }
            if (dtMin > dtMax)
            {
                refEph = null;
            }

            return refEph;
        }


        public int getEphSize()
        {
            return eph.Count();
        }

        public void addEph(EphGps eph)
        {
            this.eph.Add(eph);
        }


        public SatellitePosition computePositionGps(Observations obs, int satID, char satType, EphGps eph, double receiverClockError)
        {

            long unixTime = obs.getRefTime().getMsec();
            double obsPseudorange = obs.getSatByIDType(satID, satType).getPseudorange(0);

            //		char satType2 = eph.getSatType() ;
            if (satType != 'R')
            {  // other than GLONASS

                //					System.out.println("### other than GLONASS data");

                // Compute satellite clock error
                double satelliteClockError = computeSatelliteClockError(unixTime, eph, obsPseudorange);

                // Compute clock corrected transmission time
                double tGPS = computeClockCorrectedTransmissionTime(unixTime, satelliteClockError, obsPseudorange);

                // Compute eccentric anomaly
                double Ek = computeEccentricAnomaly(tGPS, eph);

                // Semi-major axis
                double A = eph.getRootA() * eph.getRootA();

                // Time from the ephemerides reference epoch
                double tk = checkGpsTime(tGPS - eph.getToe());

                // Position computation
                double fk = Math.Atan2(Math.Sqrt(1 - Math.Pow(eph.getE(), 2))
                        * Math.Sin(Ek), Math.Cos(Ek) - eph.getE());
                double phi = fk + eph.getOmega();
                phi = Math.IEEERemainder(phi, 2 * Math.PI);
                double u = phi + eph.getCuc() * Math.Cos(2 * phi) + eph.getCus()
                        * Math.Sin(2 * phi);
                double r = A * (1 - eph.getE() * Math.Cos(Ek)) + eph.getCrc()
                        * Math.Cos(2 * phi) + eph.getCrs() * Math.Sin(2 * phi);
                double ik = eph.getI0() + eph.getiDot() * tk + eph.getCic() * Math.Cos(2 * phi)
                        + eph.getCis() * Math.Sin(2 * phi);
                double Omega = eph.getOmega0()
                        + (eph.getOmegaDot() - Constants.EARTH_ANGULAR_VELOCITY) * tk
                        - Constants.EARTH_ANGULAR_VELOCITY * eph.getToe();
                Omega = Math.IEEERemainder(Omega + 2 * Math.PI, 2 * Math.PI);
                double x1 = Math.Cos(u) * r;
                double y1 = Math.Sin(u) * r;

                // Coordinates
                //			double[][] data = new double[3][1];
                //			data[0][0] = x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega);
                //			data[1][0] = x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega);
                //			data[2][0] = y1 * Math.Sin(ik);

                // Fill in the satellite position matrix
                //this.coord.ecef = new SimpleMatrix<Matrix>(data);
                //this.coord = Coordinates.globalXYZInstance(new SimpleMatrix<Matrix>(data));
                SatellitePosition sp = new SatellitePosition(unixTime, satID, satType, x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega),
                        x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega),
                        y1 * Math.Sin(ik));
                sp.setSatelliteClockError(satelliteClockError);

                // Apply the correction due to the Earth rotation during signal travel time
                SimpleMatrix<Matrix> R = computeEarthRotationCorrection(unixTime, receiverClockError, tGPS);
                sp.setSMMultXYZ(R);

                return sp;
                //		this.setXYZ(x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega),
                //				x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega),
                //				y1 * Math.Sin(ik));

            }
            else
            {   // GLONASS

                //					System.out.println("### GLONASS computation");
                satID = eph.getSatID();
                double X = eph.getX();  // satellite X coordinate at ephemeris reference time
                double Y = eph.getY();  // satellite Y coordinate at ephemeris reference time
                double Z = eph.getZ();  // satellite Z coordinate at ephemeris reference time
                double Xv = eph.getXv();  // satellite velocity along X at ephemeris reference time
                double Yv = eph.getYv();  // satellite velocity along Y at ephemeris reference time
                double Zv = eph.getZv();  // satellite velocity along Z at ephemeris reference time
                double Xa = eph.getXa();  // acceleration due to lunar-solar gravitational perturbation along X at ephemeris reference time
                double Ya = eph.getYa();  // acceleration due to lunar-solar gravitational perturbation along Y at ephemeris reference time
                double Za = eph.getZa();  // acceleration due to lunar-solar gravitational perturbation along Z at ephemeris reference time
                /* NOTE:  Xa,Ya,Za are considered constant within the integration interval (i.e. toe ?}15 minutes) */

                double tn = eph.getTauN();
                float gammaN = eph.getGammaN();
                double tk = eph.gettk();
                double En = eph.getEn();
                double toc = eph.getToc();
                double toe = eph.getToe();
                int freqNum = eph.getfreq_num();

                obs.getSatByIDType(satID, satType).setFreqNum(freqNum);

                /*
                String refTime = eph.getRefTime().toString();
//					refTime = refTime.substring(0,10);
                refTime = refTime.substring(0,19);
//					refTime = refTime + " 00 00 00";
                System.out.println("refTime: " + refTime);

                try {
                        // Set GMT time zone
                        TimeZone zone = TimeZone.getTimeZone("GMT Time");
//							TimeZone zone = TimeZone.getTimeZone("UTC+4");
                        DateFormat df = new java.text.SimpleDateFormat("yyyy MM dd HH mm ss");
                        df.setTimeZone(zone);

                        long ut = df.parse(refTime).getTime() ;
                        System.out.println("ut: " + ut);
                        Time tm = new Time(ut); 
                        double gpsTime = tm.getGpsTime();
//						double gpsTime = tm.getRoundedGpsTime();
                        System.out.println("gpsT: " + gpsTime);

                } catch (ParseException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
                */


                //					System.out.println("refTime: " + refTime);
                //					System.out.println("toc: " + toc);
                //					System.out.println("toe: " + toe);
                //					System.out.println("unixTime: " + unixTime);				
                //					System.out.println("satID: " + satID);
                //					System.out.println("X: " + X);
                //					System.out.println("Y: " + Y);
                //					System.out.println("Z: " + Z);
                //					System.out.println("Xv: " + Xv);
                //					System.out.println("Yv: " + Yv);
                //					System.out.println("Zv: " + Zv);
                //					System.out.println("Xa: " + Xa);
                //					System.out.println("Ya: " + Ya);
                //					System.out.println("Za: " + Za);
                //					System.out.println("tn: " + tn);
                //					System.out.println("gammaN: " + gammaN);
                ////					System.out.println("tb: " + tb);
                //					System.out.println("tk: " + tk);
                //					System.out.println("En: " + En);
                //					System.out.println("					");

                /* integration step */
                int int_step = 60; // [s]

                /* Compute satellite clock error */
                double satelliteClockError = computeSatelliteClockError(unixTime, eph, obsPseudorange);
                //				    System.out.println("satelliteClockError: " + satelliteClockError);

                /* Compute clock corrected transmission time */
                double tGPS = computeClockCorrectedTransmissionTime(unixTime, satelliteClockError, obsPseudorange);
                //				    System.out.println("tGPS: " + tGPS);

                /* Time from the ephemerides reference epoch */
                Time reftime = new Time(eph.getWeek(), tGPS);
                double tk2 = checkGpsTime(tGPS - toe - reftime.getLeapSeconds());
                //					System.out.println("tk2: " + tk2);

                /* number of iterations on "full" steps */
                int n = (int)Math.Floor(Math.Abs(tk2 / int_step));
                //					System.out.println("Number of iterations: " + n);

                /* array containing integration steps (same sign as tk) */
                double[] array = new double[n];
                Array.Fill(array, 1);
                SimpleMatrix<Matrix> tkArray = new SimpleMatrix<Matrix>(n, 1, true, array);

                //					SimpleMatrix<Matrix> tkArray2  = tkArray.scale(2);
                tkArray = tkArray.scale(int_step);
                tkArray = tkArray.scale(tk2 / Math.Abs(tk2));
                //					tkArray.print();
                //double ii = tkArray * int_step * (tk2/Math.Abs(tk2));

                /* check residual iteration step (i.e. remaining fraction of int_step) */
                double int_step_res = tk2 % int_step;
                //				    System.out.println("int_step_res: " + int_step_res);

                double[] intStepRes = new double[] { int_step_res };
                SimpleMatrix<Matrix> int_stepArray = new SimpleMatrix<Matrix>(1, 1, false, intStepRes);
                //					int_stepArray.print();

                /* adjust the total number of iterations and the array of iteration steps */
                if (int_step_res != 0)
                {
                    tkArray = tkArray.combine(n, 0, int_stepArray);
                    //				        tkArray.print();
                    n = n + 1;
                    // tkArray = [ii; int_step_res];
                }
                //				    System.out.println("n: " + n);				

                // numerical integration steps (i.e. re-calculation of satellite positions from toe to tk)
                double[] pos = { X, Y, Z };
                double[] vel = { Xv, Yv, Zv };
                double[] acc = { Xa, Ya, Za };
                double[] pos1;
                double[] vel1;

                SimpleMatrix<Matrix> posArray = new SimpleMatrix<Matrix>(1, 3, true, pos);
                SimpleMatrix<Matrix> velArray = new SimpleMatrix<Matrix>(1, 3, true, vel);
                SimpleMatrix<Matrix> accArray = new SimpleMatrix<Matrix>(1, 3, true, acc);
                SimpleMatrix<Matrix> pos1Array;
                SimpleMatrix<Matrix> vel1Array;
                SimpleMatrix<Matrix> pos2Array;
                SimpleMatrix<Matrix> vel2Array;
                SimpleMatrix<Matrix> pos3Array;
                SimpleMatrix<Matrix> vel3Array;
                SimpleMatrix<Matrix> pos4Array;
                SimpleMatrix<Matrix> vel4Array;
                SimpleMatrix<Matrix> pos1dotArray;
                SimpleMatrix<Matrix> vel1dotArray;
                SimpleMatrix<Matrix> pos2dotArray;
                SimpleMatrix<Matrix> vel2dotArray;
                SimpleMatrix<Matrix> pos3dotArray;
                SimpleMatrix<Matrix> vel3dotArray;
                SimpleMatrix<Matrix> pos4dotArray;
                SimpleMatrix<Matrix> vel4dotArray;
                SimpleMatrix<Matrix> subPosArray;
                SimpleMatrix<Matrix> subVelArray;

                for (int i = 0; i < n; i++)
                {

                    /* Runge-Kutta numerical integration algorithm */
                    // step 1
                    pos1Array = posArray;
                    //pos1 = pos;
                    vel1Array = velArray;
                    //vel1 = vel;

                    // differential position
                    pos1dotArray = velArray;
                    //double[] pos1_dot = vel;
                    vel1dotArray = satellite_motion_diff_eq(pos1Array, vel1Array, accArray, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
                    //double[] vel1_dot = satellite_motion_diff_eq(pos1, vel1, acc, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
                    //							vel1dotArray.print();

                    // step 2
                    pos2Array = pos1dotArray.scale(tkArray.get(i)).divide(2);
                    pos2Array = posArray.plus(pos2Array);
                    //double[] pos2 = pos + pos1_dot*ii(i)/2;
                    //							System.out.println("## pos2Array: " ); pos2Array.print();

                    vel2Array = vel1dotArray.scale(tkArray.get(i)).divide(2);
                    vel2Array = velArray.plus(vel2Array);
                    //double[] vel2 = vel + vel1_dot * tkArray.get(i)/2;
                    //							System.out.println("## vel2Array: " ); vel2Array.print();

                    pos2dotArray = vel2Array;
                    //double[] pos2_dot = vel2;
                    vel2dotArray = satellite_motion_diff_eq(pos2Array, vel2Array, accArray, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
                    //double[] vel2_dot = satellite_motion_diff_eq(pos2, vel2, acc, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
                    //							System.out.println("## vel2dotArray: " ); vel2dotArray.print();																			

                    // step 3
                    pos3Array = pos2dotArray.scale(tkArray.get(i)).divide(2);
                    pos3Array = posArray.plus(pos3Array);
                    //							double[] pos3 = pos + pos2_dot * tkArray.get(i)/2;
                    //							System.out.println("## pos3Array: " ); pos3Array.print();

                    vel3Array = vel2dotArray.scale(tkArray.get(i)).divide(2);
                    vel3Array = velArray.plus(vel3Array);
                    //					        double[] vel3 = vel + vel2_dot * tkArray.get(i)/2;
                    //							System.out.println("## vel3Array: " ); vel3Array.print();

                    pos3dotArray = vel3Array;
                    //double[] pos3_dot = vel3;
                    vel3dotArray = satellite_motion_diff_eq(pos3Array, vel3Array, accArray, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
                    //double[] vel3_dot = satellite_motion_diff_eq(pos3, vel3, acc, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
                    //							System.out.println("## vel3dotArray: " ); vel3dotArray.print();	

                    // step 4
                    pos4Array = pos3dotArray.scale(tkArray.get(i));
                    pos4Array = posArray.plus(pos4Array);
                    //double[] pos4 = pos + pos3_dot * tkArray.get(i);
                    //							System.out.println("## pos4Array: " ); pos4Array.print();

                    vel4Array = vel3dotArray.scale(tkArray.get(i));
                    vel4Array = velArray.plus(vel4Array);
                    //double[] vel4 = vel + vel3_dot * tkArray.get(i);
                    //							System.out.println("## vel4Array: " ); vel4Array.print();

                    pos4dotArray = vel4Array;
                    //double[] pos4_dot = vel4;
                    vel4dotArray = satellite_motion_diff_eq(pos4Array, vel4Array, accArray, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
                    //double[] vel4_dot = satellite_motion_diff_eq(pos4, vel4, acc, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
                    //							System.out.println("## vel4dotArray: " ); vel4dotArray.print();																			

                    // final position and velocity
                    subPosArray = pos1dotArray.plus(pos2dotArray.scale(2)).plus(pos3dotArray.scale(2)).plus(pos4dotArray);
                    subPosArray = subPosArray.scale(tkArray.get(i)).divide(6);
                    posArray = posArray.plus(subPosArray);
                    //pos = pos + (pos1_dot + 2*pos2_dot + 2*pos3_dot + pos4_dot)*ii(s)/6;
                    //							System.out.println("## posArray: " ); posArray.print();	

                    subVelArray = vel1dotArray.plus(vel2dotArray.scale(2)).plus(vel3dotArray.scale(2)).plus(vel4dotArray);
                    subVelArray = subVelArray.scale(tkArray.get(i)).divide(6);
                    velArray = velArray.plus(subVelArray);
                    //vel = vel + (vel1_dot + 2*vel2_dot + 2*vel3_dot + vel4_dot)*ii(s)/6;
                    //							System.out.println("## velArray: " ); velArray.print();	
                    //							System.out.println(" " );


                }

                /* transformation from PZ-90.02 to WGS-84 (G1150) */
                double x1 = posArray.get(0) - 0.36;
                double y1 = posArray.get(1) + 0.08;
                double z1 = posArray.get(2) + 0.18;

                /* satellite velocity */
                double Xv1 = velArray.get(0);
                double Yv1 = velArray.get(1);
                double Zv1 = velArray.get(2);

                /* Fill in the satellite position matrix */
                SatellitePosition sp = new SatellitePosition(unixTime, satID, satType, x1, y1, z1);
                sp.setSatelliteClockError(satelliteClockError);
                //		
                //					/* Apply the correction due to the Earth rotation during signal travel time */
                SimpleMatrix<Matrix> R = computeEarthRotationCorrection(unixTime, receiverClockError, tGPS);
                sp.setSMMultXYZ(R);

                return sp;
                //					return null ;


            }
        }

        private SimpleMatrix<Matrix> satellite_motion_diff_eq(SimpleMatrix<Matrix> pos1Array,
                                                      SimpleMatrix<Matrix> vel1Array, SimpleMatrix<Matrix> accArray, long ellAGlo,
                                                      double gmGlo, double j2Glo, double omegaeDotGlo)
        {
            // TODO Auto-generated method stub

            /* renaming variables for better readability position */
            double X = pos1Array.get(0);
            double Y = pos1Array.get(1);
            double Z = pos1Array.get(2);

            //		System.out.println("X: " + X);
            //		System.out.println("Y: " + Y);
            //		System.out.println("Z: " + Z);

            /* velocity */
            double Xv = vel1Array.get(0);
            double Yv = vel1Array.get(1);

            //		System.out.println("Xv: " + Xv);
            //		System.out.println("Yv: " + Yv);

            /* acceleration (i.e. perturbation) */
            double Xa = accArray.get(0);
            double Ya = accArray.get(1);
            double Za = accArray.get(2);

            //		System.out.println("Xa: " + Xa);
            //		System.out.println("Ya: " + Ya);
            //		System.out.println("Za: " + Za);

            /* parameters */
            double r = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
            double g = -gmGlo / Math.Pow(r, 3);
            double h = j2Glo * 1.5 * Math.Pow((ellAGlo / r), 2);
            double k = 5 * Math.Pow(Z, 2) / Math.Pow(r, 2);

            //		System.out.println("r: " + r);
            //		System.out.println("g: " + g);
            //		System.out.println("h: " + h);
            //		System.out.println("k: " + k);

            /* differential velocity */
            double[] vel_dot = new double[3];
            vel_dot[0] = g * X * (1 - h * (k - 1)) + Xa + Math.Pow(omegaeDotGlo, 2) * X + 2 * omegaeDotGlo * Yv;
            //		System.out.println("vel1: " + vel_dot[0]);

            vel_dot[1] = g * Y * (1 - h * (k - 1)) + Ya + Math.Pow(omegaeDotGlo, 2) * Y - 2 * omegaeDotGlo * Xv;
            //		System.out.println("vel2: " + vel_dot[1]);

            vel_dot[2] = g * Z * (1 - h * (k - 3)) + Za;
            //		System.out.println("vel3: " + vel_dot[2]);

            SimpleMatrix<Matrix> velDotArray = new SimpleMatrix<Matrix>(1, 3, true, vel_dot);
            //		velDotArray.print();

            return velDotArray;
        }









    }

    //// RINEX Read constructors
    //public RinexNavigationParserGps(InputStream es, File cache)
    //{

    //    this.inStreamNav = new InputStreamReader(es);
    //    if (cache != null)
    //    {
    //        File path = cache.getParentFile();
    //        if (!path.exists())
    //        {
    //            Log.i("RinexNavigationParserGps", "RinexNavigationParserGps: " + path.mkdirs());
    //        }
    //        try
    //        {
    //            cacheOutputStream = new FileOutputStream(cache);
    //            cacheStreamWriter = new OutputStreamWriter(cacheOutputStream);
    //        }
    //        catch (FileNotFoundException e)
    //        {
    //            System.err.println("Exception writing " + cache);
    //            e.printStackTrace();
    //        }
    //    }
    //} 
}