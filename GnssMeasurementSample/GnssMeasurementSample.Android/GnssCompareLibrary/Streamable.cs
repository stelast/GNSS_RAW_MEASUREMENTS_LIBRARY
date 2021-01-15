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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary
{
    public interface Streamable
	{
		public readonly static String MESSAGE_OBSERVATIONS = "obs";
		public readonly static String MESSAGE_IONO = "ion";
		public readonly static String MESSAGE_EPHEMERIS = "eph";
		public readonly static String MESSAGE_OBSERVATIONS_SET = "eps";
		public readonly static String MESSAGE_COORDINATES = "coo";

		public int write(DataOutputStream dos) /*throws IOException*/;
		public void read(DataInputStream dai, bool oldVersion) /*throws IOException*/;
	}
}