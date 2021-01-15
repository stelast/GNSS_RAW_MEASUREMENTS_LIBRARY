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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex
{
    public abstract class SuplConnectionRequest
    {
        public SuplConnectionRequest()
        {
        }

        public abstract string getServerHost();

        public abstract int getServerPort();

        public abstract bool isSslEnabled();

        public abstract bool isLoggingEnabled();

        public abstract bool isMessageLoggingEnabled();

        //public static SuplConnectionRequest.Builder builder()
        //{
        //    return (new Builder()).setMessageLoggingEnabled(false).setLoggingEnabled(false).setSslEnabled(false);
        //}

        public abstract class Builder
        {
            public Builder()
            {
            }

            public abstract SuplConnectionRequest.Builder setServerHost(string var1);

            public abstract SuplConnectionRequest.Builder setServerPort(int var1);

            public abstract SuplConnectionRequest.Builder setSslEnabled(bool var1);

            public abstract SuplConnectionRequest.Builder setLoggingEnabled(bool var1);

            public abstract SuplConnectionRequest.Builder setMessageLoggingEnabled(bool var1);

            public abstract SuplConnectionRequest build();
        }
    }
}