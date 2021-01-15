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
    public class TransposeAlgs_DDRM
    {

        /**
         * In-place transpose for a square matrix.  On most architectures it is faster than the standard transpose
         * algorithm, but on most modern computers it's slower than block transpose.
         *
         * @param A The matrix that is transposed in-place.  Modified.
         */
        public static void square(DMatrix1Row A)
        {
            //CONCURRENT_BELOW EjmlConcurrency.loopFor(0, A.numRows, i -> {
            for (int i = 0; i < A.numRows; i++)
            {
                int index = i * A.numCols + i + 1;
                int indexEnd = (i + 1) * A.numCols;

                int indexOther = (i + 1) * A.numCols + i;
                for (; index < indexEnd; index++, indexOther += A.numCols)
                {
                    double val = A.data[index];
                    A.data[index] = A.data[indexOther];
                    A.data[indexOther] = val;
                }
            }
            //CONCURRENT_ABOVE });
        }

        /**
         * Performs a transpose across block sub-matrices.  Reduces
         * the number of cache misses on larger matrices.
         *
         * *NOTE* If this is beneficial is highly dependent on the computer it is run on. e.g:
         * - Q6600 Almost twice as fast as standard.
         * - Pentium-M Same speed and some times a bit slower than standard.
         *
         * @param A Original matrix.  Not modified.
         * @param A_tran Transposed matrix.  Modified.
         * @param blockLength Length of a block.
         */
        public static void block(DMatrix1Row A, DMatrix1Row A_tran,
                                   int blockLength)
        {
            //CONCURRENT_BELOW EjmlConcurrency.loopBlocks(0, A.numRows,blockLength, (idx0,idx1) -> {
            for (int idx0 = 0; idx0 < A.numRows; idx0 += blockLength)
            {
                //CONCURRENT_REMOVE_BELOW
                int idx1 = Math.Min(A.numRows, idx0 + blockLength);
                int blockHeight = idx1 - idx0;

                int indexSrc = idx0 * A.numCols;
                int indexDst = idx0;

                for (int j = 0; j < A.numCols; j += blockLength)
                {
                    int blockWidth = Math.Min(blockLength, A.numCols - j);

                    //                int indexSrc = i*A.numCols + j;
                    //                int indexDst = j*A_tran.numCols + i;

                    int indexSrcEnd = indexSrc + blockWidth;
                    //                for( int l = 0; l < blockWidth; l++ , indexSrc++ ) {
                    for (; indexSrc < indexSrcEnd; indexSrc++)
                    {
                        int rowSrc = indexSrc;
                        int rowDst = indexDst;
                        int end = rowDst + blockHeight;
                        //                    for( int k = 0; k < blockHeight; k++ , rowSrc += A.numCols ) {
                        for (; rowDst < end; rowSrc += A.numCols)
                        {
                            // faster to write in sequence than to read in sequence
                            A_tran.data[rowDst++] = A.data[rowSrc];
                        }
                        indexDst += A_tran.numCols;
                    }
                }
            }
            //CONCURRENT_ABOVE });
        }

        /**
         * A straight forward transpose.  Good for small non-square matrices.
         *
         * @param A Original matrix.  Not modified.
         * @param A_tran Transposed matrix.  Modified.
         */
        public static void standard(DMatrix1Row A, DMatrix1Row A_tran)
        {
            //CONCURRENT_BELOW EjmlConcurrency.loopFor(0, A_tran.numRows, i -> {
            for (int i = 0; i < A_tran.numRows; i++)
            {
                int index = i * A_tran.numCols;
                int index2 = i;

                int end = index + A_tran.numCols;
                while (index < end)
                {
                    A_tran.data[index++] = A.data[index2];
                    index2 += A.numCols;
                }
            }
            //CONCURRENT_ABOVE });
        }
    }
}