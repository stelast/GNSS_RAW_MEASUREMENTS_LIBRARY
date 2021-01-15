﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex
{
    public abstract class EphemerisSystem
    {

        /**
         * @param time      (GPS time in seconds)
         * @param satID
         * @param range
         * @param approxPos
         */

        //	double[] pos ;
        public SatellitePosition computeSatPositionAndVelocities(long unixTime, double obsPseudorange, int satID, char satType, EphGps eph, double receiverClockError)
        {

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
                //this.coord.ecef = new SimpleMatrix(data);
                //this.coord = Coordinates.globalXYZInstance(new SimpleMatrix(data));
                SatellitePosition sp = new SatellitePosition(unixTime, satID, satType, x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega),
                        x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega),
                        y1 * Math.Sin(ik));
                sp.setSatelliteClockError(satelliteClockError);

                // Apply the correction due to the Earth rotation during signal travel time
                SimpleMatrix<Matrix> R = computeEarthRotationCorrection(unixTime, receiverClockError, tGPS);
                sp.setSMMultXYZ(R);

                ///////////////////////////
                // compute satellite speeds
                // The technical paper which describes the bc_velo.c program is published in
                // GPS Solutions, Volume 8, Number 2, 2004 (in press). "Computing Satellite Velocity using the Broadcast Ephemeris", by Benjamin W. Remondi
                double cus = eph.getCus();
                double cuc = eph.getCuc();
                double cis = eph.getCis();
                double crs = eph.getCrs();
                double crc = eph.getCrc();
                double cic = eph.getCic();
                double idot = eph.getiDot(); // 0.342514267094e-09;
                double e = eph.getE();

                double ek = Ek;
                double tak = fk;

                // Computed mean motion [rad/sec]
                double n0 = Math.Sqrt(Constants.EARTH_GRAVITATIONAL_CONSTANT / Math.Pow(A, 3));

                // Corrected mean motion [rad/sec]
                double n = n0 + eph.getDeltaN();

                // Mean anomaly
                double Mk = eph.getM0() + n * tk;

                double mkdot = n;
                double ekdot = mkdot / (1.0 - e * Math.Cos(ek));
                double takdot = Math.Sin(ek) * ekdot * (1.0 + e * Math.Cos(tak)) / (Math.Sin(tak) * (1.0 - e * Math.Cos(ek)));
                double omegakdot = (eph.getOmegaDot() - Constants.EARTH_ANGULAR_VELOCITY);

                double phik = phi;
                double corr_u = cus * Math.Sin(2.0 * phik) + cuc * Math.Cos(2.0 * phik);
                double corr_r = crs * Math.Sin(2.0 * phik) + crc * Math.Cos(2.0 * phik);

                double uk = phik + corr_u;
                double rk = A * (1.0 - e * Math.Cos(ek)) + corr_r;

                double ukdot = takdot + 2.0 * (cus * Math.Cos(2.0 * uk) - cuc * Math.Sin(2.0 * uk)) * takdot;
                double rkdot = A * e * Math.Sin(ek) * n / (1.0 - e * Math.Cos(ek)) + 2.0 * (crs * Math.Cos(2.0 * uk) - crc * Math.Sin(2.0 * uk)) * takdot;
                double ikdot = idot + (cis * Math.Cos(2.0 * uk) - cic * Math.Sin(2.0 * uk)) * 2.0 * takdot;

                double xpk = rk * Math.Cos(uk);
                double ypk = rk * Math.Sin(uk);

                double xpkdot = rkdot * Math.Cos(uk) - ypk * ukdot;
                double ypkdot = rkdot * Math.Sin(uk) + xpk * ukdot;

                double xkdot = (xpkdot - ypk * Math.Cos(ik) * omegakdot) * Math.Cos(Omega)
                        - (xpk * omegakdot + ypkdot * Math.Cos(ik) - ypk * Math.Sin(ik) * ikdot) * Math.Sin(Omega);
                double ykdot = (xpkdot - ypk * Math.Cos(ik) * omegakdot) * Math.Sin(Omega)
                        + (xpk * omegakdot + ypkdot * Math.Cos(ik) - ypk * Math.Sin(ik) * ikdot) * Math.Cos(Omega);
                double zkdot = ypkdot * Math.Sin(ik) + ypk * Math.Cos(ik) * ikdot;

                sp.setSpeed(xkdot, ykdot, zkdot);

                return sp;

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

                //obs.getSatByIDType(satID, satType).setFreqNum(freqNum);

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

                //					SimpleMatrix tkArray2  = tkArray.scale(2);
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


//        public SatellitePosition computePositionGps(Observations obs, int satID, char satType, EphGps eph, double receiverClockError)
//        {

//            long unixTime = obs.getRefTime().getMsec();
//            double obsPseudorange = obs.getSatByIDType(satID, satType).getPseudorange(0);

//            //		char satType2 = eph.getSatType() ;
//            if (satType != 'R')
//            {  // other than GLONASS

//                //					System.out.println("### other than GLONASS data");

//                // Compute satellite clock error
//                double satelliteClockError = computeSatelliteClockError(unixTime, eph, obsPseudorange);

//                // Compute clock corrected transmission time
//                double tGPS = computeClockCorrectedTransmissionTime(unixTime, satelliteClockError, obsPseudorange);

//                // Compute eccentric anomaly
//                double Ek = computeEccentricAnomaly(tGPS, eph);

//                // Semi-major axis
//                double A = eph.getRootA() * eph.getRootA();

//                // Time from the ephemerides reference epoch
//                double tk = checkGpsTime(tGPS - eph.getToe());

//                // Position computation
//                double fk = Math.Atan2(Math.Sqrt(1 - Math.Pow(eph.getE(), 2))
//                        * Math.Sin(Ek), Math.Cos(Ek) - eph.getE());
//                double phi = fk + eph.getOmega();
//                phi = Math.IEEERemainder(phi, 2 * Math.PI);
//                double u = phi + eph.getCuc() * Math.Cos(2 * phi) + eph.getCus()
//                        * Math.Sin(2 * phi);
//                double r = A * (1 - eph.getE() * Math.Cos(Ek)) + eph.getCrc()
//                        * Math.Cos(2 * phi) + eph.getCrs() * Math.Sin(2 * phi);
//                double ik = eph.getI0() + eph.getiDot() * tk + eph.getCic() * Math.Cos(2 * phi)
//                        + eph.getCis() * Math.Sin(2 * phi);
//                double Omega = eph.getOmega0()
//                        + (eph.getOmegaDot() - Constants.EARTH_ANGULAR_VELOCITY) * tk
//                        - Constants.EARTH_ANGULAR_VELOCITY * eph.getToe();
//                Omega = Math.IEEERemainder(Omega + 2 * Math.PI, 2 * Math.PI);
//                double x1 = Math.Cos(u) * r;
//                double y1 = Math.Sin(u) * r;

//                // Coordinates
//                //			double[][] data = new double[3][1];
//                //			data[0][0] = x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega);
//                //			data[1][0] = x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega);
//                //			data[2][0] = y1 * Math.Sin(ik);

//                // Fill in the satellite position matrix
//                //this.coord.ecef = new SimpleMatrix(data);
//                //this.coord = Coordinates.globalXYZInstance(new SimpleMatrix(data));
//                SatellitePosition sp = new SatellitePosition(unixTime, satID, satType, x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega),
//                        x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega),
//                        y1 * Math.Sin(ik));
//                sp.setSatelliteClockError(satelliteClockError);

//                // Apply the correction due to the Earth rotation during signal travel time
//                SimpleMatrix R = computeEarthRotationCorrection(unixTime, receiverClockError, tGPS);
//                sp.setSMMultXYZ(R);

//                return sp;
//                //		this.setXYZ(x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega),
//                //				x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega),
//                //				y1 * Math.Sin(ik));

//            }
//            else
//            {   // GLONASS

//                //					System.out.println("### GLONASS computation");
//                satID = eph.getSatID();
//                double X = eph.getX();  // satellite X coordinate at ephemeris reference time
//                double Y = eph.getY();  // satellite Y coordinate at ephemeris reference time
//                double Z = eph.getZ();  // satellite Z coordinate at ephemeris reference time
//                double Xv = eph.getXv();  // satellite velocity along X at ephemeris reference time
//                double Yv = eph.getYv();  // satellite velocity along Y at ephemeris reference time
//                double Zv = eph.getZv();  // satellite velocity along Z at ephemeris reference time
//                double Xa = eph.getXa();  // acceleration due to lunar-solar gravitational perturbation along X at ephemeris reference time
//                double Ya = eph.getYa();  // acceleration due to lunar-solar gravitational perturbation along Y at ephemeris reference time
//                double Za = eph.getZa();  // acceleration due to lunar-solar gravitational perturbation along Z at ephemeris reference time
//                /* NOTE:  Xa,Ya,Za are considered constant within the integration interval (i.e. toe ?}15 minutes) */

//                double tn = eph.getTauN();
//                float gammaN = eph.getGammaN();
//                double tk = eph.gettk();
//                double En = eph.getEn();
//                double toc = eph.getToc();
//                double toe = eph.getToe();
//                int freqNum = eph.getfreq_num();

//                obs.getSatByIDType(satID, satType).setFreqNum(freqNum);

//                /*
//                String refTime = eph.getRefTime().toString();
////					refTime = refTime.substring(0,10);
//                refTime = refTime.substring(0,19);
////					refTime = refTime + " 00 00 00";
//                System.out.println("refTime: " + refTime);

//                try {
//                        // Set GMT time zone
//                        TimeZone zone = TimeZone.getTimeZone("GMT Time");
////							TimeZone zone = TimeZone.getTimeZone("UTC+4");
//                        DateFormat df = new java.text.SimpleDateFormat("yyyy MM dd HH mm ss");
//                        df.setTimeZone(zone);

//                        long ut = df.parse(refTime).getTime() ;
//                        System.out.println("ut: " + ut);
//                        Time tm = new Time(ut); 
//                        double gpsTime = tm.getGpsTime();
////						double gpsTime = tm.getRoundedGpsTime();
//                        System.out.println("gpsT: " + gpsTime);

//                } catch (ParseException e) {
//                    // TODO Auto-generated catch block
//                    e.printStackTrace();
//                }
//                */


//                //					System.out.println("refTime: " + refTime);
//                //					System.out.println("toc: " + toc);
//                //					System.out.println("toe: " + toe);
//                //					System.out.println("unixTime: " + unixTime);				
//                //					System.out.println("satID: " + satID);
//                //					System.out.println("X: " + X);
//                //					System.out.println("Y: " + Y);
//                //					System.out.println("Z: " + Z);
//                //					System.out.println("Xv: " + Xv);
//                //					System.out.println("Yv: " + Yv);
//                //					System.out.println("Zv: " + Zv);
//                //					System.out.println("Xa: " + Xa);
//                //					System.out.println("Ya: " + Ya);
//                //					System.out.println("Za: " + Za);
//                //					System.out.println("tn: " + tn);
//                //					System.out.println("gammaN: " + gammaN);
//                ////					System.out.println("tb: " + tb);
//                //					System.out.println("tk: " + tk);
//                //					System.out.println("En: " + En);
//                //					System.out.println("					");

//                /* integration step */
//                int int_step = 60; // [s]

//                /* Compute satellite clock error */
//                double satelliteClockError = computeSatelliteClockError(unixTime, eph, obsPseudorange);
//                //				    System.out.println("satelliteClockError: " + satelliteClockError);

//                /* Compute clock corrected transmission time */
//                double tGPS = computeClockCorrectedTransmissionTime(unixTime, satelliteClockError, obsPseudorange);
//                //				    System.out.println("tGPS: " + tGPS);

//                /* Time from the ephemerides reference epoch */
//                Time reftime = new Time(eph.getWeek(), tGPS);
//                double tk2 = checkGpsTime(tGPS - toe - reftime.getLeapSeconds());
//                //					System.out.println("tk2: " + tk2);

//                /* number of iterations on "full" steps */
//                int n = (int)Math.Floor(Math.Abs(tk2 / int_step));
//                //					System.out.println("Number of iterations: " + n);

//                /* array containing integration steps (same sign as tk) */
//                double[] array = new double[n];
//                Array.Fill(array, 1);
//                SimpleMatrix tkArray = new SimpleMatrix(n, 1, true, array);

//                //					SimpleMatrix tkArray2  = tkArray.scale(2);
//                tkArray = tkArray.scale(int_step);
//                tkArray = tkArray.scale(tk2 / Math.Abs(tk2));
//                //					tkArray.print();
//                //double ii = tkArray * int_step * (tk2/Math.Abs(tk2));

//                /* check residual iteration step (i.e. remaining fraction of int_step) */
//                double int_step_res = tk2 % int_step;
//                //				    System.out.println("int_step_res: " + int_step_res);

//                double[] intStepRes = new double[] { int_step_res };
//                SimpleMatrix int_stepArray = new SimpleMatrix(1, 1, false, intStepRes);
//                //					int_stepArray.print();

//                /* adjust the total number of iterations and the array of iteration steps */
//                if (int_step_res != 0)
//                {
//                    tkArray = tkArray.combine(n, 0, int_stepArray);
//                    //				        tkArray.print();
//                    n = n + 1;
//                    // tkArray = [ii; int_step_res];
//                }
//                //				    System.out.println("n: " + n);				

//                // numerical integration steps (i.e. re-calculation of satellite positions from toe to tk)
//                double[] pos = { X, Y, Z };
//                double[] vel = { Xv, Yv, Zv };
//                double[] acc = { Xa, Ya, Za };
//                double[] pos1;
//                double[] vel1;

//                SimpleMatrix posArray = new SimpleMatrix(1, 3, true, pos);
//                SimpleMatrix velArray = new SimpleMatrix(1, 3, true, vel);
//                SimpleMatrix accArray = new SimpleMatrix(1, 3, true, acc);
//                SimpleMatrix pos1Array;
//                SimpleMatrix vel1Array;
//                SimpleMatrix pos2Array;
//                SimpleMatrix vel2Array;
//                SimpleMatrix pos3Array;
//                SimpleMatrix vel3Array;
//                SimpleMatrix pos4Array;
//                SimpleMatrix vel4Array;
//                SimpleMatrix pos1dotArray;
//                SimpleMatrix vel1dotArray;
//                SimpleMatrix pos2dotArray;
//                SimpleMatrix vel2dotArray;
//                SimpleMatrix pos3dotArray;
//                SimpleMatrix vel3dotArray;
//                SimpleMatrix pos4dotArray;
//                SimpleMatrix vel4dotArray;
//                SimpleMatrix subPosArray;
//                SimpleMatrix subVelArray;

//                for (int i = 0; i < n; i++)
//                {

//                    /* Runge-Kutta numerical integration algorithm */
//                    // step 1
//                    pos1Array = posArray;
//                    //pos1 = pos;
//                    vel1Array = velArray;
//                    //vel1 = vel;

//                    // differential position
//                    pos1dotArray = velArray;
//                    //double[] pos1_dot = vel;
//                    vel1dotArray = satellite_motion_diff_eq(pos1Array, vel1Array, accArray, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
//                    //double[] vel1_dot = satellite_motion_diff_eq(pos1, vel1, acc, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
//                    //							vel1dotArray.print();

//                    // step 2
//                    pos2Array = pos1dotArray.scale(tkArray.get(i)).divide(2);
//                    pos2Array = posArray.plus(pos2Array);
//                    //double[] pos2 = pos + pos1_dot*ii(i)/2;
//                    //							System.out.println("## pos2Array: " ); pos2Array.print();

//                    vel2Array = vel1dotArray.scale(tkArray.get(i)).divide(2);
//                    vel2Array = velArray.plus(vel2Array);
//                    //double[] vel2 = vel + vel1_dot * tkArray.get(i)/2;
//                    //							System.out.println("## vel2Array: " ); vel2Array.print();

//                    pos2dotArray = vel2Array;
//                    //double[] pos2_dot = vel2;
//                    vel2dotArray = satellite_motion_diff_eq(pos2Array, vel2Array, accArray, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
//                    //double[] vel2_dot = satellite_motion_diff_eq(pos2, vel2, acc, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
//                    //							System.out.println("## vel2dotArray: " ); vel2dotArray.print();																			

//                    // step 3
//                    pos3Array = pos2dotArray.scale(tkArray.get(i)).divide(2);
//                    pos3Array = posArray.plus(pos3Array);
//                    //							double[] pos3 = pos + pos2_dot * tkArray.get(i)/2;
//                    //							System.out.println("## pos3Array: " ); pos3Array.print();

//                    vel3Array = vel2dotArray.scale(tkArray.get(i)).divide(2);
//                    vel3Array = velArray.plus(vel3Array);
//                    //					        double[] vel3 = vel + vel2_dot * tkArray.get(i)/2;
//                    //							System.out.println("## vel3Array: " ); vel3Array.print();

//                    pos3dotArray = vel3Array;
//                    //double[] pos3_dot = vel3;
//                    vel3dotArray = satellite_motion_diff_eq(pos3Array, vel3Array, accArray, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
//                    //double[] vel3_dot = satellite_motion_diff_eq(pos3, vel3, acc, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
//                    //							System.out.println("## vel3dotArray: " ); vel3dotArray.print();	

//                    // step 4
//                    pos4Array = pos3dotArray.scale(tkArray.get(i));
//                    pos4Array = posArray.plus(pos4Array);
//                    //double[] pos4 = pos + pos3_dot * tkArray.get(i);
//                    //							System.out.println("## pos4Array: " ); pos4Array.print();

//                    vel4Array = vel3dotArray.scale(tkArray.get(i));
//                    vel4Array = velArray.plus(vel4Array);
//                    //double[] vel4 = vel + vel3_dot * tkArray.get(i);
//                    //							System.out.println("## vel4Array: " ); vel4Array.print();

//                    pos4dotArray = vel4Array;
//                    //double[] pos4_dot = vel4;
//                    vel4dotArray = satellite_motion_diff_eq(pos4Array, vel4Array, accArray, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
//                    //double[] vel4_dot = satellite_motion_diff_eq(pos4, vel4, acc, Constants.ELL_A_GLO, Constants.GM_GLO, Constants.J2_GLO, Constants.OMEGAE_DOT_GLO);
//                    //							System.out.println("## vel4dotArray: " ); vel4dotArray.print();																			

//                    // final position and velocity
//                    subPosArray = pos1dotArray.plus(pos2dotArray.scale(2)).plus(pos3dotArray.scale(2)).plus(pos4dotArray);
//                    subPosArray = subPosArray.scale(tkArray.get(i)).divide(6);
//                    posArray = posArray.plus(subPosArray);
//                    //pos = pos + (pos1_dot + 2*pos2_dot + 2*pos3_dot + pos4_dot)*ii(s)/6;
//                    //							System.out.println("## posArray: " ); posArray.print();	

//                    subVelArray = vel1dotArray.plus(vel2dotArray.scale(2)).plus(vel3dotArray.scale(2)).plus(vel4dotArray);
//                    subVelArray = subVelArray.scale(tkArray.get(i)).divide(6);
//                    velArray = velArray.plus(subVelArray);
//                    //vel = vel + (vel1_dot + 2*vel2_dot + 2*vel3_dot + vel4_dot)*ii(s)/6;
//                    //							System.out.println("## velArray: " ); velArray.print();	
//                    //							System.out.println(" " );


//                }

//                /* transformation from PZ-90.02 to WGS-84 (G1150) */
//                double x1 = posArray.get(0) - 0.36;
//                double y1 = posArray.get(1) + 0.08;
//                double z1 = posArray.get(2) + 0.18;

//                /* satellite velocity */
//                double Xv1 = velArray.get(0);
//                double Yv1 = velArray.get(1);
//                double Zv1 = velArray.get(2);

//                /* Fill in the satellite position matrix */
//                SatellitePosition sp = new SatellitePosition(unixTime, satID, satType, x1, y1, z1);
//                sp.setSatelliteClockError(satelliteClockError);
//                //		
//                //					/* Apply the correction due to the Earth rotation during signal travel time */
//                SimpleMatrix R = computeEarthRotationCorrection(unixTime, receiverClockError, tGPS);
//                sp.setSMMultXYZ(R);

//                return sp;
//                //					return null ;


//            }
//        }

        public SatellitePosition computePositionSpeedGps(Observations obs, int satID, char satType, EphGps eph, double receiverClockError)
        {

            long unixTime = obs.getRefTime().getMsec();
            double obsPseudorange = obs.getSatByIDType(satID, satType).getPseudorange(0);

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
            //      double[][] data = new double[3][1];
            //      data[0][0] = x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega);
            //      data[1][0] = x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega);
            //      data[2][0] = y1 * Math.Sin(ik);

            // Fill in the satellite position matrix
            //this.coord.ecef = new SimpleMatrix(data);
            //this.coord = Coordinates.globalXYZInstance(new SimpleMatrix(data));
            SatellitePosition sp = new SatellitePosition(unixTime, satID, satType, x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega),
                    x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega),
                    y1 * Math.Sin(ik));
            sp.setSatelliteClockError(satelliteClockError);

            // Apply the correction due to the Earth rotation during signal travel time
            SimpleMatrix<Matrix> R = computeEarthRotationCorrection(unixTime, receiverClockError, tGPS);
            sp.setSMMultXYZ(R);

            ///////////////////////////
            // compute satellite speeds
            // The technical paper which describes the bc_velo.c program is published in
            // GPS Solutions, Volume 8, Number 2, 2004 (in press). "Computing Satellite Velocity using the Broadcast Ephemeris", by Benjamin W. Remondi
            double cus = eph.getCus();
            double cuc = eph.getCuc();
            double cis = eph.getCis();
            double crs = eph.getCrs();
            double crc = eph.getCrc();
            double cic = eph.getCic();
            double idot = eph.getiDot(); // 0.342514267094e-09;
            double e = eph.getE();

            double ek = Ek;
            double tak = fk;

            // Computed mean motion [rad/sec]
            double n0 = Math.Sqrt(Constants.EARTH_GRAVITATIONAL_CONSTANT / Math.Pow(A, 3));

            // Corrected mean motion [rad/sec]
            double n = n0 + eph.getDeltaN();

            // Mean anomaly
            double Mk = eph.getM0() + n * tk;

            double mkdot = n;
            double ekdot = mkdot / (1.0 - e * Math.Cos(ek));
            double takdot = Math.Sin(ek) * ekdot * (1.0 + e * Math.Cos(tak)) / (Math.Sin(tak) * (1.0 - e * Math.Cos(ek)));
            double omegakdot = (eph.getOmegaDot() - Constants.EARTH_ANGULAR_VELOCITY);

            double phik = phi;
            double corr_u = cus * Math.Sin(2.0 * phik) + cuc * Math.Cos(2.0 * phik);
            double corr_r = crs * Math.Sin(2.0 * phik) + crc * Math.Cos(2.0 * phik);

            double uk = phik + corr_u;
            double rk = A * (1.0 - e * Math.Cos(ek)) + corr_r;

            double ukdot = takdot + 2.0 * (cus * Math.Cos(2.0 * uk) - cuc * Math.Sin(2.0 * uk)) * takdot;
            double rkdot = A * e * Math.Sin(ek) * n / (1.0 - e * Math.Cos(ek)) + 2.0 * (crs * Math.Cos(2.0 * uk) - crc * Math.Sin(2.0 * uk)) * takdot;
            double ikdot = idot + (cis * Math.Cos(2.0 * uk) - cic * Math.Sin(2.0 * uk)) * 2.0 * takdot;

            double xpk = rk * Math.Cos(uk);
            double ypk = rk * Math.Sin(uk);

            double xpkdot = rkdot * Math.Cos(uk) - ypk * ukdot;
            double ypkdot = rkdot * Math.Sin(uk) + xpk * ukdot;

            double xkdot = (xpkdot - ypk * Math.Cos(ik) * omegakdot) * Math.Cos(Omega)
                    - (xpk * omegakdot + ypkdot * Math.Cos(ik) - ypk * Math.Sin(ik) * ikdot) * Math.Sin(Omega);
            double ykdot = (xpkdot - ypk * Math.Cos(ik) * omegakdot) * Math.Sin(Omega)
                    + (xpk * omegakdot + ypkdot * Math.Cos(ik) - ypk * Math.Sin(ik) * ikdot) * Math.Cos(Omega);
            double zkdot = ypkdot * Math.Sin(ik) + ypk * Math.Cos(ik) * ikdot;

            sp.setSpeed(xkdot, ykdot, zkdot);

            return sp;
        }

        public SatellitePosition computePositionSpeedGps2(long unixTime, double obsPseudorange, int satID, char satType, EphGps eph, double receiverClockError)
        {

            // long unixTime = obs.getRefTime().getMsec();
            // double obsPseudorange = obs.getSatByIDType(satID, satType).getPseudorange(0);

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
            //      double[][] data = new double[3][1];
            //      data[0][0] = x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega);
            //      data[1][0] = x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega);
            //      data[2][0] = y1 * Math.Sin(ik);

            // Fill in the satellite position matrix
            //this.coord.ecef = new SimpleMatrix(data);
            //this.coord = Coordinates.globalXYZInstance(new SimpleMatrix(data));
            SatellitePosition sp = new SatellitePosition(unixTime, satID, satType, x1 * Math.Cos(Omega) - y1 * Math.Cos(ik) * Math.Sin(Omega),
                    x1 * Math.Sin(Omega) + y1 * Math.Cos(ik) * Math.Cos(Omega),
                    y1 * Math.Sin(ik));
            sp.setSatelliteClockError(satelliteClockError);

            // Apply the correction due to the Earth rotation during signal travel time
            SimpleMatrix<Matrix> R = computeEarthRotationCorrection(unixTime, receiverClockError, tGPS);
            sp.setSMMultXYZ(R);

            ///////////////////////////
            // compute satellite speeds
            // The technical paper which describes the bc_velo.c program is published in
            // GPS Solutions, Volume 8, Number 2, 2004 (in press). "Computing Satellite Velocity using the Broadcast Ephemeris", by Benjamin W. Remondi
            double cus = eph.getCus();
            double cuc = eph.getCuc();
            double cis = eph.getCis();
            double crs = eph.getCrs();
            double crc = eph.getCrc();
            double cic = eph.getCic();
            double idot = eph.getiDot(); // 0.342514267094e-09;
            double e = eph.getE();

            double ek = Ek;
            double tak = fk;

            // Computed mean motion [rad/sec]
            double n0 = Math.Sqrt(Constants.EARTH_GRAVITATIONAL_CONSTANT / Math.Pow(A, 3));

            // Corrected mean motion [rad/sec]
            double n = n0 + eph.getDeltaN();

            // Mean anomaly
            double Mk = eph.getM0() + n * tk;

            double mkdot = n;
            double ekdot = mkdot / (1.0 - e * Math.Cos(ek));
            double takdot = Math.Sin(ek) * ekdot * (1.0 + e * Math.Cos(tak)) / (Math.Sin(tak) * (1.0 - e * Math.Cos(ek)));
            double omegakdot = (eph.getOmegaDot() - Constants.EARTH_ANGULAR_VELOCITY);

            double phik = phi;
            double corr_u = cus * Math.Sin(2.0 * phik) + cuc * Math.Cos(2.0 * phik);
            double corr_r = crs * Math.Sin(2.0 * phik) + crc * Math.Cos(2.0 * phik);

            double uk = phik + corr_u;
            double rk = A * (1.0 - e * Math.Cos(ek)) + corr_r;

            double ukdot = takdot + 2.0 * (cus * Math.Cos(2.0 * uk) - cuc * Math.Sin(2.0 * uk)) * takdot;
            double rkdot = A * e * Math.Sin(ek) * n / (1.0 - e * Math.Cos(ek)) + 2.0 * (crs * Math.Cos(2.0 * uk) - crc * Math.Sin(2.0 * uk)) * takdot;
            double ikdot = idot + (cis * Math.Cos(2.0 * uk) - cic * Math.Sin(2.0 * uk)) * 2.0 * takdot;

            double xpk = rk * Math.Cos(uk);
            double ypk = rk * Math.Sin(uk);

            double xpkdot = rkdot * Math.Cos(uk) - ypk * ukdot;
            double ypkdot = rkdot * Math.Sin(uk) + xpk * ukdot;

            double xkdot = (xpkdot - ypk * Math.Cos(ik) * omegakdot) * Math.Cos(Omega)
                    - (xpk * omegakdot + ypkdot * Math.Cos(ik) - ypk * Math.Sin(ik) * ikdot) * Math.Sin(Omega);
            double ykdot = (xpkdot - ypk * Math.Cos(ik) * omegakdot) * Math.Sin(Omega)
                    + (xpk * omegakdot + ypkdot * Math.Cos(ik) - ypk * Math.Sin(ik) * ikdot) * Math.Cos(Omega);
            double zkdot = ypkdot * Math.Sin(ik) + ypk * Math.Cos(ik) * ikdot;

            sp.setSpeed(xkdot, ykdot, zkdot);

            return sp;
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


        /**
         * @param time (Uncorrected GPS time)
         * @return GPS time accounting for beginning or end of week crossover
         */
        protected double checkGpsTime(double time)
        {

            // Account for beginning or end of week crossover
            if (time > Constants.SEC_IN_HALF_WEEK)
            {
                time = time - 2 * Constants.SEC_IN_HALF_WEEK;
            }
            else if (time < -Constants.SEC_IN_HALF_WEEK)
            {
                time = time + 2 * Constants.SEC_IN_HALF_WEEK;
            }
            return time;
        }

        /**
         * @param traveltime
         */
        protected SimpleMatrix<Matrix> computeEarthRotationCorrection(long unixTime, double receiverClockError, double transmissionTime)
        {

            // Computation of signal travel time
            // SimpleMatrix diff = satellitePosition.minusXYZ(approxPos);//this.coord.minusXYZ(approxPos);
            // double rho2 = Math.Pow(diff.get(0), 2) + Math.Pow(diff.get(1), 2)
            // 		+ Math.Pow(diff.get(2), 2);
            // double traveltime = Math.Sqrt(rho2) / Constants.SPEED_OF_LIGHT;
            double receptionTime = (new Time(unixTime)).getGpsTime();
            double traveltime = receptionTime + receiverClockError - transmissionTime;

            // Compute rotation angle
            double omegatau = Constants.EARTH_ANGULAR_VELOCITY * traveltime;

            // Rotation matrix
            double[][] data = new double[3][];
            for(int ii=0; ii<3; ii++)
            {
                data[ii] = new double[3];
            }
            data[0][0] = Math.Cos(omegatau);
            data[0][1] = Math.Sin(omegatau);
            data[0][2] = 0;
            data[1][0] = -Math.Sin(omegatau);
            data[1][1] = Math.Cos(omegatau);
            data[1][2] = 0;
            data[2][0] = 0;
            data[2][1] = 0;
            data[2][2] = 1;
            SimpleMatrix<Matrix> R = new SimpleMatrix<Matrix>(data);

            return R;
            // Apply rotation
            //this.coord.ecef = R.mult(this.coord.ecef);
            //this.coord.setSMMultXYZ(R);// = R.mult(this.coord.ecef);
            //satellitePosition.setSMMultXYZ(R);// = R.mult(this.coord.ecef);

        }

        /**
         * @param eph
         * @return Clock-corrected GPS transmission time
         */
        protected double computeClockCorrectedTransmissionTime(long unixTime, double satelliteClockError, double obsPseudorange)
        {

            double gpsTime = (new Time(unixTime)).getGpsTime();

            // Remove signal travel time from observation time
            double tRaw = (gpsTime - obsPseudorange /*this.range*/ / Constants.SPEED_OF_LIGHT);

            return tRaw - satelliteClockError;
        }

        /**
         * @param eph
         * @return Satellite clock error
         */
        protected double computeSatelliteClockError(long unixTime, EphGps eph, double obsPseudorange)
        {

            if (eph.getSatType() == 'R')
            {   // In case of GLONASS

                double gpsTime = (new Time(unixTime)).getGpsTime();
                //				System.out.println("gpsTime: " + gpsTime);
                //				System.out.println("obsPseudorange: " + obsPseudorange);

                // Remove signal travel time from observation time
                double tRaw = (gpsTime - obsPseudorange /*this.range*/ / Constants.SPEED_OF_LIGHT);
                //				System.out.println("tRaw: " + tRaw);

                // Clock error computation
                double dt = checkGpsTime(tRaw - eph.getToe());
                //				System.out.println("dt: " + dt);

                double timeCorrection = eph.getTauN() + eph.getGammaN() * dt;
                //				double timeCorrection =  - eph.getTauN() + eph.getGammaN() * dt ;					

                return timeCorrection;

            }
            else
            {        // other than GLONASS
                double gpsTime = (new Time(unixTime)).getGpsTime();
                // Remove signal travel time from observation time
                double tRaw = (gpsTime - obsPseudorange /*this.range*/ / Constants.SPEED_OF_LIGHT);

                // Compute eccentric anomaly
                double Ek = computeEccentricAnomaly(tRaw, eph);

                // Relativistic correction term computation
                //double dtr = Constants.RELATIVISTIC_ERROR_CONSTANT * eph.getE() * eph.getRootA() * Math.Sin(Ek);

                // Added by Sebastian (20.01.2018)
                double dtr = -2.0 * ((Math.Sqrt(Constants.EARTH_GRAVITATIONAL_CONSTANT) * eph.getRootA()) / (Constants.SPEED_OF_LIGHT * Constants.SPEED_OF_LIGHT)) * eph.getE() * Math.Sin(Ek);


                // Clock error computation
                double dt = checkGpsTime(tRaw - eph.getToc());
                double timeCorrection = (eph.getAf2() * dt + eph.getAf1()) * dt + eph.getAf0() + dtr - eph.getTgd();
                double tGPS = tRaw - timeCorrection;
                dt = checkGpsTime(tGPS - eph.getToc());
                timeCorrection = (eph.getAf2() * dt + eph.getAf1()) * dt + eph.getAf0() + dtr - eph.getTgd();

                return timeCorrection;
            }
        }

        /**
         * @param time (GPS time in seconds)
         * @param eph
         * @return Eccentric anomaly
         */
        protected double computeEccentricAnomaly(double time, EphGps eph)
        {

            // Semi-major axis
            double A = eph.getRootA() * eph.getRootA();

            // Time from the ephemerides reference epoch
            double tk = checkGpsTime(time - eph.getToe());

            // Computed mean motion [rad/sec]
            double n0 = Math.Sqrt(Constants.EARTH_GRAVITATIONAL_CONSTANT / Math.Pow(A, 3));

            // Corrected mean motion [rad/sec]
            double n = n0 + eph.getDeltaN();

            // Mean anomaly
            double Mk = eph.getM0() + n * tk;

            // Eccentric anomaly starting value
            Mk = Math.IEEERemainder(Mk + 2 * Math.PI, 2 * Math.PI);
            double Ek = Mk;

            int i;
            double EkOld, dEk;

            // Eccentric anomaly iterative computation
            int maxNumIter = 12;
            for (i = 0; i < maxNumIter; i++)
            {
                EkOld = Ek;
                Ek = Mk + eph.getE() * Math.Sin(Ek);
                dEk = Math.IEEERemainder(Ek - EkOld, 2 * Math.PI);
                if (Math.Abs(dEk) < 1e-12)
                    break;
            }

            // TODO Display/log warning message
            if (i == maxNumIter)
                //System.out.println("Warning: Eccentric anomaly does not converge.");
                Log.Debug("EphemerisSystem", "Warning: Eccentric anomaly does not converge.");

            return Ek;

        }

    }
}