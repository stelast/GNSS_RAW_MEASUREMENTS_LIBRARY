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
    public class MatrixOps_DDRB
    {
        //CONCURRENT_OMIT_BEGIN

        /**
         * Converts a row major matrix into a row major block matrix.
         *
         * @param src Original DMatrixRMaj. Not modified.
         * @param dst Equivalent DMatrixRBlock. Modified.
         */
        public static void convert(DMatrixRMaj src, DMatrixRBlock dst)
        {
            DConvertMatrixStruct.convert(src, dst);
        }

        /**
         * Converts a row major matrix into a row major block matrix. Both matrices will contain
         * the same data array. Useful when you wish to avoid declaring two large matrices.
         *
         * @param src Original DMatrixRMaj. Modified.
         * @param dst Equivalent DMatrixRBlock. Modified.
         */
        public static DMatrixRBlock convertInplace(DMatrixRMaj src,  DMatrixRBlock dst,
                                                     DGrowArray workspace )
        {
            if (dst == null)
                dst = new DMatrixRBlock();

            dst.data = src.data;
            dst.blockLength = EjmlParameters.BLOCK_WIDTH;
            dst.numRows = src.numRows;
            dst.numCols = src.numCols;
            convertRowToBlock(src.numRows, src.numCols, dst.blockLength, src.data, workspace);

            return dst;
        }

        /**
         * <p>
         * Converts matrix data stored is a row major format into a block row major format in place.
         * </p>
         *
         * @param numRows number of rows in the matrix.
         * @param numCols number of columns in the matrix.
         * @param blockLength Block size in the converted matrix.
         * @param data Matrix data in a row-major format. Modified.
         * @param workspace Optional internal workspace. Nullable.
         */
        public static void convertRowToBlock(int numRows, int numCols, int blockLength,
                                              double[] data,  DGrowArray workspace )
        {
            //int minLength = Math.Min(blockLength, numRows) * numCols;
            //double[] tmp = UtilEjml.adjust(workspace, minLength);

            //for (int i = 0; i < numRows; i += blockLength)
            //{
            //    int blockHeight = Math.Min(blockLength, numRows - i);

            //    System.Array.Copy(data, i * numCols, tmp, 0, blockHeight * numCols);


            //    for (int j = 0; j < numCols; j += blockLength)
            //    {
            //        int blockWidth = Math.Min(blockLength, numCols - j);

            //        int indexDst = i * numCols + blockHeight * j;
            //        int indexSrcRow = j;

            //        for (int k = 0; k < blockHeight; k++)
            //        {
            //            System.Array.Copy(tmp, indexSrcRow, data, indexDst, blockWidth);
            //            indexDst += blockWidth;
            //            indexSrcRow += numCols;
            //        }
            //    }
            //}
        }

        /**
         * Converts a row major block matrix into a row major matrix.
         *
         * @param src Original DMatrixRBlock.. Not modified.
         * @param dst Equivalent DMatrixRMaj. Modified.
         */
        public static DMatrixRMaj convert(DMatrixRBlock src, DMatrixRMaj dst)
        {
            return DConvertMatrixStruct.convert(src, dst);
        }

        /**
         * Converts a row major block matrix into a row major matrix. Both matrices will contain
         * the same data array. Useful when you wish to avoid declaring two large matrices.
         *
         * @param src Original DMatrixRBlock. Modified.
         * @param dst Equivalent DMatrixRMaj. Modified.
         */
        public static DMatrixRMaj convertInplace(DMatrixRBlock src,  DMatrixRMaj dst,
                                                   DGrowArray workspace )
        {
            if (dst == null)
                dst = new DMatrixRMaj();

            dst.data = src.data;
            dst.numRows = src.numRows;
            dst.numCols = src.numCols;
            convertBlockToRow(src.numRows, src.numCols, src.blockLength, src.data, workspace);

            return dst;
        }

        /**
         * <p>
         * Converts matrix data stored is a block row major format into a row major format in place.
         * </p>
         *
         * @param numRows number of rows in the matrix.
         * @param numCols number of columns in the matrix.
         * @param blockLength Block size in the converted matrix.
         * @param data Matrix data in a block row-major format. Modified.
         * @param workspace Optional internal workspace. Nullable.
         */
        public static void convertBlockToRow(int numRows, int numCols, int blockLength,
                                              double[] data,  DGrowArray workspace )
        {
            //int minLength = Math.Min(blockLength, numRows) * numCols;
            //double[] tmp = UtilEjml.adjust(workspace, minLength);

            //for (int i = 0; i < numRows; i += blockLength)
            //{
            //    int blockHeight = Math.Min(blockLength, numRows - i);

            //    System.Array.Copy(data, i * numCols, tmp, 0, blockHeight * numCols);

            //    for (int j = 0; j < numCols; j += blockLength)
            //    {
            //        int blockWidth = Math.Min(blockLength, numCols - j);

            //        int indexSrc = blockHeight * j;
            //        int indexDstRow = i * numCols + j;

            //        for (int k = 0; k < blockHeight; k++)
            //        {
            //            System.Array.Copy(tmp, indexSrc, data, indexDstRow, blockWidth);
            //            indexSrc += blockWidth;
            //            indexDstRow += numCols;
            //        }
            //    }
            //}
        }

        /**
         * Converts the transpose of a row major matrix into a row major block matrix.
         *
         * @param src Original DMatrixRMaj. Not modified.
         * @param dst Equivalent DMatrixRBlock. Modified.
         */
        public static void convertTranSrc(DMatrixRMaj src, DMatrixRBlock dst)
        {
            if (src.numRows != dst.numCols || src.numCols != dst.numRows)
                throw new ArgumentException("Incompatible matrix shapes.");

            for (int i = 0; i < dst.numRows; i += dst.blockLength)
            {
                int blockHeight = Math.Min(dst.blockLength, dst.numRows - i);

                for (int j = 0; j < dst.numCols; j += dst.blockLength)
                {
                    int blockWidth = Math.Min(dst.blockLength, dst.numCols - j);

                    int indexDst = i * dst.numCols + blockHeight * j;
                    int indexSrc = j * src.numCols + i;

                    for (int l = 0; l < blockWidth; l++)
                    {
                        int rowSrc = indexSrc + l * src.numCols;
                        int rowDst = indexDst + l;
                        for (int k = 0; k < blockHeight; k++, rowDst += blockWidth)
                        {
                            dst.data[rowDst] = src.data[rowSrc++];
                        }
                    }
                }
            }
        }
        //CONCURRENT_OMIT_END

        // This can be speed up by inlining the multBlock* calls, reducing number of multiplications
        // and other stuff. doesn't seem to have any speed advantage over mult_reorder()
        public static void mult(DMatrixRBlock A, DMatrixRBlock B, DMatrixRBlock C)
        {
            if (A.numCols != B.numRows)
                throw new ArgumentException("Columns in A are incompatible with rows in B");
            if (A.numRows != C.numRows)
                throw new ArgumentException("Rows in A are incompatible with rows in C");
            if (B.numCols != C.numCols)
                throw new ArgumentException("Columns in B are incompatible with columns in C");
            if (A.blockLength != B.blockLength || A.blockLength != C.blockLength)
                throw new ArgumentException("Block lengths are not all the same.");

            int blockLength = A.blockLength;

            DSubmatrixD1 Asub = new DSubmatrixD1(A, 0, A.numRows, 0, A.numCols);
            DSubmatrixD1 Bsub = new DSubmatrixD1(B, 0, B.numRows, 0, B.numCols);
            DSubmatrixD1 Csub = new DSubmatrixD1(C, 0, C.numRows, 0, C.numCols);

            MatrixMult_DDRB.mult(blockLength, Asub, Bsub, Csub);
        }

        public static void multTransA(DMatrixRBlock A, DMatrixRBlock B, DMatrixRBlock C)
        {
            if (A.numRows != B.numRows)
                throw new ArgumentException("Rows in A are incompatible with rows in B");
            if (A.numCols != C.numRows)
                throw new ArgumentException("Columns in A are incompatible with rows in C");
            if (B.numCols != C.numCols)
                throw new ArgumentException("Columns in B are incompatible with columns in C");
            if (A.blockLength != B.blockLength || A.blockLength != C.blockLength)
                throw new ArgumentException("Block lengths are not all the same.");

            int blockLength = A.blockLength;

            DSubmatrixD1 Asub = new DSubmatrixD1(A, 0, A.numRows, 0, A.numCols);
            DSubmatrixD1 Bsub = new DSubmatrixD1(B, 0, B.numRows, 0, B.numCols);
            DSubmatrixD1 Csub = new DSubmatrixD1(C, 0, C.numRows, 0, C.numCols);

            MatrixMult_DDRB.multTransA(blockLength, Asub, Bsub, Csub);
        }

        public static void multTransB(DMatrixRBlock A, DMatrixRBlock B, DMatrixRBlock C)
        {
            if (A.numCols != B.numCols)
                throw new ArgumentException("Columns in A are incompatible with columns in B");
            if (A.numRows != C.numRows)
                throw new ArgumentException("Rows in A are incompatible with rows in C");
            if (B.numRows != C.numCols)
                throw new ArgumentException("Rows in B are incompatible with columns in C");
            if (A.blockLength != B.blockLength || A.blockLength != C.blockLength)
                throw new ArgumentException("Block lengths are not all the same.");

            int blockLength = A.blockLength;

            DSubmatrixD1 Asub = new DSubmatrixD1(A, 0, A.numRows, 0, A.numCols);
            DSubmatrixD1 Bsub = new DSubmatrixD1(B, 0, B.numRows, 0, B.numCols);
            DSubmatrixD1 Csub = new DSubmatrixD1(C, 0, C.numRows, 0, C.numCols);

            MatrixMult_DDRB.multTransB(blockLength, Asub, Bsub, Csub);
        }

        //CONCURRENT_OMIT_BEGIN

        /**
         * Transposes a block matrix.
         *
         * @param A Original matrix. Not modified.
         * @param A_tran Transposed matrix. Modified.
         */
        public static DMatrixRBlock transpose(DMatrixRBlock A,  DMatrixRBlock A_tran )
        {
            if (A_tran != null)
            {
                if (A.numRows != A_tran.numCols || A.numCols != A_tran.numRows)
                    throw new ArgumentException("Incompatible dimensions.");
                if (A.blockLength != A_tran.blockLength)
                    throw new ArgumentException("Incompatible block size.");
            }
            else
            {
                A_tran = new DMatrixRBlock(A.numCols, A.numRows, A.blockLength);
            }

            for (int i = 0; i < A.numRows; i += A.blockLength)
            {
                int blockHeight = Math.Min(A.blockLength, A.numRows - i);

                for (int j = 0; j < A.numCols; j += A.blockLength)
                {
                    int blockWidth = Math.Min(A.blockLength, A.numCols - j);

                    int indexA = i * A.numCols + blockHeight * j;
                    int indexC = j * A_tran.numCols + blockWidth * i;

                    transposeBlock(A, A_tran, indexA, indexC, blockWidth, blockHeight);
                }
            }

            return A_tran;
        }

        /**
         * Transposes an individual block inside a block matrix.
         */
        private static void transposeBlock(DMatrixRBlock A, DMatrixRBlock A_tran,
                                            int indexA, int indexC,
                                            int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                int rowIndexC = indexC + i;
                int rowIndexA = indexA + width * i;
                int end = rowIndexA + width;
                for (; rowIndexA < end; rowIndexC += height, rowIndexA++)
                {
                    A_tran.data[rowIndexC] = A.data[rowIndexA];
                }
            }
        }

        public static DMatrixRBlock createRandom(int numRows, int numCols,
                                                  double min, double max, Random rand)
        {
            DMatrixRBlock ret = new DMatrixRBlock(numRows, numCols);
            Java.Util.Random rd = new Java.Util.Random();
            RandomMatrices_DDRM.fillUniform(ret, min, max, rd);

            return ret;
        }

        public static DMatrixRBlock createRandom(int numRows, int numCols,
                                                  double min, double max, Random rand,
                                                  int blockLength)
        {
            DMatrixRBlock ret = new DMatrixRBlock(numRows, numCols, blockLength);

            Java.Util.Random rd = new Java.Util.Random();
            RandomMatrices_DDRM.fillUniform(ret, min, max, rd);

            return ret;
        }

        public static DMatrixRBlock convert(DMatrixRMaj A, int blockLength)
        {
            DMatrixRBlock ret = new DMatrixRBlock(A.numRows, A.numCols, blockLength);
            convert(A, ret);
            return ret;
        }

        public static DMatrixRBlock convert(DMatrixRMaj A)
        {
            DMatrixRBlock ret = new DMatrixRBlock(A.numRows, A.numCols);
            convert(A, ret);
            return ret;
        }

        public static bool isEquals(DMatrixRBlock A, DMatrixRBlock B)
        {
            if (A.blockLength != B.blockLength)
                return false;

            return MatrixFeatures_DDRM.isEquals(A, B);
        }

        public static bool isEquals(DMatrixRBlock A, DMatrixRBlock B, double tol)
        {
            if (A.blockLength != B.blockLength)
                return false;

            return MatrixFeatures_DDRM.isEquals(A, B, tol);
        }

        /**
         * Sets either the upper or low triangle of a matrix to zero
         */
        public static void zeroTriangle(bool upper, DMatrixRBlock A)
        {
            int blockLength = A.blockLength;

            if (upper)
            {
                for (int i = 0; i < A.numRows; i += blockLength)
                {
                    int h = Math.Min(blockLength, A.numRows - i);

                    for (int j = i; j < A.numCols; j += blockLength)
                    {
                        int w = Math.Min(blockLength, A.numCols - j);

                        int index = i * A.numCols + h * j;

                        if (j == i)
                        {
                            for (int k = 0; k < h; k++)
                            {
                                for (int l = k + 1; l < w; l++)
                                {
                                    A.data[index + w * k + l] = 0;
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < h; k++)
                            {
                                for (int l = 0; l < w; l++)
                                {
                                    A.data[index + w * k + l] = 0;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < A.numRows; i += blockLength)
                {
                    int h = Math.Min(blockLength, A.numRows - i);

                    for (int j = 0; j <= i; j += blockLength)
                    {
                        int w = Math.Min(blockLength, A.numCols - j);

                        int index = i * A.numCols + h * j;

                        if (j == i)
                        {
                            for (int k = 0; k < h; k++)
                            {
                                int z = Math.Min(k, w);
                                for (int l = 0; l < z; l++)
                                {
                                    A.data[index + w * k + l] = 0;
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < h; k++)
                            {
                                for (int l = 0; l < w; l++)
                                {
                                    A.data[index + w * k + l] = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        /**
         * Copies either the upper or lower triangular portion of src into dst. Dst can be smaller
         * than src.
         *
         * @param upper If the upper or lower triangle is copied.
         * @param src The source matrix. Not modified.
         * @param dst The destination matrix. Modified.
         */
        public static void copyTriangle(bool upper, DMatrixRBlock src, DMatrixRBlock dst)
        {
            if (src.blockLength != dst.blockLength)
                throw new ArgumentException("Block size is different");
            if (src.numRows < dst.numRows)
                throw new ArgumentException("The src has fewer rows than dst");
            if (src.numCols < dst.numCols)
                throw new ArgumentException("The src has fewer columns than dst");

            int blockLength = src.blockLength;

            int numRows = Math.Min(src.numRows, dst.numRows);
            int numCols = Math.Min(src.numCols, dst.numCols);

            if (upper)
            {
                for (int i = 0; i < numRows; i += blockLength)
                {
                    int heightSrc = Math.Min(blockLength, src.numRows - i);
                    int heightDst = Math.Min(blockLength, dst.numRows - i);

                    for (int j = i; j < numCols; j += blockLength)
                    {
                        int widthSrc = Math.Min(blockLength, src.numCols - j);
                        int widthDst = Math.Min(blockLength, dst.numCols - j);

                        int indexSrc = i * src.numCols + heightSrc * j;
                        int indexDst = i * dst.numCols + heightDst * j;

                        if (j == i)
                        {
                            for (int k = 0; k < heightDst; k++)
                            {
                                for (int l = k; l < widthDst; l++)
                                {
                                    dst.data[indexDst + widthDst * k + l] = src.data[indexSrc + widthSrc * k + l];
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < heightDst; k++)
                            {
                                System.Array.Copy(src.data, indexSrc + widthSrc * k, dst.data, indexDst + widthDst * k, widthDst);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < numRows; i += blockLength)
                {
                    int heightSrc = Math.Min(blockLength, src.numRows - i);
                    int heightDst = Math.Min(blockLength, dst.numRows - i);

                    for (int j = 0; j <= i; j += blockLength)
                    {
                        int widthSrc = Math.Min(blockLength, src.numCols - j);
                        int widthDst = Math.Min(blockLength, dst.numCols - j);

                        int indexSrc = i * src.numCols + heightSrc * j;
                        int indexDst = i * dst.numCols + heightDst * j;

                        if (j == i)
                        {
                            for (int k = 0; k < heightDst; k++)
                            {
                                int z = Math.Min(k + 1, widthDst);
                                for (int l = 0; l < z; l++)
                                {
                                    dst.data[indexDst + widthDst * k + l] = src.data[indexSrc + widthSrc * k + l];
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < heightDst; k++)
                            {
                                System.Array.Copy(src.data, indexSrc + widthSrc * k, dst.data, indexDst + widthDst * k, widthDst);
                            }
                        }
                    }
                }
            }
        }

        /**
         * <p>
         * Sets every element in the matrix to the specified value.<br>
         * <br>
         * a<sub>ij</sub> = value
         * <p>
         *
         * @param A A matrix whose elements are about to be set. Modified.
         * @param value The value each element will have.
         */
        public static void set(DMatrixRBlock A, double value)
        {
            CommonOps_DDRM.fill(A, value);
        }

        /**
         * <p>Sets the value of A to all zeros except along the diagonal.</p>
         *
         * @param A Block matrix.
         */
        public static void setIdentity(DMatrixRBlock A)
        {
            int minLength = Math.Min(A.numRows, A.numCols);

            CommonOps_DDRM.fill(A, 0);

            int blockLength = A.blockLength;

            for (int i = 0; i < minLength; i += blockLength)
            {
                int h = Math.Min(blockLength, A.numRows - i);
                int w = Math.Min(blockLength, A.numCols - i);

                int index = i * A.numCols + h * i;

                int m = Math.Min(h, w);
                for (int k = 0; k < m; k++)
                {
                    A.data[index + k * w + k] = 1;
                }
            }
        }

        /**
         * <p>
         * Returns a new matrix with ones along the diagonal and zeros everywhere else.
         * </p>
         *
         * @param numRows Number of rows.
         * @param numCols NUmber of columns.
         * @param blockLength Block length.
         * @return An identify matrix.
         */
        public static DMatrixRBlock identity(int numRows, int numCols, int blockLength)
        {
            DMatrixRBlock A = new DMatrixRBlock(numRows, numCols, blockLength);

            int minLength = Math.Min(numRows, numCols);

            for (int i = 0; i < minLength; i += blockLength)
            {
                int h = Math.Min(blockLength, A.numRows - i);
                int w = Math.Min(blockLength, A.numCols - i);

                int index = i * A.numCols + h * i;

                int m = Math.Min(h, w);
                for (int k = 0; k < m; k++)
                {
                    A.data[index + k * w + k] = 1;
                }
            }

            return A;
        }

        /**
         * <p>
         * Checks to see if the two matrices have an identical shape an block size.
         * </p>
         *
         * @param A Matrix.
         * @param B Matrix.
         */
        public static void checkIdenticalShape(DMatrixRBlock A, DMatrixRBlock B)
        {
            if (A.blockLength != B.blockLength)
                throw new ArgumentException("Block size is different");
            if (A.numRows != B.numRows)
                throw new ArgumentException("Number of rows is different");
            if (A.numCols != B.numCols)
                throw new ArgumentException("NUmber of columns is different");
        }

        /**
         * <p>
         * Extracts a matrix from src into dst. The submatrix which is copied has its initial coordinate
         * at (0,0) and ends at (dst.numRows,dst.numCols). The end rows/columns must be aligned along blocks
         * or else it will silently screw things up.
         * </p>
         *
         * @param src Matrix which a submatrix is being extracted from. Not modified.
         * @param dst Where the submatrix is written to. Its rows and columns be less than or equal to 'src'. Modified.
         */
        public static void extractAligned(DMatrixRBlock src, DMatrixRBlock dst)
        {
            if (src.blockLength != dst.blockLength)
                throw new ArgumentException("Block size is different");
            if (src.numRows < dst.numRows)
                throw new ArgumentException("The src has fewer rows than dst");
            if (src.numCols < dst.numCols)
                throw new ArgumentException("The src has fewer columns than dst");

            int blockLength = src.blockLength;

            int numRows = Math.Min(src.numRows, dst.numRows);
            int numCols = Math.Min(src.numCols, dst.numCols);

            for (int i = 0; i < numRows; i += blockLength)
            {
                int heightSrc = Math.Min(blockLength, src.numRows - i);
                int heightDst = Math.Min(blockLength, dst.numRows - i);

                for (int j = 0; j < numCols; j += blockLength)
                {
                    int widthSrc = Math.Min(blockLength, src.numCols - j);
                    int widthDst = Math.Min(blockLength, dst.numCols - j);

                    int indexSrc = i * src.numCols + heightSrc * j;
                    int indexDst = i * dst.numCols + heightDst * j;

                    for (int k = 0; k < heightDst; k++)
                    {
                        System.Array.Copy(src.data, indexSrc + widthSrc * k, dst.data, indexDst + widthDst * k, widthDst);
                    }
                }
            }
        }

        /**
         * Checks to see if the submatrix has its boundaries along inner blocks.
         *
         * @param blockLength Size of an inner block.
         * @param A Submatrix.
         * @return If it is block aligned or not.
         */
        public static bool blockAligned(int blockLength, DSubmatrixD1 A)
        {
            if (A.col0 % blockLength != 0)
                return false;
            if (A.row0 % blockLength != 0)
                return false;

            if (A.col1 % blockLength != 0 && A.col1 != A.original.numCols)
            {
                return false;
            }

            if (A.row1 % blockLength != 0 && A.row1 != A.original.numRows)
            {
                return false;
            }

            return true;
        }

        public static void checkShapeMult(int blockLength,
                                    DSubmatrixD1 A, DSubmatrixD1 B,
                                    DSubmatrixD1 C)
        {
            //@formatter:off
            int Arow = A.Rows; int Acol = A.Cols;
            int Brow = B.Rows; int Bcol = B.Cols;
            int Crow = C.Rows; int Ccol = C.Cols;
            //@formatter:on

            if (Arow != Crow)
                throw new SystemException("Mismatch A and C rows");
            if (Bcol != Ccol)
                throw new SystemException("Mismatch B and C columns");
            if (Acol != Brow)
                throw new SystemException("Mismatch A columns and B rows");

            if (!MatrixOps_DDRB.blockAligned(blockLength, A))
                throw new SystemException("Sub-Matrix A is not block aligned");

            if (!MatrixOps_DDRB.blockAligned(blockLength, B))
                throw new SystemException("Sub-Matrix B is not block aligned");

            if (!MatrixOps_DDRB.blockAligned(blockLength, C))
                throw new SystemException("Sub-Matrix C is not block aligned");
        }
        //CONCURRENT_OMIT_END
    }
}