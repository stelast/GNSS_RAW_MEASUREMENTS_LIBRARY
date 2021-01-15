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
    public class DConvertArrays
    {
        public static DMatrixRMaj convert(double[][] src, DMatrixRMaj? dst )
        {
            int rows = src.Count();
            if (rows == 0)
                throw new ArgumentException("Rows of src can't be zero");
            int cols = src[0].Count();

            UtilEjml.checkTooLarge(rows, cols);

            if (dst == null)
            {
                dst = new DMatrixRMaj(rows, cols);
            }
            else
            {
                dst.reshape(rows, cols);
            }
            int pos = 0;
            for (int i = 0; i < rows; i++)
            {
                double[] row = src[i];

                if (row.Count() != cols)
                {
                    throw new ArgumentException("All rows must have the same length");
                }

                System.Array.Copy(row, 0, dst.data, pos, cols);

                pos += cols;
            }

            return dst;
        }

        //    public static DMatrixSparseCSC convert(double[][]src , @Nullable DMatrixSparseCSC dst ) {
        //        int rows = src.length;
        //        if( rows == 0 )
        //            throw new IllegalArgumentException("Rows of src can't be zero");
        //        int cols = src[0].length;
        //
        //        if( dst == null ) {
        //            dst = new DMatrixSparseCSC(rows,cols);
        //        } else {
        //            dst.reshape(rows,cols);
        //        }
        //
        //        for (int col = 0; col < cols; col++) {
        //            for (int row = 0; row < rows; row++) {
        //                double v = src[row][col];
        //                if( v == 0 )
        //                    continue;
        //                // make sure there's enoguh data to store the new element and a bit extra
        //                if( dst.nz_values.length <= dst.nz_length ) {
        //                    dst.growMaxLength(dst.nz_values.length*2+2,true);
        //                }
        //                dst.nz_values[dst.nz_length] = v;
        //                dst.nz_rows[dst.nz_length++] = row;
        //            }
        //            dst.col_idx[col+1] = dst.nz_length;
        //        }
        //        dst.indicesSorted = true;
        //
        //
        //        return dst;
        //    }

        public static DMatrix4 convert(double[][] src, DMatrix4? dst )
        {
            if (dst == null)
                dst = new DMatrix4();

            if (src.Count() == 4)
            {
                if (src[0].Count() == 1)
                    throw new ArgumentException("Expected a vector");
                dst.a1 = src[0][0];
                dst.a2 = src[1][0];
                dst.a3 = src[2][0];
                dst.a4 = src[3][0];
            }
            else if (src.Count() == 1 && src[0].Count() == 4)
            {
                dst.a1 = src[0][0];
                dst.a2 = src[0][1];
                dst.a3 = src[0][2];
                dst.a4 = src[0][3];
            }
            else
            {
                throw new ArgumentException("Expected a 4x1 or 1x4 vector");
            }

            return dst;
        }
    }
}