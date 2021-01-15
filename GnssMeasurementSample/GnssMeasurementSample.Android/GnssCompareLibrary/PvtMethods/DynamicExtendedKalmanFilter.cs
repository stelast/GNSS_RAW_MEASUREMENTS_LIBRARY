using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.PvtMethods
{
    public class DynamicExtendedKalmanFilter : PvtMethod
    {/** initial guess for the clock bias in the state vector. Default: 0.0
     */
        private static readonly double INITIAL_CLOCK_BIAS = 0.0;
        /** initial sigma of the position in meters for the process noise matrix Q
         */
        private static readonly double INITIAL_SIGMAPOS = 2.0; // 100 meters
        /** initial guess of sigma of the speed in the horizontal in meters per second
         */
        private static readonly double INITIAL_SIGMASPEED = 0.0;
        /** initial guess of sigma of the clock bias in meters for the process noise matrix Q
         */
        private static readonly double INITIAL_SIGMACLOCKBIAS = 3000.0;
        /** initial guess of sigma of the clock bias drift in meters.
         */
        private static readonly double INITIAL_SIGMACLOCKDRIFT = 10.0;
        /** sqrt(sigma) in the horizontal dimension (x, y) of the dynamic model
         */
        private static readonly double S_xy = 0.5;
        /** sqrt(sigma) in the vertical dimension(z) of the dynamic model
         */
        private static readonly double S_z = 0.01;
        /** name of the pvt method as it appears in the selection menu for pvt methods
         */
        private static readonly String NAME = "Dynamic EKF";
        /** Kalman filter parameters file logger
         */
        //private KalmanFilterFileLogger kalmanParamLogger = new KalmanFilterFileLogger();

        // dimensions and indices for matrices.
        // these numbers should change if a dynamic Kalman filter is implemented.
        /** number of entries of the state vector. 8: x, speed in x, y, speed in y, z, spped in z,
         * clock bias and clock bias drift
         */
        private int numStates = 8;
        /** index of x coordinate of the state vector
         */
        private int idxX = 0; // index for state vector component x
        /** index of speed in x coordinate of the state vector
         */
        private int idxU = 1; // index for state vector component speed in x
        /** index of y coordinate of the state vector
         */
        private int idxY = 2; // index for state vector component y 
        private int idxV = 3; // index for state vector component speed in y
        /** index of z coordinate of the state vector
         */
        private int idxZ = 4; // index for state vector component z
        private int idxW = 5; // index for state vector component speed in z
        /** index of the clock bias of the state vector
         */
        private int idxClockBias = 6;
        /** index of clock drift of the state vector
         */
        private int idxClockDrift = 7;

        /** vector for the predicted state
         */
        SimpleMatrix<Matrix> x_pred;
        /** state transition matrix F
         */
        SimpleMatrix<Matrix> F;
        /** process noise matrix Q
         */
        SimpleMatrix<Matrix> Q;
        /** variance-covariance matrix of the state vector
         */
        SimpleMatrix<Matrix> P_pred;

        // h_* constants basically depend on the chipset of the receiver.
        /** Allan variance coefficient h_{-2} in meters. TCXO low quality
         */
        private static readonly double h_2 = 2.0e-20 * Math.Pow(Constants.SPEED_OF_LIGHT, 2);
        /** Allan variance coefficient h_0 in meters. TCXO low quality
         */
        private static readonly double h_0 = 2.0e-19 * Math.Pow(Constants.SPEED_OF_LIGHT, 2);
        /** receiver clock phase error
         */
        private static readonly double S_g = 2.0 * Math.Pow(Constants.PI_ORBIT, 2) * h_2;
        /** receiver clock frequency error
         */
        private double S_f = h_0 / 2.0; // receiver clock frequency error

        /** time between measurements in seconds
         */
        readonly double DELTA_T = 1.0; // time between measurements in seconds TODO

        // Define the parameters for the elevation dependent weighting method [Jaume Subirana et al. GNSS Data Processing: Fundamentals and Algorithms]
        private double a = 0.13;
        private double b = 0.53;
        private double elev;

        /** measurement vector with numStates entries
         */
        SimpleMatrix<Matrix> x_meas;
        /** variance-covariance matrix of the measurement vector
         */
        SimpleMatrix<Matrix> P_meas;
        /** flag to control the initialization of the variables if not initialized yet.
         */
        private bool firstExecution = true;

        public DynamicExtendedKalmanFilter()
        {
            x_pred = new SimpleMatrix<Matrix>(numStates, 1);
            F = SimpleMatrix<Matrix>.identity(numStates);
            Q = new SimpleMatrix<Matrix>(numStates, numStates);
            P_pred = new SimpleMatrix<Matrix>(numStates, numStates);
            x_meas = new SimpleMatrix<Matrix>(numStates, 1);
            P_meas = new SimpleMatrix<Matrix>(numStates, numStates);

            // Initialization of the state transition matrix
            F.set(idxX, idxU, DELTA_T);
            F.set(idxY, idxV, DELTA_T);
            F.set(idxZ, idxW, DELTA_T);
            F.set(idxClockBias, idxClockDrift, DELTA_T);

            // Initialization of the process noise matrix
            initQ();

        }

        /**
         * initialize process noise matrix Q using receiver clock frequency and phase errors, DELTA_T
         * and the noise on the position S_xy and S_z
         */
        private void initQ()
        {

            Q.set(idxX, idxX, Math.Pow(S_xy, 2.0) * Math.Pow(DELTA_T, 3) / 3.0);
            Q.set(idxU, idxX, Math.Pow(S_xy, 2.0) * Math.Pow(DELTA_T, 2) / 2.0);
            Q.set(idxX, idxU, Q.get(idxU, idxX)); // assure symmetry of matrix
            Q.set(idxU, idxU, Math.Pow(S_xy, 2.0) * DELTA_T);

            Q.set(idxY, idxY, Math.Pow(S_xy, 2.0) * Math.Pow(DELTA_T, 3) / 3.0);
            Q.set(idxV, idxY, Math.Pow(S_xy, 2.0) * Math.Pow(DELTA_T, 2) / 2.0);
            Q.set(idxY, idxV, Q.get(idxV, idxY)); // symmetry
            Q.set(idxV, idxV, Math.Pow(S_xy, 2.0) * DELTA_T);

            Q.set(idxZ, idxZ, Math.Pow(S_z, 2.0) * Math.Pow(DELTA_T, 3) / 3.0);
            Q.set(idxZ, idxW, Math.Pow(S_z, 2.0) * Math.Pow(DELTA_T, 2) / 2.0);
            Q.set(idxW, idxZ, Q.get(idxZ, idxW)); // symmetry
            Q.set(idxW, idxW, Math.Pow(S_z, 2.0) * DELTA_T);


            // Tuning of the process noise matrix (Q)
            Q.set(idxClockBias, idxClockBias, S_f + S_g * Math.Pow(DELTA_T, 3) / 3.0);
            Q.set(idxClockBias, idxClockDrift, S_g * Math.Pow(DELTA_T, 2) / 2.0);
            Q.set(idxClockDrift, idxClockBias, S_g * Math.Pow(DELTA_T, 2) / 2.0);
            Q.set(idxClockDrift, idxClockDrift, S_g * DELTA_T);
        }

        public override Coordinates<Matrix> calculatePose(Constellation constellation)
        {
            /** number of satellites in constellation
             */
            int CONSTELLATION_SIZE = constellation.getUsedConstellationSize();

            /** innovation sequence vector
             */
            SimpleMatrix<Matrix> gamma;

            // Initialize a variable to hold the predicted (geometric) distance towards each satellite
            /** geometric distance in meters to every satellite. internal variable.
             */
            double distPred = 0.0; // geometric Distance
            /** counter for the satellites used in the position computation
             */
            int usedInCalculations = 0;

            // Get an approximate position of the receiver
            /**
             * approximate position of the receiver in ECEF.
             */
            SimpleMatrix<Matrix> rxPosSimpleVector = Constellation.getRxPosAsVector(constellation.getRxPos());

            if (firstExecution)
            {
                // Initialize the state vector
                // speed is equal to zero in x, y and z direction
                x_pred.set(idxX, rxPosSimpleVector.get(0));
                x_pred.set(idxY, rxPosSimpleVector.get(1));
                x_pred.set(idxZ, rxPosSimpleVector.get(2));
                x_pred.set(idxClockBias, INITIAL_CLOCK_BIAS);
                x_pred.set(idxClockDrift, 0.0); // clock bias drift

                // Initialize the VCM of the state vector
                P_pred.set(idxX, idxX, Math.Pow(INITIAL_SIGMAPOS, 2));
                P_pred.set(idxY, idxY, Math.Pow(INITIAL_SIGMAPOS, 2));
                P_pred.set(idxZ, idxZ, Math.Pow(INITIAL_SIGMAPOS, 2));
                P_pred.set(idxU, idxU, Math.Pow(INITIAL_SIGMASPEED, 2));
                P_pred.set(idxV, idxV, Math.Pow(INITIAL_SIGMASPEED, 2));
                P_pred.set(idxW, idxW, Math.Pow(INITIAL_SIGMASPEED, 2));
                P_pred.set(idxClockBias, idxClockBias, Math.Pow(INITIAL_SIGMACLOCKBIAS, 2));
                P_pred.set(idxClockDrift, idxClockDrift, Math.Pow(INITIAL_SIGMACLOCKDRIFT, 2));

                x_pred.set(F.mult(x_pred));
                P_pred = F.mult(P_pred.mult(F.transpose())).plus(Q);

            }
            else
            {
                x_pred.set(F.mult(x_meas));
                P_pred = F.mult(P_meas.mult(F.transpose())).plus(Q);
            }

            /** Kalman gain matrix K
             */
            SimpleMatrix<Matrix> K;
            /** Innovation covariance
             */
            SimpleMatrix<Matrix> S;

            // Initialize the variables related to the measurement model
            /** Observation Matrix H
             */
            SimpleMatrix<Matrix> H = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, numStates);
            /** pseudorange vector, one entry for every used satellite
             */
            SimpleMatrix<Matrix> prVect = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1); // pseurorange
            /** predicted pseudoranges vector, one entry for every used satellite
             */
            SimpleMatrix<Matrix> measPred = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1); // pseudor. predicted
            /** variance-covariance matrix of the measurements R
             */
            SimpleMatrix<Matrix> R = SimpleMatrix<Matrix>.identity(CONSTELLATION_SIZE);
            //        R = R.divide(1.0/100.0);

            // meas variance of each satellite
            //        SimpleMatrix<Matrix> sigma2C1 = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1);
            double sigma2Meas = Math.Pow(5, 2);




            // Form the observation matrix H
            for (int k = 0; k < CONSTELLATION_SIZE; k++)
            {
                if (constellation.getSatellite(k).getSatellitePosition() == null)
                    continue;

                // Get the raw pseudoranges for each satellite
                prVect.set(k, constellation.getSatellite(k).getPseudorange());

                // Compute the predicted (geometric) distance towards each satellite
                distPred = Math.Sqrt(
                        Math.Pow(constellation.getSatellite(k).getSatellitePosition().getX()
                                - x_pred.get(idxX), 2)
                                + Math.Pow(constellation.getSatellite(k).getSatellitePosition().getY()
                                - x_pred.get(idxY), 2)
                                + Math.Pow(constellation.getSatellite(k).getSatellitePosition().getZ()
                                - x_pred.get(idxZ), 2)
                );

                // Set the values inside the H matrix
                // velocity values are zero.
                H.set(k, idxX, (x_pred.get(idxX) - constellation.getSatellite(k).getSatellitePosition().getX()) / distPred);
                H.set(k, idxY, (x_pred.get(idxY) - constellation.getSatellite(k).getSatellitePosition().getY()) / distPred);
                H.set(k, idxZ, (x_pred.get(idxZ) - constellation.getSatellite(k).getSatellitePosition().getZ()) / distPred);
                H.set(k, idxClockBias, 1.0);

                // Form the predicted measurement towards each satellite
                measPred.set(k, distPred + x_pred.get(idxClockBias)
                        - constellation.getSatellite(k).getClockBias()
                        + constellation.getSatellite(k).getAccumulatedCorrection());

                // Form the VCM of the measurements (R)
                elev = constellation.getSatellite(k).getRxTopo().getElevation() * (Math.PI / 180.0);
                R.set(k, k, sigma2Meas * Math.Pow(a + b * Math.Exp(-elev / 10.0), 2));

                usedInCalculations++;
            }

            if (usedInCalculations > 0)
            {
                // Compute the Kalman Gain
                try
                {
                    K = P_pred.mult(H.transpose().mult((H.mult(P_pred.mult(H.transpose())).plus(R)).invert()));
                    S = H.mult(P_pred.mult(H.transpose())).plus(R);
                }
                catch (Exception e)
                {
                    Log.Error(NAME, " Matrix inversion failed", e);
                    return Coordinates<Matrix>.globalXYZInstance(
                            rxPosSimpleVector.get(0),
                            rxPosSimpleVector.get(1),
                            rxPosSimpleVector.get(2));
                }


                // Compute the Kalman innovation sequence
                gamma = prVect.minus(measPred);

                // Perform the measurement update
                x_meas = x_pred.plus(K.mult(gamma));
                P_meas = (SimpleMatrix<Matrix>.identity(numStates).minus((K.mult(H)))).mult(P_pred);

                //if (kalmanParamLogger.isStarted())
                //    kalmanParamLogger.logKalmanParam(x_meas, P_meas, numStates, gamma, S, CONSTELLATION_SIZE, constellation);

                firstExecution = false;

                // x_meas and P_meas are being used for the next set of measurements
                return Coordinates<Matrix>.globalXYZInstance(x_meas.get(idxX), x_meas.get(idxY), x_meas.get(idxZ));
            }
            else
            {
                return Coordinates<Matrix>.globalXYZInstance(
                        rxPosSimpleVector.get(0),
                        rxPosSimpleVector.get(1),
                        rxPosSimpleVector.get(2));
            }
        }

        public override double getClockBias()
        {
            return x_meas.get(idxClockBias);
        }

        public override string getName()
        {
            return NAME;
        }
    }
}