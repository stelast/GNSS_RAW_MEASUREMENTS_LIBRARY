using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex
{
    public class ObservationSet : Streamable
	{
		private readonly static int STREAM_V = 1;


		public readonly static int L1 = 0;
		public readonly static int L2 = 1;


		private int satID;  /* Satellite number */
		private char satType;   /* Satellite Type */

		/* Array of [L1,L2] */
		private double[] codeC = { double.NaN, double.NaN };            /* C Coarse/Acquisition (C/A) code [m] */
		private double[] codeP = { double.NaN, double.NaN };            /* P Code Pseudorange [m] */
		private double[] phase = { double.NaN, double.NaN };            /* L Carrier Phase [cycle] */
		private float[] signalStrength = { float.NaN, float.NaN };      /* C/N0 (signal strength) [dBHz] */
		private float[] doppler = { float.NaN, float.NaN };         /* Doppler value [Hz] */

		private int[] qualityInd = { -1, -1 };  /* Nav Measurements Quality Ind. ublox proprietary? */

		/*
		 * Loss of lock indicator (LLI). Range: 0-7
		 *  0 or blank: OK or not known
		 *  Bit 0 set : Lost lock between previous and current observation: cycle slip possible
		 *  Bit 1 set : Opposite wavelength factor to the one defined for the satellite by a previous WAVELENGTH FACT L1/2 line. Valid for the current epoch only.
		 *  Bit 2 set : Observation under Antispoofing (may suffer from increased noise)
		 * Bits 0 and 1 for phase only.
		 */
		private int[] lossLockInd = { -1, -1 };

		/*
		 * Signal strength indicator projected into interval 1-9:
		 *  1: minimum possible signal strength
		 *  5: threshold for good S/N ratio
		 *  9: maximum possible signal strength
		 * 0 or blank: not known, don't care
		 */
		private int[] signalStrengthInd = { -1, -1 };

		private int freqNum;

		/* Sets whether this obs is in use or not:
			 could be below the elevation threshold for example
		 or unhealthy
	  */
		private bool inUse = false;

		/* residual error */
		public double eRes;

		/**
		 * topocentric elevation
		 */
		public double el;

		public ObservationSet()
		{
		}

		public ObservationSet(DataInputStream dai, bool oldVersion) /*throws IOException*/
		{
			read(dai, oldVersion);
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
		 * @return the phase range (in meters)
		 */
		public double getPhaserange(int i)
		{
			return phase[i] * getWavelength(i);
		}

		public double getWavelength(int i)
		{
			double frequency = 0;
			switch (this.satType)
			{
				case 'G': frequency = (i == 0) ? Constants.FL1 : Constants.FL2;break;
				case 'R': frequency = (i == 0) ? freqNum * Constants.FR1_delta + Constants.FR1_base : freqNum * Constants.FR2_delta + Constants.FR2_base; break;
				case 'E': frequency = (i == 0) ? Constants.FE1 : Constants.FE5a; break;
				case 'C': frequency = (i == 0) ? Constants.FC2 : Constants.FC5b; break;
				case 'J': frequency = (i == 0) ? Constants.FJ1 : Constants.FJ2; break;
			}
			return Constants.SPEED_OF_LIGHT / frequency;
		}

		/**
		 * @return the pseudorange (in meters)
		 */
		public double getPseudorange(int i)
		{
			return Double.IsNaN(codeP[i]) ? codeC[i] : codeP[i];
		}

		public bool isPseudorangeP(int i)
		{
			return !Double.IsNaN(codeP[i]);
		}

		/**
		 * @return the c
		 */
		public double getCodeC(int i)
		{
			return codeC[i];
		}

		/**
		 * @param c the c to set
		 */
		public void setCodeC(int i, double c)
		{
			codeC[i] = c;
		}

		/**
		 * @return the p
		 */
		public double getCodeP(int i)
		{
			return codeP[i];
		}

		/**
		 * @param p the p to set
		 */
		public void setCodeP(int i, double p)
		{
			codeP[i] = p;
		}

		/**
		 * @return the l
		 */
		public double getPhaseCycles(int i)
		{
			return phase[i];
		}

		/**
		 * @param l the l to set
		 */
		public void setPhaseCycles(int i, double l)
		{
			phase[i] = l;
		}

		/**
		 * @return the s
		 */
		public float getSignalStrength(int i)
		{
			return signalStrength[i];
		}

		/**
		 * @param s the s to set
		 */
		public void setSignalStrength(int i, float s)
		{
			signalStrength[i] = s;
		}

		/**
		 * @return the d
		 */
		public float getDoppler(int i)
		{
			return doppler[i];
		}

		/**
		 * @param d the d to set
		 */
		public void setDoppler(int i, float d)
		{
			doppler[i] = d;
		}

		/**
		 * @param signalStrengthInd the signalStrengthInd to set
		 */
		public void setFreqNum(int freqNum)
		{
			this.freqNum = freqNum;
		}
		public void read(DataInputStream dai, bool oldVersion)
        {
            throw new NotImplementedException();
        }

        public int write(DataOutputStream dos)
        {
            throw new NotImplementedException();
        }
    }
}