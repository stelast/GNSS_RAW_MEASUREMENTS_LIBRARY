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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class DGrowArray
    {
        public double[] data;
        public int length2;

        public DGrowArray(int length)
        {
            this.data = new double[length];
            this.length2 = length;
        }

        public DGrowArray() : this(0)
        {
        }

        public int length()
        {
            return length2;
        }

        public void reset() { reshape(0); }

        /**
         * Changes the array's length and doesn't attempt to preserve previous values if a new array is required
         *
         * @param length New array length
         */
        public DGrowArray reshape(int length)
        {
            if (data.Count() < length)
            {
                data = new double[length];
            }
            this.length2 = length;
            return this;
        }

        /**
         * Increases the internal array's length by the specified amount. Previous values are preserved.
         * The length value is not modified since this does not change the 'meaning' of the array, just
         * increases the amount of data which can be stored in it.
         *
         * this.data = new data_type[ data.length + amount ]
         *
         * @param amount Number of elements added to the internal array's length
         */
        public void growInternal(int amount)
        {
            double[] tmp = new double[data.Count() + amount];

            System.Array.Copy(data, 0, tmp, 0, data.Count());
            this.data = tmp;
        }

        public void setTo(DGrowArray original)
        {
            reshape(original.length2);
            System.Array.Copy(original.data, 0, data, 0, original.length2);
        }

        public double get(int index)
        {
            if (index < 0 || index >= length2)
                throw new ArgumentException("Out of bounds");
            return data[index];
        }

        public void set(int index, double value)
        {
            if (index < 0 || index >= length2)
                throw new ArgumentException("Out of bounds");
            data[index] = value;
        }

        public void free()
        {
            data = new double[0];
            length2 = 0;
        }
    }
}