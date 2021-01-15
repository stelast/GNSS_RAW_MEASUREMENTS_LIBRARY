using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.linsol
{
    public class LinearSolverSparseSafe<S, D> : LinearSolverSparse<S, D>
        where S : DMatrixSparse
        where D : DMatrixSparse
    {
        // the solver it is wrapped around
        private readonly LinearSolverSparse<S, D> alg;

        // local copies of input matrices that can be modified.
        private  S A;
        private  D B;

        /**
         * @param alg The solver it is wrapped around.
         */
        public LinearSolverSparseSafe(LinearSolverSparse<S, D> alg)
        {
            this.alg = alg;
        }

        //public DecompositionInterface<Matrix> getDecomposition()
        //{
        //    return alg.getDecomposition();
        //}

        public Decomposition getDescomposition<Decomposition, T>()
            where Decomposition : DecompositionInterface<T>
            where T : Matrix
        {
            throw new NotImplementedException();
        }

        public bool isStructureLocked()
        {
            throw new NotImplementedException();
        }

        public bool modifiesA()
        {
            throw new NotImplementedException();
        }

        public bool modifiesB()
        {
            throw new NotImplementedException();
        }

        public double quality()
        {
            throw new NotImplementedException();
        }

        public bool setA(S A)
        {
            throw new NotImplementedException();
        }

        public void setStructureLocked(bool locked)
        {
            throw new NotImplementedException();
        }

        public void solve(D B, D X)
        {
            throw new NotImplementedException();
        }

        public void solveSparse(S B, S X)
        {
            throw new NotImplementedException();
        }
    }
}