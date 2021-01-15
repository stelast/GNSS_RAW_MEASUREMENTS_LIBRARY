using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex
{
    public class IonoGalileo : Streamable
	{
		private readonly static int STREAM_V = 1;


		/** Bitmask, every bit represenst a GPS SV (1-32). If the bit is set the SV is healthy. */
		private long health = 0;

		/** UTC - parameter A1. */
		private double utcA1;

		/** UTC - parameter A0. */
		private double utcA0;

		/** UTC - reference time of week. */
		private long utcTOW;

		/** UTC - reference week number. */
		private int utcWNT;

		/** UTC - time difference due to leap seconds before event. */
		private int utcLS;

		/** UTC - week number when next leap second event occurs. */
		private int utcWNF;

		/** UTC - day of week when next leap second event occurs. */
		private int utcDN;

		/** UTC - time difference due to leap seconds after event. */
		private int utcLSF;

		/** NeQuick Ionospheric model. */
		private float[] alpha = new float[4];

		/** Klobuchar - beta. */
		private float[] beta = new float[4];

		/** Healthmask field in this message is valid. */
		private bool validHealth;

		/** UTC parameter fields in this message are valid. */
		private bool validUTC;

		/** Klobuchar parameter fields in this message are valid. */
		private bool validKlobuchar;

		/** Reference time. */
		private Time refTime;

		public IonoGalileo()
		{

		}
		public IonoGalileo(DataInputStream dai, bool oldVersion) /*throws IOException*/
		{
			read(dai, oldVersion);
		}

		//public IonoGalileo(Ephemeris.IonosphericModelProto ionoProto2)
		//{
		//	this.alpha[0] = (float)ionoProto2.getAlpha(0);
		//	this.alpha[1] = (float)ionoProto2.getAlpha(1);
		//	this.alpha[2] = (float)ionoProto2.getAlpha(2);
		//}

		/**
		 * Gets the reference time.
		 *
		 * @return the refTime
		 */
		public Time getRefTime()
		{
			return refTime;
		}

		/**
		 * Sets the reference time.
		 *
		 * @param refTime the refTime to set
		 */
		public void setRefTime(Time refTime)
		{
			this.refTime = refTime;
		}

		/**
		 * Instantiates a new iono gps.
		 */
		public IonoGalileo(Time refTime)
		{
			this.refTime = refTime;
		}

		/**
		 * Gets the bitmask, every bit represenst a GPS SV (1-32).
		 *
		 * @return the health
		 */
		public long getHealth()
		{
			return health;
		}

		/**
		 * Sets the bitmask, every bit represenst a GPS SV (1-32).
		 *
		 * @param health the health to set
		 */
		public void setHealth(long health)
		{
			this.health = health;
		}

		/**
		 * Gets the UTC - parameter A1.
		 *
		 * @return the utcA1
		 */
		public double getUtcA1()
		{
			return utcA1;
		}

		/**
		 * Sets the UTC - parameter A1.
		 *
		 * @param utcA1 the utcA1 to set
		 */
		public void setUtcA1(double utcA1)
		{
			this.utcA1 = utcA1;
		}

		/**
		 * Gets the UTC - parameter A0.
		 *
		 * @return the utcA0
		 */
		public double getUtcA0()
		{
			return utcA0;
		}

		/**
		 * Sets the UTC - parameter A0.
		 *
		 * @param utcA0 the utcA0 to set
		 */
		public void setUtcA0(double utcA0)
		{
			this.utcA0 = utcA0;
		}

		/**
		 * Gets the UTC - reference time of week.
		 *
		 * @return the utcTOW
		 */
		public long getUtcTOW()
		{
			return utcTOW;
		}

		/**
		 * Sets the UTC - reference time of week.
		 *
		 * @param utcTOW the utcTOW to set
		 */
		public void setUtcTOW(long utcTOW)
		{
			this.utcTOW = utcTOW;
		}

		/**
		 * Gets the UTC - reference week number.
		 *
		 * @return the utcWNT
		 */
		public int getUtcWNT()
		{
			return utcWNT;
		}

		/**
		 * Sets the UTC - reference week number.
		 *
		 * @param utcWNT the utcWNT to set
		 */
		public void setUtcWNT(int utcWNT)
		{
			this.utcWNT = utcWNT;
		}

		/**
		 * Gets the UTC - time difference due to leap seconds before event.
		 *
		 * @return the utcLS
		 */
		public int getUtcLS()
		{
			return utcLS;
		}

		/**
		 * Sets the UTC - time difference due to leap seconds before event.
		 *
		 * @param utcLS the utcLS to set
		 */
		public void setUtcLS(int utcLS)
		{
			this.utcLS = utcLS;
		}

		/**
		 * Gets the UTC - week number when next leap second event occurs.
		 *
		 * @return the utcWNF
		 */
		public int getUtcWNF()
		{
			return utcWNF;
		}

		/**
		 * Sets the UTC - week number when next leap second event occurs.
		 *
		 * @param utcWNF the utcWNF to set
		 */
		public void setUtcWNF(int utcWNF)
		{
			this.utcWNF = utcWNF;
		}

		/**
		 * Gets the UTC - day of week when next leap second event occurs.
		 *
		 * @return the utcDN
		 */
		public int getUtcDN()
		{
			return utcDN;
		}

		/**
		 * Sets the UTC - day of week when next leap second event occurs.
		 *
		 * @param utcDN the utcDN to set
		 */
		public void setUtcDN(int utcDN)
		{
			this.utcDN = utcDN;
		}

		/**
		 * Gets the UTC - time difference due to leap seconds after event.
		 *
		 * @return the utcLSF
		 */
		public int getUtcLSF()
		{
			return utcLSF;
		}

		/**
		 * Sets the UTC - time difference due to leap seconds after event.
		 *
		 * @param utcLSF the utcLSF to set
		 */
		public void setUtcLSF(int utcLSF)
		{
			this.utcLSF = utcLSF;
		}

		/**
		 * Gets the klobuchar - alpha.
		 *
		 * @param i the i<sup>th<sup> value in the range 0-3
		 * @return the alpha
		 */
		public float getAlpha(int i)
		{
			return alpha[i];
		}

		/**
		 * Sets the klobuchar - alpha.
		 *
		 * @param alpha the alpha to set
		 */
		public void setAlpha(float[] alpha)
		{
			this.alpha = alpha;
		}

		/**
		 * Gets the klobuchar - beta.
		 *
		 * @param i the i<sup>th<sup> value in the range 0-3
		 * @return the beta
		 */
		public float getBeta(int i)
		{
			return beta[i];
		}

		/**
		 * Sets the klobuchar - beta.
		 *
		 * @param beta the beta to set
		 */
		public void setBeta(float[] beta)
		{
			this.beta = beta;
		}

		/**
		 * Checks if is healthmask field in this message is valid.
		 *
		 * @return the validHealth
		 */
		public bool isValidHealth()
		{
			return validHealth;
		}

		/**
		 * Sets the healthmask field in this message is valid.
		 *
		 * @param validHealth the validHealth to set
		 */
		public void setValidHealth(bool validHealth)
		{
			this.validHealth = validHealth;
		}

		/**
		 * Checks if is UTC parameter fields in this message are valid.
		 *
		 * @return the validUTC
		 */
		public bool isValidUTC()
		{
			return validUTC;
		}

		/**
		 * Sets the UTC parameter fields in this message are valid.
		 *
		 * @param validUTC the validUTC to set
		 */
		public void setValidUTC(bool validUTC)
		{
			this.validUTC = validUTC;
		}

		/**
		 * Checks if is klobuchar parameter fields in this message are valid.
		 *
		 * @return the validKlobuchar
		 */
		public bool isValidKlobuchar()
		{
			return validKlobuchar;
		}

		/**
		 * Sets the klobuchar parameter fields in this message are valid.
		 *
		 * @param validKlobuchar the validKlobuchar to set
		 */
		public void setValidKlobuchar(bool validKlobuchar)
		{
			this.validKlobuchar = validKlobuchar;
		}
		/* (non-Javadoc)
		 * @see org.gogpsproject.Streamable#write(java.io.DataOutputStream)
		 */
		public void read(DataInputStream dai, bool oldVersion)
		{
			int v = 1;
			if (!oldVersion) v = dai.ReadInt();

			if (v == 1)
			{

				health = dai.ReadLong();
				utcA1 = dai.ReadDouble();
				utcA0 = dai.ReadDouble();
				utcTOW = dai.ReadLong();
				utcWNT = dai.ReadInt();
				utcLS = dai.ReadInt();
				utcWNF = dai.ReadInt();
				utcDN = dai.ReadInt();
				utcLSF = dai.ReadInt();
				for (int i = 0; i < alpha.Count(); i++)
				{
					alpha[i] = dai.ReadFloat();
				}
				for (int i = 0; i < beta.Count(); i++)
				{
					beta[i] = dai.ReadFloat();
				}
				validHealth = dai.ReadBoolean();
				validUTC = dai.ReadBoolean();
				validKlobuchar = dai.ReadBoolean();
				long l = dai.ReadLong();
				refTime = new Time(l > 0 ? l : JavaSystem.CurrentTimeMillis());
			}
			else
			{
				throw new IOException("Unknown format version:" + v);
			}
		}

        public int write(DataOutputStream dos)
		{
			int size = 5;
			dos.WriteUTF(Streamable.MESSAGE_IONO); // 5
			dos.WriteInt(STREAM_V); size += 4;
			dos.WriteLong(health); size += 8;
			dos.WriteDouble(utcA1); size += 8;
			dos.WriteDouble(utcA0); size += 8;
			dos.WriteLong(utcTOW); size += 8;
			dos.WriteInt(utcWNT); size += 4;
			dos.WriteInt(utcLS); size += 4;
			dos.WriteInt(utcWNF); size += 4;
			dos.WriteInt(utcDN); size += 4;
			dos.WriteInt(utcLSF); size += 4;
			for (int i = 0; i < alpha.Count(); i++)
			{
				dos.WriteFloat(alpha[i]); size += 4;
			}
			for (int i = 0; i < beta.Count(); i++)
			{
				dos.WriteFloat(beta[i]); size += 4;
			}
			dos.WriteBoolean(validHealth); size += 1;
			dos.WriteBoolean(validUTC); size += 1;
			dos.WriteBoolean(validKlobuchar); size += 1;
			dos.WriteLong(refTime == null ? -1 : refTime.getMsec()); size += 8;

			return size;
		}
    }
}