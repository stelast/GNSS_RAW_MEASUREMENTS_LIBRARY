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
    public class SolveNullSpaceSvd_DDRM : SolveNullSpace<DMatrixRMaj>
    {
        bool compact = true;
        //SingularValueDecomposition_F64<DMatrixRMaj> svd = DecompositionFactory_DDRM.svd(1, 1, false, true, true);
        DMatrixRMaj V;

        public bool inputModified()
        {
            throw new NotImplementedException();
        }

        public bool process(DMatrixRMaj input, int numberOfSingular, DMatrixRMaj nullspace)
        {
            throw new NotImplementedException();
        }

        //public SingularValueDecomposition_F64<DMatrixRMaj> getSvd()
        //{
        //    return svd;
        //}

        //public double[] getSingularValues()
        //{
        //    return svd.getSingularValues();
        //}

        //public bool process(DMatrixRMaj input, int numberOfSingular, DMatrixRMaj nullspace)
        //{
        //    if (input.numCols > input.numRows)
        //    {
        //        if (compact)
        //        {
        //            svd = DecompositionFactory_DDRM.svd(1, 1, false, true, false);
        //            compact = false;
        //        }
        //    }
        //    else
        //    {
        //        if (!compact)
        //        {
        //            svd = DecompositionFactory_DDRM.svd(1, 1, false, true, true);
        //            compact = true;
        //        }
        //    }

        //    if (!svd.decompose(input))
        //        return false;

        //    double[] singularValues = svd.getSingularValues();
        //    V = svd.getV(V, false);

        //    SingularOps_DDRM.descendingOrder(null, false, singularValues, svd.numberOfSingularValues(), V, false);

        //    nullspace.reshape(V.numRows, numberOfSingular);
        //    CommonOps_DDRM.extract(V, 0, V.numRows, V.numCols - numberOfSingular, V.numCols, nullspace, 0, 0);

        //    return true;
        //}

        //public bool inputModified()
        //{
        //    return svd.inputModified();
        //}
    }
}