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
	/// The row and column of an element in a Matrix
	/// 
	/// @author Peter Abeles
	/// </summary>
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @Data public class ElementLocation
	public class ElementLocation
	{
		/// <summary>
		/// Row coordinate of an element </summary>
		public int row;

		/// <summary>
		/// Column coordinate of an element </summary>
		public int col;

		public ElementLocation()
		{
		}

		public ElementLocation(int row, int col)
		{
			this.row = row;
			this.col = col;
		}

		public virtual ElementLocation To
		{
			set
			{
				this.row = value.row;
				this.col = value.col;
			}
		}

		public virtual void setTo(int row, int col)
		{
			this.row = row;
			this.col = col;
		}
	}
}