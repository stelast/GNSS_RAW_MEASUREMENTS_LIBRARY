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
    public class EphemerisResponse
    {
        public readonly List<GnssEphemeris> ephList;
        //public readonly IonosphericModelProto ionoProto;
        //public readonly IonosphericModelProto ionoProto2;

        //public EphemerisResponse(List<GnssEphemeris> ephList, IonosphericModelProto ionoProto, IonosphericModelProto ionoProto2)
        //{
        //    this.ephList = ephList;
        //    this.ionoProto = ionoProto;
        //    this.ionoProto2 = ionoProto2;
        //}
    }
}