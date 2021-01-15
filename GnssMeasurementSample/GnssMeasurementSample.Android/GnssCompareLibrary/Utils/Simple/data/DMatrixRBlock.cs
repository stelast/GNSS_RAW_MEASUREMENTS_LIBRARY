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
    public class DMatrixRBlock : DMatrixD1
    {
        public int blockLength;

        public DMatrixRBlock(int numRows, int numCols, int blockLength)
        {
            UtilEjml.checkTooLarge(numRows, numCols);
            this.data = new double[numRows * numCols];
            this.blockLength = blockLength;
            this.numRows = numRows;
            this.numCols = numCols;
        }

        public DMatrixRBlock(int numRows, int numCols) : this(numRows, numCols, EjmlParameters.BLOCK_WIDTH)
        {
        }

        public DMatrixRBlock() { }

        public void set(DMatrixRBlock A)
        {
            this.blockLength = A.blockLength;
            this.numRows = A.numRows;
            this.numCols = A.numCols;

            int N = numCols * numRows;

            if (data.Count() < N)
                data = new double[N];

            Array.Copy(A.data, 0, data, 0, N);
        }

        public static DMatrixRBlock wrap(double[] data , int numRows, int numCols, int blockLength)
        {
            DMatrixRBlock ret = new DMatrixRBlock();
            ret.data = data;
            ret.numRows = numRows;
            ret.numCols = numCols;
            ret.blockLength = blockLength;

            return ret;
        }
        public override int getIndex(int row, int col)
        {
            // find the block it is inside
            int blockRow = row / blockLength;
            int blockCol = col / blockLength;

            int localHeight = Math.Min(numRows - blockRow * blockLength, blockLength);

            int index = blockRow * blockLength * numCols + blockCol * localHeight * blockLength;

            int localLength = Math.Min(numCols - blockLength * blockCol, blockLength);

            row = row % blockLength;
            col = col % blockLength;

            return index + localLength * row + col;
        }

        public override void reshape(int numRows, int numCols, bool saveValues)
        {
            UtilEjml.checkTooLarge(numRows, numCols);
            if (numRows * numCols <= data.Count())
            {
                this.numRows = numRows;
                this.numCols = numCols;
            }
            else
            {
                double[] data = new double[numRows * numCols];

                if (saveValues)
                {
                    System.Array.Copy(this.data, 0, data, 0, NumElements);
                }

                this.numRows = numRows;
                this.numCols = numCols;
                this.data = data;
            }
        } 

        public void reshape(int numRows, int numCols, int blockLength, bool saveValues)
        {
            this.blockLength = blockLength;
            this.reshape(numRows, numCols, saveValues);
        }

        public override DMatrixRBlock create<DMatrixRBlock>(int numRows, int numCols)
        {
            //return new DMatrixRBlock(numRows, numCols, blockLength);
            return base.create<DMatrixRBlock>(numRows, numCols);
        }
    }
}