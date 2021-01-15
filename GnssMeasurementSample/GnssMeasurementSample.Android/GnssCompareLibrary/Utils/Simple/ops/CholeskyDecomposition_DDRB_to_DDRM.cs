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
    public class CholeskyDecomposition_DDRB_to_DDRM : BaseDecomposition_DDRB_to_DDRM, CholeskyDecomposition_F64<DMatrixRMaj>
    {
        public CholeskyDecomposition_DDRB_to_DDRM(bool lower) : base(new CholeskyOuterForm_DDRB(lower), EjmlParameters.BLOCK_WIDTH)
        {
        }

        public CholeskyDecomposition_DDRB_to_DDRM(DecompositionInterface<DMatrixRBlock> alg, int blockLength) : base(alg, blockLength)
        {
        }

        public bool isLower()
        {
            return ((CholeskyOuterForm_DDRB)alg).isLower();
        }

        public Complex_F64 computeDeterminant()
        {
            return ((CholeskyOuterForm_DDRB)alg).computeDeterminant();
        }

        public DMatrixRMaj getT(DMatrixRMaj T)
        {
            DMatrixRBlock T_block = ((CholeskyOuterForm_DDRB)alg).getT(null);

            if (T == null)
            {
                T = new DMatrixRMaj(T_block.numRows, T_block.numCols);
            }

            MatrixOps_DDRB.convert(T_block, T);
            // todo set zeros
            return T;
        }
    }
}