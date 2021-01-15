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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations
{
    public abstract class GnssEphemeris
    {
        public readonly int svid;

        protected GnssEphemeris(Builder builder)
        {
            this.svid = builder.svid;
        }

        public abstract class Builder
        {
            public int svid;

            public Builder()
            {
            }

            public abstract Builder getThis();

            public Builder setSvid(int svid)
            {
                this.svid = svid;
                return this.getThis();
            }
        }
    }
}