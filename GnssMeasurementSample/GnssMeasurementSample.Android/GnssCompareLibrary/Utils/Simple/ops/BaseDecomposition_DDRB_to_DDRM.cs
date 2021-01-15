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
    public class BaseDecomposition_DDRB_to_DDRM : DecompositionInterface<DMatrixRMaj>
    {
        protected DecompositionInterface<DMatrixRBlock> alg;

        protected DGrowArray workspace = new DGrowArray();
        protected DMatrixRBlock Ablock = new DMatrixRBlock();
        protected int blockLength;

        public BaseDecomposition_DDRB_to_DDRM(DecompositionInterface<DMatrixRBlock> alg,
                                               int blockLength)
        {
            this.alg = alg;
            this.blockLength = blockLength;
        }

        public bool decompose(DMatrixRMaj A)
        {
            Ablock.numRows = A.numRows;
            Ablock.numCols = A.numCols;
            Ablock.blockLength = blockLength;
            Ablock.data = A.data;

            // doing an in-place convert is much more memory efficient at the cost of a little
            // but of CPU
            MatrixOps_DDRB.convertRowToBlock(A.numRows, A.numCols, Ablock.blockLength, A.data, workspace);

            bool ret = alg.decompose(Ablock);

            // convert it back to the normal format if it wouldn't have been modified
            if (!alg.inputModified())
            {
                MatrixOps_DDRB.convertBlockToRow(A.numRows, A.numCols, Ablock.blockLength, A.data, workspace);
            }

            return ret;
        }

        public void convertBlockToRow(int numRows, int numCols, double[] data)
        {
            MatrixOps_DDRB.convertBlockToRow(numRows, numCols, Ablock.blockLength, data, workspace);
        }

        public bool inputModified()
        {
            return alg.inputModified();
        }
    }
}