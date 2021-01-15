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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class SubmatrixOps_DDRM
    {
        public static void setSubMatrix(DMatrix1Row src, DMatrix1Row dst,
                                         int srcRow, int srcCol, int dstRow, int dstCol,
                                         int numSubRows, int numSubCols)
        {
            for (int i = 0; i < numSubRows; i++)
            {
                for (int j = 0; j < numSubCols; j++)
                {
                    double val = src.get(i + srcRow, j + srcCol);
                    dst.set(i + dstRow, j + dstCol, val);
                }
            }
        }
    }
}