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
    public class ImplCommonOps_DDRM
    {
        public static void extract(DMatrixRMaj src,
                                   int srcY0, int srcX0,
                                   DMatrixRMaj dst,
                                   int dstY0, int dstX0,
                                   int numRows, int numCols)
        {
            for (int y = 0; y < numRows; y++)
            {
                int indexSrc = src.getIndex(y + srcY0, srcX0);
                int indexDst = dst.getIndex(y + dstY0, dstX0);
                System.Array.Copy(src.data, indexSrc, dst.data, indexDst, numCols);
            }
        }

        public static double elementMax(DMatrixD1 a,  ElementLocation loc )
        {
            int size = a.NumElements;

            int bestIndex = 0;
            double max = a.get(0);
            for (int i = 1; i < size; i++)
            {
                double val = a.get(i);
                if (val >= max)
                {
                    bestIndex = i;
                    max = val;
                }
            }

            if (loc != null)
            {
                loc.row = bestIndex / a.numCols;
                loc.col = bestIndex % a.numCols;
            }

            return max;
        }

        public static double elementMaxAbs(DMatrixD1 a,  ElementLocation loc )
        {
            int size = a.NumElements;

            int bestIndex = 0;
            double max = 0;
            for (int i = 0; i < size; i++)
            {
                double val = Math.Abs(a.get(i));
                if (val > max)
                {
                    bestIndex = i;
                    max = val;
                }
            }

            if (loc != null)
            {
                loc.row = bestIndex / a.numCols;
                loc.col = bestIndex % a.numCols;
            }

            return max;
        }

        public static double elementMin(DMatrixD1 a,  ElementLocation loc )
        {
            int size = a.NumElements;

            int bestIndex = 0;
            double min = a.get(0);
            for (int i = 1; i < size; i++)
            {
                double val = a.get(i);
                if (val < min)
                {
                    bestIndex = i;
                    min = val;
                }
            }

            if (loc != null)
            {
                loc.row = bestIndex / a.numCols;
                loc.col = bestIndex % a.numCols;
            }

            return min;
        }

        public static double elementMinAbs(DMatrixD1 a,  ElementLocation loc )
        {
            int size = a.NumElements;

            int bestIndex = 0;
            double min = Double.MaxValue;
            for (int i = 0; i < size; i++)
            {
                double val = Math.Abs(a.get(i));
                if (val < min)
                {
                    bestIndex = i;
                    min = val;
                }
            }

            if (loc != null)
            {
                loc.row = bestIndex / a.numCols;
                loc.col = bestIndex % a.numCols;
            }

            return min;
        }

        public static void elementMult(DMatrixD1 A, DMatrixD1 B)
        {
            UtilEjml.checkSameShape(A, B, true);

            int length = A.NumElements;

            for (int i = 0; i < length; i++)
            {
                A.times(i, B.get(i));
            }
        }

        public static T elementMult<T>(T A, T B,  T output) where T : DMatrixD1
        {
            UtilEjml.checkSameShape(A, B, true);
            output = UtilEjml.reshapeOrDeclare(output, A);

            int length = A.NumElements;

            for (int i = 0; i < length; i++)
            {
                output.set(i, A.get(i) * B.get(i));
            }

            return output;
        }

        public static void elementDiv(DMatrixD1 A, DMatrixD1 B)
        {
            UtilEjml.checkSameShape(A, B, true);

            int length = A.NumElements;

            for (int i = 0; i < length; i++)
            {
                A.div(i, B.get(i));
            }
        }

        public static T elementDiv<T>(T A, T B,  T output ) where T: DMatrixD1
        {
            UtilEjml.checkSameShape(A, B, true);
            output = UtilEjml.reshapeOrDeclare(output, A);

            int length = A.NumElements;

            for (int i = 0; i < length; i++)
            {
                output.set(i, A.get(i) / B.get(i));
            }

            return output;
        }

        public static double elementSum(DMatrixD1 mat)
        {
            double total = 0;
             
            int size = mat.NumElements;

            for (int i = 0; i < size; i++)
            {
                total += mat.get(i);
            }

            return total;
        }

        public static double elementSumAbs(DMatrixD1 mat)
        {
            double total = 0;

            int size = mat.NumElements;

            for (int i = 0; i < size; i++)
            {
                total += Math.Abs(mat.get(i));
            }

            return total;
        }

        public static T elementPower<T>(T A, T B,  T output ) 
            where T: DMatrixD1
        {
            UtilEjml.checkSameShape(A, B, true);
            output = UtilEjml.reshapeOrDeclare(output, A);

            int size = A.NumElements;
            for (int i = 0; i < size; i++)
            {
                output.data[i] = Math.Pow(A.data[i], B.data[i]);
            }

            return output;
        }

        public static T elementPower<T>(double a, T B,  T output)
            where T : DMatrixD1
        {
            output = UtilEjml.reshapeOrDeclare(output, B);

            int size = B.NumElements;
            for (int i = 0; i < size; i++)
            {
                output.data[i] = Math.Pow(a, B.data[i]);
            }

            return output;
        }

        public static T elementPower<T>(T A, double b,  T output)
            where T : DMatrixD1
        {
            output = UtilEjml.reshapeOrDeclare(output, A);

            int size = A.NumElements;
            for (int i = 0; i < size; i++)
            {
                output.data[i] = Math.Pow(A.data[i], b);
            }

            return output;
        }

        public static T elementLog<T>(T A,  T output)
            where T : DMatrixD1
        {
            output = UtilEjml.reshapeOrDeclare(output, A);

            int size = A.NumElements;
            for (int i = 0; i < size; i++)
            {
                output.data[i] = Math.Log(A.data[i]);
            }

            return output;
        }

        public static T elementExp<T>(T A,  T output)
            where T : DMatrixD1
        {
            output = UtilEjml.reshapeOrDeclare(output, A);

            int size = A.NumElements;
            for (int i = 0; i < size; i++)
            {
                output.data[i] = Math.Exp(A.data[i]);
            }

            return output;
        }
    }
}