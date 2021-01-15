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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations
{
	public class EphGalileo : Streamable
	{
		private readonly static int STREAM_V = 1;

		private Time refTime; /* Reference time of the dataset */
		private char satType; /* Satellite Type */
		private int satID; /* Satellite ID number */
		private int week; /* GPS week number */

		private int L2Code; /* Code on L2 */
		private int L2Flag; /* L2 P data flag */

		private int svAccur; /* SV accuracy (URA index) */
		private int svHealth; /* SV health */

		private int iode; /* Issue of data (ephemeris) */
		private int iodc; /* Issue of data (clock) */

		private double toc; /* clock data reference time */
		private double toe; /* ephemeris reference time */
		private double tom; /* transmission time of message */

		/* satellite clock parameters */
		private double af0;
		private double af1;
		private double af2;
		private double tgd;

		/* satellite orbital parameters */
		private double rootA; /* Square root of the semimajor axis */
		private double e; /* Eccentricity */
		private double i0; /* Inclination angle at reference time */
		private double iDot; /* Rate of inclination angle */
		private double omega; /* Argument of perigee */
		private double omega0; /*
	 * Longitude of ascending node of orbit plane at beginning
	 * of week
	 */
		private double omegaDot; /* Rate of right ascension */
		private double M0; /* Mean anomaly at reference time */
		private double deltaN; /* Mean motion difference from computed value */
		private double crc, crs, cuc, cus, cic, cis; /*
	 * Amplitude of second-order harmonic
	 * perturbations
	 */
		private long fitInt; /* Fit interval */

		/* for GLONASS data */
		private float tow;

		private float tauN;
		private float gammaN;
		private double tk;

		private double X;
		private double Xv;
		private double Xa;
		private double Bn;

		private double Y;
		private double Yv;
		private double Ya;
		private int freq_num;
		private double tb;

		private double Z;
		private double Zv;
		private double Za;
		private double En;

		public static readonly EphGps UnhealthyEph = new EphGps();


		public EphGalileo()
		{

		}
		public EphGalileo(DataInputStream dai, bool oldVersion)
		{
			read(dai, oldVersion);
		}

		public EphGalileo(GalEphemeris ephemerids)
		{
			this.satID = ephemerids.svid;
			this.satType = 'E';
			this.week = ephemerids.week;
			this.svAccur = (int)ephemerids.sisaM; // not sure if this is correct SuplClient variable
			this.svHealth = ephemerids.health;
			this.iodc = 0; //ephemerids.iodc; -> not found in the new eph from SuplClient
			this.iode = ephemerids.iode;
			this.toc = ephemerids.tocS;
			this.toe = ephemerids.keplerModel.toeS;
			this.af0 = ephemerids.af0S;
			this.af1 = ephemerids.af1SecPerSec;
			this.af2 = ephemerids.af2SecPerSec2;
			this.tgd = ephemerids.tgdS;
			this.rootA = ephemerids.keplerModel.sqrtA;
			this.e = ephemerids.keplerModel.eccentricity;
			this.i0 = ephemerids.keplerModel.i0;
			this.iDot = ephemerids.keplerModel.iDot;
			this.omega = ephemerids.keplerModel.omega;
			this.omega0 = ephemerids.keplerModel.omega0;
			this.omegaDot = ephemerids.keplerModel.omegaDot;
			this.M0 = ephemerids.keplerModel.m0;
			this.deltaN = ephemerids.keplerModel.deltaN;
			this.cic = ephemerids.keplerModel.cic;
			this.cis = ephemerids.keplerModel.cis;
			this.crc = ephemerids.keplerModel.crc;
			this.crs = ephemerids.keplerModel.crs;
			this.cuc = ephemerids.keplerModel.cuc;
			this.cus = ephemerids.keplerModel.cus;
			this.fitInt = 0; //(long) ephemerids.fitInterval -> not found in the new eph from SuplClient
			this.refTime = new Time(week, toc, satType);
		}

		/**
	 * @return the refTime
	 */
		public Time getRefTime()
		{
			return refTime;
		}
		/**
		 * @param refTime the refTime to set
		 */
		public void setRefTime(Time refTime)
		{
			this.refTime = refTime;
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
		 * @return the week
		 */
		public int getWeek()
		{
			return week;
		}
		/**
		 * @param week the week to set
		 */
		public void setWeek(int week)
		{
			this.week = week;
		}
		/**
		 * @return the l2Code
		 */
		public int getL2Code()
		{
			return L2Code;
		}
		/**
		 * @param l2Code the l2Code to set
		 */
		public void setL2Code(int l2Code)
		{
			L2Code = l2Code;
		}
		/**
		 * @return the l2Flag
		 */
		public int getL2Flag()
		{
			return L2Flag;
		}
		/**
		 * @param l2Flag the l2Flag to set
		 */
		public void setL2Flag(int l2Flag)
		{
			L2Flag = l2Flag;
		}
		/**
		 * @return the svAccur
		 */
		public int getSvAccur()
		{
			return svAccur;
		}
		/**
		 * @param svAccur the svAccur to set
		 */
		public void setSvAccur(int svAccur)
		{
			this.svAccur = svAccur;
		}
		/**
		 * @return the svHealth
		 */
		public int getSvHealth()
		{
			return svHealth;
		}
		/**
		 * @param svHealth the svHealth to set
		 */
		public void setSvHealth(int svHealth)
		{
			this.svHealth = svHealth;
		}
		/**
		 * @return the iode
		 */
		public int getIode()
		{
			return iode;
		}
		/**
		 * @param iode the iode to set
		 */
		public void setIode(int iode)
		{
			this.iode = iode;
		}
		/**
		 * @return the iodc
		 */
		public int getIodc()
		{
			return iodc;
		}
		/**
		 * @param iodc the iodc to set
		 */
		public void setIodc(int iodc)
		{
			this.iodc = iodc;
		}
		/**
		 * @return the toc
		 */
		public double getToc()
		{
			return toc;
		}
		/**
		 * @param toc the toc to set
		 */
		public void setToc(double toc)
		{
			this.toc = toc;
		}
		/**
		 * @return the toe
		 */
		public double getToe()
		{
			return toe;
		}
		/**
		 * @param toe the toe to set
		 */
		public void setToe(double toe)
		{
			this.toe = toe;
		}
		/**
		 * @return the tom
		 */
		public double getTom()
		{
			return tom;
		}
		/**
		 * @param tom the tom to set
		 */
		public void setTom(double tom)
		{
			this.tom = tom;
		}


		/**
		 * @return the af0
		 */
		public double getAf0()
		{
			return af0;
		}
		/**
		 * @param af0 the af0 to set
		 */
		public void setAf0(double af0)
		{
			this.af0 = af0;
		}
		/**
		 * @return the af1
		 */
		public double getAf1()
		{
			return af1;
		}
		/**
		 * @param af1 the af1 to set
		 */
		public void setAf1(double af1)
		{
			this.af1 = af1;
		}
		/**
		 * @return the af2
		 */
		public double getAf2()
		{
			return af2;
		}
		/**
		 * @param af2 the af2 to set
		 */
		public void setAf2(double af2)
		{
			this.af2 = af2;
		}
		/**
		 * @return the tgd
		 */
		public double getTgd()
		{
			return tgd;
		}
		/**
		 * @param tgd the tgd to set
		 */
		public void setTgd(double tgd)
		{
			this.tgd = tgd;
		}
		/**
		 * @return the rootA
		 */
		public double getRootA()
		{
			return rootA;
		}
		/**
		 * @param rootA the rootA to set
		 */
		public void setRootA(double rootA)
		{
			this.rootA = rootA;
		}
		/**
		 * @return the e
		 */
		public double getE()
		{
			return e;
		}
		/**
		 * @param e the e to set
		 */
		public void setE(double e)
		{
			this.e = e;
		}
		/**
		 * @return the i0
		 */
		public double getI0()
		{
			return i0;
		}
		/**
		 * @param i0 the i0 to set
		 */
		public void setI0(double i0)
		{
			this.i0 = i0;
		}
		/**
		 * @return the iDot
		 */
		public double getiDot()
		{
			return iDot;
		}
		/**
		 * @param iDot the iDot to set
		 */
		public void setiDot(double iDot)
		{
			this.iDot = iDot;
		}
		/**
		 * @return the omega
		 */
		public double getOmega()
		{
			return omega;
		}
		/**
		 * @param omega the omega to set
		 */
		public void setOmega(double omega)
		{
			this.omega = omega;
		}
		/**
		 * @return the omega0
		 */
		public double getOmega0()
		{
			return omega0;
		}
		/**
		 * @param omega0 the omega0 to set
		 */
		public void setOmega0(double omega0)
		{
			this.omega0 = omega0;
		}
		/**
		 * @return the omegaDot
		 */
		public double getOmegaDot()
		{
			return omegaDot;
		}
		/**
		 * @param omegaDot the omegaDot to set
		 */
		public void setOmegaDot(double omegaDot)
		{
			this.omegaDot = omegaDot;
		}
		/**
		 * @return the m0
		 */
		public double getM0()
		{
			return M0;
		}
		/**
		 * @param m0 the m0 to set
		 */
		public void setM0(double m0)
		{
			M0 = m0;
		}
		/**
		 * @return the deltaN
		 */
		public double getDeltaN()
		{
			return deltaN;
		}
		/**
		 * @param deltaN the deltaN to set
		 */
		public void setDeltaN(double deltaN)
		{
			this.deltaN = deltaN;
		}
		/**
		 * @return the crc
		 */
		public double getCrc()
		{
			return crc;
		}
		/**
		 * @param crc the crc to set
		 */
		public void setCrc(double crc)
		{
			this.crc = crc;
		}
		/**
		 * @return the crs
		 */
		public double getCrs()
		{
			return crs;
		}
		/**
		 * @param crs the crs to set
		 */
		public void setCrs(double crs)
		{
			this.crs = crs;
		}
		/**
		 * @return the cuc
		 */
		public double getCuc()
		{
			return cuc;
		}
		/**
		 * @param cuc the cuc to set
		 */
		public void setCuc(double cuc)
		{
			this.cuc = cuc;
		}
		/**
		 * @return the cus
		 */
		public double getCus()
		{
			return cus;
		}
		/**
		 * @param cus the cus to set
		 */
		public void setCus(double cus)
		{
			this.cus = cus;
		}
		/**
		 * @return the cic
		 */
		public double getCic()
		{
			return cic;
		}
		/**
		 * @param cic the cic to set
		 */
		public void setCic(double cic)
		{
			this.cic = cic;
		}
		/**
		 * @return the cis
		 */
		public double getCis()
		{
			return cis;
		}
		/**
		 * @param cis the cis to set
		 */
		public void setCis(double cis)
		{
			this.cis = cis;
		}
		/**
		 * @return the fitInt
		 */
		public long getFitInt()
		{
			return fitInt;
		}
		/**
		 * @param fitInt the fitInt to set
		 */
		public void setFitInt(long fitInt)
		{
			this.fitInt = fitInt;
		}





		/* (non-Javadoc)
		 * @see org.gogpsproject.Streamable#write(java.io.DataOutputStream)
		 */
	public int write(DataOutputStream dos)
		{
		int size = 5;
		dos.WriteUTF(Streamable.MESSAGE_EPHEMERIS); // 5
		dos.WriteInt(STREAM_V); size+=4; // 4

		dos.WriteLong(refTime==null?-1:refTime.getMsec());  size +=8;
		dos.Write(satID);  size +=1;
		dos.WriteInt(week); size +=4;

		dos.WriteInt(L2Code); size +=4;
		dos.WriteInt(L2Flag); size +=4;

		dos.WriteInt(svAccur); size +=4;
		dos.WriteInt(svHealth); size +=4;

		dos.WriteInt(iode); size +=4;
		dos.WriteInt(iodc); size +=4;

		dos.WriteDouble(toc); size +=8;
		dos.WriteDouble(toe); size +=8;

		dos.WriteDouble(af0); size +=8;
		dos.WriteDouble(af1); size +=8;
		dos.WriteDouble(af2); size +=8;
		dos.WriteDouble(tgd); size +=8;


		dos.WriteDouble(rootA); size +=8;
		dos.WriteDouble(e); size +=8;
		dos.WriteDouble(i0); size +=8;
		dos.WriteDouble(iDot); size +=8;
		dos.WriteDouble(omega); size +=8;
		dos.WriteDouble(omega0); size +=8;

		dos.WriteDouble(omegaDot); size +=8;
		dos.WriteDouble(M0); size +=8;
		dos.WriteDouble(deltaN); size +=8;
		dos.WriteDouble(crc); size +=8;
		dos.WriteDouble(crs); size +=8;
		dos.WriteDouble(cuc); size +=8;
		dos.WriteDouble(cus); size +=8;
		dos.WriteDouble(cic); size +=8;
		dos.WriteDouble(cis); size +=8;

		dos.WriteDouble(fitInt); size +=8;

		return size;
	}
	/* (non-Javadoc)
	 * @see org.gogpsproject.Streamable#read(java.io.DataInputStream)
	 */
	public void read(DataInputStream dai, bool oldVersion)  
	{
		int v=1;
		if(!oldVersion) v=dai.ReadInt();

		if(v==1){
			long l = dai.ReadLong();
			refTime = new Time(l > 0 ? l : GetCurrentMilli());
			satID = dai.Read();
			week = dai.ReadInt();
			L2Code = dai.ReadInt();
			L2Flag = dai.ReadInt();
			svAccur = dai.ReadInt();
			svHealth = dai.ReadInt();
			iode = dai.ReadInt();
			iodc = dai.ReadInt();
			toc = dai.ReadDouble();
			toe = dai.ReadDouble();
			af0 = dai.ReadDouble();
			af1 = dai.ReadDouble();
			af2 = dai.ReadDouble();
			tgd = dai.ReadDouble();
			rootA = dai.ReadDouble();
			e = dai.ReadDouble();
			i0 = dai.ReadDouble();
			iDot = dai.ReadDouble();
			omega = dai.ReadDouble();
			omega0 = dai.ReadDouble();
			omegaDot = dai.ReadDouble();
			M0 = dai.ReadDouble();
			deltaN = dai.ReadDouble();
			crc = dai.ReadDouble();
			crs = dai.ReadDouble();
			cuc = dai.ReadDouble();
			cus = dai.ReadDouble();
			cic = dai.ReadDouble();
			cis = dai.ReadDouble();
			fitInt = dai.ReadLong();
		}else{
			throw new IOException("Unknown format version:" + v);
		}
	}
		public static long GetCurrentMilli()
		{
			DateTime Jan1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan javaSpan = DateTime.UtcNow - Jan1970;
			return javaSpan.TotalMilliseconds;
		}
	}
}
