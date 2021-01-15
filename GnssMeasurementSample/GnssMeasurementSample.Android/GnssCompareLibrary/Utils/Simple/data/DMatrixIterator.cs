using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data
{
    public class DMatrixIterator : IEnumerator<Double>
    {
        private DMatrixD1 a;

        // should it iterate through by row or by column
        private bool rowMajor;

        // the first row and column it returns
        private int minCol;
        private int minRow;

        // where in the iteration it is
        private int index = 0;
        // how many elements inside will it return
        private int size;

        // how wide the submatrix is
        private int submatrixStride;

        // the current element
        int subRow, subCol;

        /**
         * Creates a new iterator for traversing through a submatrix inside this matrix.  It can be traversed
         * by row or by column.  Range of elements is inclusive, e.g. minRow = 0 and maxRow = 1 will include rows
         * 0 and 1.  The iteration starts at (minRow,minCol) and ends at (maxRow,maxCol)
         *
         * @param a the matrix it is iterating through
         * @param rowMajor true means it will traverse through the submatrix by row first, false by columns.
         * @param minRow first row it will start at.
         * @param minCol first column it will start at.
         * @param maxRow last row it will stop at.
         * @param maxCol last column it will stop at.
         */
        public DMatrixIterator(DMatrixD1 a, bool rowMajor,
                               int minRow, int minCol, int maxRow, int maxCol
        )
        {
            if (maxCol < minCol)
                throw new ArgumentException("maxCol has to be more than or equal to minCol");
            if (maxRow < minRow)
                throw new ArgumentException("maxRow has to be more than or equal to minCol");
            if (maxCol >= a.numCols)
                throw new ArgumentException("maxCol must be < numCols");
            if (maxRow >= a.numRows)
                throw new ArgumentException("maxRow must be < numCRows");

            this.a = a;
            this.rowMajor = rowMajor;
            this.minCol = minCol;
            this.minRow = minRow;

            size = (maxCol - minCol + 1) * (maxRow - minRow + 1);

            if (rowMajor)
                submatrixStride = maxCol - minCol + 1;
            else
                submatrixStride = maxRow - minRow + 1;
        }

        public double Current {
            get;
            set;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            throw new SystemException("Operation not supported");
        }

        public bool MoveNext()
        {
            //Avoids going beyond the end of the collection.
            if (++index >= size)
            {
                return false;
            }
            else
            {
                if (rowMajor)
                {
                    subRow = index / submatrixStride;
                    subCol = index % submatrixStride;
                }
                else
                {
                    subRow = index % submatrixStride;
                    subCol = index / submatrixStride;
                }
                index++;
                Current = a.get(subRow + minRow, subCol + minCol);
            }
            return true;   
        }

        public void Reset()
        {
            index = -1;
        }

        /**
         * Which element in the submatrix was returned by next()
         *
         * @return Submatrix element's index.
         */
        public int getIndex()
        {
            return index - 1;
        }

        /**
         * True if it is iterating through the matrix by rows and false if by columns.
         * @return row major or column major
         */
        public bool isRowMajor()
        {
            return rowMajor;
        }

        /**
         * Sets the value of the current element.
         *
         * @param value The element's new value.
         */
        public void set(double value)
        {
            a.set(subRow + minRow, subCol + minCol, value);
        }
    }
}