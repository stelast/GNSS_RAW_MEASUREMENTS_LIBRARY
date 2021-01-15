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
    public abstract class CholeskyDecompositionCommon_DDRM : CholeskyDecomposition_F64<DMatrixRMaj>
    {
        // it can decompose a matrix up to this width
        protected int maxWidth = -1;

        // width and height of the matrix
        protected int n;

        // the decomposed matrix
        protected DMatrixRMaj T;
        protected double[] t;

        // tempoary variable used by various functions
        protected double[] vv;

        // is it a lower triangular matrix or an upper triangular matrix
        protected bool lower;

        // storage for computed determinant
        protected Complex_F64 det = new Complex_F64();

        /**
         * Specifies if a lower or upper variant should be constructed.
         *
         * @param lower should a lower or upper triangular matrix be used.
         */
        protected CholeskyDecompositionCommon_DDRM(bool lower)
        {
            this.lower = lower;
        }

        public void setExpectedMaxSize(int numRows, int numCols)
        {
            if (numRows != numCols)
            {
                throw new ArgumentException("Can only decompose square matrices");
            }

            this.maxWidth = numCols;

            this.vv = new double[maxWidth];
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

        /**
         * Returns the triangular matrix from the decomposition.
         *
         * @return A lower or upper triangular matrix.
         */
        public DMatrixRMaj getT()
        {
            return T;
        }

        public double[] _getVV()
        {
            return vv;
        }

        public Complex_F64 computeDeterminant()
        {
            double prod = 1;

            int total = n * n;
            for (int i = 0; i < total; i += n + 1)
            {
                prod *= t[i];
            }

            det.real = prod * prod;
            det.imaginary = 0;

            return det;
        }

        public bool isLower()
        {
            throw new NotImplementedException();
        }

        public DMatrixRMaj getT(DMatrixRMaj T)
        {

            // write the values to T
            if (lower)
            {
                T = UtilDecompositons_DDRM.checkZerosUT(T, n, n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        T.unsafe_set(i, j, this.T.unsafe_get(i, j));
                    }
                }
            }
            else
            {
                T = UtilDecompositons_DDRM.checkZerosLT(T, n, n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = i; j < n; j++)
                    {
                        T.unsafe_set(i, j, this.T.unsafe_get(i, j));
                    }
                }
            }

            return T;
        }

        public bool decompose(DMatrixRMaj mat)
        {
            if (mat.numRows > maxWidth)
            {
                setExpectedMaxSize(mat.numRows, mat.numCols);
            }
            else if (mat.numRows != mat.numCols)
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

        public bool inputModified()
        {
            return true;
        }
    }
}