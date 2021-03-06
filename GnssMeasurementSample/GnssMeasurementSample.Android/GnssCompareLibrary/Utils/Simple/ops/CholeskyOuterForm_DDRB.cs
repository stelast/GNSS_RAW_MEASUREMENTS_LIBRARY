﻿using Android.App;
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
    public class CholeskyOuterForm_DDRB : CholeskyDecomposition_F64<DMatrixRBlock>
    {
        // if it should compute an upper or lower triangular matrix
        private bool lower;
        // The decomposed matrix.
        private DMatrixRBlock T;

        // predeclare local work space
        private DSubmatrixD1 subA = new DSubmatrixD1();
        private DSubmatrixD1 subB = new DSubmatrixD1();
        private DSubmatrixD1 subC = new DSubmatrixD1();

        // storage for the determinant
        private Complex_F64 det = new Complex_F64();

        /**
         * Creates a new BlockCholeskyOuterForm
         *
         * @param lower Should it decompose it into a lower triangular matrix or not.
         */
        public CholeskyOuterForm_DDRB(bool lower)
        {
            this.lower = lower;
        }

        public Complex_F64 computeDeterminant()
        {
            double prod = 1.0;

            int blockLength = T.blockLength;
            for (int i = 0; i < T.numCols; i += blockLength)
            {
                // width of the submatrix
                int widthA = Math.Min(blockLength, T.numCols - i);

                // index of the first element in the block
                int indexT = i * T.numCols + i * widthA;

                // product along the diagonal
                for (int j = 0; j < widthA; j++)
                {
                    prod *= T.data[indexT];
                    indexT += widthA + 1;
                }
            }

            det.real = prod * prod;
            det.imaginary = 0;

            return det;
        }

        public bool decompose(DMatrixRBlock A)
        {
            if (A.numCols != A.numRows)
                throw new ArgumentException("A must be square");

            this.T = A;

            if (lower)
                return decomposeLower();
            else
                return decomposeUpper();
        }

        public DMatrixRBlock getT(DMatrixRBlock T)
        {
            if (T == null)
                return this.T;
            T.set(this.T);

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

        private bool decomposeLower()
        {
            int blockLength = T.blockLength;

            subA.set(T);
            subB.set(T);
            subC.set(T);

            for (int i = 0; i < T.numCols; i += blockLength)
            {
                int widthA = Math.Min(blockLength, T.numCols - i);

                //@formatter:off
                subA.col0 = i; subA.col1 = i + widthA;
                subA.row0 = subA.col0; subA.row1 = subA.col1;

                subB.col0 = i; subB.col1 = i + widthA;
                subB.row0 = i + widthA; subB.row1 = T.numRows;

                subC.col0 = i + widthA; subC.col1 = T.numRows;
                subC.row0 = i + widthA; subC.row1 = T.numRows;
                //@formatter:on

                // cholesky on inner block A
                if (!InnerCholesky_DDRB.lower(subA))
                    return false;

                // on the last block these operations are not needed.
                if (widthA == blockLength)
                {
                    // B = L^-1 B
                    TriangularSolver_DDRB.solveBlock(blockLength, false, subA, subB, false, true);

                    // C = C - B * B^T
                    InnerRankUpdate_DDRB.symmRankNMinus_L(blockLength, subC, subB);
                }
            }

            MatrixOps_DDRB.zeroTriangle(true, T);

            return true;
        }

        private bool decomposeUpper()
        {
            int blockLength = T.blockLength;

            subA.set(T);
            subB.set(T);
            subC.set(T);

            for (int i = 0; i < T.numCols; i += blockLength)
            {
                int widthA = Math.Min(blockLength, T.numCols - i);

                //@formatter:off
                subA.col0 = i; subA.col1 = i + widthA;
                subA.row0 = subA.col0; subA.row1 = subA.col1;

                subB.col0 = i + widthA; subB.col1 = T.numCols;
                subB.row0 = i; subB.row1 = i + widthA;

                subC.col0 = i + widthA; subC.col1 = T.numCols;
                subC.row0 = i + widthA; subC.row1 = T.numCols;
                //@formatter:on

                // cholesky on inner block A
                if (!InnerCholesky_DDRB.upper(subA))
                    return false;

                // on the last block these operations are not needed.
                if (widthA == blockLength)
                {
                    // B = U^-1 B
                    TriangularSolver_DDRB.solveBlock(blockLength, true, subA, subB, true, false);

                    // C = C - B^T * B
                    InnerRankUpdate_DDRB.symmRankNMinus_U(blockLength, subC, subB);
                }
            }

            MatrixOps_DDRB.zeroTriangle(false, T);

            return true;
        }
    }
}