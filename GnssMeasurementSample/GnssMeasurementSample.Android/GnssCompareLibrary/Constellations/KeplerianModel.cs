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
    public class KeplerianModel
    {
        public readonly double toeS;
        public readonly double deltaN;
        public readonly double m0;
        public readonly double eccentricity;
        public readonly double sqrtA;
        public readonly double omegaDot;
        public readonly double omega;
        public readonly double omega0;
        public readonly double iDot;
        public readonly double i0;
        public readonly double cic;
        public readonly double cis;
        public readonly double crc;
        public readonly double crs;
        public readonly double cuc;
        public readonly double cus;

        public KeplerianModel(KeplerianModel.Builder builder)
        {
            this.toeS = builder.toeS;
            this.deltaN = builder.deltaN;
            this.m0 = builder.m0;
            this.eccentricity = builder.eccentricity;
            this.sqrtA = builder.sqrtA;
            this.omegaDot = builder.omegaDot;
            this.omega = builder.omega;
            this.iDot = builder.iDot;
            this.i0 = builder.i0;
            this.omega0 = builder.omega0;
            this.cic = builder.cic;
            this.cis = builder.cis;
            this.crc = builder.crc;
            this.crs = builder.crs;
            this.cuc = builder.cuc;
            this.cus = builder.cus;
        }

        public static KeplerianModel.Builder newBuilder()
        {
            return new KeplerianModel.Builder();
        }

        public class Builder
        {
            public double toeS;
            public double deltaN;
            public double m0;
            public double eccentricity;
            public double sqrtA;
            public double omegaDot;
            public double omega;
            public double iDot;
            public double i0;
            public double omega0;
            public double cic;
            public double cis;
            public double crc;
            public double crs;
            public double cuc;
            public double cus;

            public Builder()
            {
            }

            public KeplerianModel.Builder setToeS(double toeS)
            {
                this.toeS = toeS;
                return this;
            }

            public KeplerianModel.Builder setDeltaN(double deltaN)
            {
                this.deltaN = deltaN;
                return this;
            }

            public KeplerianModel.Builder setM0(double m0)
            {
                this.m0 = m0;
                return this;
            }

            public KeplerianModel.Builder setEccentricity(double eccentricity)
            {
                this.eccentricity = eccentricity;
                return this;
            }

            public KeplerianModel.Builder setSqrtA(double sqrtA)
            {
                this.sqrtA = sqrtA;
                return this;
            }

            public KeplerianModel.Builder setOmegaDot(double omegaDot)
            {
                this.omegaDot = omegaDot;
                return this;
            }

            public KeplerianModel.Builder setOmega(double omega)
            {
                this.omega = omega;
                return this;
            }

            public KeplerianModel.Builder setIDot(double iDot)
            {
                this.iDot = iDot;
                return this;
            }

            public KeplerianModel.Builder setI0(double i0)
            {
                this.i0 = i0;
                return this;
            }

            public KeplerianModel.Builder setOmega0(double omega0)
            {
                this.omega0 = omega0;
                return this;
            }

            public KeplerianModel.Builder setCic(double cic)
            {
                this.cic = cic;
                return this;
            }

            public KeplerianModel.Builder setCis(double cis)
            {
                this.cis = cis;
                return this;
            }

            public KeplerianModel.Builder setCrc(double crc)
            {
                this.crc = crc;
                return this;
            }

            public KeplerianModel.Builder setCrs(double crs)
            {
                this.crs = crs;
                return this;
            }

            public KeplerianModel.Builder setCuc(double cuc)
            {
                this.cuc = cuc;
                return this;
            }

            public KeplerianModel.Builder setCus(double cus)
            {
                this.cus = cus;
                return this;
            }

            public KeplerianModel build()
            {
                return new KeplerianModel(this);
            }
        }
    }
}