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
    public class SampleRinex : NavigationProducer
    {
        public SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
        {
            throw new NotImplementedException();
        }

        public IonoGps getIono(long unixTime, Location initialLocation)
        {
            throw new NotImplementedException();
        }

        public IonoGalileo getIonoNeQuick(long unixTime, Location initialLocation)
        {
            throw new NotImplementedException();
        }

        public void init()
        {
            throw new NotImplementedException();
        }

        public void release(bool waitForThread, long timeoutMs)
        {
            throw new NotImplementedException();
        }
    }
}