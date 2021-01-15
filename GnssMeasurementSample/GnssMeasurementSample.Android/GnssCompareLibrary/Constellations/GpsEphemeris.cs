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
    public class GpsEphemeris : KeplerianEphemeris
    {
        public readonly double accuracyM;
        public readonly double tgdS;
        public readonly bool fitIntvFlag;
        public readonly bool l2PDataFlag;
        public readonly int codeL2;
        public readonly int iodc;

        private GpsEphemeris(Builder builder) : base(builder)
        {
            accuracyM = builder.accuracyM;
            tgdS = builder.tgdS;
            fitIntvFlag = builder.fitIntvFlag;
            l2PDataFlag = builder.l2PDataFlag;
            codeL2 = builder.codeL2;
            iodc = builder.iodc;
        }

        public static Builder newBuilder()
        {
            return new Builder();
        }

        public class Builder : KeplerianEphemeris.Builder
        {
            // For documentation, see corresponding fields in {@link GpsEphemeris}.
            public double accuracyM;
            public double tgdS;
            public bool fitIntvFlag;
            public bool l2PDataFlag;
            public int codeL2;
            public int iodc;

            public Builder() { }

            public override GnssEphemeris.Builder getThis()
            {
                return this;
            }


            /** Sets the satellite user range accuracy (meters). */
            public GnssEphemeris.Builder setAccuracyM(double accuracyM)
            {
                this.accuracyM = accuracyM;
                return getThis();
            }

            /** Sets the group delay term (seconds). */
            public GnssEphemeris.Builder setTgdS(double tgdS)
            {
                this.tgdS = tgdS;
                return getThis();
            }

            /** Sets the flag of the fit interval of the ephemeris: 0=4 hours, 1=more than 4 hours. */
            public GnssEphemeris.Builder setFitIntvFlag(bool fitIntvFlag)
            {
                this.fitIntvFlag = fitIntvFlag;
                return getThis();
            }

            /** Sets the flag of L2 P data. */
            public GnssEphemeris.Builder setL2PDataFlag(bool l2PDataFlag)
            {
                this.l2PDataFlag = l2PDataFlag;
                return getThis();
            }

            /** Sets the flag of codes on L2 channel. */
            public GnssEphemeris.Builder setCodeL2(int codeL2)
            {
                this.codeL2 = codeL2;
                return getThis();
            }

            /** Sets the issue of data, clock. */
            public GnssEphemeris.Builder setIodc(int iodc)
            {
                this.iodc = iodc;
                return getThis();
            }

            /** Builds a {@link GpsEphemeris} object as specified by this builder. */
            public GpsEphemeris build()
            {
                return new GpsEphemeris(this);
            }

        }
    }
}