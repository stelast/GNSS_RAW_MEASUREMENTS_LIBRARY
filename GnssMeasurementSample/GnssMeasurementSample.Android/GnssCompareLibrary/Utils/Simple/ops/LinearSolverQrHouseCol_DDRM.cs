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
    public class LinearSolverQrHouseCol_DDRM : LinearSolverAbstract_DDRM
    {
        protected QRDecompositionHouseholderColumn_DDRM decomposer;

        protected DMatrixRMaj a = new DMatrixRMaj(1, 1);
        protected DMatrixRMaj temp = new DMatrixRMaj(1, 1);

        protected int maxRows = -1;
        protected int maxCols = -1;

        protected double[][] QR; // a column major QR matrix
        protected DMatrixRMaj R = new DMatrixRMaj(1, 1);
        protected double[] gammas;

        /**
         * Creates a linear solver that uses QR decomposition.
         */
        public LinearSolverQrHouseCol_DDRM() 
        {
            decomposer = new QRDecompositionHouseholderColumn_DDRM();
        }

        protected LinearSolverQrHouseCol_DDRM(QRDecompositionHouseholderColumn_DDRM decomposer)
        {
            this.decomposer = decomposer;
        }

        public void setMaxSize(int maxRows, int maxCols)
        {
            this.maxRows = maxRows;
            this.maxCols = maxCols;
        }
    }
}