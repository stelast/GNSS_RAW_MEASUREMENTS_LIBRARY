package crc64d7ed6293fddb7e75;


public class GnssCompare_LastLocationFailure
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.tasks.OnFailureListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onFailure:(Ljava/lang/Exception;)V:GetOnFailure_Ljava_lang_Exception_Handler:Android.Gms.Tasks.IOnFailureListenerInvoker, Xamarin.GooglePlayServices.Tasks\n" +
			"";
		mono.android.Runtime.register ("GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+LastLocationFailure, GnssMeasurementSample.Android", GnssCompare_LastLocationFailure.class, __md_methods);
	}


	public GnssCompare_LastLocationFailure ()
	{
		super ();
		if (getClass () == GnssCompare_LastLocationFailure.class)
			mono.android.TypeManager.Activate ("GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+LastLocationFailure, GnssMeasurementSample.Android", "", this, new java.lang.Object[] {  });
	}

	public GnssCompare_LastLocationFailure (crc6421597d060953b6ae.MainActivity p0, crc64d7ed6293fddb7e75.GnssCompare_MyGnssMeasurementEventCallback p1)
	{
		super ();
		if (getClass () == GnssCompare_LastLocationFailure.class)
			mono.android.TypeManager.Activate ("GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+LastLocationFailure, GnssMeasurementSample.Android", "GnssMeasurementSample.Droid.MainActivity, GnssMeasurementSample.Android:GnssMeasurementSample.Droid.GnssCompareLibrary.GnssCompare+MyGnssMeasurementEventCallback, GnssMeasurementSample.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public void onFailure (java.lang.Exception p0)
	{
		n_onFailure (p0);
	}

	private native void n_onFailure (java.lang.Exception p0);

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
