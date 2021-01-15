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
    public class QRDecompositionHouseholder_DDRB : QRDecomposition<DMatrixRBlock>
    {
        // the input matrix which is overwritten with the decomposition.
        // Reflectors are stored in the lower triangular portion. The R matrix is stored
        // in the upper triangle portion
        private DMatrixRBlock dataA;

        // where the computed W matrix is stored
        private DMatrixRBlock dataW = new DMatrixRBlock(1, 1);
        // Matrix used to store an intermediate calculation
        private DMatrixRBlock dataWTA = new DMatrixRBlock(1, 1);

        // size of the inner matrix block.
        private int blockLength;

        // The submatrices which are being manipulated in each iteration
        private DSubmatrixD1 A = new DSubmatrixD1();
        private DSubmatrixD1 Y = new DSubmatrixD1();
        private DSubmatrixD1 W;
        private DSubmatrixD1 WTA;
        //private GrowArray<DGrowArray> workspace = new GrowArray<>(DGrowArray::new);
        private GrowArray<DGrowArray> workspace = null;
        // stores the computed gammas
        private double[] gammas = new double[1];

        // save the W matrix the first time it is computed in the decomposition
        private bool saveW = false;

        public QRDecompositionHouseholder_DDRB()
        {
            W = new DSubmatrixD1(dataW);
            WTA = new DSubmatrixD1(dataWTA);
        }

        /**
         * This is the input matrix after it has been overwritten with the decomposition.
         *
         * @return Internal matrix used to store decomposition.
         */
        public DMatrixRBlock getQR()
        {
            return dataA;
        }

        /**
         * <p>
         * Sets if it should internally save the W matrix before performing the decomposition. Must
         * be set before decomposition the matrix.
         * </p>
         *
         * <p>
         * Saving W can result in about a 5% savings when solving systems around a height of 5k. The
         * price is that it needs to save a matrix the size of the input matrix.
         * </p>
         *
         * @param saveW If the W matrix should be saved or not.
         */
        public void setSaveW(bool saveW)
        {
            this.saveW = saveW;
        }

        /**
         * Adjust submatrices and helper data structures for the input matrix. Must be called
         * before the decomposition can be computed.
         */
        private void setup(DMatrixRBlock orig)
        {
            blockLength = orig.blockLength;
            dataW.blockLength = blockLength;
            dataWTA.blockLength = blockLength;

            this.dataA = orig;
            A.original = dataA;

            int l = Math.Min(blockLength, orig.numCols);
            dataW.reshape(orig.numRows, l, false);
            dataWTA.reshape(l, orig.numRows, false);
            Y.original = orig;
            Y.row1 = W.row1 = orig.numRows;
            if (gammas.Count() < orig.numCols)
                gammas = new double[orig.numCols];

            if (saveW)
            {
                dataW.reshape(orig.numRows, orig.numCols, false);
            }
        }

        /**
         * <p>
         * A = (I + W Y<sup>T</sup>)<sup>T</sup>A<BR>
         * A = A + Y (W<sup>T</sup>A)<BR>
         * <br>
         * where A is a submatrix of the input matrix.
         * </p>
         */
        /*
        protected void updateA(DSubmatrixD1 A)
        {
            setW();

            A.row0 = Y.row0;
            A.row1 = Y.row1;
            A.col0 = Y.col1;
            A.col1 = Y.original.numCols;

            WTA.row0 = 0;
            WTA.col0 = 0;
            WTA.row1 = W.col1 - W.col0;
            WTA.col1 = A.col1 - A.col0;
            WTA.original.reshape(WTA.row1, WTA.col1, false);

            if (A.col1 > A.col0)
            {
                BlockHouseHolder_DDRB.computeW_Column(blockLength, Y, W, workspace, gammas, Y.col0);

                MatrixMult_DDRB.multTransA(blockLength, W, A, WTA);
                BlockHouseHolder_DDRB.multAdd_zeros(blockLength, Y, WTA, A);
            }
            else if (saveW)
            {
                BlockHouseHolder_DDRB.computeW_Column(blockLength, Y, W, workspace, gammas, Y.col0);
            }
        }*/

        /**
         * Sets the submatrix of W up give Y is already configured and if it is being cached or not.
         */
        private void setW()
        {
            if (saveW)
            {
                W.col0 = Y.col0;
                W.col1 = Y.col1;
                W.row0 = Y.row0;
                W.row1 = Y.row1;
            }
            else
            {
                W.col1 = Y.col1 - Y.col0;
                W.row0 = Y.row0;
            }
        }

        /**
         * Sanity checks the input or declares a new matrix. Return matrix is an identity matrix.
         */
        public static DMatrixRBlock initializeQ( DMatrixRBlock Q,
                                                 int numRows, int numCols, int blockLength,
                                                 bool compact)
        {
            int minLength = Math.Min(numRows, numCols);
            if (compact)
            {
                if (Q == null)
                {
                    Q = new DMatrixRBlock(numRows, minLength, blockLength);
                    MatrixOps_DDRB.setIdentity(Q);
                }
                else
                {
                    if (Q.numRows != numRows || Q.numCols != minLength)
                    {
                        throw new ArgumentException("Unexpected matrix dimension. Found " + Q.numRows + " " + Q.numCols);
                    }
                    else
                    {
                        MatrixOps_DDRB.setIdentity(Q);
                    }
                }
            }
            else
            {
                if (Q == null)
                {
                    Q = new DMatrixRBlock(numRows, numRows, blockLength);
                    MatrixOps_DDRB.setIdentity(Q);
                }
                else
                {
                    if (Q.numRows != numRows || Q.numCols != numRows)
                    {
                        throw new ArgumentException("Unexpected matrix dimension. Found " + Q.numRows + " " + Q.numCols);
                    }
                    else
                    {
                        MatrixOps_DDRB.setIdentity(Q);
                    }
                }
            }
            return Q;
        }

        /**
         * <p>
         * Multiplies the provided matrix by Q using householder reflectors. This is more
         * efficient that computing Q then applying it to the matrix.
         * </p>
         *
         * <p>
         * B = Q * B
         * </p>
         *
         * @param B Matrix which Q is applied to. Modified.
         */
        /*
        public void applyQ(DMatrixRBlock B)
        {
            applyQ(B, false);
        }
        */
        /**
         * Specialized version of applyQ() that allows the zeros in an identity matrix
         * to be taken advantage of depending on if isIdentity is true or not.
         *
         * @param isIdentity If B is an identity matrix.
         */
        /*
        public void applyQ(DMatrixRBlock B, bool isIdentity)
        {
            int minDimen = Math.Min(dataA.numCols, dataA.numRows);

            DSubmatrixD1 subB = new DSubmatrixD1(B);

            W.col0 = W.row0 = 0;
            Y.row1 = W.row1 = dataA.numRows;
            WTA.row0 = WTA.col0 = 0;

            int start = minDimen - minDimen % blockLength;
            if (start == minDimen)
                start -= blockLength;
            if (start < 0)
                start = 0;

            // (Q1^T * (Q2^T * (Q3^t * A)))
            for (int i = start; i >= 0; i -= blockLength)
            {

                Y.col0 = i;
                Y.col1 = Math.Min(Y.col0 + blockLength, dataA.numCols);
                Y.row0 = i;
                if (isIdentity)
                    subB.col0 = i;
                subB.row0 = i;

                setW();
                WTA.row1 = Y.col1 - Y.col0;
                WTA.col1 = subB.col1 - subB.col0;
                WTA.original.reshape(WTA.row1, WTA.col1, false);

                // Compute W matrix from reflectors stored in Y
                if (!saveW)
                    BlockHouseHolder_DDRB.computeW_Column(blockLength, Y, W, workspace, gammas, Y.col0);

                // Apply the Qi to Q
                BlockHouseHolder_DDRB.multTransA_vecCol(blockLength, Y, subB, WTA);
                MatrixMult_DDRB.multPlus(blockLength, W, WTA, subB);
            }
        }*/

        /**
         * <p>
         * Multiplies the provided matrix by Q<sup>T</sup> using householder reflectors. This is more
         * efficient that computing Q then applying it to the matrix.
         * </p>
         *
         * <p>
         * Q = Q*(I - &gamma; W*Y^T)<br>
         * QR = A &ge; R = Q^T*A  = (Q3^T * (Q2^T * (Q1^t * A)))
         * </p>
         *
         * @param B Matrix which Q is applied to. Modified.
         */
        /*
        public void applyQTran(DMatrixRBlock B)
        {
            int minDimen = Math.Min(dataA.numCols, dataA.numRows);

            DSubmatrixD1 subB = new DSubmatrixD1(B);

            W.col0 = W.row0 = 0;
            Y.row1 = W.row1 = dataA.numRows;
            WTA.row0 = WTA.col0 = 0;

            // (Q3^T * (Q2^T * (Q1^t * A)))
            for (int i = 0; i < minDimen; i += blockLength)
            {

                Y.col0 = i;
                Y.col1 = Math.Min(Y.col0 + blockLength, dataA.numCols);
                Y.row0 = i;

                subB.row0 = i;
                //            subB.row1 = B.numRows;
                //            subB.col0 = 0;
                //            subB.col1 = B.numCols;

                setW();
                //            W.original.reshape(W.row1,W.col1,false);
                WTA.row0 = 0;
                WTA.col0 = 0;
                WTA.row1 = W.col1 - W.col0;
                WTA.col1 = subB.col1 - subB.col0;
                WTA.original.reshape(WTA.row1, WTA.col1, false);

                // Compute W matrix from reflectors stored in Y
                if (!saveW)
                    BlockHouseHolder_DDRB.computeW_Column(blockLength, Y, W, workspace, gammas, Y.col0);

                // Apply the Qi to Q
                MatrixMult_DDRB.multTransA(blockLength, W, subB, WTA);
                BlockHouseHolder_DDRB.multAdd_zeros(blockLength, Y, WTA, subB);
            }
        }*/

        /**
         * The input matrix is always modified.
         *
         * @return Returns true since the input matrix is modified.
         */
        public bool inputModified()
        {
            return true;
        }/*
        public bool decompose(DMatrixRBlock orig)
        {
            setup(orig);

            int m = Math.Min(orig.numCols, orig.numRows);

            // process the matrix one column block at a time and overwrite the input matrix
            for (int j = 0; j < m; j += blockLength)
            {
                Y.col0 = j;
                Y.col1 = Math.Min(orig.numCols, Y.col0 + blockLength);
                Y.row0 = j;

                // compute the QR decomposition of the left most block column
                // this overwrites the original input matrix
                if (!BlockHouseHolder_DDRB.decomposeQR_block_col(blockLength, Y, gammas))
                {
                    return false;
                }

                // Update the remainder of the matrix using the reflectors just computed
                updateA(A);
            }

            return true;
        }*/
        public DMatrixRBlock getQ(DMatrixRBlock Q, bool compact)
        {
            throw new NotImplementedException();
        }

        public DMatrixRBlock getR(DMatrixRBlock R, bool compact)
        {
            throw new NotImplementedException();
        }

        public bool decompose(DMatrixRBlock orig)
        {
            throw new NotImplementedException();
        }
    }
}