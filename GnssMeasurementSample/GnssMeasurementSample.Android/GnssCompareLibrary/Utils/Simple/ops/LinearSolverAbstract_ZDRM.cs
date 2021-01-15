using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.linsol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public abstract class LinearSolverAbstract_ZDRM : LinearSolverDense<ZMatrixRMaj>
    { 
        protected ZMatrixRMaj A;
        protected int numCols;
        protected int numRows;
        protected int stride;

        virtual public ZMatrixRMaj getA()
        {
            return A;
        }

        virtual protected void _setA(ZMatrixRMaj A)
        {
            this.A = A;
            this.numRows = A.numRows;
            this.numCols = A.numCols;
            this.stride = numCols * 2;
        }
        virtual public void invert(ZMatrixRMaj A_inv)
        {
            InvertUsingSolve_ZDRM.invert(this, A, A_inv);
        }


        virtual public bool setA(ZMatrixRMaj A)
        {
            throw new NotImplementedException();
        }

        virtual public double quality()
        {
            throw new NotImplementedException();
        }

        virtual public void solve(ZMatrixRMaj B, ZMatrixRMaj X)
        {
            throw new NotImplementedException();
        }

        virtual public bool modifiesA()
        {
            throw new NotImplementedException();
        }

        virtual public bool modifiesB()
        {
            throw new NotImplementedException();
        }

        virtual public Decomposition getDescomposition<Decomposition, T>()
            where Decomposition : DecompositionInterface<T>
            where T : Matrix
        {
            throw new NotImplementedException();
        }
    }
}