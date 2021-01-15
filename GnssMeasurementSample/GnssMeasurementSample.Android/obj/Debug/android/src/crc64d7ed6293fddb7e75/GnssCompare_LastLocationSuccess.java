package crc64d7ed6293fddb7e75;


public class GnssCompare_LastLocationSuccess
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.tasks.OnSuccessListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSuccess:(Ljava/lang/Object;)V:GetOnSuccess_Ljava_lang_Object_Handler:Android.Gms.Tasks.IOnSuccessListenerInvoker, Xamarin.GooglePlayServices.Tasks\n" +
			"";
		mono.android.Runtime.register ("GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+LastLocationSuccess, GnssMeasurementSample.Android", GnssCompare_LastLocationSuccess.class, __md_methods);
	}


	public GnssCompare_LastLocationSuccess ()
	{
		super ();
		if (getClass () == GnssCompare_LastLocationSuccess.class)
			mono.android.TypeManager.Activate ("GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+LastLocationSuccess, GnssMeasurementSample.Android", "", this, new java.lang.Object[] {  });
	}

	public GnssCompare_LastLocationSuccess (crc6421597d060953b6ae.MainActivity p0, crc64d7ed6293fddb7e75.GnssCompare_MyGnssMeasurementEventCallback p1)
	{
		super ();
		if (getClass () == GnssCompare_LastLocationSuccess.class)
			mono.android.TypeManager.Activate ("GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+LastLocationSuccess, GnssMeasurementSample.Android", "GnssMeasurementSample.Droid.MainActivity, GnssMeasurementSample.Android:GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+MyGnssMeasurementEventCallback, GnssMeasurementSample.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public void onSuccess (java.lang.Object p0)
	{
		n_onSuccess (p0);
	}

	private native void n_onSuccess (java.lang.Object p0);

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
