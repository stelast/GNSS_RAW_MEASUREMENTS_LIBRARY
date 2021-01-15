using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using Android.Util;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Corrections;
using Java.IO;
using Java.Lang;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex
{
    public class RinexNavigationGps : NavigationProducer
    {

        public readonly static string GARNER_NAVIGATION_AUTO = "ftp://garner.ucsd.edu/pub/nav/${yyyy}/${ddd}/auto${ddd}0.${yy}n.Z";
        public readonly static string IGN_MULTI_NAVIGATION_DAILY = "ftp://igs.ign.fr/pub/igs/data/campaign/mgex/daily/rinex3/${yyyy}/${ddd}/brdm${ddd}0.${yy}p.Z";
        public readonly static string GARNER_NAVIGATION_ZIM2 = "ftp://garner.ucsd.edu/pub/nav/${yyyy}/${ddd}/zim2${ddd}0.${yy}n.Z";
        public readonly static string IGN_NAVIGATION_HOURLY_ZIM2 = "ftp://igs.ensg.ign.fr/pub/igs/data/hourly/${yyyy}/${ddd}/zim2${ddd}${h}.${yy}n.Z";
        public readonly static string NASA_NAVIGATION_DAILY = "ftp://cddis.gsfc.nasa.gov/pub/gps/data/daily/${yyyy}/${ddd}/${yy}n/brdc${ddd}0.${yy}n.Z";
        public readonly static string NASA_NAVIGATION_HOURLY = "ftp://cddis.gsfc.nasa.gov/pub/gps/data/hourly/${yyyy}/${ddd}/hour${ddd}0.${yy}n.Z";
        public readonly static string GARNER_NAVIGATION_AUTO_HTTP = "http://garner.ucsd.edu/pub/rinex/${yyyy}/${ddd}/auto${ddd}0.${yy}n.Z"; // ex http://garner.ucsd.edu/pub/rinex/2016/034/auto0340.16n.Z
        public readonly static string BKG_HOURLY_SUPER_SEVER = "ftp://igs.bkg.bund.de/IGS/BRDC/${yyyy}/${ddd}/brdc${ddd}0.${yy}n.Z";

        private readonly static string TAG = "RinexNavigationGps";

        /**
         * cache for negative answers
         */
        private Dictionary<string, Date> negativeChache = new Dictionary<string, Date>();

        /**
         * Folder containing downloaded files
         */
        public string RNP_CACHE = "./rnp-cache";

        private bool waitForData = true;

        /**
         * Template string where to retrieve files on the net
         */
        private string urltemplate;
        private Dictionary<string, RinexNavigationParserGps> pool = new Dictionary<string, RinexNavigationParserGps>();

        /**
         * Instantiates a new RINEX navigation retriever and parser.
         *
         * @param urltemplate the template URL where to get the files on the net.
         */
        public RinexNavigationGps(string urltemplate)
        {
            this.urltemplate = urltemplate;

        }

        /** Compute the GPS satellite coordinates

        INPUT:
          @param unixTime       = time of measurement reception - UNIX        [milliseconds]
          @param range          = pseudorange measuremnent                          [meters]
          @param satID          = satellite ID
          @param satType        = satellite type indicating the constellation (E: Galileo,
                                G: GPS)
          @param receiverClockError = 0.0
        */
        public SatellitePosition getSatPositionAndVelocities(long unixTime, double range, int satID, char satType, double receiverClockError, Location initialLocation)
        {

            //long unixTime = obs.getRefTime().getMsec();
            //double range = obs.getSatByIDType(satID, satType).getPseudorange(0);

            RinexNavigationParserGps rnp = getRNPByTimestamp(unixTime, initialLocation);

            if (rnp != null)
            {
                if (rnp.isTimestampInEpocsRange(unixTime))
                {
                    return rnp.getSatPositionAndVelocities(unixTime, range, satID, satType, receiverClockError);
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public EphGps findEph(long unixTime, int satID, char satType, Location initialLocation)
        {
            long requestedTime = unixTime;
            EphGps eph = null;
            int maxBack = 12;
            while (eph == null && (maxBack--) > 0)
            {

                RinexNavigationParserGps rnp = getRNPByTimestamp(requestedTime, initialLocation);

                if (rnp != null)
                {
                    if (rnp.isTimestampInEpocsRange(unixTime))
                    {
                        eph = rnp.findEph(unixTime, satID, satType);
                    }
                }
                if (eph == null)
                    requestedTime -= (1L * 3600L * 1000L);
            }

            return eph;
        }

        ///* Convenience method for adding an rnp to memory cache*/
        //public void Put2(long reqTime, RinexNavigationParserGps rnp)
        //{
        //    Time t = new Time(reqTime);
        //    string url = t.formatTemplate(urltemplate);
        //    if (!pool.ContainsKey(url))
        //        pool[url] = rnp;
        //}

        List<string> retrievingFromServer = new List<string>();

        RinexNavigationParserGps getRNPByTimestamp(long unixTime, Location initialLocation)
        {

            RinexNavigationParserGps rnp = null;
            long reqTime = unixTime;

            //        do {
            // found none, retrieve from urltemplate
            Time t = new Time(reqTime);
            //System.out.println("request: "+unixTime+" "+(new Date(t.getMsec()))+" week:"+t.getGpsWeek()+" "+t.getGpsWeekDay());

            //final string url = t.formatTemplate(urltemplate);
            string url = "supl.google.com";

            if (pool.ContainsKey(url))
            {
                lock(this) {
                    rnp = pool[url];
                }
            }
            else
            {
                if (!retrievingFromServer.Contains(url))
                {
                    retrievingFromServer.Add(url);
                    MyRunnable mr = new MyRunnable();
                    mr.url = url;
                    mr.initialLocation = initialLocation;
                    mr.retrievingFromServer = retrievingFromServer;
                    mr.pool = pool;

                    ThreadStart childref = new ThreadStart(mr.DoStuff);
                    System.Threading.Thread childThread = new System.Threading.Thread(childref);
                    childThread.Start();
                        

                    /*
                    (new Thread(new Runnable() {
                        @Override
                        public void run()
                    {
                    }
                })).start();*/
                }
                return null;
             }
                
            return rnp;

//        } while (waitForData && rnp == null);
        }

        public class MyRunnable
        {
            private readonly static string TAG = "RinexNavigationGps";
            public string RNP_CACHE = "./rnp-cache";

            public string url;
            public Location initialLocation;
            public List<string> retrievingFromServer;
            public Dictionary<string, RinexNavigationParserGps> pool;


            public void DoStuff()
            {
                RinexNavigationParserGps rnp;

                try
                {
                    rnp = getFromSUPL(url, initialLocation);
                    lock (this)
                    {
                        if (rnp != null)
                        {
                            pool[url] = rnp;
                        }
                        retrievingFromServer.Remove(url);
                    }
                }
                catch (IOException e)
                {
                    //System.out.println(e.getClass().getName() + " url: " + url);
                }
            }
             
            private RinexNavigationParserGps getFromSUPL(string url, Location initialLocation) /*throws IOException*/
            {
                RinexNavigationParserGps rnp = null;

                string suplName = url;
                int serverPort = 7275;
                bool sslEnabled = true;
                bool messageLoggingEnabled = true;
                bool loggingEnabled = true;
                File rnf = new File(RNP_CACHE, suplName);

                if (rnf.Exists())
                {
                    //System.out.println("Supl from cache file " + rnf);
                    rnp = SuplFileToRnpParserGps(rnf);
                    return rnp;
                }

                //try
                //{

                //Log.Warn(TAG, "getFromSUPL: Getting data using SUPL client...");
                //SuplConnectionRequest request =
                //        SuplConnectionRequest.builder()
                //                .setServerHost(suplName)
                //                .setServerPort(serverPort)
                //                .setSslEnabled(sslEnabled)
                //                .setMessageLoggingEnabled(messageLoggingEnabled)
                //                .setLoggingEnabled(loggingEnabled)
                //                .build();

                //SuplController mSuplController = new SuplController(request);

                //mSuplController.sendSuplRequest((long)(initialLocation.Latitude * 1e7), (long)(initialLocation.Longitude * 1e7));
                //EphemerisResponse ephResponse = mSuplController.generateEphResponse((long)(initialLocation.Latitude * 1e7), (long)(initialLocation.Longitude * 1e7));

                //if (ephResponse != null)
                //{
                //    rnp = new RinexNavigationParserGps(ephResponse);
                //}

                Log.Warn(TAG, "getFromSUPL: Received data from SUPL server");

                //}
                //catch (/*NullPointerException |
                //UnsupportedOperationException |
                //IllegalArgumentException |
                //IndexOutOfBoundsException e*/Exception e) {
                //Log.Error(TAG, "Exception thrown getting msg from SUPL server", e);
                //e.printStackTrace();
                //}
                return rnp;
            }

            private RinexNavigationParserGps SuplFileToRnpParserGps(File rnf)
            {
                RinexNavigationParserGps rnp = null;
                return rnp;
            }
        }; 








        public SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
        {
            return null;
        }

        public IonoGps getIono(long unixTime, Location initialLocation)
        {
            //RinexNavigationParserGps rnp = getRNPByTimestamp(unixTime, initialLocation);
            //if (rnp != null) return rnp.getIono(unixTime, initialLocation);
            return null;
        }

        public IonoGalileo getIonoNeQuick(long unixTime, Location initialLocation)
        {
            return null;
        }

        public void init()
        {
        }

        public void release(bool waitForThread, long timeoutMs)
        {
        }

        //        /**
        //         * @param //args
        //         */
        //        public static void main(string[] args)
        //        {

        //            Java.Util.TimeZone.Default = Java.Util.TimeZone.GetTimeZone("UTC");

        //            Calendar c = Calendar.Instance;

        //            /*
        //            c.set(Calendar.YEAR, 2018);
        //            c.set(Calendar.MONTH, 2);
        //            c.set(Calendar.DAY_OF_MONTH, 15);
        //            c.set(Calendar.HOUR_OF_DAY, 15);
        //            c.set(Calendar.MINUTE, 30);
        //            c.set(Calendar.SECOND, 0);
        //            c.set(Calendar.MILLISECOND, 0);
        //            c.setTimeZone(new SimpleTimeZone(0,""));
        //            */
        //            Time t = new Time(c.TimeInMillis);

        //            System.Console.WriteLine("ts: " + t.getMsec() + " " + (new Date(t.getMsec())));
        //            System.Console.WriteLine("week: " + t.getGpsWeek());
        //            System.Console.WriteLine("week sec: " + t.getGpsWeekSec());
        //            System.Console.WriteLine("week day: " + t.getGpsWeekDay());
        //            System.Console.WriteLine("week hour in day: " + t.getGpsHourInDay());


        //            System.Console.WriteLine("ts2: " + (new Time(t.getGpsWeek(), t.getGpsWeekSec())).getMsec());

        //            //RinexNavigation rn = new RinexNavigation(IGN_NAVIGATION_HOURLY_ZIM2);
        //            //RinexNavigationGps rn = new RinexNavigationGps(NASA_NAVIGATION_HOURLY);
        //            RinexNavigationGps rn = new RinexNavigationGps(BKG_HOURLY_SUPER_SEVER);

        //            rn.init();
        //            //		SatellitePosition sp = rn.getGpsSatPosition(c.getTimeInMillis(), 2, 0, 0);
        //            Observations obs = new Observations(new Time(c.getTimeInMillis()), 0);
        //            SatellitePosition sp = rn.getGpsSatPosition(obs, 2, 'G', 0);

        //            if (sp != null)
        //            {
        //                System.Console.WriteLine("found " + (new Date(sp.getUtcTime())) + " " + (sp.isPredicted() ? " predicted" : ""));
        //            }
        //            else
        //            {
        //                System.Console.WriteLine("Epoch not found " + (new Date(c.getTimeInMillis())));
        //            }


        //        }

        //        /**
        //         * Template string where to retrieve files on the net
        //         */
        //      private Dictionary<string, RinexNavigationParserGps> pool = new Dictionary<string, RinexNavigationParserGps>();

        //        /**
        //         * Instantiates a new RINEX navigation retriever and parser.
        //         *
        //         * @param urltemplate the template URL where to get the files on the net.
        //         */

        /** Compute the GPS satellite coordinates

        INPUT:
//          @param unixTime       = time of measurement reception - UNIX        [milliseconds]
//          @param range          = pseudorange measuremnent                          [meters]
//          @param satID          = satellite ID
//          @param satType        = satellite type indicating the constellation (E: Galileo,
//                                G: GPS)
//          @param receiverClockError = 0.0
//        */


        //        public EphGps findEph(long unixTime, int satID, char satType, Location initialLocation)
        //        {
        //            long requestedTime = unixTime;
        //            EphGps eph = null;
        //            int maxBack = 12;
        //            while (eph == null && (maxBack--) > 0)
        //            {

        //                RinexNavigationParserGps rnp = getRNPByTimestamp(requestedTime, initialLocation);

        //                if (rnp != null)
        //                {
        //                    if (rnp.isTimestampInEpocsRange(unixTime))
        //                    {
        //                        eph = rnp.findEph(unixTime, satID, satType);
        //                    }
        //                }
        //                if (eph == null)
        //                    requestedTime -= (1L * 3600L * 1000L);
        //            }

        //            return eph;
        //        }

        //        /* Convenience method for adding an rnp to memory cache*/
        //        public void put(long reqTime, RinexNavigationParserGps rnp)
        //        {
        //            Time t = new Time(reqTime);
        //            string url = t.formatTemplate(urltemplate);
        //            if (!pool.containsKey(url))
        //                pool.put(url, rnp);
        //        }

        //        ArraySet<string> retrievingFromServer = new ArraySet<>();

        //        protected RinexNavigationParserGps getRNPByTimestamp(long unixTime, Location initialLocation)
        //         {

        //            RinexNavigationParserGps rnp = null;
        //            long reqTime = unixTime;

        //            //        do {
        //            // found none, retrieve from urltemplate
        //            Time t = new Time(reqTime);
        //            //System.Console.WriteLine("request: "+unixTime+" "+(new Date(t.getMsec()))+" week:"+t.getGpsWeek()+" "+t.getGpsWeekDay());

        //            //readonly string url = t.formatTemplate(urltemplate);
        //            string url = "supl.google.com";

        //            if (pool.containsKey(url))
        //            {
        //                synchronized(this) {
        //                    rnp = pool.get(url);
        //                }
        //            }
        //            else
        //            {
        //                if (!retrievingFromServer.contains(url))
        //                {
        //                    retrievingFromServer.add(url);
        //                    (new Thread(new Runnable() {
        //                        override
        //                        public void run()
        //                    {
        //                        RinexNavigationParserGps rnp;

        //                        try
        //                        {
        //                            rnp = getFromSUPL(url, initialLocation);
        //                            synchronized(RinexNavigationGps.this) {
        //                                if (rnp != null)
        //                                {
        //                                    pool.put(url, rnp);
        //                                }
        //                                retrievingFromServer.remove(url);
        //                            }
        //                        }
        //                        catch (IOException e)
        //                        {
        //                            System.Console.WriteLine(e.getClass().getName() + " url: " + url);
        //                        }
        //                    }
        //                })).start();
        //            }
        //            return null;
        //        }
        //            return rnp;

        ////        } while (waitForData && rnp == null);
        //   }

        //    private RinexNavigationParserGps getFromSUPL(string url, Location initialLocation) throws IOException
        //    {
        //        RinexNavigationParserGps rnp = null;

        //        string suplName = url;
        //        readonly int serverPort = 7275;
        //        readonly bool sslEnabled = true;
        //        readonly bool messageLoggingEnabled = true;
        //        readonly bool loggingEnabled = true;
        //        File rnf = new File(RNP_CACHE, suplName);

        //        if (rnf.exists()) {
        //            System.Console.WriteLine("Supl from cache file " + rnf);
        //            try {
        //                rnp = SuplFileToRnpParserGps(rnf);
        //                return rnp;
        //            } catch (Exception e)
        //{
        //    rnf.delete();
        //}
        //        }

        //        try
        //{

        //    Log.w(TAG, "getFromSUPL: Getting data using SUPL client...");
        //    SuplConnectionRequest request =
        //            SuplConnectionRequest.builder()
        //                    .setServerHost(suplName)
        //                    .setServerPort(serverPort)
        //                    .setSslEnabled(sslEnabled)
        //                    .setMessageLoggingEnabled(messageLoggingEnabled)
        //                    .setLoggingEnabled(loggingEnabled)
        //                    .build();

        //    SuplController mSuplController = new SuplController(request);

        //    mSuplController.sendSuplRequest((long)(initialLocation.getLatitude() * 1e7), (long)(initialLocation.getLongitude() * 1e7));
        //    EphemerisResponse ephResponse = mSuplController.generateEphResponse((long)(initialLocation.getLatitude() * 1e7), (long)(initialLocation.getLongitude() * 1e7));

        //    if (ephResponse != null)
        //    {
        //        rnp = new RinexNavigationParserGps(ephResponse);
        //    }

        //    Log.w(TAG, "getFromSUPL: Received data from SUPL server");

        //}
        //catch (NullPointerException |
        //      UnsupportedOperationException |
        //      IllegalArgumentException |
        //      IndexOutOfBoundsException e) {
        //    Log.e(TAG, "Exception thrown getting msg from SUPL server", e);
        //    e.printStackTrace();
        //}
        //return rnp;
        //}

        //private RinexNavigationParserGps SuplFileToRnpParserGps(File rnf)
        //{
        //    RinexNavigationParserGps rnp = null;


        //    return rnp;
        //}

        //private RinexNavigationParserGps getFromFTP(string url) throws IOException
        //{
        //    RinexNavigationParserGps rnp = null;

        //    string origurl = url;
        //        if (negativeChache.containsKey(url)) {
        //        if (System.currentTimeMillis() - negativeChache.get(url).getTime() < 60 * 60 * 1000)
        //        {
        //            throw new FileNotFoundException("cached answer");
        //        }
        //        else
        //        {
        //            negativeChache.remove(url);
        //        }
        //    }

        //    string filename = url.replaceAll("[ ,/:]", "_");
        //        if (filename.endsWith(".Z")) filename = filename.substring(0, filename.length() - 2);
        //    File rnf = new File(RNP_CACHE, filename);

        //if (rnf.exists())
        //{
        //    System.Console.WriteLine(url + " from cache file " + rnf);
        //    rnp = new RinexNavigationParserGps(rnf);
        //    try
        //    {
        //        rnp.init();
        //        return rnp;
        //    }
        //    catch (Exception e)
        //    {
        //        rnf.delete();
        //    }
        //}

        //// if the file doesn't exist of is invalid
        //System.Console.WriteLine(url + " from the net.");
        //FTPClient ftp = new FTPClient();

        //try
        //{

        //    Log.w(TAG, "getFromFTP: Getting data from FTP server...");

        //    int reply;
        //    System.Console.WriteLine("URL: " + url);
        //    url = url.substring("ftp://".length());
        //    string server = url.substring(0, url.indexOf('/'));
        //    string remoteFile = url.substring(url.indexOf('/'));
        //    string remotePath = remoteFile.substring(0, remoteFile.lastIndexOf('/'));
        //    remoteFile = remoteFile.substring(remoteFile.lastIndexOf('/') + 1);

        //    ftp.connect(server);
        //    ftp.login("anonymous", "info@eriadne.org");

        //    System.out.print(ftp.getReplystring());

        //    // After connection attempt, you should check the reply code to
        //    // verify
        //    // success.
        //    reply = ftp.getReplyCode();

        //    if (!FTPReply.isPositiveCompletion(reply))
        //    {
        //        ftp.disconnect();
        //        System.err.println("FTP server refused connection.");
        //        return null;
        //    }

        //    ftp.enterLocalPassiveMode();
        //    ftp.setRemoteVerificationEnabled(false);

        //    System.Console.WriteLine("cwd to " + remotePath + " " + ftp.changeWorkingDirectory(remotePath));
        //    System.Console.WriteLine(ftp.getReplystring());
        //    ftp.setFileType(FTP.BINARY_FILE_TYPE);
        //    System.Console.WriteLine(ftp.getReplystring());

        //    System.Console.WriteLine("open " + remoteFile);
        //    InputStream is = ftp.retrieveFileStream(remoteFile);
        //    System.Console.WriteLine(ftp.getReplystring());
        //    if (ftp.getReplystring().startsWith("550"))
        //    {
        //        negativeChache.put(origurl, new Date());
        //        throw new FileNotFoundException();
        //    }
        //    InputStream uis = is;

        //    if (remoteFile.endsWith(".Z"))
        //    {
        //        uis = new UncompressInputStream(is);
        //    }

        //    rnp = new RinexNavigationParserGps(uis, rnf);
        //    rnp.init();
        //            is.close();


        //    ftp.completePendingCommand();

        //    ftp.logout();

        //    Log.w(TAG, "getFromFTP: Received data from server");

        //}
        //readonlyly
        //{
        //    if (ftp.isConnected())
        //    {
        //        try
        //        {
        //            ftp.disconnect();
        //        }
        //        catch (IOException ioe)
        //        {
        //            // do nothing
        //        }
        //    }
        //}
        //return rnp;
        //    }

        //        private RinexNavigationParserGps getFromHTTP(string tUrl) 
        //            {
        //                RinexNavigationParserGps rnp = null;

        //                string origurl = tUrl;
        //                    if (negativeChache.containsKey(tUrl)) {
        //                    if (System.currentTimeMillis() - negativeChache.get(tUrl).getTime() < 60 * 60 * 1000)
        //                    {
        //                        throw new FileNotFoundException("cached answer");
        //                    }
        //                    else
        //                    {
        //                        negativeChache.remove(tUrl);
        //                    }
        //                }

        //                string filename = tUrl.replaceAll("[ ,/:]", "_");
        //                    if (filename.endsWith(".Z")) filename = filename.substring(0, filename.length() - 2);
        //                File rnf = new File(RNP_CACHE, filename);

        //            if (rnf.exists())
        //            {
        //                System.Console.WriteLine(tUrl + " from cache file " + rnf);
        //                rnp = new RinexNavigationParserGps(rnf);
        //                rnp.init();
        //            }
        //            else
        //            {
        //                System.Console.WriteLine(tUrl + " from the net.");

        //                System.Console.WriteLine("URL: " + tUrl);
        //                tUrl = tUrl.substring("http://".length());
        //                string remoteFile = tUrl.substring(tUrl.indexOf('/'));
        //                remoteFile = remoteFile.substring(remoteFile.lastIndexOf('/') + 1);

        //                URL url = new URL("http://" + tUrl);
        //                HttpURLConnection con = (HttpURLConnection)url.openConnection();
        //                con.setRequestMethod("GET");
        //                con.setRequestProperty("Authorization", "Basic " + new string(Base64.encode(("anonymous:info@eriadne.org").getBytes(), Base64.DEFAULT)));
        //                //            con.setRequestProperty("Authorization", "Basic " + new string(Base64.getEncoder().encode((new string("anonymous:info@eriadne.org").getBytes()))));

        //                int reply = con.getResponseCode();

        //                if (reply > 200)
        //                {
        //                    if (reply == 404)
        //                        System.err.println("404 Not Found");
        //                    else
        //                        System.err.println("HTTP server refused connection.");
        //                    //        System.out.print(new string(res.getContent()));

        //                    return null;
        //                }

        //                try
        //                {
        //                    if (remoteFile.endsWith(".Z"))
        //                    {
        //                        try
        //                        {
        //                            //            InputStream is = new ByteArrayInputStream(res.getContent());
        //                            InputStream is = con.getInputStream();
        //                            InputStream uis = new UncompressInputStream(is);
        //                            rnp = new RinexNavigationParserGps(uis, rnf);
        //                            rnp.init();
        //                            uis.close();
        //                        }
        //                        catch (IOException e)
        //                        {
        //                            InputStream is = con.getInputStream();
        //                            InputStream uis = new GZIPInputStream(is);
        //                            rnp = new RinexNavigationParserGps(uis, rnf);
        //                            rnp.init();
        //                            //        Reader decoder = new InputStreamReader(gzipStream, encoding);
        //                            //        BufferedReader buffered = new BufferedReader(decoder);
        //                            uis.close();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        InputStream is = con.getInputStream();
        //                        rnp = new RinexNavigationParserGps(is, rnf);
        //                        rnp.init();
        //                                is.close();
        //                    }
        //                }
        //                catch (IOException e)
        //                {
        //                    e.printStackTrace();
        //                    // TODO delete file, maybe it's corrupt
        //                }
        //            }
        //            return rnp;
        //        }


        //        /* (non-Javadoc)
        //         * @see org.gogpsproject.NavigationProducer#getIono(int)
        //         */
        //        override
        //        public IonoGps getIono(long unixTime, Location initialLocation)
        //        {
        //            RinexNavigationParserGps rnp = getRNPByTimestamp(unixTime, initialLocation);
        //            if (rnp != null) return rnp.getIono(unixTime, initialLocation);
        //            return null;
        //        }


        //        public override IonoGalileo getIonoNeQuick(long unixTime, Location initialLocation)
        //        {
        //            return null;
        //        }

        //        /* (non-Javadoc)
        //         * @see org.gogpsproject.NavigationProducer#init()
        //         */
        //        public override void init()
        //        {

        //        }

        //        /* (non-Javadoc)
        //         * @see org.gogpsproject.NavigationProducer#release()
        //         */
        //        public override void release(bool waitForThread, long timeoutMs)
        //        {
        //            waitForData = false;
        //        }


        //        public override SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
        //        {
        //            // TODO Auto-generated method stub
        //            return null;
        //        }


    }
}