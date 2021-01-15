using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Corrections;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex
{
    public class RinexNavigationGalileo : NavigationProducer
    {
        public readonly static string GARNER_NAVIGATION_AUTO = "ftp://garner.ucsd.edu/pub/nav/${yyyy}/${ddd}/auto${ddd}0.${yy}n.Z";
        public readonly static string IGN_MULTI_NAVIGATION_DAILY = "ftp://igs.ign.fr/pub/igs/data/campaign/mgex/daily/rinex3/${yyyy}/${ddd}/brdm${ddd}0.${yy}p.Z";
        public readonly static string GARNER_NAVIGATION_ZIM2 = "ftp://garner.ucsd.edu/pub/nav/${yyyy}/${ddd}/zim2${ddd}0.${yy}n.Z";
        public readonly static string IGN_NAVIGATION_HOURLY_ZIM2 = "ftp://igs.ensg.ign.fr/pub/igs/data/hourly/${yyyy}/${ddd}/zim2${ddd}${h}.${yy}n.Z";
        public readonly static string NASA_NAVIGATION_DAILY = "ftp://cddis.gsfc.nasa.gov/pub/gps/data/daily/${yyyy}/${ddd}/${yy}n/brdc${ddd}0.${yy}n.Z";
        public readonly static string NASA_NAVIGATION_HOURLY = "ftp://cddis.gsfc.nasa.gov/pub/gps/data/hourly/${yyyy}/${ddd}/hour${ddd}0.${yy}n.Z";
        public readonly static string GARNER_NAVIGATION_AUTO_HTTP = "http://garner.ucsd.edu/pub/rinex/${yyyy}/${ddd}/auto${ddd}0.${yy}n.Z"; // ex http://garner.ucsd.edu/pub/rinex/2016/034/auto0340.16n.Z
        public readonly static string BKG_GALILEO_RINEX = "ftp://igs.bkg.bund.de/EUREF/BRDC/${yyyy}/${ddd}/BRDC00WRD_R_${yyyy}${ddd}0000_01D_EN.rnx.gz";
        public readonly static string ESA_GALILEO_RINEX = "ftp://gssc.esa.int/gnss/data/daily/${yyyy}/${ddd}/ankr${ddd}0.${yy}l.Z";

        private readonly static string TAG = "RinexNavigationGalileo";


        /**
         * cache for negative answers
         */
        private Dictionary<string, Date> negativeChache = new Dictionary<string, Date>();

        /** Folder containing downloaded files */
        public string RNP_CACHE = "./rnp-cache";

        private bool waitForData = true;

        public SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
        {
            RinexNavigationParserGalileo rnp = getRNPByTimestamp(unixTime, initialLocation);
            if (rnp != null)
            {
                if (rnp.isTimestampInEpocsRange(unixTime))
                {
                    return rnp.getGalileoSatPosition(unixTime, range, satID, satType, receiverClockError);
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        //public IonoGps getIono(long unixTime, Location initialLocation)
        //{
        //    RinexNavigationParserGalileo rnp = getRNPByTimestamp(unixTime, initialLocation);
        //    if (rnp != null) return rnp.getIono(unixTime, initialLocation);
        //    return null;
        //}

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

        public IonoGps getIono(long unixTime, Location initialLocation)
        {
            throw new NotImplementedException();
        }
    }
}