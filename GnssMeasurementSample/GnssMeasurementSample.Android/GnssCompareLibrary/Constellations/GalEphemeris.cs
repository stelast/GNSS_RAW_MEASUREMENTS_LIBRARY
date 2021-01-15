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
    public class GalEphemeris : KeplerianEphemeris
    {
        public readonly double sisaM;
        public readonly double tgdS;
        public readonly bool isINav;

        private GalEphemeris(Builder builder) : base(builder)
        {
            // super(builder);
            this.sisaM = builder.sisaM;
            this.tgdS = builder.tgdS;
            this.isINav = builder.isINav;
        }

        public static Builder newBuilder()
        {
            return new Builder();
        }

        public class Builder : KeplerianEphemeris.Builder
        {
            // For documentation, see corresponding fields in {@link GpsEphemeris}.
            public double sisaM;
            public double tgdS;
            public bool isINav;

            public Builder() { }

         
            public GalEphemeris.Builder setSisaM(double sisaM)
            {
                this.sisaM = sisaM;
                return this;
            }

            public GalEphemeris.Builder setTgdS(double tgdS)
            {
                this.tgdS = tgdS;
                return this;
            }

            public GalEphemeris.Builder setIsINav(bool isINav)
            {
                this.isINav = isINav;
                return this;
            }

            public GalEphemeris build()
            {
                return new GalEphemeris(this);
            }

            public override GnssEphemeris.Builder getThis()
            {
                return this;
            }
        }
    }
}