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
	/// This exception is thrown if an operation can not be finished because the matrix is singular.
	/// It is a RuntimeException to allow the code to be written cleaner and also because singular
	/// matrices are not always detected.  Forcing an exception to be caught provides a false sense
	/// of security.
	/// 
	/// @author Peter Abeles
	/// </summary>
	public class SingularMatrixException : Exception
	{

		public SingularMatrixException()
		{
		}

		public SingularMatrixException(string message) : base(message)
		{
		}
	}
}