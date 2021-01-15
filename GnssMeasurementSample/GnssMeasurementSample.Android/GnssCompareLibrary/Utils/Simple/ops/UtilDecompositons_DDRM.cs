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
    public class UtilDecompositons_DDRM
    {
        public static DMatrixRMaj ensureIdentity( DMatrixRMaj A, int numRows, int numCols)
        {
            if (A == null)
            {
                return CommonOps_DDRM.identity(numRows, numCols);
            }
            A.reshape(numRows, numCols);
            CommonOps_DDRM.setIdentity(A);
            return A;
        }

        public static DMatrixRMaj ensureZeros( DMatrixRMaj A, int numRows, int numCols)
        {
            if (A == null)
            {
                return new DMatrixRMaj(numRows, numCols);
            }
            A.reshape(numRows, numCols);
            A.zero();
            return A;
        }

        /**
         * Creates a zeros matrix only if A does not already exist.  If it does exist it will fill
         * the lower triangular portion with zeros.
         */
        public static DMatrixRMaj checkZerosLT( DMatrixRMaj A, int numRows, int numCols)
        {
            if (A == null)
            {
                return new DMatrixRMaj(numRows, numCols);
            }
            else if (numRows != A.numRows || numCols != A.numCols)
            {
                A.reshape(numRows, numCols);
                A.zero();
            }
            else
            {
                for (int i = 0; i < A.numRows; i++)
                {
                    int index = i * A.numCols;
                    int end = index + Math.Min(i, A.numCols);
                    while (index < end)
                    {
                        A.data[index++] = 0;
                    }
                }
            }
            return A;
        }

        /**
         * Creates a zeros matrix only if A does not already exist.  If it does exist it will fill
         * the upper triangular portion with zeros.
         */
        public static DMatrixRMaj checkZerosUT( DMatrixRMaj A, int numRows, int numCols)
        {
            if (A == null)
            {
                return new DMatrixRMaj(numRows, numCols);
            }
            else if (numRows != A.numRows || numCols != A.numCols)
                throw new ArgumentException("Input is not " + numRows + " x " + numCols + " matrix");
            else
            {
                int maxRows = Math.Min(A.numRows, A.numCols);
                for (int i = 0; i < maxRows; i++)
                {
                    int index = i * A.numCols + i + 1;
                    int end = i * A.numCols + A.numCols;
                    while (index < end)
                    {
                        A.data[index++] = 0;
                    }
                }
            }
            return A;
        }
    }
}