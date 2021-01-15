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
    public abstract class KeplerianEphemeris : GnssEphemeris
    {
        public readonly int week;
        public readonly double tocS;
        public readonly int health;
        public readonly double ttxS;
        public readonly int iode;
        public readonly KeplerianModel keplerModel;
        public readonly double af0S;
        public readonly double af1SecPerSec;
        public readonly double af2SecPerSec2;

        protected KeplerianEphemeris(Builder builder) : base(builder)
        {
            this.week = builder.week;
            this.tocS = builder.tocS;
            this.ttxS = builder.ttxS;
            this.af0S = builder.af0S;
            this.iode = builder.iode;
            this.af1SecPerSec = builder.af1SecPerSec;
            this.af2SecPerSec2 = builder.af2SecPerSec2;
            this.keplerModel = builder.keplerModel;
            this.health = builder.health;
        }

        public abstract class Builder : GnssEphemeris.Builder
        {
            public int week;
            public double tocS;
            public int health;
            public double ttxS;
            public double af0S;
            public int iode;
            public double af1SecPerSec;
            public double af2SecPerSec2;
            public KeplerianModel keplerModel;

            protected Builder()
            {
            }

            public override abstract GnssEphemeris.Builder getThis();

            public GnssEphemeris.Builder setTocS(double tocS)
            {
                this.tocS = tocS;
                return this.getThis();
            }

            public GnssEphemeris.Builder setHealth(int health)
            {
                this.health = health;
                return this.getThis();
            }

            public GnssEphemeris.Builder setIode(int iode)
            {
                this.iode = iode;
                return this.getThis();
            }

            public GnssEphemeris.Builder setAf0S(double af0S)
            {
                this.af0S = af0S;
                return this.getThis();
            }

            public GnssEphemeris.Builder setAf1SecPerSec(double af1SecPerSec)
            {
                this.af1SecPerSec = af1SecPerSec;
                return this.getThis();
            }

            public GnssEphemeris.Builder setAf2SecPerSec2(double af2SecPerSec2)
            {
                this.af2SecPerSec2 = af2SecPerSec2;
                return this.getThis();
            }

            public GnssEphemeris.Builder setWeek(int week)
            {
                this.week = week;
                return this.getThis();
            }

            public GnssEphemeris.Builder setTtxS(double ttxS)
            {
                this.ttxS = ttxS;
                return this.getThis();
            }

            public GnssEphemeris.Builder setKeplerianModel(KeplerianModel keplerModel)
            {
                this.keplerModel = keplerModel;
                return this.getThis();
            }
        }
    }
}