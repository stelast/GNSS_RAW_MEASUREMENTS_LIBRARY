using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public abstract class CholeskyDecompositionCommon_ZDRM : CholeskyDecomposition_F64<ZMatrixRMaj>
    {
        // width and height of the matrix
        protected int n;

        // the decomposed matrix
        protected ZMatrixRMaj T;
        protected double[] t;

        // is it a lower triangular matrix or an upper triangular matrix
        protected bool lower;

        // storage for the determinant
        protected Complex_F64 det = new Complex_F64();

        /**
         * Specifies if a lower or upper variant should be constructed.
         *
         * @param lower should a lower or upper triangular matrix be used.
         */
        protected CholeskyDecompositionCommon_ZDRM(bool lower)
        {
            this.lower = lower;
        }

        /**
         * Returns the raw decomposed matrix.
         *
         * @return A lower or upper triangular matrix.
         */
        public ZMatrixRMaj _getT()
        {
            return T;
        }


        public Complex_F64 computeDeterminant()
        {
            double prod = 1;

            // take advantage of the diagonal elements all being real
            int total = n * n * 2;
            for (int i = 0; i < total; i += 2 * (n + 1))
            {
                prod *= t[i];
            }

            det.real = prod * prod;
            det.imaginary = 0;

            return det;
        }

        public bool decompose(ZMatrixRMaj mat)
        {
            if (mat.numRows != mat.numCols)
            {
                throw new ArgumentException("Must be a square matrix.");
            }

            n = mat.numRows;

            T = mat;
            t = T.data;

            if (lower)
            {
                return decomposeLower();
            }
            else
            {
                return decomposeUpper();
            }
        }


        /**
         * Performs an lower triangular decomposition.
         *
         * @return true if the matrix was decomposed.
         */
        protected abstract bool decomposeLower();

        /**
         * Performs an upper triangular decomposition.
         *
         * @return true if the matrix was decomposed.
         */
        protected abstract bool decomposeUpper();

        ZMatrixRMaj CholeskyDecomposition<ZMatrixRMaj>.getT(ZMatrixRMaj T)
        {
            // write the values to T
            if (lower)
            {
                T = UtilDecompositons_ZDRM.checkZerosUT(T, n, n);
                for (int i = 0; i < n; i++)
                {
                    int index = i * n * 2;
                    for (int j = 0; j <= i; j++)
                    {
                        T.data[index] = this.T.data[index];
                        index++;
                        T.data[index] = this.T.data[index];
                        index++;
                    }
                }
            }
            else
            {
                T = UtilDecompositons_ZDRM.checkZerosLT(T, n, n);
                for (int i = 0; i < n; i++)
                {
                    int index = (i * n + i) * 2;
                    for (int j = i; j < n; j++)
                    {
                        T.data[index] = this.T.data[index];
                        index++;
                        T.data[index] = this.T.data[index];
                        index++;
                    }
                }
            }

            return T;
        }

        public bool inputModified()
        {
            return true;
        }

        public bool isLower()
        {
            return lower;
        }

        bool CholeskyDecomposition<ZMatrixRMaj>.isLower()
        {
            return lower;
        }

        bool DecompositionInterface<ZMatrixRMaj>.decompose(ZMatrixRMaj mat)
        {
            if (mat.numRows != mat.numCols)
            {
                throw new ArgumentException("Must be a square matrix.");
            }

            n = mat.numRows;

            T = mat;
            t = T.data;

            if (lower)
            {
                return decomposeLower();
            }
            else
            {
                return decomposeUpper();
            }
        }

        bool DecompositionInterface<ZMatrixRMaj>.inputModified()
        {
            return true;
        }
    }
}