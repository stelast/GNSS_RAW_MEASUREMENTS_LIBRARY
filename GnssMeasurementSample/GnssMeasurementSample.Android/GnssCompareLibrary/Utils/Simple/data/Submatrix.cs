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
	/// <para>
	/// Describes a rectangular submatrix.
	/// </para>
	/// 
	/// <para>
	/// Rows are row0 &le; i &lt; row1 and Columns are col0 &le; j &lt; col1
	/// </para>
	/// 
	/// @author Peter Abeles
	/// </summary>
	public abstract class Submatrix<M> where M : Matrix
	{
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @SuppressWarnings("NullAway.Init") public M original;
		public M original;

		// bounding rows and columns
		public int row0, col0;
		public int row1, col1;

		public virtual void set(M original, int row0, int row1, int col0, int col1)
		{
			this.original = original;
			this.row0 = row0;
			this.col0 = col0;
			this.row1 = row1;
			this.col1 = col1;
		}

		public virtual void set(M original)
		{
			this.original = original;
			row1 = original.NumRows;
			col1 = original.NumCols;
		}

		public virtual int Rows
		{
			get
			{
				return row1 - row0;
			}
		}

		public virtual int Cols
		{
			get
			{
				return col1 - col0;
			}
		}

		public abstract void print();
	}
}