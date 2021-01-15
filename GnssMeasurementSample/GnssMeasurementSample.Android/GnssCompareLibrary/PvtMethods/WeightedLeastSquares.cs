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
    public class WeightedLeastSquares : PvtMethod
    {
        private readonly static String NAME = "Weighted Least Squares";

        private readonly int NUMBER_OF_ITERATIONS = 10;
        private readonly static String TAG = "WLS";

        private readonly static Coordinates<Matrix> ZERO_POSE = Coordinates<Matrix>.globalGeodInstance(51.000, 3.000, 0.000); // Right at the edge of the plot
        private double clockBias; // =0.0;
                                  //private readonly static Coordinates<Matrix> ZERO_POSE = Coordinates<Matrix>.globalGeodInstance(47.642795, 23.622892, 350.0);   // Romania

        // Define the parameters for the elevation dependent weighting method [Jaume Subirana et al. GNSS Data Processing: Fundamentals and Algorithms]
        private double a = 0.13;
        private double b = 0.53;
        private double sigma2Meas = Math.Pow(5, 2);

        public WeightedLeastSquares() : base()
        {
        }

        public override Coordinates<Matrix> calculatePose(Constellation constellation)
        {
            int CONSTELLATION_SIZE = constellation.getUsedConstellationSize();

            // Initialize matrices for data storage

            SimpleMatrix<Matrix> rxPosSimpleVector = Constellation.getRxPosAsVector(constellation.getRxPos());
            SimpleMatrix<Matrix> satPosMat = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 3);
            SimpleMatrix<Matrix> tropoCorr = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1);
            SimpleMatrix<Matrix> svClkBias = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1);
            SimpleMatrix<Matrix> ionoCorr = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1);
            SimpleMatrix<Matrix> shapiroCorr = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1);
            SimpleMatrix<Matrix> sigma2 = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1);
            SimpleMatrix<Matrix> prVect = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1);
            SimpleMatrix<Matrix> vcvMeasurement;
            SimpleMatrix<Matrix> PDOP = new SimpleMatrix<Matrix>(1, 1);

            double elevation, measVar, measVarC1;
            int CN0;

            TopocentricCoordinates<Matrix> topo = new TopocentricCoordinates<Matrix>();
            Coordinates<Matrix> origin; // = new Coordinates<Matrix>(){};
            Coordinates<Matrix> target; // = new Coordinates<Matrix>(){};

            ///////////////////////////// SV Coordinates<Matrix>/velocities + PR corrections computation ////////////////////////////////////////////////////
            try
            {

                for (int ii = 0; ii < CONSTELLATION_SIZE; ii++)
                {

                    // Set the measurements into a vector
                    prVect.set(ii, constellation.getSatellite(ii).getPseudorange());

                    // Compute the satellite Coordinates<Matrix>
                    svClkBias.set(ii, constellation.getSatellite(ii).getClockBias());

                    ///////////////////////////// PR corrections computations ////////////////////////////////////////////////////


                    // Assign the computed SV Coordinates<Matrix> into a matrix
                    satPosMat.set(ii, 0, constellation.getSatellite(ii).getSatellitePosition().getX());
                    satPosMat.set(ii, 1, constellation.getSatellite(ii).getSatellitePosition().getY());
                    satPosMat.set(ii, 2, constellation.getSatellite(ii).getSatellitePosition().getZ());


                    // Compute the elevation and azimuth angles for each satellite
                    origin = Coordinates<Matrix>.globalXYZInstance(rxPosSimpleVector.get(0), rxPosSimpleVector.get(1), rxPosSimpleVector.get(2));
                    target = Coordinates<Matrix>.globalXYZInstance(satPosMat.get(ii, 0), satPosMat.get(ii, 1), satPosMat.get(ii, 2));
                    //                origin.setXYZ(rxPosSimpleVector.get(0), rxPosSimpleVector.get(1),rxPosSimpleVector.get(2));
                    //                target.setXYZ(satPosMat.get(ii, 0),satPosMat.get(ii, 1),satPosMat.get(ii, 2) );
                    topo.computeTopocentric(origin, target);
                    elevation = topo.getElevation() * (Math.PI / 180.0);

                    // Set the variance of the measurement for each satellite
                    measVar = sigma2Meas * Math.Pow(a + b * Math.Exp(-elevation / 10.0), 2);
                    sigma2.set(ii, measVar);

                    ///////////////////////////// Printing results  ////////////////////////////////////////////////////

                    // Print the computed satellite Coordinates<Matrix> \ velocities
                    /*
                    System.out.println();
                    System.out.println();
                    System.out.println("The GPS ECEF Coordinates<Matrix> of " + "SV" + constellation.getSatellite(ii).getSatId() + " are:");
                    System.out.printf("%.4f", satPosMat.get(ii, 0));
                    System.out.println();
                    System.out.printf("%.4f", satPosMat.get(ii, 1));
                    System.out.println();
                    System.out.printf("%.4f", satPosMat.get(ii, 2));
                    System.out.println();
                    System.out.printf("Tropo corr: %.4f", tropoCorr.get(ii));
                    System.out.println();
                    System.out.printf("Iono corr: %.4f", ionoCorr.get(ii));
                    System.out.println();
                    */
                    //System.out.printf("Shapiro corr: %.4f", shapiroCorr.get(ii));

                }
            }
            catch (Exception e)
            {
                //e.printStackTrace();

                //if (e.getClass() == IndexOutOfBoundsException.class){
                //    Log.e(TAG, "calculatePose: Satellites cleared before calculating result!");
                //}

                constellation.setRxPos(ZERO_POSE); // Right at the edge of the plot
                rxPosSimpleVector = Constellation.getRxPosAsVector(constellation.getRxPos());
                return Coordinates<Matrix>.globalXYZInstance(rxPosSimpleVector.get(0), rxPosSimpleVector.get(1), rxPosSimpleVector.get(2));
            }


            /*
             * [WEIGHTED LEAST SQUARES] Determination of the position + clock bias
             */

            try
            {
                // VCV matrix of the pseudorange measurements
                vcvMeasurement = ((SimpleBase<SimpleMatrix<Matrix>, Matrix>)sigma2).diag();

                SimpleMatrix<Matrix> W = vcvMeasurement.invert();

                // Initialization of the required matrices and vectors
                SimpleMatrix<Matrix> xHat = new SimpleMatrix<Matrix>(4, 1);              // vector holding the WLS estimates
                SimpleMatrix<Matrix> z = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1);        // observation vector
                SimpleMatrix<Matrix> H = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 4);        // observation matrix
                SimpleMatrix<Matrix> distPred = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1); // predicted distances
                SimpleMatrix<Matrix> measPred = new SimpleMatrix<Matrix>(CONSTELLATION_SIZE, 1); // predicted measurements


                // Start the estimation (10 loops)
                for (int iter = 0; iter < NUMBER_OF_ITERATIONS; iter++)
                {

                    // Calculation of the components within the observation matrix (H)
                    for (int k = 0; k < CONSTELLATION_SIZE; k++)
                    {

                        // Computation of the geometric distance
                        distPred.set(k, Math.Sqrt(
                                Math.Pow(satPosMat.get(k, 0) - rxPosSimpleVector.get(0), 2)
                                + Math.Pow(satPosMat.get(k, 1) - rxPosSimpleVector.get(1), 2)
                                + Math.Pow(satPosMat.get(k, 2) - rxPosSimpleVector.get(2), 2)
                        ));


                        // Measurement prediction
                        measPred.set(k, distPred.get(k)
                                        + constellation.getSatellite(k).getAccumulatedCorrection() - svClkBias.get(k));

                        // Compute the observation matrix (H)
                        H.set(k, 0, (constellation.getRxPos().getX() - satPosMat.get(k, 0)) / distPred.get(k));
                        H.set(k, 1, (constellation.getRxPos().getY() - satPosMat.get(k, 1)) / distPred.get(k));
                        H.set(k, 2, (constellation.getRxPos().getZ() - satPosMat.get(k, 2)) / distPred.get(k));
                        H.set(k, 3, 1.0);

                    }

                    // Compute the prefit vector (z)
                    z.set(prVect.minus(measPred));

                    // Estimate the unknowns (dxHat)
                    SimpleMatrix<Matrix> Cov = H.transpose().mult(W).mult(H);
                    SimpleMatrix<Matrix> CovDOP = H.transpose().mult(H);
                    SimpleMatrix<Matrix> dopMatrix = CovDOP.invert();
                    xHat.set(Cov.invert().mult(H.transpose()).mult(W).mult(z));
                    PDOP.set(Math.Sqrt(dopMatrix.get(0, 0) + dopMatrix.get(1, 1)));

                    // Update the receiver position
                    // rxPosSimpleVector.set(rxPosSimpleVector.plus(xHat));
                    rxPosSimpleVector.set(0, rxPosSimpleVector.get(0) + xHat.get(0));
                    rxPosSimpleVector.set(1, rxPosSimpleVector.get(1) + xHat.get(1));
                    rxPosSimpleVector.set(2, rxPosSimpleVector.get(2) + xHat.get(2));
                    rxPosSimpleVector.set(3, xHat.get(3));

                }

                clockBias = rxPosSimpleVector.get(3);

            }
            catch (Exception e)
            {
                //if (e.getClass() == IndexOutOfBoundsException.class){
                //    Log.e(TAG, "calculatePose: Satellites cleared before calculating result!");
                //} else if (e.getClass() == SingularMatrixException.class) {
                //    Log.e(TAG, "calculatePose: SingularMatrixException caught!");
                //}
                constellation.setRxPos(ZERO_POSE); // Right at the edge of the plot
                rxPosSimpleVector = Constellation.getRxPosAsVector(constellation.getRxPos());
                //e.printStackTrace();
            }

            //System.out.println();

            // Print the estimated receiver position
            //        rxPosSimpleVector.print();

            Log.Debug(TAG, "calculatePose: rxPosSimpleVector (ECEF): " + rxPosSimpleVector.get(0) + ", " + rxPosSimpleVector.get(1) + ", " + rxPosSimpleVector.get(2) + ";");

            Coordinates<Matrix> pose = Coordinates<Matrix>.globalXYZInstance(rxPosSimpleVector.get(0), rxPosSimpleVector.get(1), rxPosSimpleVector.get(2));
            Log.Debug(TAG, "calculatePose: pose (ECEF): " + pose.getX() + ", " + pose.getY() + ", " + pose.getZ() + ";");
            Log.Debug(TAG, "calculatePose: pose (lat-lon): " + pose.getGeodeticLatitude() + ", " + pose.getGeodeticLongitude() + ", " + pose.getGeodeticHeight() + ";");
            Log.Debug(TAG, "calculated PDOP:" + PDOP.get(0) + ";");

            return pose;

        }

        public override double getClockBias()
        {
            return clockBias;
        }

        public override string getName()
        {
            return NAME;
        }
    }
}