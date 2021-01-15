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
    public class SolveNullSpaceQR_DDRM : SolveNullSpace<DMatrixRMaj>
    {
        CustomizedQR decomposition = new CustomizedQR();

        // Storage for Q matrix
        DMatrixRMaj Q = new DMatrixRMaj(1, 1);
        public DMatrixRMaj getQ()
        {
            return Q;
        }

        public bool inputModified()
        {
            throw new NotImplementedException();
        }

        public bool process(DMatrixRMaj A, int numSingularValues, DMatrixRMaj nullspace)
        {
            decomposition.decompose(A);

            if (A.numRows > A.numCols)
            {
                Q.reshape(A.numCols, Math.Min(A.numRows, A.numCols));
                decomposition.getQ(Q, true);
            }
            else
            {
                Q.reshape(A.numCols, A.numCols);
                decomposition.getQ(Q, false);
            }

            nullspace.reshape(Q.numRows, numSingularValues);
            CommonOps_DDRM.extract(Q, 0, Q.numRows, Q.numCols - numSingularValues, Q.numCols, nullspace, 0, 0);

            return true;
        }
        private class CustomizedQR : QRDecompositionHouseholderTran_DDRM
        {
            public override void setExpectedMaxSize(int numRows, int numCols)
            {
                this.numCols = numCols;
                this.numRows = numRows;
                minLength = Math.Min(numCols, numRows);
                int maxLength = Math.Max(numCols, numRows);

                // Don't delcare QR. It will use the input matrix for worspace
                if (v == null)
                {
                    v = new double[maxLength];
                    gammas = new double[minLength];
                }

                if (v.Count() < maxLength)
                {
                    v = new double[maxLength];
                }
                if (gammas.Count() < minLength)
                {
                    gammas = new double[minLength];
                }
            }

            /**
             * Modified decomposition which assumes the input is a transpose of the matrix
             */            
            public override bool decompose(DMatrixRMaj A_tran)
            {
                // There is a "subtle" hack in the line below. Instead of passing in (cols,rows) I'm passing in
                // (cols,cols) that's because we don't care about updating everything past the cols
                setExpectedMaxSize(A_tran.numCols, Math.Min(A_tran.numRows, A_tran.numCols));

                // use the input matrix for its workspace
                this.QR = A_tran;

                error = false;

                for (int j = 0; j < minLength; j++)
                {
                    householder(j);
                    updateA(j);
                }

                return !error;
            }
        }
    }
}