package crc64d7ed6293fddb7e75;


public class GnssCompare_MyGnssMeasurementEventCallback
	extends android.location.GnssMeasurementsEvent.Callback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onGnssMeasurementsReceived:(Landroid/location/GnssMeasurementsEvent;)V:GetOnGnssMeasurementsReceived_Landroid_location_GnssMeasurementsEvent_Handler\n" +
			"n_onStatusChanged:(I)V:GetOnStatusChanged_IHandler\n" +
			"";
		mono.android.Runtime.register ("GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+MyGnssMeasurementEventCallback, GnssMeasurementSample.Android", GnssCompare_MyGnssMeasurementEventCallback.class, __md_methods);
	}


	public GnssCompare_MyGnssMeasurementEventCallback ()
	{
		super ();
		if (getClass () == GnssCompare_MyGnssMeasurementEventCallback.class)
			mono.android.TypeManager.Activate ("GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+MyGnssMeasurementEventCallback, GnssMeasurementSample.Android", "", this, new java.lang.Object[] {  });
	}


	public void onGnssMeasurementsReceived (android.location.GnssMeasurementsEvent p0)
	{
		n_onGnssMeasurementsReceived (p0);
	}

	private native void n_onGnssMeasurementsReceived (android.location.GnssMeasurementsEvent p0);


	public void onStatusChanged (int p0)
	{
		n_onStatusChanged (p0);
	}

	private native void n_onStatusChanged (int p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
