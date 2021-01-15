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
    public class QrHouseHolderSolver_DDRB : LinearSolverDense<DMatrixRBlock>
    {
        // QR decomposition algorithm
        protected QRDecompositionHouseholder_DDRB decomposer = new QRDecompositionHouseholder_DDRB();

        // the input matrix which has been decomposed
        protected DMatrixRBlock QR;

        public QrHouseHolderSolver_DDRB()
        {
            decomposer.setSaveW(false);
        }

        public Decomposition getDescomposition<Decomposition, T>()
            where Decomposition : DecompositionInterface<T>
            where T : Matrix
        {
            throw new NotImplementedException();
        }

        public void invert(DMatrixRBlock A_inv)
        {
            throw new NotImplementedException();
        }

        public bool modifiesA()
        {
            return decomposer.inputModified();
        }

        public bool modifiesB()
        {
            return true;
        }

        public double quality()
        {
            return SpecializedOps_DDRM.qualityTriangular(decomposer.getQR());
        }

        public bool setA(DMatrixRBlock A)
        {
            if (A.numRows < A.numCols)
                throw new ArgumentException("Number of rows must be more than or equal to the number of columns. " +
                        "Can't solve an underdetermined system.");

            if (!decomposer.decompose(A))
                return false;

            this.QR = decomposer.getQR();

            return true;
        }

        public void solve(DMatrixRBlock B, DMatrixRBlock X)
        {

            if (B.numRows != QR.numRows)
                throw new ArgumentException("Row of B and A do not match");

            X.reshape(QR.numCols, B.numCols);

            // The system being solved for can be described as:
            // Q*R*X = B

            // First apply householder reflectors to B
            // Y = Q^T*B
            //decomposer.applyQTran(B);

            // Second solve for Y using the upper triangle matrix R and the just computed Y
            // X = R^-1 * Y
            MatrixOps_DDRB.extractAligned(B, X);

            // extract a block aligned matrix
            int M = Math.Min(QR.numRows, QR.numCols);

            TriangularSolver_DDRB.solve(QR.blockLength, true,
                    new DSubmatrixD1(QR, 0, M, 0, M), new DSubmatrixD1(X), false);
        }
    }
}