using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple;
using Java.IO;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary
{
    public class Coordinates<W> : Streamable
		where W:Matrix
	{
		private readonly static int STREAM_V = 1;

		// Global systems
		private SimpleMatrix<W> ecef = null; /* Earth-Centered, Earth-Fixed (X, Y, Z) */
		private SimpleMatrix<W> geod = null; /* Longitude (lam), latitude (phi), height (h) */

		// Local systems (require to specify an origin)
		private SimpleMatrix<W> enu; /* Local coordinates (East, North, Up) */

		private Time refTime = null;

		public Coordinates()
		{
			ecef = new SimpleMatrix<W>(3, 1);
			geod = new SimpleMatrix<W>(3, 1);
			enu = new SimpleMatrix<W>(3, 1);
		}

		public static Coordinates<W> readFromStream(DataInputStream dai, bool oldVersion) /*throws IOException*/
		{
			Coordinates<W> c = new Coordinates<W>();
			c.read(dai, oldVersion);
			return c;
		}

		public static Coordinates<W> globalXYZInstance(double x, double y, double z)
		{
			Coordinates<W> c = new Coordinates<W>();
			//c.ecef = new SimpleMatrix<W>(3, 1);
			c.setXYZ(x, y, z);
			c.computeGeodetic();
			return c;
		}
		public static Coordinates<W> globalXYZInstance(SimpleMatrix<W> ecef){
			Coordinates<W> c = new Coordinates<W>();
			c.ecef = ecef.copy();
			return c;
		}
		public static Coordinates<W> globalENUInstance(SimpleMatrix<W> ecef)
		{
			Coordinates<W> c = new Coordinates<W>();
			c.enu = ecef.copy();
			return c;
		}


		/**
		 Converts the differences between two sets of coordinates from Latitude, Longitude to
		 North and East.

		 The considered ellipsoid is WGS-84

		 @param lat      =  the latitude of the reference position [radians]
		 @param height   =  the height on the ellipsoid of the reference position [m]
		 @param deltaLat =  differences in latitude [radians]
		 @param deltaLon =  differences in longitude [radians]

		 */
		public static double[] deltaGeodeticToDeltaMeters(double lat, double height, double deltaLat, double deltaLon)
		{

			// Declare the required WGS-84 ellipsoid parameters
			double a = 6378137.0;     // Semi-major axisW
			double b = 6356752.31425; // Semi-minor axis

			// Compute the second eccentricity
			double e2 = (Math.Pow(a, 2) - Math.Pow(b, 2)) / Math.Pow(a, 2);

			// Compute additional parameter required in the processing
			double W = Math.Sqrt(1.0 - e2 * (Math.Pow(Math.Sin(lat), 2)));

			// Compute the meridian radius of curvature at the given latitude
			double M = (a * (1.0 - e2)) / Math.Pow(W, 3);

			// Compute the the prime vertical radius of curvature at the given latitude
			double N = a / W;

			// Compute the differences on North and East
			double deltaN = deltaLat * (M + height);
			double deltaE = deltaLon * (N + height) * Math.Cos(lat);

			return new double[] { deltaN, deltaE };
		}


		public static Coordinates<W> globalGeodInstance(double lat, double lon, double alt)
		{
			Coordinates<W> c = new Coordinates<W>();
			//c.ecef = new SimpleMatrix<W>(3, 1);
			c.setGeod(lat, lon, alt);
			c.computeECEF();

			if (!c.isValidXYZ())
				throw new SystemException("Invalid ECEF: " + c);
			return c;
		}

		public SimpleMatrix<W> minusXYZ(Coordinates<W> coord)
		{
			return this.ecef.minus(coord.ecef);
		}
		/**
		 *
		 */
		public void computeGeodetic()
		{
			double X = this.ecef.get(0);
			double Y = this.ecef.get(1);
			double Z = this.ecef.get(2);

			//this.geod = new SimpleMatrix<W>(3, 1);

			double a = Constants.WGS84_SEMI_MAJOR_AXIS;
			double e = Constants.WGS84_ECCENTRICITY;

			// Radius computation
			double r = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));

			// Geocentric longitude
			double lamGeoc = Math.Atan2(Y, X);

			// Geocentric latitude
			double phiGeoc = Math.Atan(Z / Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2)));

			// Computation of geodetic coordinates
			double psi = Math.Atan(Math.Tan(phiGeoc) / Math.Sqrt(1 - Math.Pow(e, 2)));
			double phiGeod = Math.Atan((r * Math.Sin(phiGeoc) + Math.Pow(e, 2) * a
					/ Math.Sqrt(1 - Math.Pow(e, 2)) * Math.Pow(Math.Sin(psi), 3))
					/ (r * Math.Cos(phiGeoc) - Math.Pow(e, 2) * a * Math.Pow(Math.Cos(psi), 3)));
			double lamGeod = lamGeoc;
			double N = a / Math.Sqrt(1 - Math.Pow(e, 2) * Math.Pow(Math.Sin(phiGeod), 2));
			double h = r * Math.Cos(phiGeoc) / Math.Cos(phiGeod) - N;

			//this.geod.set(0, 0, Math.toDegrees(lamGeod));
			//this.geod.set(1, 0, Math.toDegrees(phiGeod));
			this.geod.set(0, 0, lamGeod / Math.PI * 180.0);
			this.geod.set(2, 0, phiGeod / Math.PI * 180.0);
		}

		/*
		 function [X,Y,Z] = frgeod( a, finv, dphi, dlambda, h )
			 %FRGEOD  Subroutine to calculate Cartesian coordinates X,Y,Z
			 %       given geodetic coordinates latitude, longitude (east),
			 %       and height above reference ellipsoid along with
			 %       reference ellipsoid values semi-major axis (a) and
			 %       the inverse of flattening (finv)

			 % The units of linear parameters h,a must agree (m,km,mi,..etc).
			 % The input units of angular quantities must be in decimal degrees.
			 % The output units of X,Y,Z will be the same as the units of h and a.
			 % Copyright (C) 1987 C. Goad, Columbus, Ohio
			 % Reprinted with permission of author, 1996
			 % Original Fortran code rewritten into MATLAB
			 % Kai Borre 03-03-96
		 */
		public void computeECEF()
		{
			long a = 6378137;
			double finv = 298.257223563d;

			double dphi = this.geod.get(1);
			double dlambda = this.geod.get(0);
			double h = this.geod.get(2);

			// compute degree-to-radian factor
			double dtr = Math.PI / 180;

			// compute square of eccentricity
			double esq = (2 - 1 / finv) / finv;
			double sinphi = Math.Sin(dphi * dtr);
			// compute radius of curvature in prime vertical
			double N_phi = a / Math.Sqrt(1 - esq * sinphi * sinphi);

			// compute P and Z
			// P is distance from Z axis
			double P = (N_phi + h) * Math.Cos(dphi * dtr);
			double Z = (N_phi * (1 - esq) + h) * sinphi;
			double X = P * Math.Cos(dlambda * dtr);
			double Y = P * Math.Sin(dlambda * dtr);

			this.ecef.set(0, 0, X);
			this.ecef.set(1, 0, Y);
			this.ecef.set(2, 0, Z);
		}

		/**
		 * @param target
		 * @return Local (ENU) coordinates
		 */
		public void computeLocal(Coordinates<W> target)
		{
			if (this.geod == null) computeGeodetic();

			SimpleMatrix<W> R = rotationMatrix(this);

			enu = R.mult(target.minusXYZ(this));

		}
		public void computeLocalV2(Coordinates<W> target)
		{
			if (this.geod == null) computeGeodetic();

			SimpleMatrix<W> R = rotationMatrix(this);

			enu = R.mult(target.minusXYZ(this));

		}


		public double getGeodeticLongitude()
		{
			if (this.geod == null) computeGeodetic();
			return this.geod.get(0);
		}
		public double getGeodeticLatitude()
		{
			if (this.geod == null) computeGeodetic();
			return this.geod.get(1);
		}
		public double getGeodeticHeight()
		{
			if (this.geod == null) computeGeodetic();
			return this.geod.get(2);
		}
		public double getX()
		{
			return ecef.get(0);
		}
		public double getY()
		{
			return ecef.get(1);
		}
		public double getZ()
		{
			return ecef.get(2);
		}

		public void setENU(double e, double n, double u)
		{
			this.enu.set(0, 0, e);
			this.enu.set(1, 0, n);
			this.enu.set(2, 0, u);
		}
		public double getE()
		{
			return enu.get(0);
		}
		public double getN()
		{
			return enu.get(1);
		}
		public double getU()
		{
			return enu.get(2);
		}


		public void setXYZ(double x, double y, double z)
		{
			//if(this.ecef==null) this.ecef = new SimpleMatrix<W>(3, 1);
			this.ecef.set(0, 0, x);
			this.ecef.set(1, 0, y);
			this.ecef.set(2, 0, z);
		}
		public void setGeod(double lat, double lon, double alt)
		{
			//if(this.ecef==null) this.ecef = new SimpleMatrix<W>(3, 1);
			this.geod.set(1, 0, lat);
			this.geod.set(0, 0, lon);
			this.geod.set(2, 0, alt);
		}
		//public void setPlusXYZ(SimpleMatrix<W> sm)
		//{
		//	this.ecef.set(ecef.plus(sm));
		//}
		public void setSMMultXYZ(SimpleMatrix<W> sm)
		{
			this.ecef = sm.mult(this.ecef);
		}

		public bool isValidXYZ()
		{
			var s1 = this.ecef.elementSum();
			var ec1 = ecef.get(0);
			var ec2 = ecef.get(1);
			var ec3 = ecef.get(2);
			var a1 = double.IsNaN(this.ecef.get(0));
			var a2 = double.IsNaN(this.ecef.get(1));
			var a3 = double.IsNaN(this.ecef.get(2));
			var b1 = double.IsInfinity(this.ecef.get(0));
			var b2 = double.IsInfinity(this.ecef.get(1));
			var b3 = double.IsInfinity(this.ecef.get(2));

			return (this.ecef != null && s1 != 0
			&& !a1 && !a2 && !a3
			&& !b1 && !b2 && !b3
			&& (ec1 != 0 && ec2 != 0 && ec3 != 0)
				);
		}

		public Object clone()
		{
			Coordinates<W> c = new Coordinates<W>();
			cloneInto(c);
			return c;
		}

		public void cloneInto(Coordinates<W> c)
		{
			c.ecef = this.ecef.copy();
			c.enu = this.enu.copy();
			c.geod = this.geod.copy();

			if (refTime != null) c.refTime = (Time)refTime.Clone();
		}
		/**
		 * @param origin
		 * @return Rotation matrix used to switch from global to local reference systems (and vice-versa)
		 */
		public static SimpleMatrix<W> rotationMatrix(Coordinates<W> origin)
		{

			double lam = origin.getGeodeticLongitude()/180.0*Math.PI;
			double phi = origin.getGeodeticLatitude() / 180.0 * Math.PI;

			double cosLam = Math.Cos(lam);
			double cosPhi = Math.Cos(phi);
			double sinLam = Math.Sin(lam);
			double sinPhi = Math.Sin(phi);

			double[][] data = new double[3][];
			for(int jj=0; jj<3; jj++)
            {
				data[jj] = new double[3];
			}
			data[0][0] = -sinLam;
			data[0][1] = cosLam;
			data[0][2] = 0;
			data[1][0] = -sinPhi * cosLam;
			data[1][1] = -sinPhi * sinLam;
			data[1][2] = cosPhi;
			data[2][0] = cosPhi * cosLam;
			data[2][1] = cosPhi * sinLam;
			data[2][2] = sinPhi;

			SimpleMatrix<W> R = new SimpleMatrix<W>(data);

			return R;
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
		public void read(DataInputStream dai, bool oldVersion)
		{
			int v = dai.ReadInt();

			if (v == 1)
			{
				long l = dai.ReadLong();
				refTime = l == -1 ? null : new Time(l);
				for (int i = 0; i < 3; i++)
				{
					ecef.set(i, dai.ReadDouble());
				}
				for (int i = 0; i < 3; i++)
				{
					enu.set(i, dai.ReadDouble());
				}
				for (int i = 0; i < 3; i++)
				{
					geod.set(i, dai.ReadDouble());
				}
			}
			else
			{
				throw new IOException("Unknown format version:" + v);
			}
		}

		public int write(DataOutputStream dos)
		{
			int size = 0;
			//dos.writeUTF(MESSAGE_COORDINATES); size += 5;// 5
			//dos.writeInt(STREAM_V); size += 4; // 4

			//dos.writeLong(refTime == null ? -1 : refTime.getMsec()); size += 8; // 8

			//for (int i = 0; i < 3; i++)
			//{
			//	dos.writeDouble(ecef.get(i)); size += 8;
			//}
			//for (int i = 0; i < 3; i++)
			//{
			//	dos.writeDouble(enu.get(i)); size += 8;
			//}
			//for (int i = 0; i < 3; i++)
			//{
			//	dos.writeDouble(geod.get(i)); size += 8;
			//}

			return size;
		}

		public String toString()
		{
			//String lineBreak = System.getProperty("line.separator");

			//String out= String.format("Coord ECEF: X:" + getX() + " Y:" + getY() + " Z:" + getZ() + lineBreak +
			//"       ENU: E:" + getE() + " N:" + getN() + " U:" + getU() + lineBreak +
			//"      GEOD: Lon:" + getGeodeticLongitude() + " Lat:" + getGeodeticLatitude() + " H:" + getGeodeticHeight() + lineBreak +
			//"      http://maps.google.com?q=%3.4f,%3.4f" + lineBreak, getGeodeticLatitude(), getGeodeticLongitude());
			//return out;
			return "";
		}
	}
}