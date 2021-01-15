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
	/// Interface which all fixed sized matrices must implement
	/// 
	/// @author Peter Abeles
	/// </summary>
	public interface DMatrixFixed : DMatrix
	{
		//JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java default interface methods unless the C# 2019 extended interface option is selected:
		//		default <T> T create(int numRows, int numCols)
		//	{
		//		if(numRows == getNumRows() && numCols == getNumCols())
		//			return createLike();
		//		throw new RuntimeException("Fixed sized matrices can't be used to create matrices of arbitrary shape");
		//	}
	}
}