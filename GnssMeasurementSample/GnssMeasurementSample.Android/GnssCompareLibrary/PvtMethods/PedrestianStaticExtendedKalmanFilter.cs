using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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
    public class PedrestianStaticExtendedKalmanFilter : PvtMethod
    {
        /** initial guess for the clock bias in the state vector. Default: 0.0
         */
        private static readonly double INITIAL_CLOCK_BIAS = 0.0;
        /** initial sigma of the position in meters for the process noise matrix Q
         */
        private static readonly double INITIAL_SIGMAPOS = 1.0;
        /** initial guess of sigma of the clock bias in meters for the process noise matrix Q
         */
        private static readonly double INITIAL_SIGMACLOCKBIAS = 500.0;
        /** initial guess of sigma of the clock bias drift in meters.
         */
        private static readonly double INITIAL_SIGMACLOCKDRIFT = 50.0;
        /** noise in the horizontal direction on the position to allow positioning at pedestrians
         * velocity. This value is already in units of meters squared.
         */
        private static readonly double xyNoise = 0.4;
        /** name of the pvt method as it appears in the selection menu for pvt methods
         */
        private static readonly String NAME = "Pedestrian EKF";


        // dimensions and indices for matrices.
        // these numbers should change if a dynamic Kalman filter is implemented.
        /** number of entries of the state vector. 5: x, y, z, clock bias and clock bias drift
         */
        private int numStates = 5;
        /** index of x coordinate of the state vector
         */
        private int idxX = 0;
        /** index of y coordinate of the state vector
         */
        private int idxY = 1;
        /** index of z coordinate of the state vector
         */
        private int idxZ = 2;
        /** index of the clock bias of the state vector
         */
        private int idxClockBias = 3;
        /** index of clock drift of the state vector
         */
        private int idxClockDrift = 4;
        /** Kalman filter parameters file logger
         */
        //private KalmanFilterFileLogger kalmanParamLogger = new KalmanFilterFileLogger();

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
        private double S_f = h_0 / 2.0;

        /** time between measurements in seconds
         */
        readonly double DELTA_T = 1.0;

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
        /** flag to control the initialization of the variables if not initialiyed yet.
         */
        private bool firstExecution = true;


        /**
         * static extended Kalman filter with noise on the horizontal part of the process noise matrix
         */
        public PedrestianStaticExtendedKalmanFilter()
        {
            /** vector for the predicted state
             */
            x_pred = new SimpleMatrix<Matrix>(numStates, 1);
            /** state transition matrix F
             */
            F = SimpleMatrix<Matrix>.identity(numStates);
            /** process noise matrix Q
             */
            Q = new SimpleMatrix<Matrix>(numStates, numStates);
            /** variance-covariance matrix of the state vector
             */
            P_pred = new SimpleMatrix<Matrix>(numStates, numStates);
            /** measurement vector with numStates entries
             */
            x_meas = new SimpleMatrix<Matrix>(numStates, 1);
            /** variance-covariance matrix of the measurement vector
             */
            P_meas = new SimpleMatrix<Matrix>(numStates, numStates);

            // Initialization of the state transition matrix
            F.set(idxClockBias, idxClockDrift, DELTA_T);

            // Initialization of the process noise matrix
            initQ();

        }

        /**
         * initialize process noise matrix Q using receiver clock frequency and phase errors, DELTA_T
         */
        private void initQ()
        {
            // Tuning of the process noise matrix (Q)
            Q.set(idxClockBias, idxClockBias, S_f + S_g * Math.Pow(DELTA_T, 3) / 3.0);
            Q.set(idxClockBias, idxClockDrift, S_g * Math.Pow(DELTA_T, 2) / 2.0);
            Q.set(idxClockDrift, idxClockBias, S_g * Math.Pow(DELTA_T, 2) / 2.0);
            Q.set(idxClockDrift, idxClockDrift, S_g * DELTA_T);

            Q.set(idxX, idxX, xyNoise);
            Q.set(idxY, idxY, xyNoise);
        }

        public override Coordinates<Matrix> calculatePose(Constellation constellation)
        {
            throw new NotImplementedException();
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