using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.linsol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class LinearSolverFactory_ZDRM
    {
        /**
         * Creates a linear solver which uses LU decomposition internally
         *
         * @param matrixSize Approximate of rows and columns
         * @return Linear solver
         */
        public static LinearSolverDense<ZMatrixRMaj> lu(int matrixSize)
        {
            return new LinearSolverLu_ZDRM(new LUDecompositionAlt_ZDRM());
        }

        /**
         * Creates a linear solver which uses Cholesky decomposition internally
         *
         * @param matrixSize Approximate of rows and columns
         * @return Linear solver
         */
        public static LinearSolverDense<ZMatrixRMaj> chol(int matrixSize)
        {
            return new LinearSolverChol_ZDRM(new CholeskyDecompositionInner_ZDRM());
        }

        /**
         * Creates a linear solver which uses QR decomposition internally
         *
         * @param numRows Approximate of rows
         * @param numCols Approximate of columns
         * @return Linear solver
         */
        public static LinearSolverDense<ZMatrixRMaj> qr(int numRows, int numCols)
        {
            return new LinearSolverQrHouseCol_ZDRM();
        }
    }
}