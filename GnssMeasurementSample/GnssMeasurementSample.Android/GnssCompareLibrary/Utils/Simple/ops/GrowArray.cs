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
    public class GrowArray<D>
    {
        //ConcurrencyOps.NewInstance<D> factory;
        //ConcurrencyOps.Reset<D> reset;

        D[] array;
        int size2 { get; set; }
        /*
        public GrowArray(ConcurrencyOps.NewInstance<D> factory) : base(factory, )
        {
            this(factory, (o)-> { });
        }

        public GrowArray(ConcurrencyOps.NewInstance<D> factory, ConcurrencyOps.Reset<D> reset)
        {
            this.factory = factory;
            this.reset = reset;

            array = createArray(0);
            size = 0;
        }*/

        private D[] createArray(int length)
        {
            return new D[length];
         //   return (D[])Array.newInstance(factory.newInstance().getClass(), length);
        }

        public void reset()
        {
            size2 = 0;
        }

        /**
         * Increases the size of the array so that it contains the specified number of elements. If the new length
         * is bigger than the old size then reset is called on the new elements
         */
        public void resize(int length)
        {
            if (length >= array.Count())
            {
                D[] tmp = createArray(length);
                System.Array.Copy(array, 0, tmp, 0, array.Count());
                for (int i = array.Count(); i < tmp.Count(); i++)
                {
                    //tmp[i] = factory.newInstance();
                }
                this.array = tmp;
            }
            for (int i = size2; i < length; i++)
            {
                //reset.reset(array[i]);
            }
            this.size2 = length;
        }

        /**
         * Add a new element to the array. Reset is called on it and it's then returned.
         */
        public D grow()
        {
            if (size2 == array.Count())
            {
                int length = Math.Max(10, size2 < 1000 ? size2 * 2 : size2 * 5 / 3);
                D[] tmp = createArray(length);
                System.Array.Copy(array, 0, tmp, 0, array.Count());
                for (int i = array.Count(); i < tmp.Count(); i++)
                {
                    //tmp[i] = factory.newInstance();
                }
                this.array = tmp;
            }
            D ret = array[size2++];
            //reset.reset(ret);
            return ret;
        }

        public D get(int index)
        {
            return array[index];
        }

        public int size()
        {
            return size2;
        }
    }
}