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
    public class TriangularSolver_DDRB
    {
        /**
         * Inverts an upper or lower triangular block submatrix. Uses a row-oriented approach.
         *
         * @param upper Is it upper or lower triangular.
         * @param T Triangular matrix that is to be inverted. Must be block aligned. Not Modified.
         * @param T_inv Where the inverse is stored. This can be the same as T. Modified.
         * @param workspace Work space variable that is size blockLength*blockLength.
         */
        public static void invert(int blockLength,
                                   bool upper,
                                   DSubmatrixD1 T,
                                   DSubmatrixD1 T_inv,
                                    GrowArray<DGrowArray> workspace )
        {
            if (upper)
                throw new ArgumentException("Upper triangular matrices not supported yet");

            //CONCURRENT_INLINE if (T.original == T_inv.original)
            //CONCURRENT_INLINE    throw new ArgumentException("Same instance not allowed for concurrent");

            //if (workspace == null)
            //    workspace = new GrowArray<DGrowArray>(new DGrowArray());
            //else
            //    workspace.reset();

            if (T.row0 != T_inv.row0 || T.row1 != T_inv.row1 || T.col0 != T_inv.col0 || T.col1 != T_inv.col1)
                throw new ArgumentException("T and T_inv must be at the same elements in the matrix");

            int blockSize = blockLength * blockLength;
            //CONCURRENT_REMOVE_BELOW
            double[] temp = workspace.grow().reshape(blockSize).data;

            int M = T.row1 - T.row0;

            double[] dataT = T.original.data;
            double[] dataX = T_inv.original.data;

            int offsetT = T.row0 * T.original.numCols + M * T.col0;

            for (int rowT = 0; rowT < M; rowT += blockLength)
            {
                int _rowT = rowT; // Needed for concurrent lambdas
                int heightT = Math.Min(T.row1 - (rowT + T.row0), blockLength);

                int indexII = offsetT + T.original.numCols * (rowT + T.row0) + heightT * (rowT + T.col0);

                //CONCURRENT_BELOW EjmlConcurrency.loopFor(0, rowT, blockLength, workspace, ( work, colT ) -> {
                for (int colT = 0; colT < rowT; colT += blockLength)
                {
                    //CONCURRENT_INLINE double[] temp = work.reshape(blockSize).data;
                    int widthX = Math.Min(T.col1 - (colT + T.col0), blockLength);

                    Array.Fill(temp, 0);

                    for (int k = colT; k < _rowT; k += blockLength)
                    {
                        int widthT = Math.Min(T.col1 - (k + T.col0), blockLength);

                        int indexL2 = offsetT + T.original.numCols * (_rowT + T.row0) + heightT * (k + T.col0);
                        int indexX2 = offsetT + T.original.numCols * (k + T.row0) + widthT * (colT + T.col0);

                        InnerMultiplication_DDRB.blockMultMinus(dataT, dataX, temp, indexL2, indexX2, 0, heightT, widthT, widthX);
                    }

                    int indexX = offsetT + T.original.numCols * (_rowT + T.row0) + heightT * (colT + T.col0);

                    InnerTriangularSolver_DDRB.solveL(dataT, temp, heightT, widthX, heightT, indexII, 0);
                    System.Array.Copy(temp, 0, dataX, indexX, widthX * heightT);
                }
                //CONCURRENT_ABOVE });
                InnerTriangularSolver_DDRB.invertLower(dataT, dataX, heightT, indexII, indexII);
            }
        }

        //CONCURRENT_OMIT_BEGIN

        /**
         * Inverts an upper or lower triangular block submatrix. Uses a row oriented approach.
         *
         * @param upper Is it upper or lower triangular.
         * @param T Triangular matrix that is to be inverted. Overwritten with solution. Modified.
         * @param workspace Work space variable that is size blockLength*blockLength.
         */
        public static void invert(int blockLength,
                                   bool upper,
                                   DSubmatrixD1 T,
                                    GrowArray<DGrowArray> workspace )
        {
            if (upper)
                throw new ArgumentException("Upper triangular matrices not supported yet");

            // FORCE to disable
            workspace = null;

            if (workspace == null)
            {
                //workspace = new GrowArray<>(DGrowArray::new);
            }
            else
                workspace.reset();

            int blockSize = blockLength * blockLength;
            double[] temp = workspace.grow().reshape(blockSize).data;

            int M = T.row1 - T.row0;

            double[] dataT = T.original.data;
            int offsetT = T.row0 * T.original.numCols + M * T.col0;

            for (int i = 0; i < M; i += blockLength)
            {
                int heightT = Math.Min(T.row1 - (i + T.row0), blockLength);

                int indexII = offsetT + T.original.numCols * (i + T.row0) + heightT * (i + T.col0);

                for (int j = 0; j < i; j += blockLength)
                {
                    int widthX = Math.Min(T.col1 - (j + T.col0), blockLength);

                    Array.Fill(temp, 0);

                    for (int k = j; k < i; k += blockLength)
                    {
                        int widthT = Math.Min(T.col1 - (k + T.col0), blockLength);

                        int indexL2 = offsetT + T.original.numCols * (i + T.row0) + heightT * (k + T.col0);
                        int indexX2 = offsetT + T.original.numCols * (k + T.row0) + widthT * (j + T.col0);

                        InnerMultiplication_DDRB.blockMultMinus(dataT, dataT, temp, indexL2, indexX2, 0, heightT, widthT, widthX);
                    }

                    int indexX = offsetT + T.original.numCols * (i + T.row0) + heightT * (j + T.col0);

                    InnerTriangularSolver_DDRB.solveL(dataT, temp, heightT, widthX, heightT, indexII, 0);
                    System.Array.Copy(temp, 0, dataT, indexX, widthX * heightT);
                }
                InnerTriangularSolver_DDRB.invertLower(dataT, heightT, indexII);
            }
        }
        //CONCURRENT_OMIT_END

        /**
         * <p>
         * Performs an in-place solve operation on the provided block aligned sub-matrices.<br>
         * <br>
         * B = T<sup>-1</sup> B<br>
         * <br>
         * where T is a triangular matrix. T or B can be transposed. T is a square matrix of arbitrary
         * size and B has the same number of rows as T and an arbitrary number of columns.
         * </p>
         *
         * @param blockLength Size of the inner blocks.
         * @param upper If T is upper or lower triangular.
         * @param T An upper or lower triangular matrix. Not modified.
         * @param B A matrix whose height is the same as T's width. Solution is written here. Modified.
         */
        public static void solve(int blockLength,
                                  bool upper,
                                  DSubmatrixD1 T,
                                  DSubmatrixD1 B,
                                  bool transT )
        {

            if (upper)
            {
                solveR(blockLength, T, B, transT);
            }
            else
            {
                solveL(blockLength, T, B, transT);
            }
        }

        /**
         * <p>
         * Performs an in-place solve operation where T is contained in a single block.<br>
         * <br>
         * B = T<sup>-1</sup> B<br>
         * <br>
         * where T is a triangular matrix contained in an inner block. T or B can be transposed. T must be a single complete inner block
         * and B is either a column block vector or row block vector.
         * </p>
         *
         * @param blockLength Size of the inner blocks in the block matrix.
         * @param upper If T is upper or lower triangular.
         * @param T An upper or lower triangular matrix that is contained in an inner block. Not modified.
         * @param B A block aligned row or column submatrix. Modified.
         * @param transT If T is transposed or not.
         * @param transB If B is transposed or not.
         */
        public static void solveBlock(int blockLength,
                                       bool upper, DSubmatrixD1 T,
                                       DSubmatrixD1 B,
                                       bool transT, bool transB )
        {
            int Trows = T.row1 - T.row0;
            if (Trows > blockLength)
                throw new ArgumentException("T can be at most the size of a block");
            // number of rows in a block. The submatrix can be smaller than a block
            int blockT_rows = Math.Min(blockLength, T.original.numRows - T.row0);
            int blockT_cols = Math.Min(blockLength, T.original.numCols - T.col0);

            int offsetT = T.row0 * T.original.numCols + blockT_rows * T.col0;

            double[] dataT = T.original.data;
            double[] dataB = B.original.data;

            if (transB)
            {
                if (upper)
                {
                    if (transT)
                    {
                        throw new ArgumentException("Operation not yet supported");
                    }
                    else
                    {
                        throw new ArgumentException("Operation not yet supported");
                    }
                }
                else
                {
                    if (transT)
                    {
                        throw new ArgumentException("Operation not yet supported");
                    }
                    else
                    {
                        //CONCURRENT_BELOW EjmlConcurrency.loopFor(B.row0, B.row1, blockLength, i -> {
                        for (int i = B.row0; i < B.row1; i += blockLength)
                        {
                            int N = Math.Min(B.row1, i + blockLength) - i;

                            int offsetB = i * B.original.numCols + N * B.col0;

                            InnerTriangularSolver_DDRB.solveLTransB(dataT, dataB, blockT_rows, N, blockT_rows, offsetT, offsetB);
                        }
                        //CONCURRENT_ABOVE });
                    }
                }
            }
            else
            {
                if (Trows != B.row1 - B.row0)
                    throw new ArgumentException("T and B must have the same number of rows.");

                if (upper)
                {
                    if (transT)
                    {
                        //CONCURRENT_BELOW EjmlConcurrency.loopFor(B.col0, B.col1, blockLength, i -> {
                        for (int i = B.col0; i < B.col1; i += blockLength)
                        {
                            int offsetB = B.row0 * B.original.numCols + Trows * i;

                            int N = Math.Min(B.col1, i + blockLength) - i;
                            InnerTriangularSolver_DDRB.solveTransU(dataT, dataB, Trows, N, Trows, offsetT, offsetB);
                        }
                        //CONCURRENT_ABOVE });
                    }
                    else
                    {
                        //CONCURRENT_BELOW EjmlConcurrency.loopFor(B.col0, B.col1, blockLength, i -> {
                        for (int i = B.col0; i < B.col1; i += blockLength)
                        {
                            int offsetB = B.row0 * B.original.numCols + Trows * i;

                            int N = Math.Min(B.col1, i + blockLength) - i;
                            InnerTriangularSolver_DDRB.solveU(dataT, dataB, Trows, N, Trows, offsetT, offsetB);
                        }
                        //CONCURRENT_ABOVE });
                    }
                }
                else
                {
                    if (transT)
                    {
                        //CONCURRENT_BELOW EjmlConcurrency.loopFor(B.col0, B.col1, blockLength, i -> {
                        for (int i = B.col0; i < B.col1; i += blockLength)
                        {
                            int offsetB = B.row0 * B.original.numCols + Trows * i;

                            int N = Math.Min(B.col1, i + blockLength) - i;
                            InnerTriangularSolver_DDRB.solveTransL(dataT, dataB, Trows, N, blockT_cols, offsetT, offsetB);
                        }
                        //CONCURRENT_ABOVE });
                    }
                    else
                    {
                        //CONCURRENT_BELOW EjmlConcurrency.loopFor(B.col0, B.col1, blockLength, i -> {
                        for (int i = B.col0; i < B.col1; i += blockLength)
                        {
                            int offsetB = B.row0 * B.original.numCols + Trows * i;

                            int N = Math.Min(B.col1, i + blockLength) - i;
                            InnerTriangularSolver_DDRB.solveL(dataT, dataB, Trows, N, blockT_cols, offsetT, offsetB);
                        }
                        //CONCURRENT_ABOVE });
                    }
                }
            }
        }

        /**
         * <p>
         * Solves lower triangular systems:<br>
         * <br>
         * B = L<sup>-1</sup> B<br>
         * <br>
         * </p>
         *
         * <p> Reverse or forward substitution is used depending upon L being transposed or not. </p>
         *
         * @param L Lower triangular with dimensions m by m. Not modified.
         * @param B A matrix with dimensions m by n. Solution is written into here. Modified.
         * @param transL Is the triangular matrix transposed?
         */
        public static void solveL(int blockLength,
                                   DSubmatrixD1 L,
                                   DSubmatrixD1 B,
                                   bool transL)
        {

            DSubmatrixD1 Y = new DSubmatrixD1(B.original);

            DSubmatrixD1 Linner = new DSubmatrixD1(L.original);
            DSubmatrixD1 Binner = new DSubmatrixD1(B.original);

            int lengthL = B.row1 - B.row0;

            int startI, stepI;

            if (transL)
            {
                startI = lengthL - lengthL % blockLength;
                if (startI == lengthL && lengthL >= blockLength)
                    startI -= blockLength;

                stepI = -blockLength;
            }
            else
            {
                startI = 0;
                stepI = blockLength;
            }

            for (int i = startI; ; i += stepI)
            {
                if (transL)
                {
                    if (i < 0) break;
                }
                else
                {
                    if (i >= lengthL) break;
                }

                // width and height of the inner T(i,i) block
                int widthT = Math.Min(blockLength, lengthL - i);

                Linner.col0 = L.col0 + i;
                Linner.col1 = Linner.col0 + widthT;
                Linner.row0 = L.row0 + i;
                Linner.row1 = Linner.row0 + widthT;

                Binner.col0 = B.col0;
                Binner.col1 = B.col1;
                Binner.row0 = B.row0 + i;
                Binner.row1 = Binner.row0 + widthT;

                // solve the top row block
                // B(i,:) = T(i,i)^-1 Y(i,:)
                solveBlock(blockLength, false, Linner, Binner, transL, false);

                bool updateY;
                if (transL)
                {
                    updateY = Linner.row0 > 0;
                }
                else
                {
                    updateY = Linner.row1 < L.row1;
                }
                if (updateY)
                {
                    // Y[i,:] = Y[i,:] - sum j=1:i-1 { T[i,j] B[j,i] }
                    // where i is the next block down
                    // The summation is a block inner product
                    if (transL)
                    {
                        Linner.col1 = Linner.col0;
                        Linner.col0 = Linner.col1 - blockLength;
                        Linner.row1 = L.row1;
                        //Tinner.col1 = Tinner.col1;

                        //                    Binner.row0 = Binner.row0;
                        Binner.row1 = B.row1;

                        Y.row0 = Binner.row0 - blockLength;
                        Y.row1 = Binner.row0;
                    }
                    else
                    {
                        Linner.row0 = Linner.row1;
                        Linner.row1 = Math.Min(Linner.row0 + blockLength, L.row1);
                        Linner.col0 = L.col0;
                        //Tinner.col1 = Tinner.col1;

                        Binner.row0 = B.row0;
                        //Binner.row1 = Binner.row1;

                        Y.row0 = Binner.row1;
                        Y.row1 = Math.Min(Y.row0 + blockLength, B.row1);
                    }

                    // step through each block column
                    for (int k = B.col0; k < B.col1; k += blockLength)
                    {

                        Binner.col0 = k;
                        Binner.col1 = Math.Min(k + blockLength, B.col1);

                        Y.col0 = Binner.col0;
                        Y.col1 = Binner.col1;

                        if (transL)
                        {
                            // Y = Y - T^T * B
                            MatrixMult_DDRB.multMinusTransA(blockLength, Linner, Binner, Y);
                        }
                        else
                        {

                            // Y = Y - T * B
                            MatrixMult_DDRB.multMinus(blockLength, Linner, Binner, Y);
                        }
                    }
                }
            }
        }

        /**
         * <p>
         * Solves upper triangular systems:<br>
         * <br>
         * B = R<sup>-1</sup> B<br>
         * <br>
         * </p>
         *
         * <p>Only the first B.numRows rows in R will be processed. Lower triangular elements are ignored.<p>
         *
         * <p> Reverse or forward substitution is used depending upon L being transposed or not. </p>
         *
         * @param R Upper triangular with dimensions m by m. Not modified.
         * @param B A matrix with dimensions m by n. Solution is written into here. Modified.
         * @param transR Is the triangular matrix transposed?
         */
        public static void solveR(int blockLength,
                                   DSubmatrixD1 R,
                                   DSubmatrixD1 B,
                                   bool transR)
        {

            int lengthR = B.row1 - B.row0;
            if (R.Cols != lengthR)
            {
                throw new ArgumentException("Number of columns in R must be equal to the number of rows in B");
            }
            else if (R.Rows != lengthR)
            {
                throw new ArgumentException("Number of rows in R must be equal to the number of rows in B");
            }

            DSubmatrixD1 Y = new DSubmatrixD1(B.original);

            DSubmatrixD1 Rinner = new DSubmatrixD1(R.original);
            DSubmatrixD1 Binner = new DSubmatrixD1(B.original);

            int startI, stepI;

            if (transR)
            {
                startI = 0;
                stepI = blockLength;
            }
            else
            {
                startI = lengthR - lengthR % blockLength;
                if (startI == lengthR && lengthR >= blockLength)
                    startI -= blockLength;

                stepI = -blockLength;
            }

            for (int i = startI; ; i += stepI)
            {
                if (transR)
                {
                    if (i >= lengthR) break;
                }
                else
                {
                    if (i < 0) break;
                }

                // width and height of the inner T(i,i) block
                int widthT = Math.Min(blockLength, lengthR - i);

                Rinner.col0 = R.col0 + i;
                Rinner.col1 = Rinner.col0 + widthT;
                Rinner.row0 = R.row0 + i;
                Rinner.row1 = Rinner.row0 + widthT;

                Binner.col0 = B.col0;
                Binner.col1 = B.col1;
                Binner.row0 = B.row0 + i;
                Binner.row1 = Binner.row0 + widthT;

                // solve the top row block
                // B(i,:) = T(i,i)^-1 Y(i,:)
                solveBlock(blockLength, true, Rinner, Binner, transR, false);

                bool updateY;
                if (transR)
                {
                    updateY = Rinner.row1 < R.row1;
                }
                else
                {
                    updateY = Rinner.row0 > 0;
                }
                if (updateY)
                {
                    // Y[i,:] = Y[i,:] - sum j=1:i-1 { T[i,j] B[j,i] }
                    // where i is the next block down
                    // The summation is a block inner product
                    if (transR)
                    {
                        Rinner.col0 = Rinner.col1;
                        Rinner.col1 = Math.Min(Rinner.col0 + blockLength, R.col1);
                        Rinner.row0 = R.row0;
                        //Rinner.row1 = Rinner.row1;

                        Binner.row0 = B.row0;
                        //Binner.row1 = Binner.row1;

                        Y.row0 = Binner.row1;
                        Y.row1 = Math.Min(Y.row0 + blockLength, B.row1);
                    }
                    else
                    {
                        Rinner.row1 = Rinner.row0;
                        Rinner.row0 = Rinner.row1 - blockLength;
                        Rinner.col1 = R.col1;

                        //                    Binner.row0 = Binner.row0;
                        Binner.row1 = B.row1;

                        Y.row0 = Binner.row0 - blockLength;
                        Y.row1 = Binner.row0;
                    }

                    // step through each block column
                    for (int k = B.col0; k < B.col1; k += blockLength)
                    {

                        Binner.col0 = k;
                        Binner.col1 = Math.Min(k + blockLength, B.col1);

                        Y.col0 = Binner.col0;
                        Y.col1 = Binner.col1;

                        if (transR)
                        {
                            // Y = Y - T^T * B
                            MatrixMult_DDRB.multMinusTransA(blockLength, Rinner, Binner, Y);
                        }
                        else
                        {
                            // Y = Y - T * B
                            MatrixMult_DDRB.multMinus(blockLength, Rinner, Binner, Y);
                        }
                    }
                }
            }
        }
    }
}