using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple
{

	/// <summary>
	/// Extension to <seealso cref="SimpleOperations"/> for sparse matrices
	/// 
	/// @author Peter Abeles
	/// </summary>
	public interface SimpleSparseOperations<S, D> : SimpleOperations<S> where S : MatrixSparse where D : Matrix
	{

		void extractDiag(S input, D output);

		void multTransA(S A, D B, D output);

		void mult(S A, D B, D output);
	}
}