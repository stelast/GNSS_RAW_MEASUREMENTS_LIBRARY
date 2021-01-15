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
    public class MatrixIO
    {
        /** Default printf float format */
        public static String DEFAULT_FLOAT_FORMAT = "%11.4E";
        /** Number of digits in pretty format */
        public static int DEFAULT_LENGTH = 11;
        /** Specified the printf format used when printing out in Matlab format */
        public static String MATLAB_FORMAT = "%.8E";

        public static void printFancy(DMatrix mat, int length)
        {
            // TODO print en consola
            /*
            printTypeSize(out, mat);
            DecimalFormat format = new DecimalFormat("#");

            final int cols = mat.getNumCols();

            for (int row = 0; row < mat.getNumRows(); row++)
            {
                for (int col = 0; col < cols; col++)
                {
                out.print(fancyStringF(mat.get(row, col), format, length, 4));
                    if (col != cols - 1)
                    out.print(" ");
                }
            out.println();
            }
            */
        }

        public static void printFancy(ZMatrix mat, int length)
        {
            // TODO print en consola
            /*
            printTypeSize(out, mat);
            DecimalFormat format = new DecimalFormat("#");

            final int cols = mat.getNumCols();

            for (int row = 0; row < mat.getNumRows(); row++)
            {
                for (int col = 0; col < cols; col++)
                {
                out.print(fancyStringF(mat.get(row, col), format, length, 4));
                    if (col != cols - 1)
                    out.print(" ");
                }
            out.println();
            }
            */
        }

        public static void print( string mat)
        {
            // TODO print en consola
            /*
            String format = DEFAULT_FLOAT_FORMAT;

            switch (mat.getType())
            {
                case DDRM: print(out, (DMatrix)mat, format); break;
                case FDRM: print(out, (FMatrix)mat, format); break;
                case ZDRM: print(out, (ZMatrix)mat, format); break;
                case CDRM: print(out, (CMatrix)mat, format); break;
                case DSCC: print(out, (DMatrixSparseCSC)mat, format); break;
                case DTRIPLET: print(out, (DMatrixSparseTriplet)mat, format); break;
                case FSCC: print(out, (FMatrixSparseCSC)mat, format); break;
                case FTRIPLET: print(out, (FMatrixSparseTriplet)mat, format); break;
                default: throw new RuntimeException("Unknown type " + mat.getType());
            }*/
        }

        public static void print(Matrix matrix, string mat)
        {
            // TODO print en consola
            /*
            String format = DEFAULT_FLOAT_FORMAT;

            switch (mat.getType())
            {
                case DDRM: print(out, (DMatrix)mat, format); break;
                case FDRM: print(out, (FMatrix)mat, format); break;
                case ZDRM: print(out, (ZMatrix)mat, format); break;
                case CDRM: print(out, (CMatrix)mat, format); break;
                case DSCC: print(out, (DMatrixSparseCSC)mat, format); break;
                case DTRIPLET: print(out, (DMatrixSparseTriplet)mat, format); break;
                case FSCC: print(out, (FMatrixSparseCSC)mat, format); break;
                case FTRIPLET: print(out, (FMatrixSparseTriplet)mat, format); break;
                default: throw new RuntimeException("Unknown type " + mat.getType());
            }*/
        }


    }
}