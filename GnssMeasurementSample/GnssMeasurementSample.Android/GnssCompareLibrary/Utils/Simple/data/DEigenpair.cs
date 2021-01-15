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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data
{

	/// <summary>
	/// An eigenpair is a set composed of an eigenvalue and an eigenvector.  In this library since only real
	/// matrices are supported, all eigenpairs are real valued.
	/// 
	/// @author Peter Abeles
	/// </summary>
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @Data public class DEigenpair
	public class DEigenpair
	{
		public double value;
		public DMatrixRMaj vector;

		public DEigenpair(double value, DMatrixRMaj vector)
		{
			this.value = value;
			this.vector = vector;
		}
	}
}