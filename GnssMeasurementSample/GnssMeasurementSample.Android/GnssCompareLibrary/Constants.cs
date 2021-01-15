using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary
{
    public class Constants
    {

		// Speed of Light [m/s]
		public static readonly double SPEED_OF_LIGHT = 299792458.0;

		// Physical quantities as in IS-GPS
		public static readonly double EARTH_GRAVITATIONAL_CONSTANT = 3.986005e14;
		public static readonly double EARTH_ANGULAR_VELOCITY = 7.2921151467e-5;
		public static readonly double RELATIVISTIC_ERROR_CONSTANT = -4.442807633e-10;

		// GPS signal approximate travel time
		public static readonly double GPS_APPROX_TRAVEL_TIME = 0.072;

		// WGS84 ellipsoid features
		public static readonly double WGS84_SEMI_MAJOR_AXIS = 6378137;
		public static readonly double WGS84_FLATTENING = 1 / 298.257222101;
		public static readonly double WGS84_ECCENTRICITY = Math.Sqrt(1 - Math.Pow(
				(1 - WGS84_FLATTENING), 2));

		// Time-related values
		public static readonly double HUNDREDSMILLI = 1e-1; // 100 ms in seconds
		public static readonly double ONEMILLI = 1e-3;      // 1 ms in seconds

		public static readonly long DAYS_IN_WEEK = 7L;
		public static readonly long SEC_IN_DAY = 86400L;
		public static readonly long SEC_IN_HOUR = 3600L;
		public static readonly long MILLISEC_IN_SEC = 1000L;
		public static readonly long SEC_IN_HALF_WEEK = 302400L;
		// Days difference between UNIX time and GPS time
		public static readonly long UNIX_GPS_DAYS_DIFF = 3657L;
		public static readonly long UNIX_GST_DAYS_DIFF = 10825L; //935280000 seconds
															  // MFB just add nanoseconds in a week
		public static readonly long NUMBER_NANO_SECONDS_PER_WEEK = 604800000000000L;
		public static readonly long WEEKSEC = 604800;

		public static readonly double NumberNanoSeconds100Milli = 1e8;         // 100 ms expressed in nanoseconds
		public static readonly double NumberNanoSeconds1Milli = 1e6;           // 100 ms expressed in nanoseconds

		// Standard atmosphere - Berg, 1948 (Bernese)
		public static readonly double STANDARD_PRESSURE = 1013.25;
		public static readonly double STANDARD_TEMPERATURE = 291.15;

		// Parameters to weigh observations by signal-to-noise ratio
		public static readonly float SNR_a = 30;
		public static readonly float SNR_A = 30;
		public static readonly float SNR_0 = 10;
		public static readonly float SNR_1 = 50;

		/*
		 CONSTELLATION REF 
		 CRS parameters, according to each GNSS system CRS definition
		 (ICD document in brackets):

		 *_GPS --> WGS-84  (IS-GPS200E)
		 *_GLO --> PZ-90   (GLONASS-ICD 5.1)
		 *_GAL --> GTRF    (Galileo-ICD 1.1)
		 *_BDS --> CSG2000 (BeiDou-ICD 1.0)
		 *_QZS --> WGS-84  (IS-QZSS 1.5D)
		*/

		//GNSS frequencies
		public static readonly double FL1 = 1575.420e6; // GPS
		public static readonly double FL2 = 1227.600e6;
		public static readonly double FL5 = 1176.450e6;

		public static readonly double FR1_base = 1602.000e6; // GLONASS
		public static readonly double FR2_base = 1246.000e6;
		public static readonly double FR1_delta = 0.5625;
		public static readonly double FR2_delta = 0.4375;

		public static readonly double FE1 = FL1; // Galileo
		public static readonly double FE5a = FL5;
		public static readonly double FE5b = 1207.140e6;
		public static readonly double FE5 = 1191.795e6;
		public static readonly double FE6 = 1278.750e6;

		public static readonly double FC1 = 1589.740e6; // BeiDou
		public static readonly double FC2 = 1561.098e6;
		public static readonly double FC5b = FE5b;
		public static readonly double FC6 = 1268.520e6;

		public static readonly double FJ1 = FL1; // QZSS
		public static readonly double FJ2 = FL2;
		public static readonly double FJ5 = FL5;
		public static readonly double FJ6 = FE6;

		// other GNSS parameters
		public static readonly long ELL_A_GPS = 6378137;                          // GPS (WGS-84)     Ellipsoid semi-major axis [m]
		public static readonly long ELL_A_GLO = 6378136;                          // GLONASS (PZ-90)  Ellipsoid semi-major axis [m]
		public static readonly long ELL_A_GAL = 6378137;                          // Galileo (GTRF)   Ellipsoid semi-major axis [m]
		public static readonly long ELL_A_BDS = 6378136;                          // BeiDou (CSG2000) Ellipsoid semi-major axis [m]
		public static readonly long ELL_A_QZS = 6378137;                          // QZSS (WGS-84)    Ellipsoid semi-major axis [m]

		public static readonly double ELL_F_GPS = 1 / 298.257222101;                  // GPS (WGS-84)     Ellipsoid flattening
		public static readonly double ELL_F_GLO = 1 / 298.257222101;                  // GLONASS (PZ-90)  Ellipsoid flattening
		public static readonly double ELL_F_GAL = 1 / 298.257222101;                  // Galileo (GTRF)   Ellipsoid flattening
		public static readonly double ELL_F_BDS = 1 / 298.257222101;                  // BeiDou (CSG2000) Ellipsoid flattening
		public static readonly double ELL_F_QZS = 1 / 298.257222101;                  // QZSS (WGS-84)    Ellipsoid flattening

		public static readonly double ELL_E_GPS = Math.Sqrt(1 - (1 - Math.Pow(ELL_F_GPS, 2)));   // GPS (WGS-84)     Eccentricity
		public static readonly double ELL_E_GLO = Math.Sqrt(1 - (1 - Math.Pow(ELL_F_GLO, 2)));   // GLONASS (PZ-90)  Eccentricity
		public static readonly double ELL_E_GAL = Math.Sqrt(1 - (1 - Math.Pow(ELL_F_GAL, 2)));   // Galileo (GTRF)   Eccentricity
		public static readonly double ELL_E_BDS = Math.Sqrt(1 - (1 - Math.Pow(ELL_F_BDS, 2)));   // BeiDou (CSG2000) Eccentricity
		public static readonly double ELL_E_QZS = Math.Sqrt(1 - (1 - Math.Pow(ELL_F_QZS, 2)));   // QZSS (WGS-84)    Eccentricity

		public static readonly double GM_GPS = 3.986005e14;                     // GPS     Gravitational constant * (mass of Earth) [m^3/s^2]
		public static readonly double GM_GLO = 3.9860044e14;                    // GLONASS Gravitational constant * (mass of Earth) [m^3/s^2]
		public static readonly double GM_GAL = 3.986004418e14;                  // Galileo Gravitational constant * (mass of Earth) [m^3/s^2]
		public static readonly double GM_BDS = 3.986004418e14;                  // BeiDou  Gravitational constant * (mass of Earth) [m^3/s^2]
		public static readonly double GM_QZS = 3.986005e14;                     // QZSS    Gravitational constant * (mass of Earth) [m^3/s^2]

		public static readonly double OMEGAE_DOT_GPS = 7.2921151467e-5;             // GPS     Angular velocity of the Earth rotation [rad/s]
		public static readonly double OMEGAE_DOT_GLO = 7.292115e-5;                 // GLONASS Angular velocity of the Earth rotation [rad/s]
		public static readonly double OMEGAE_DOT_GAL = 7.2921151467e-5;             // Galileo Angular velocity of the Earth rotation [rad/s]
		public static readonly double OMEGAE_DOT_BDS = 7.292115e-5;                 // BeiDou  Angular velocity of the Earth rotation [rad/s]
		public static readonly double OMEGAE_DOT_QZS = 7.2921151467e-5;             // QZSS    Angular velocity of the Earth rotation [rad/s]

		public static readonly double J2_GLO = 1.0826257e-3;                        // GLONASS second zonal harmonic of the geopotential

		public static readonly double PI_ORBIT = 3.1415926535898;                   // pi value used for orbit computation
		public static readonly double CIRCLE_RAD = 2 * PI_ORBIT;                    // 2 pi
	}
}