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
	/// A double array which can have its size changed
	/// 
	/// @author Peter Abeles
	/// </summary>
	public class DGrowArray
	{
		public double[] data;
		//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods of the current type:
		public int length_Conflict;

		public DGrowArray(int length)
		{
			this.data = new double[length];
			this.length_Conflict = length;
		}

		public DGrowArray() : this(0)
		{
		}

		public virtual int length()
		{
			return length_Conflict;
		}

		public virtual void reset()
		{
			reshape(0);
		}

		/// <summary>
		/// Changes the array's length and doesn't attempt to preserve previous values if a new array is required
		/// </summary>
		/// <param name="length"> New array length </param>
		public virtual DGrowArray reshape(int length)
		{
			if (data.Length < length)
			{
				data = new double[length];
			}
			this.length_Conflict = length;
			return this;
		}

		/// <summary>
		/// Increases the internal array's length by the specified amount. Previous values are preserved.
		/// The length value is not modified since this does not change the 'meaning' of the array, just
		/// increases the amount of data which can be stored in it.
		/// 
		/// this.data = new data_type[ data.length + amount ]
		/// </summary>
		/// <param name="amount"> Number of elements added to the internal array's length </param>
		public virtual void growInternal(int amount)
		{
			double[] tmp = new double[data.Length + amount];

			Array.Copy(data, 0, tmp, 0, data.Length);
			this.data = tmp;
		}

		public virtual DGrowArray To
		{
			set
			{
				reshape(value.length_Conflict);
				Array.Copy(value.data, 0, data, 0, value.length_Conflict);
			}
		}

		public virtual double get(int index)
		{
			if (index < 0 || index >= length_Conflict)
			{
				throw new System.ArgumentException("Out of bounds");
			}
			return data[index];
		}

		public virtual void set(int index, double value)
		{
			if (index < 0 || index >= length_Conflict)
			{
				throw new System.ArgumentException("Out of bounds");
			}
			data[index] = value;
		}

		public virtual void free()
		{
			data = new double[0];
			length_Conflict = 0;
		}
	}
}