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
    public class SymmetricQRAlgorithmDecomposition_DDRM : EigenDecomposition_F64<DMatrixRMaj>
    {
        // computes a tridiagonal matrix whose eigenvalues are the same as the original
        // matrix and can be easily computed.
        private readonly TridiagonalSimilarDecomposition_F64<DMatrixRMaj> decomp;
        // helper class for eigenvalue and eigenvector algorithms
        private readonly SymmetricQREigenHelper_DDRM helper;
        // computes the eigenvectors
        private readonly SymmetricQrAlgorithm_DDRM vector;

        // should it compute eigenvectors at the same time as the eigenvalues?
        private bool computeVectorsWithValues = false;

        // where the found eigenvalues are stored
        private double[] values;

        // where the tridiagonal matrix is stored
        private double[] diag;
        private double[] off;

        private double[] diagSaved;
        private double[] offSaved;

        // temporary variable used to store/compute eigenvectors
        private DMatrixRMaj V;
        // the extracted eigenvectors
        private DMatrixRMaj[] eigenvectors;

        // should it compute eigenvectors or just eigenvalues
        bool computeVectors;

        public SymmetricQRAlgorithmDecomposition_DDRM(TridiagonalSimilarDecomposition_F64<DMatrixRMaj> decomp,
                                                       bool computeVectors)
        {

            this.decomp = decomp;
            this.computeVectors = computeVectors;

            helper = new SymmetricQREigenHelper_DDRM();
            vector = new SymmetricQrAlgorithm_DDRM(helper);
        }

        //public SymmetricQRAlgorithmDecomposition_DDRM(bool computeVectors) : this(DecompositionFactory_DDRM.tridiagonal(0), computeVectors)
        //{

        //}

        public void setComputeVectorsWithValues(bool computeVectorsWithValues)
        {
            if (!computeVectors)
                throw new ArgumentException("Compute eigenvalues has been set to false");

            this.computeVectorsWithValues = computeVectorsWithValues;
        }

        /**
         * Used to limit the number of internal QR iterations that the QR algorithm performs.  20
         * should be enough for most applications.
         *
         * @param max The maximum number of QR iterations it will perform.
         */
        //public void setMaxIterations(int max)
        //{
        //    vector.setMaxIterations(max);
        //}

        
        public int getNumberOfEigenvalues()
        {
            return helper.getMatrixSize();
        }

        
        public Complex_F64 getEigenvalue(int index)
        {
            return new Complex_F64(values[index], 0);
        }

        
        public DMatrixRMaj getEigenVector(int index)
        {
            return eigenvectors[index];
        }

        /**
         * Decomposes the matrix using the QR algorithm.  Care was taken to minimize unnecessary memory copying
         * and cache skipping.
         *
         * @param orig The matrix which is being decomposed.  Not modified.
         * @return true if it decomposed the matrix or false if an error was detected.  This will not catch all errors.
         */
        
        //public bool decompose(DMatrixRMaj orig)
        //{
        //    if (orig.numCols != orig.numRows)
        //        throw new ArgumentException("Matrix must be square.");
        //    if (orig.numCols <= 0)
        //        return false;

        //    int N = orig.numRows;

        //    // compute a similar tridiagonal matrix
        //    if (!decomp.decompose(orig))
        //        return false;

        //    double[] diag = this.diag;
        //    double[] off = this.off;
        //    if (diag == null || diag.Count() < N)
        //    {
        //        this.diag = diag = new double[N];
        //        this.off = off = new double[N - 1];
        //    }
        //    decomp.getDiagonal(diag, off);

        //    // Tell the helper to work with this matrix
        //    helper.init(diag, off, N);

        //    if (computeVectors)
        //    {
        //        if (computeVectorsWithValues)
        //        {
        //            return extractTogether();
        //        }
        //        else
        //        {
        //            return extractSeparate(N);
        //        }
        //    }
        //    else
        //    {
        //        return computeEigenValues();
        //    }
        //}

        
    public bool inputModified()
        {
            return decomp.inputModified();
        }

        public bool decompose(DMatrixRMaj orig)
        {
            throw new NotImplementedException();
        }

        //private bool extractTogether()
        //{
        //    // extract the orthogonal from the similar transform
        //    V = decomp.getQ(V, true);

        //    // tell eigenvector algorithm to update this matrix as it computes the rotators
        //    helper.setQ(V);

        //    vector.setFastEigenvalues(false);

        //    // extract the eigenvalues
        //    if (!vector.process(-1, null, null))
        //        return false;

        //    // the V matrix contains the eigenvectors.  Convert those into column vectors
        //    eigenvectors = CommonOps_DDRM.rowsToVector(V, eigenvectors);

        //    // save a copy of them since this data structure will be recycled next
        //    values = helper.copyEigenvalues(values);

        //    return true;
        //}

        //private bool extractSeparate(int numCols)
        //{
        //    if (!computeEigenValues())
        //        return false;

        //    // ---- set up the helper to decompose the same tridiagonal matrix
        //    // swap arrays instead of copying them to make it slightly faster
        //    helper.reset(numCols);
        //    diagSaved = helper.swapDiag(diagSaved);
        //    offSaved = helper.swapOff(offSaved);

        //    // extract the orthogonal from the similar transform
        //    V = decomp.getQ(V, true);

        //    // tell eigenvector algorithm to update this matrix as it computes the rotators
        //    vector.setQ(V);

        //    // extract eigenvectors
        //    if (!vector.process(-1, null, null, values))
        //        return false;

        //    // the ordering of the eigenvalues might have changed
        //    values = helper.copyEigenvalues(values);
        //    // the V matrix contains the eigenvectors.  Convert those into column vectors
        //    eigenvectors = CommonOps_DDRM.rowsToVector(V, eigenvectors);

        //    return true;
        //}

        /**
         * Computes eigenvalues only
         */
        //private bool computeEigenValues()
        //{
        //    // make a copy of the internal tridiagonal matrix data for later use
        //    diagSaved = helper.copyDiag(diagSaved);
        //    offSaved = helper.copyOff(offSaved);

        //    vector.setQ(null);
        //    vector.setFastEigenvalues(true);

        //    // extract the eigenvalues
        //    if (!vector.process(-1, null, null))
        //        return false;

        //    // save a copy of them since this data structure will be recycled next
        //    values = helper.copyEigenvalues(values);
        //    return true;
        //}
    }
}