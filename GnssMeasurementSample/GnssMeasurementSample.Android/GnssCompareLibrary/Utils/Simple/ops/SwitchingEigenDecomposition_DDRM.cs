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
    class SwitchingEigenDecomposition_DDRM : EigenDecomposition_F64<DMatrixRMaj>
    {
        // tolerance used in deciding if a matrix is symmetric or not
        private double tol;

        EigenDecomposition_F64<DMatrixRMaj> symmetricAlg;
        EigenDecomposition_F64<DMatrixRMaj> generalAlg;

        bool symmetric;
        // should it compute eigenvectors or just eigenvalues?
        bool computeVectors;

        DMatrixRMaj A = new DMatrixRMaj(1, 1);

        /**
         * @param tol Tolerance for a matrix being symmetric
         */
        //public SwitchingEigenDecomposition_DDRM(int matrixSize, bool computeVectors, double tol)
        //{
        //    symmetricAlg = DecompositionFactory_DDRM.eig(matrixSize, computeVectors, true);
        //    generalAlg = DecompositionFactory_DDRM.eig(matrixSize, computeVectors, false);
        //    this.computeVectors = computeVectors;
        //    this.tol = tol;
        //}

        public SwitchingEigenDecomposition_DDRM(EigenDecomposition_F64<DMatrixRMaj> symmetricAlg,
                                                 EigenDecomposition_F64<DMatrixRMaj> generalAlg, double tol)
        {
            this.symmetricAlg = symmetricAlg;
            this.generalAlg = generalAlg;
            this.tol = tol;
        }

        //public SwitchingEigenDecomposition_DDRM(int matrixSize) : this(matrixSize, true, UtilEjml.TEST_F64)
        //{
        //}

        public bool decompose(DMatrixRMaj orig)
        {
            A.setTo(orig);

            symmetric = MatrixFeatures_DDRM.isSymmetric(A, tol);

            return symmetric ?
                    symmetricAlg.decompose(A) :
                    generalAlg.decompose(A);
        }

        public Complex_F64 getEigenvalue(int index)
        {
            return symmetric ? symmetricAlg.getEigenvalue(index) :
                    generalAlg.getEigenvalue(index);
        }

        public DMatrixRMaj getEigenVector(int index)
        {
            if (!computeVectors)
                throw new ArgumentException("Configured to not compute eignevectors");

            return symmetric ? symmetricAlg.getEigenVector(index) :
                    generalAlg.getEigenVector(index);
        }

        public int getNumberOfEigenvalues()
        {
            return symmetric ? symmetricAlg.getNumberOfEigenvalues() :
                    generalAlg.getNumberOfEigenvalues();
        }

        public bool inputModified()
        {
            // since it doesn't know which algorithm will be used until a matrix is provided make a copy
            // of all inputs
            return false;
        }
    }
}