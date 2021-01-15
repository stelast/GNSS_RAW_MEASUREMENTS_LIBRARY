using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Corrections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex
{
    public interface NavigationProducer : StreamResource
    {
        public SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError);
        public IonoGps getIono(long unixTime, Location initialLocation);
        public IonoGalileo getIonoNeQuick(long unixTime, Location initialLocation);
    }
}