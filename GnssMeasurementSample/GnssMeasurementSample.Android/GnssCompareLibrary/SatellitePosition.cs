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
    public class SatellitePosition : Coordinates<Matrix>
    {
		public static readonly SatellitePosition UnhealthySat = new SatellitePosition(0, 0, '0', 0, 0, 0);

		private int satID; /* Satellite ID number */
		private char satType;
		private double satelliteClockError; /* Correction due to satellite clock error in seconds*/
		//private double range;
		private long unixTime;
		private bool predicted;
		private bool maneuver;
		private SimpleMatrix<Matrix> speed;

		public SatellitePosition(long unixTime, int satID, char satType, double x, double y, double z) : base()
		{
			this.unixTime = unixTime;
			this.satID = satID;
			this.satType = satType;

			this.setXYZ(x, y, z);
			this.speed = new SimpleMatrix<Matrix>(3, 1);
		}

		/**
		 * @return the satID
		 */
		public int getSatID()
		{
			return satID;
		}

		/**
		 * @param satID the satID to set
		 */
		public void setSatID(int satID)
		{
			this.satID = satID;
		}

		/**
		 * @return the satType
		 */
		public char getSatType()
		{
			return satType;
		}

		/**
		 * @param satType the satType to set
		 */
		public void setSatType(char satType)
		{
			this.satType = satType;
		}

		/**
		 * @return the timeCorrection
		 */
		public double getSatelliteClockError()
		{
			return satelliteClockError;
		}

		/**
		 * @param timeCorrection the timeCorrection to set
		 */
		public void setSatelliteClockError(double timeCorrection)
		{
			this.satelliteClockError = timeCorrection;
		}

		/**
		 * @return the time
		 */
		public long getUtcTime()
		{
			return unixTime;
		}

		/**
		 * @param predicted the predicted to set
		 */
		public void setPredicted(bool predicted)
		{
			this.predicted = predicted;
		}

		/**
		 * @return the predicted
		 */
		public bool isPredicted()
		{
			return predicted;
		}

		/**
		 * @param maneuver the maneuver to set
		 */
		public void setManeuver(bool maneuver)
		{
			this.maneuver = maneuver;
		}

		/**
		 * @return the maneuver
		 */
		public bool isManeuver()
		{
			return maneuver;
		}

		public SimpleMatrix<Matrix> getSpeed()
		{
			return speed;
		}

		public void setSpeed(double xdot, double ydot, double zdot)
		{
			this.speed.set(0, xdot);
			this.speed.set(1, ydot);
			this.speed.set(2, zdot);
		}

		public String toString()
		{
			return "X:" + this.getX() + " Y:" + this.getY() + " Z:" + getZ() + " clkCorr:" + getSatelliteClockError();
		}

		public Object clone()
		{
			SatellitePosition sp = new SatellitePosition(this.unixTime, this.satID, this.satType, this.getX(), this.getY(), this.getZ());
			sp.maneuver = this.maneuver;
			sp.predicted = this.predicted;
			sp.satelliteClockError = this.satelliteClockError;
			sp.setSpeed(speed.get(0), speed.get(1), speed.get(2));
			return sp;
		}
	}
}