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
    public abstract class LinearSolverAbstract_DDRM : LinearSolverDense<DMatrixRMaj>
    {
        protected DMatrixRMaj A;
        protected int numRows;
        protected int numCols;
        protected void _setA(DMatrixRMaj A)
        {
            this.A = A;
            this.numRows = A.numRows;
            this.numCols = A.numCols;
        }
        public DMatrixRMaj getA()
        {
            return A;
        }
        virtual public void invert(DMatrixRMaj A_inv)
        {
            if (A == null)
                throw new SystemException("Must call setA() first");
            InvertUsingSolve_DDRM.invert(this, A, A_inv);
        }

        virtual public bool setA(DMatrixRMaj A)
        {
            throw new NotImplementedException();
        }

        virtual public double quality()
        {
            throw new NotImplementedException();
        }

        virtual public void solve(DMatrixRMaj B, DMatrixRMaj X)
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