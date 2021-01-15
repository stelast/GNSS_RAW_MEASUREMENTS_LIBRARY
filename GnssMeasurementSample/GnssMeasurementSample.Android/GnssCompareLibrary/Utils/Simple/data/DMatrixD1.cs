using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data
{
    public abstract class DMatrixD1 : ReshapeMatrix, DMatrix
    {
        /**
         * Where the raw data for the matrix is stored.  The format is type dependent.
         */
        public double[] data = UtilEjml.ZERO_LENGTH_F64;

        /**
         * Number of rows in the matrix.
         */
        public int numRows;
        /**
         * Number of columns in the matrix.
         */
        public int numCols;

        /// <summary>
        /// Sets the number of rows.
        /// </summary>
        /// <param name="numRows"> Number of rows </param>
        public virtual int NumCols
        {
            get
            {
                return this.numCols;
            }
            set
            {
                this.numCols = value;
            }
        }

        /// <summary>
        /// Sets the number of rows.
        /// </summary>
        /// <param name="numRows"> Number of rows </param>
        public virtual int NumRows
        {
            get
            {
                return this.numRows;
            }
            set
            {
                this.numRows = value;
            }
        }
        /// <summary>
        /// Used to get a reference to the internal data.
        /// </summary>
        /// <returns> Reference to the matrix's data. </returns>
        public virtual double[] Data
        {
            get
            {
                return data;
            }
            set
            {
                this.data = value;
            }
        }

        public void print()
        {
            MatrixIO.printFancy(this, MatrixIO.DEFAULT_LENGTH);
        }

        public void print(string format)
        {
            MatrixIO.print(format);
        }

        /**
         * Used to get a reference to the internal data.
         *
         * @return Reference to the matrix's data.
         */
        public double[] getData()
        {
            return data;
        }

        /**
         * Changes the internal array reference.
         */
        public void setData(double[] data)
        {
            this.data = data;
        }

        /**
         * Returns the internal array index for the specified row and column.
         *
         * @param row Row index.
         * @param col Column index.
         * @return Internal array index.
         */
        public abstract int getIndex(int row, int col);

        /// <summary>
        /// Sets the value of this matrix to be the same as the value of the provided matrix.  Both
        /// matrices must have the same shape:<br>
        /// <br>
        /// a<sub>ij</sub> = b<sub>ij</sub><br>
        /// <br>
        /// </summary>
        /// <param name="b"> The matrix that this matrix is to be set equal to. </param>
        public virtual void setTo(DMatrixD1 b)
        {
            if (numRows != b.numRows || numCols != b.numCols)
            {
                throw new MatrixDimensionException("The two matrices do not have compatible shapes.");
            }

            int dataLength = b.DataLength;

            Array.Copy(b.data, 0, this.data, 0, dataLength);
        }

        public virtual int DataLength { get; }

        /**
         * Returns the value of the matrix at the specified internal array index. The element at which row and column
         * returned by this function depends upon the matrix's internal structure, e.g. row-major, column-major, or block.
         *
         * @param index Internal array index.
         * @return Value at the specified index.
         */
        public double get(int index)
        {
            return data[index];
        }

        /**
         * Sets the element's value at the specified index.  The element at which row and column
         * modified by this function depends upon the matrix's internal structure, e.g. row-major, column-major, or block.
         *
         * @param index Index of element that is to be set.
         * @param val The new value of the index.
         */
        public double set(int index, double val)
        {
            // See benchmarkFunctionReturn.  Pointless return does not degrade performance.  Tested on JDK 1.6.0_21
            return data[index] = val;
        }

        /**
         * <p>
         * Adds the specified value to the internal data array at the specified index.<br>
         * <br>
         * Equivalent to: this.data[index] += val;
         * </p>
         *
         * <p>
         * Intended for use in highly optimized code.  The  row/column coordinate of the modified element is
         * dependent upon the matrix's internal structure.
         * </p>
         *
         * @param index The index which is being modified.
         * @param val The value that is being added.
         */
        public double plus(int index, double val)
        {
            // See benchmarkFunctionReturn.  Pointless return does not degrade performance.  Tested on JDK 1.6.0_21
            return data[index] += val;
        }

        /**
         * <p>
         * Subtracts the specified value to the internal data array at the specified index.<br>
         * <br>
         * Equivalent to: this.data[index] -= val;
         * </p>
         *
         * <p>
         * Intended for use in highly optimized code.  The  row/column coordinate of the modified element is
         * dependent upon the matrix's internal structure.
         * </p>
         *
         * @param index The index which is being modified.
         * @param val The value that is being subtracted.
         */
        public double minus(int index, double val)
        {
            // See benchmarkFunctionReturn.  Pointless return does not degrade performance.  Tested on JDK 1.6.0_21
            return data[index] -= val;
        }

        /**
         * <p>
         * Multiplies the specified value to the internal data array at the specified index.<br>
         * <br>
         * Equivalent to: this.data[index] *= val;
         * </p>
         *
         * <p>
         * Intended for use in highly optimized code.  The  row/column coordinate of the modified element is
         * dependent upon the matrix's internal structure.
         * </p>
         *
         * @param index The index which is being modified.
         * @param val The value that is being multiplied.
         */
        public double times(int index, double val)
        {
            // See benchmarkFunctionReturn.  Pointless return does not degrade performance.  Tested on JDK 1.6.0_21
            return data[index] *= val;
        }

        /**
         * <p>
         * Divides the specified value to the internal data array at the specified index.<br>
         * <br>
         * Equivalent to: this.data[index] /= val;
         * </p>
         *
         * <p>
         * Intended for use in highly optimized code.  The  row/column coordinate of the modified element is
         * dependent upon the matrix's internal structure.
         * </p>
         *
         * @param index The index which is being modified.
         * @param val The value that is being divided.
         */
        public double div(int index, double val)
        {
            // See benchmarkFunctionReturn.  Pointless return does not degrade performance.  Tested on JDK 1.6.0_21
            return data[index] /= val;
        }

        /**
         * Creates a new iterator for traversing through a submatrix inside this matrix.  It can be traversed
         * by row or by column.  Range of elements is inclusive, e.g. minRow = 0 and maxRow = 1 will include rows
         * 0 and 1.  The iteration starts at (minRow,minCol) and ends at (maxRow,maxCol)
         *
         * @param rowMajor true means it will traverse through the submatrix by row first, false by columns.
         * @param minRow first row it will start at.
         * @param minCol first column it will start at.
         * @param maxRow last row it will stop at.
         * @param maxCol last column it will stop at.
         * @return A new MatrixIterator
         */
        public DMatrixIterator iterator(bool rowMajor, int minRow, int minCol, int maxRow, int maxCol)
        {
            return new DMatrixIterator(this, rowMajor, minRow, minCol, maxRow, maxCol);
        }

        public Matrix To { set => throw new NotImplementedException(); }

        virtual public MatrixType Type { get; set; } 

        public int NumElements
        {
            get
            {
                return numRows * numCols;
            }
            set { }
        }

        public abstract void reshape(int numRows, int numCols, bool saveValues);

        public void reshape(int numRows, int numCols)
        {
            reshape(numRows, numCols, false);
        }

        public void zero()
        {
            throw new NotImplementedException();
        }

        public double get(int row, int col)
        {
            throw new NotImplementedException();
        }

        public double unsafe_get(int row, int col)
        {
            throw new NotImplementedException();
        }

        public void set(int row, int col, double val)
        {
            throw new NotImplementedException();
        }

        public void unsafe_set(int row, int col, double val)
        {
            throw new NotImplementedException();
        }

        virtual public T copy<T>() where T : Matrix
        {
            throw new NotImplementedException();
        }

        virtual public T createLike<T>() where T : Matrix
        {
            throw new NotImplementedException();
        }

        virtual public T create<T>(int numRows, int numCols) where T : Matrix
        {
            throw new NotImplementedException();
        }
    }
}