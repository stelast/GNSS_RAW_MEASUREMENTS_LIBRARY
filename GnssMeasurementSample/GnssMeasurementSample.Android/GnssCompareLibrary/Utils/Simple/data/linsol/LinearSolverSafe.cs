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
    /**
     * Ensures that any linear solver it is wrapped around will never modify
     * the input matrices.
     *
     * @author Peter Abeles
     */
    public class LinearSolverSafe<T> : LinearSolverDense<T>
        where T : ReshapeMatrix
    {
        // the solver it is wrapped around
        private readonly LinearSolverDense<T> alg;
        // local copies of input matrices that can be modified.
        private T A;
        private T B;
        /**
         * @param alg The solver it is wrapped around.
         */
        public LinearSolverSafe(LinearSolverDense<T> alg)
        {
            this.alg = alg;
        }
        public void invert(T A_inv)
        {
            alg.invert(A_inv);
        }

        public bool modifiesA()
        {
            return false;
        }

        public bool modifiesB()
        {
            return false;
        }

        public double quality()
        {
            return alg.quality();
        }

        public bool setA(T A)
        {

            if (alg.modifiesA())
            {
                this.A = (T)UtilEjml.reshapeOrDeclare(this.A, A);
                this.A.To = A;
                return alg.setA(this.A);
            }

            return alg.setA(A);
        }

        public void solve(T B, T X)
        {
            if (alg.modifiesB())
            {
                this.B = UtilEjml.reshapeOrDeclare(this.B, B);
                this.B.To = B;
                B = this.B;
            }

            alg.solve(B, X);
        }

        public Decomposition getDescomposition<Decomposition, T1>()
            where Decomposition : DecompositionInterface<T1>
            where T1 : Matrix
        {
            throw new NotImplementedException();
        }
    }
}