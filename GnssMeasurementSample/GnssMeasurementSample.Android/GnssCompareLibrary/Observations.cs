using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Constellations.Rinex;
using Java.IO;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary
{
    public class Observations : Streamable
    {
        //SimpleDateFormat sdfHeader = getGMTdf();
        DecimalFormat dfX4 = new DecimalFormat("0.0000");

        private readonly static int STREAM_V = 1;

        private GnssCompareLibrary.Constellations.Time refTime; /* Reference time of the dataset */
        private int eventFlag; /* Event flag */

        private List<ObservationSet> obsSet; /* sets of observations */
        private int issueOfData = -1;
        public int index;

        /**
		 * The Rinex filename
		 */
        public string rinexFileName;

        public int write(DataOutputStream dos)
        {
            throw new NotImplementedException();
        }

        public void read(DataInputStream dai, bool oldVersion)
        {
            throw new NotImplementedException();
        }

        public static SimpleDateFormat getGMTdf()
        {
        	SimpleDateFormat sdfHeader = new SimpleDateFormat("dd-MMM-yy HH:mm:ss");
        	sdfHeader.TimeZone = Android.Icu.Util.TimeZone.GetTimeZone("GMT");
        	return sdfHeader;
        }

        	//public Object clone()
        	//{
        	//	try
        	//	{
         //           ByteArrayOutputStream baos = new ByteArrayOutputStream();
        	//		this.write(new DataOutputStream(baos));
        	//		DataInputStream dis = new DataInputStream(new ByteArrayInputStream(baos.ToByteArray()));
        	//		baos.reset();
        	//		dis.readUTF();
        	//		return new Observations(dis, false);
        	//	}
        	//	catch (IOException ioe)
        	//	{
        	//		ioe.PrintStackTrace();
        	//	}
        	//	return null;
        	//}

        	public Observations(Constellations.Time time, int flag)
        	{
        		this.refTime = time;
        		this.eventFlag = flag;
        	}

        	public Observations(DataInputStream dai, bool oldVersion) /*throws IOException*/
        	{
        		read(dai, oldVersion);
            }

            public void cleanObservations()
            {
        	    if (obsSet != null)
        		    for (int i = obsSet.Count() - 1; i >= 0; i--)
        			    if (obsSet[i] == null || double.IsNaN(obsSet[i].getPseudorange(0)))
        				    obsSet.RemoveAt(i);
            }

        public int getNumSat()
        {
        	if (obsSet == null) return 0;
        	int nsat = 0;
        	for (int i = 0; i < obsSet.Count(); i++)
        		if (obsSet[i] != null) nsat++;
        	return obsSet == null ? -1 : nsat;
        }

        public ObservationSet getSatByIdx(int idx)
        {
        	return obsSet[idx];
        }

        public ObservationSet getSatByID(int satID)
        {
        	if (obsSet == null || satID == null) return null;
        	for (int i = 0; i < obsSet.Count(); i++)
        		if (obsSet[i] != null && obsSet[i].getSatID() == satID) return obsSet[i];
        	return null;
        }

        public ObservationSet getSatByIDType(int satID, char satType)
        {
        	if (obsSet == null || satID == null) return null;
        	for (int i = 0; i < obsSet.Count(); i++)
        		if (obsSet[i] != null && obsSet[i].getSatID() == satID && obsSet[i].getSatType() == satType) return obsSet[i];
        	return null;
        }

        ////	public ObservationSet getGpsByID(char satGnss){
        ////		string sub = string.valueOf(satGnss); 
        ////		string str = sub.substring(0, 1);  
        ////		char satType = str.charAt(0);
        ////		sub = sub.substring(1, 3);  
        ////		Integer satID = Integer.parseInt(sub);
        ////		
        ////		if(gps == null || satID==null) return null;
        ////		for(int i=0;i<gps.size();i++)
        ////			if(gps[i]!=null && gps[i].getSatID()==satID.intValue() && gps[i].getSatType()==satType) return gps[i];
        ////		return null;
        ////	}

        public int getSatID(int idx)
        {
        	return getSatByIdx(idx).getSatID();
        }

        public char getGnssType(int idx)
        {
        	return getSatByIdx(idx).getSatType();
        }

        public bool containsSatID(int id)
        {
        	return getSatByID(id) != null;
        }

        public bool containsSatIDType(int id, char satType)
        {
        	return getSatByIDType(id, satType) != null;
        }

        ///**
        // * @return the refTime
        // */
        public GnssCompareLibrary.Constellations.Time getRefTime()
        {
        	return refTime;
        }

        ///**
        // * @param refTime the refTime to set
        // */
        public void setRefTime(Constellations.Time refTime)
        {
        	this.refTime = refTime;
        }

        ///**
        // * Epoch flag
        // * 0: OK
        // * 1: power failure between previous and current epoch
        // * >1: Special event
        // *  2: start moving antenna
        //    *  3: new site occupation
        //    *  (end of kinem. data)
        //    * (at least MARKER NAME record
        //    * follows)
        //    * 4: header information follows
        //    * 5: external event (epoch is significant)
        //    * 6: cycle slip records follow
        //    * to optionally report detected
        //    * and repaired cycle slips
        //    * (same format as OBSERVATIONS
        //    * records; slip instead of observation;
        //    * LLI and signal strength blank)
        //    *
        // * @return the eventFlag
        // */
        public int getEventFlag()
        {
        	return eventFlag;
        }

        ///**
        // * @param eventFlag the eventFlag to set
        // */
        public void setEventFlag(int eventFlag)
        {
        	this.eventFlag = eventFlag;
        }

        ////	public void init(int nGps, int nGlo, int nSbs){
        ////		gpsSat = new ArrayList<Integer>(nGps);
        ////		gloSat = new ArrayList<Integer>(nGlo);
        ////		sbsSat = new ArrayList<Integer>(nSbs);
        ////
        ////		// Allocate array of observation objects
        ////		if (nGps > 0) gps = new ObservationSet[nGps];
        ////		if (nGlo > 0) glo = new ObservationSet[nGlo];
        ////		if (nSbs > 0) sbs = new ObservationSet[nSbs];
        ////	}

        public void setGps(int i, ObservationSet os)
        {
        	if (obsSet == null) obsSet = new List<ObservationSet>(i + 1);
        	if (i == obsSet.Count())
        	{
        		obsSet.Add(os);
        	}
        	else
        	{
        		int c = obsSet.Count();
        		while (c++ <= i) obsSet.Add(null);
        		obsSet[i] = os;
        	}
        	//gps[i] = os;
        	//gpsSat.add(os.getSatID());
        }

        //public int write(DataOutputStream dos) throws IOException
        //{
        //	dos.writeUTF(MESSAGE_OBSERVATIONS); // 5
        //	dos.writeInt(STREAM_V); // 4
        //	dos.writeLong(refTime==null?-1:refTime.getMsec()); // 13
        //	dos.writeDouble(refTime==null?-1:refTime.getFraction());
        //	dos.write(eventFlag); // 14
        //	dos.write(obsSet==null?0:obsSet.size()); // 15
        //	int size=19;
        //	if(obsSet!=null){
        //		for (int i = 0; i < obsSet.size(); i++)
        //		{
        //			size += ((ObservationSet)obsSet[i]).write(dos);
        //		}
        //	}
        //	return size;
        //}

        //public string tostring()
        //{

        //	string lineBreak = System.getProperty("line.separator");

        //	string out= " GPS Time:" + getRefTime().getGpsTime() + " " + sdfHeader.format(new Date(getRefTime().getMsec())) + " evt:" + eventFlag + lineBreak;
        //	for (int i = 0; i < getNumSat(); i++)
        //	{
        //		ObservationSet os = getSatByIdx(i);
        //		out+= "satType:" + os.getSatType() + "  satID:" + os.getSatID() + "\tC:" + fd(os.getCodeC(0))
        //			+ " cP:" + fd(os.getCodeP(0))
        //			+ " Ph:" + fd(os.getPhaseCycles(0))
        //			+ " Dp:" + fd(os.getDoppler(0))
        //			+ " Ss:" + fd(os.getSignalStrength(0))
        //			+ " LL:" + fd(os.getLossLockInd(0))
        //			+ " LL2:" + fd(os.getLossLockInd(1))
        //			+ lineBreak;
        //	}
        //	return out;
        //}

        private string fd(double n)
        {
        	return double.IsNaN(n) ? "NaN" : dfX4.Format(n);
        }

        ///* (non-Javadoc)
        // * @see org.gogpsproject.Streamable#read(java.io.DataInputStream)
        // */
        //@Override
        //public void read(DataInputStream dai, bool oldVersion) throws IOException
        //{
        //	int v=1;
        //	if(!oldVersion) v=dai.readInt();

        //	if(v==1){
        //		refTime = new Time(dai.readLong(), dai.readDouble());
        //		eventFlag = dai.read();
        //		int size = dai.read();
        //		obsSet = new ArrayList<ObservationSet>(size);

        //		for (int i = 0; i < size; i++)
        //		{
        //			if (!oldVersion) dai.readUTF();
        //			ObservationSet os = new ObservationSet(dai, oldVersion);
        //			obsSet.add(os);
        //		}
        //	}else{
        //		throw new IOException("Unknown format version:" + v);
        //	}
        //}

        //public void setIssueOfData(int iOD)
        //{
        //	this.issueOfData = iOD;
        //}

        //public int getIssueOfData()
        //{
        //	return this.issueOfData;
        //}

    }
}