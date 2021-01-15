using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary
{
    public class TopocentricCoordinates<W>
		where W:Matrix
    {

        private SimpleMatrix<W> topocentric = new SimpleMatrix<W>(3, 1); /* Azimuth (az), elevation (el), distance (d) */

        public TopocentricCoordinates()
        {

        }

        public TopocentricCoordinates(Coordinates<W> origin, Coordinates<W> target)
        {
            computeTopocentric(origin, target);
        }

		/**
		 * @param origin
		 */
		public TopocentricCoordinates<W> computeTopocentric(Coordinates<W> origin, Coordinates<W> target)
		{

			//		// Build rotation matrix from global to local reference systems
			//		SimpleMatrix R = globalToLocalMatrix(origin);
			//
			//		// Compute local vector from origin to this object coordinates
			//		//SimpleMatrix enu = R.mult(target.ecef.minus(origin.ecef));
			//		SimpleMatrix enu = R.mult(target.minusXYZ(origin));

			origin.computeLocalV2(target);

			double E = origin.getE();//enu.get(0);
			double N = origin.getN();//enu.get(1);
			double U = origin.getU();//enu.get(2);

			// Compute horizontal distance from origin to this object
			double hDist = Math.Sqrt(Math.Pow(E, 2) + Math.Pow(N, 2));

			// If this object is at zenith ...
			if (hDist < 1e-20)
			{
				// ... set azimuth = 0 and elevation = 90, ...
				topocentric.set(0, 0, 0);
				topocentric.set(1, 0, 90);

			}
			else
			{

				// ... otherwise compute azimuth ...
				topocentric.set(0, 0, Math.Atan2(E, N)/Math.PI*180.0);

				// ... and elevation
				topocentric.set(1, 0, Math.Atan2(U, hDist) / Math.PI * 180.0);

				if (topocentric.get(0) < 0)
					topocentric.set(0, 0, topocentric.get(0) + 360);
			}

			// Compute distance
			topocentric.set(2, 0, Math.Sqrt(Math.Pow(E, 2) + Math.Pow(N, 2) + Math.Pow(U, 2)));

			return this;
		}

		public double getAzimuth()
		{
			return topocentric.get(0);
		}

		public double getElevation()
		{
			return topocentric.get(1);
		}

		public double getDistance()
		{
			return topocentric.get(2);
		}

		//	/**
		//	 * @param origin
		//	 * @return Rotation matrix from global to local reference systems
		//	 */
		//	private SimpleMatrix globalToLocalMatrix(Coordinates origin) {
		//
		//		double lam = Math.toRadians(origin.getGeodeticLongitude());
		//		double phi = Math.toRadians(origin.getGeodeticLatitude());
		//
		//		double cosLam = Math.cos(lam);
		//		double cosPhi = Math.cos(phi);
		//		double sinLam = Math.sin(lam);
		//		double sinPhi = Math.sin(phi);
		//
		//		double[][] data = new double[3][3];
		//		data[0][0] = -sinLam;
		//		data[0][1] = cosLam;
		//		data[0][2] = 0;
		//		data[1][0] = -sinPhi * cosLam;
		//		data[1][1] = -sinPhi * sinLam;
		//		data[1][2] = cosPhi;
		//		data[2][0] = cosPhi * cosLam;
		//		data[2][1] = cosPhi * sinLam;
		//		data[2][2] = sinPhi;
		//
		//		SimpleMatrix R = new SimpleMatrix(data);
		//
		//		return R;
		//	}


	}
}