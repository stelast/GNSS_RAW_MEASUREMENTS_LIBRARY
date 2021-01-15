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
	/// An integer array which can have its size changed
	/// 
	/// @author Peter Abeles
	/// </summary>
	public class IGrowArray
	{
		public int[] data;
		//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods of the current type:
		public int length_Conflict;

		public IGrowArray(int length)
		{
			this.data = new int[length];
			this.length_Conflict = length;
		}

		public IGrowArray() : this(0)
		{
		}

		public virtual int length()
		{
			return length_Conflict;
		}

		public virtual void reshape(int length)
		{
			if (data.Length < length)
			{
				data = new int[length];
			}
			this.length_Conflict = length;
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
			int[] tmp = new int[data.Length + amount];

			Array.Copy(data, 0, tmp, 0, data.Length);
			this.data = tmp;
		}

		public virtual IGrowArray To
		{
			set
			{
				reshape(value.length_Conflict);
				Array.Copy(value.data, 0, data, 0, value.length_Conflict);
			}
		}

		public virtual int get(int index)
		{
			if (index < 0 || index >= length_Conflict)
			{
				throw new System.ArgumentException("Out of bounds");
			}
			return data[index];
		}

		public virtual void set(int index, int value)
		{
			if (index < 0 || index >= length_Conflict)
			{
				throw new System.ArgumentException("Out of bounds");
			}
			data[index] = value;
		}

		public virtual void add(int value)
		{
			if (length_Conflict == data.Length)
			{
				growInternal(Math.Min(10_000, 1 + data.Length));
			}
			data[length_Conflict++] = value;
		}

		public virtual void clear()
		{
			length_Conflict = 0;
		}

		public virtual void free()
		{
			data = new int[0];
			length_Conflict = 0;
		}
	}
}