using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations
{
    public class Time
    {
		private long msec; /* time in milliseconds since January 1, 1970 (UNIX standard) */
		private double fraction; /* fraction of millisecond */

		private Java.Util.Date[] leapDates;
		private Calendar gc = GregorianCalendar.Instance;
		Android.Icu.Util.TimeZone zone = Android.Icu.Util.TimeZone.GetTimeZone("GMT Time");
		DateFormat df = new SimpleDateFormat("yyyy MM dd HH mm ss.SSS");
		DateFormat logTime = new SimpleDateFormat("HHmmss.SSS");

		void initleapDates()
		{
			leapDates = new Date[19];
			leapDates[0]  = df.Parse("1980 01 06 00 00 00.0");
			leapDates[1]  = df.Parse("1981 07 01 00 00 00.0");
			leapDates[2]  = df.Parse("1982 07 01 00 00 00.0");
			leapDates[3]  = df.Parse("1983 07 01 00 00 00.0");
			leapDates[4]  = df.Parse("1985 07 01 00 00 00.0");
			leapDates[5]  = df.Parse("1988 01 01 00 00 00.0");
			leapDates[6]  = df.Parse("1990 01 01 00 00 00.0");
			leapDates[7]  = df.Parse("1991 01 01 00 00 00.0");
			leapDates[8]  = df.Parse("1992 07 01 00 00 00.0");
			leapDates[9]  = df.Parse("1993 07 01 00 00 00.0");
			leapDates[10] = df.Parse("1994 07 01 00 00 00.0");
			leapDates[11] = df.Parse("1996 01 01 00 00 00.0");
			leapDates[12] = df.Parse("1997 07 01 00 00 00.0");
			leapDates[13] = df.Parse("1999 01 01 00 00 00.0");
			leapDates[14] = df.Parse("2006 01 01 00 00 00.0");
			leapDates[15] = df.Parse("2009 01 01 00 00 00.0");
			leapDates[16] = df.Parse("2012 07 01 00 00 00.0");
			leapDates[17] = df.Parse("2015 07 01 00 00 00.0");
			leapDates[18] = df.Parse("2017 01 01 00 00 00.0");
		}

		public Time(long msec)
		{
			df.TimeZone = zone;
			gc.TimeZone = zone;
			this.gc.TimeInMillis = msec;
			this.msec = msec;
			this.fraction = 0;
		}
		public Time(long msec, double fraction)
		{
			df.TimeZone = zone;
			gc.TimeZone = zone;
			this.msec = msec;
			this.gc.TimeInMillis = msec;
			this.fraction = fraction;
		}
		public Time(string dateStr)
		{
			df.TimeZone = zone;
			gc.TimeZone = zone;
			this.msec = datestringToTime(dateStr);
			this.gc.TimeInMillis = this.msec;
			this.fraction = 0;
		}
		public Time(int gpsWeek, double weekSec)
		{
			df.TimeZone = zone;
			gc.TimeZone = zone;
			double fullTime = (Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY + gpsWeek * Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY + weekSec) * 1000L;
			this.msec = (long)(fullTime);
			this.fraction = fullTime - this.msec;
			this.gc.TimeInMillis = this.msec;
		}

		public Time(int week, double weekSec, char satID)
		{
			if (satID == 'E')
			{
				df.TimeZone = zone;
				gc.TimeZone = zone;
				double fullTime = (Constants.UNIX_GST_DAYS_DIFF * Constants.SEC_IN_DAY + week * Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY + weekSec) * 1000L;
				this.msec = (long)(fullTime);
				this.fraction = fullTime - this.msec;
				this.gc.TimeInMillis = this.msec;
			}
			else
				{
				df.TimeZone = zone;
				gc.TimeZone = zone;
				double fullTime = (Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY + week * Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY + weekSec) * 1000L;
				this.msec = (long)(fullTime);
				this.fraction = fullTime - this.msec;
				this.gc.TimeInMillis = this.msec;
			}
		}

		/**
		 * @param dateStr
		 * @return
		 * @throws ParseException
		 */
		private long datestringToTime(string dateStr) 
		{

			long dateTime = 0;

			try {
				Java.Util.Date dateObj = df.Parse(dateStr);
				dateTime = dateObj.Time;
			} catch (Exception e) {
				throw e;
			}

			return dateTime;
		}

		//	/**
		//	 * @param time
		//	 *            (GPS time in seconds)
		//	 * @return UNIX standard time in milliseconds
		//	 */
		//	private static long gpsToUnixTime(double time, int week) {
		//		// Shift from GPS time (January 6, 1980 - sec)
		//		// to UNIX time (January 1, 1970 - msec)
		//		time = (time + (week * Constants.DAYS_IN_WEEK + Constants.UNIX_GPS_DAYS_DIFF) * Constants.SEC_IN_DAY) * Constants.MILLISEC_IN_SEC;
		//
		//		return (long)time;
		//	}

		/**
		 * @param time
		 *            (GPS time in seconds)
		 * @return UNIX standard time in milliseconds
		 */
		public static long gpsToUnixTime(double time)
		{
			// Shift from GPS time (January 6, 1980 - sec)
			// to UNIX time (January 1, 1970 - msec)
			time = (time + Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY) * Constants.MILLISEC_IN_SEC;


			return (long)time;
		}

		/**
		 * @param time
		 *            (UNIX standard time in milliseconds)
		 * @return GPS time in seconds
		 */
		private static double unixToGpsTime(double time)
		{
			// Shift from UNIX time (January 1, 1970 - msec)
			// to GPS time (January 6, 1980 - sec)
			time = time / Constants.MILLISEC_IN_SEC - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
			time = time % (Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY);
			return time;
		}

		public int getGpsWeek()
		{
			// Shift from UNIX time (January 1, 1970 - msec)
			// to GPS time (January 6, 1980 - sec)
			long time = msec / Constants.MILLISEC_IN_SEC - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
			return (int)(time / (Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY));
		}
		public int getGpsWeekSec()
		{
			// Shift from UNIX time (January 1, 1970 - msec)
			// to GPS time (January 6, 1980 - sec)
			long time = msec / Constants.MILLISEC_IN_SEC - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
			return (int)(time % (Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY));
		}
		public int getGpsWeekDay()
		{
			return (int)(getGpsWeekSec() / Constants.SEC_IN_DAY);
		}
		public int getGpsHourInDay()
		{
			long time = msec / Constants.MILLISEC_IN_SEC - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
			return (int)((time % (Constants.SEC_IN_DAY)) / Constants.SEC_IN_HOUR);
		}
		public int getYear()
		{
			return gc.Get(CalendarField.Year);
		}
		public int getYear2c()
		{
			return gc.Get(CalendarField.Year) - 2000;
		}
		public int getDayOfYear()
		{
			return gc.Get(CalendarField.DayOfYear);
		}
		public string getHourOfDayLetter()
		{
			char c = (char)('a' + getGpsHourInDay());
			return "" + c;
		}

		/*
		 * Locating IGS data, products, and format definitions	Key to directory and file name variables
		 * d	day of week (0-6)
		 * ssss	4-character IGS site ID or 4-character LEO ID
		 * yyyy	4-digit year
		 * yy	2-digit year
		 * wwww	4-digit GPS week
		 * ww	2-digit week of year(01-53)
		 * ddd	day of year (1-366)
		 * hh	2-digit hour of day (00-23)
		 * h	single letter for hour of day (a-x = 0-23)
		 * mm	minutes within hour
		 *
		 */
		public string formatTemplate(string template)
		{
			//Replace por replaceall
			string tmpl = template.Replace("\\$\\{wwww\\}", (new DecimalFormat("0000")).Format(this.getGpsWeek()));
			tmpl = tmpl.Replace("\\$\\{d\\}", (new DecimalFormat("0")).Format(this.getGpsWeekDay()));
			tmpl = tmpl.Replace("\\$\\{ddd\\}", (new DecimalFormat("000")).Format(this.getDayOfYear()));
			tmpl = tmpl.Replace("\\$\\{yy\\}", (new DecimalFormat("00")).Format(this.getYear2c()));
			tmpl = tmpl.Replace("\\$\\{yyyy\\}", (new DecimalFormat("0000")).Format(this.getYear()));
			int hh4 = this.getGpsHourInDay();
			tmpl = tmpl.Replace("\\$\\{hh\\}", (new DecimalFormat("00")).Format(hh4));
			if (0 <= hh4 && hh4 < 6) hh4 = 0;
			if (6 <= hh4 && hh4 < 12) hh4 = 6;
			if (12 <= hh4 && hh4 < 18) hh4 = 12;
			if (18 <= hh4 && hh4 < 24) hh4 = 18;
			tmpl = tmpl.Replace("\\$\\{hh4\\}", (new DecimalFormat("00")).Format(hh4));
			tmpl = tmpl.Replace("\\$\\{h\\}", this.getHourOfDayLetter());
			return tmpl;
		}

		public double getGpsTime()
		{
			return unixToGpsTime(msec);
		}

		public double getRoundedGpsTime()
		{
			double tow = unixToGpsTime((msec + 499) / 1000 * 1000);
			return tow;
		}

		public int getLeapSeconds()
		{
			if (leapDates == null)
				try
				{
					initleapDates();
				}
				catch (Exception e)
				{
					// TODO Auto-generated catch block
					//e.Message;
				}

			int leapSeconds = leapDates.Count() - 1;
			double delta;
			for (int d = 0; d < leapDates.Count(); d++)
			{
				delta = leapDates[d].Time - msec;
				if (delta > 0)
				{
					leapSeconds = d - 1;
					break;
				}
			}
			return leapSeconds;
		}

		//
		//	private static double unixToGpsTime(double time) {
		//		// Shift from UNIX time (January 1, 1970 - msec)
		//		// to GPS time (January 6, 1980 - sec)
		//		time = (long)(time / Constants.MILLISEC_IN_SEC) - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
		//
		//		// Remove integer weeks, to get Time Of Week
		//		double dividend  = time;
		//		double divisor = Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY;
		//		time = dividend  - (divisor * round(dividend / divisor));
		//
		//		//time = Math.IEEEremainder(time, Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY);
		//
		//		return time;
		//	}



		/**
		 * @return the msec
		 */
		public long getMsec()
		{
			return msec;
		}

		/**
		 * @param msec the msec to set
		 */
		public void setMsec(long msec)
		{
			this.msec = msec;
		}

		/**
		 * @return the fraction
		 */
		public double getFraction()
		{
			return fraction;
		}

		/**
		 * @param fraction the fraction to set
		 */
		public void setFraction(double fraction)
		{
			this.fraction = fraction;
		}

		public object clone()
		{
			return new Time(this.msec, this.fraction);
		}

		public string tostring()
		{
			return df.Format(gc.Time) + " " + gc.Time;
		}

		public string toLogstring()
		{
			return logTime.Format(gc.Time);
		}

		public int getMonth()
		{

			return df.Calendar.Get(CalendarField.Month);


		}

		public int getHourUTC()
		{

			return df.Calendar.Get(CalendarField.HourOfDay);


		}
	}
}