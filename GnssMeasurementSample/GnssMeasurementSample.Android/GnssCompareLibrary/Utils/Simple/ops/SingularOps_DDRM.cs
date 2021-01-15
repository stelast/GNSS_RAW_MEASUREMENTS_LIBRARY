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
    public class SingularOps_DDRM
    {

        /**
         * Returns an array of all the singular values in A sorted in ascending order
         *
         * @param A Matrix. Not modified.
         * @return singular values
         */
        //public static double[] singularValues(DMatrixRMaj A)
        //{
        //    SingularValueDecomposition_F64<DMatrixRMaj> svd = DecompositionFactory_DDRM.svd(A.numRows, A.numCols, false, true, true);

        //    if (svd.inputModified())
        //    {
        //        A = A.copy();
        //    }
        //    if (!svd.decompose(A))
        //    {
        //        throw new SystemException("SVD Failed!");
        //    }

        //    double[] sv = svd.getSingularValues();
        //    Array.Sort(sv, 0, svd.numberOfSingularValues());

        //    // change the ordering to ascending
        //    for (int i = 0; i < sv.Count() / 2; i++)
        //    {
        //        double tmp = sv[i];
        //        sv[i] = sv[sv.Count() - i - 1];
        //        sv[sv.Count() - i - 1] = tmp;
        //    }

        //    return sv;
        //}

        /**
         * Computes the ratio of the smallest value to the largest. Does not assume
         * the array is sorted first
         *
         * @param sv array
         * @return smallest / largest
         */
        public static double ratioSmallestOverLargest(double[] sv)
        {
            if (sv.Count() == 0)
                return Double.NaN;

            double min = sv[0];
            double max = min;

            for (int i = 1; i < sv.Count(); i++)
            {
                double v = sv[i];
                if (v > max)
                    max = v;
                else if (v < min)
                    min = v;
            }

            return min / max;
        }

        /**
         * Returns the matrix's rank
         *
         * @param A Matrix. Not modified.
         * @param threshold Tolerance used to determine of a singular value is singular.
         * @return The rank of the decomposed matrix.
         */
        //public static int rank(DMatrixRMaj A, double threshold)
        //{
        //    SingularValueDecomposition_F64<DMatrixRMaj> svd = DecompositionFactory_DDRM.svd(A.numRows, A.numCols, false, true, true);

        //    if (svd.inputModified())
        //    {
        //        A = A.copy();
        //    }
        //    if (!svd.decompose(A))
        //    {
        //        throw new SystemException("SVD Failed!");
        //    }

        //    double[] sv = svd.getSingularValues();

        //    int count = 0;
        //    for (int i = 0; i < sv.Count(); i++)
        //    {
        //        if (sv[i] >= threshold)
        //        {
        //            count++;
        //        }
        //    }
        //    return count;
        //}

        /**
         * Returns the matrix's rank. Automatic selection of threshold
         *
         * @param A Matrix. Not modified.
         * @return The rank of the decomposed matrix.
         */
        //public static int rank(DMatrixRMaj A)
        //{
        //    SingularValueDecomposition_F64<DMatrixRMaj> svd = DecompositionFactory_DDRM.svd(A.numRows, A.numCols, false, true, true);

        //    if (svd.inputModified())
        //    {
        //        A = A.copy();
        //    }
        //    if (!svd.decompose(A))
        //    {
        //        throw new SystemException("SVD Failed!");
        //    }

        //    int N = svd.numberOfSingularValues();
        //    double[] sv = svd.getSingularValues();

        //    double threshold = singularThreshold(sv, N);
        //    int count = 0;
        //    for (int i = 0; i < sv.Count(); i++)
        //    {
        //        if (sv[i] >= threshold)
        //        {
        //            count++;
        //        }
        //    }
        //    return count;
        //}

        /**
         * Computes the SVD and sorts singular values in descending order. While easier to use this can reduce performance
         * when performed on small matrices numerous times.
         *
         * U*W*V<sup>T</sup> = A
         *
         * @param A (Input) Matrix being decomposed
         * @param U (Output) Storage for U. If null then it's ignored.
         * @param sv (Output) sorted list of singular values. Can be null.
         * @param Vt (Output) Storage for transposed V. Can be null.
         */
        //public static bool svd(DMatrixRMaj A,  DMatrixRMaj U, DGrowArray sv,  DMatrixRMaj Vt )
        //{

        //    bool needU = U != null;
        //    bool needV = Vt != null;

        //    SingularValueDecomposition_F64<DMatrixRMaj> svd =
        //            DecompositionFactory_DDRM.svd(A.numRows, A.numCols, needU, needV, true);

        //    if (svd.inputModified())
        //    {
        //        A = A.copy();
        //    }

        //    if (!svd.decompose(A))
        //    {
        //        return false;
        //    }

        //    int N = Math.Min(A.numCols, A.numRows);

        //    if (needU)
        //        svd.getU(U, false);
        //    if (needV)
        //        svd.getV(Vt, true);

        //    sv.reshape(N);
        //    System.Array.Copy(svd.getSingularValues(), 0, sv.data, 0, N);

        //    descendingOrder(U, false, sv.data, N, Vt, true);

        //    return true;
        //}

        /**
         * <p>
         * Adjusts the matrices so that the singular values are in descending order.
         * </p>
         *
         * <p>
         * In most implementations of SVD the singular values are automatically arranged in in descending
         * order.  In EJML this is not the case since it is often not needed and some computations can
         * be saved by not doing that.
         * </p>
         *
         * @param U Matrix. Modified.
         * @param tranU is U transposed or not.
         * @param W Diagonal matrix with singular values. Modified.
         * @param V Matrix. Modified.
         * @param tranV is V transposed or not.
         */
        // TODO the number of copies can probably be reduced here
        public static void descendingOrder(DMatrixRMaj U, bool tranU,
                                            DMatrixRMaj W,
                                            DMatrixRMaj V, bool tranV)
        {
            int numSingular = Math.Min(W.numRows, W.numCols);

            checkSvdMatrixSize(U, tranU, W, V, tranV);

            for (int i = 0; i < numSingular; i++)
            {
                double bigValue = -1;
                int bigIndex = -1;

                // find the smallest singular value in the submatrix
                for (int j = i; j < numSingular; j++)
                {
                    double v = W.get(j, j);

                    if (v > bigValue)
                    {
                        bigValue = v;
                        bigIndex = j;
                    }
                }

                // only swap if the current index is not the smallest
                if (bigIndex == i)
                    continue;

                if (bigIndex == -1)
                {
                    // there is at least one uncountable singular value.  just stop here
                    break;
                }

                double tmp = W.get(i, i);
                W.set(i, i, bigValue);
                W.set(bigIndex, bigIndex, tmp);

                if (V != null)
                {
                    swapRowOrCol(V, tranV, i, bigIndex);
                }

                if (U != null)
                {
                    swapRowOrCol(U, tranU, i, bigIndex);
                }
            }
        }

        /**
         * <p>
         * Similar to {@link #descendingOrder(DMatrixRMaj, bool, DMatrixRMaj, DMatrixRMaj, bool)}
         * but takes in an array of singular values instead.
         * </p>
         *
         * @param U Matrix. Modified.
         * @param tranU is U transposed or not.
         * @param singularValues Array of singular values. Modified.
         * @param singularLength Number of elements in singularValues array
         * @param V Matrix. Modified.
         * @param tranV is V transposed or not.
         */
        public static void descendingOrder( DMatrixRMaj U, bool tranU,
                                            double[] singularValues,
                                            int singularLength,
                                             DMatrixRMaj V, bool tranV)
        {
            //        checkSvdMatrixSize(U, tranU, W, V, tranV);

            for (int i = 0; i < singularLength; i++)
            {
                double bigValue = -1;
                int bigIndex = -1;

                // find the smallest singular value in the submatrix
                for (int j = i; j < singularLength; j++)
                {
                    double v = singularValues[j];

                    if (v > bigValue)
                    {
                        bigValue = v;
                        bigIndex = j;
                    }
                }

                // only swap if the current index is not the smallest
                if (bigIndex == i)
                    continue;

                if (bigIndex == -1)
                {
                    // there is at least one uncountable singular value.  just stop here
                    break;
                }

                double tmp = singularValues[i];
                singularValues[i] = bigValue;
                singularValues[bigIndex] = tmp;

                if (V != null)
                {
                    swapRowOrCol(V, tranV, i, bigIndex);
                }

                if (U != null)
                {
                    swapRowOrCol(U, tranU, i, bigIndex);
                }
            }
        }

        /**
         * Checks to see if all the provided matrices are the expected size for an SVD.  If an error is encountered
         * then an exception is thrown.  This automatically handles compact and non-compact formats
         */
        public static void checkSvdMatrixSize( DMatrixRMaj U, bool tranU, DMatrixRMaj W,
                                                DMatrixRMaj V, bool tranV)
        {
            int numSingular = Math.Min(W.numRows, W.numCols);
            bool compact = W.numRows == W.numCols;

            if (compact)
            {
                if (U != null)
                {
                    if (tranU && U.numRows != numSingular)
                        throw new ArgumentException("Unexpected size of matrix U");
                    else if (!tranU && U.numCols != numSingular)
                        throw new ArgumentException("Unexpected size of matrix U");
                }

                if (V != null)
                {
                    if (tranV && V.numRows != numSingular)
                        throw new ArgumentException("Unexpected size of matrix V");
                    else if (!tranV && V.numCols != numSingular)
                        throw new ArgumentException("Unexpected size of matrix V");
                }
            }
            else
            {
                if (U != null && U.numRows != U.numCols)
                    throw new ArgumentException("Unexpected size of matrix U");
                if (V != null && V.numRows != V.numCols)
                    throw new ArgumentException("Unexpected size of matrix V");
                if (U != null && U.numRows != W.numRows)
                    throw new ArgumentException("Unexpected size of W");
                if (V != null && V.numRows != W.numCols)
                    throw new ArgumentException("Unexpected size of W");
            }
        }

        private static void swapRowOrCol(DMatrixRMaj M, bool tran, int i, int bigIndex)
        {
            double tmp;
            if (tran)
            {
                // swap the rows
                for (int col = 0; col < M.numCols; col++)
                {
                    tmp = M.get(i, col);
                    M.set(i, col, M.get(bigIndex, col));
                    M.set(bigIndex, col, tmp);
                }
            }
            else
            {
                // swap the columns
                for (int row = 0; row < M.numRows; row++)
                {
                    tmp = M.get(row, i);
                    M.set(row, i, M.get(row, bigIndex));
                    M.set(row, bigIndex, tmp);
                }
            }
        }

        /**
         * <p>
         * Returns the null-space from the singular value decomposition. The null space is a set of non-zero vectors that
         * when multiplied by the original matrix return zero.
         * </p>
         *
         * <p>
         * The null space is found by extracting the columns in V that are associated singular values less than
         * or equal to the threshold. In some situations a non-compact SVD is required.
         * </p>
         *
         * @param svd A precomputed decomposition.  Not modified.
         * @param nullSpace Storage for null space.  Will be reshaped as needed.  Modified.
         * @param tol Threshold for selecting singular values.  Try UtilEjml.EPS.
         * @return The null space.
         */
        public static DMatrixRMaj nullSpace(SingularValueDecomposition_F64<DMatrixRMaj> svd,
                                              DMatrixRMaj nullSpace, double tol)
        {
            int N = svd.numberOfSingularValues();
            double[] s = svd.getSingularValues();

            DMatrixRMaj V = svd.getV(null, true);

            if (V.numRows != svd.numCols())
            {
                throw new ArgumentException("Can't compute the null space using a compact SVD for a matrix of this size.");
            }

            // first determine the size of the null space
            int numVectors = svd.numCols() - N;

            for (int i = 0; i < N; i++)
            {
                if (s[i] <= tol)
                {
                    numVectors++;
                }
            }

            // declare output data
            if (nullSpace == null)
            {
                nullSpace = new DMatrixRMaj(numVectors, svd.numCols());
            }
            else
            {
                nullSpace.reshape(numVectors, svd.numCols());
            }

            // now extract the vectors
            int count = 0;
            for (int i = 0; i < N; i++)
            {
                if (s[i] <= tol)
                {
                    CommonOps_DDRM.extract(V, i, i + 1, 0, V.numCols, nullSpace, count++, 0);
                }
            }
            for (int i = N; i < svd.numCols(); i++)
            {
                CommonOps_DDRM.extract(V, i, i + 1, 0, V.numCols, nullSpace, count++, 0);
            }

            CommonOps_DDRM.transpose(nullSpace);

            return nullSpace;
        }

        /**
         * Computes the null space using QR decomposition. This is much faster than using SVD
         *
         * @param A (Input) Matrix
         * @param totalSingular Number of singular values
         * @return Null space
         */
        public static DMatrixRMaj nullspaceQR(DMatrixRMaj A, int totalSingular)
        {
            SolveNullSpaceQR_DDRM solver = new SolveNullSpaceQR_DDRM();

            DMatrixRMaj nullspace = new DMatrixRMaj(1, 1);

            if (!solver.process(A, totalSingular, nullspace))
                throw new SystemException("Solver failed. try SVD based method instead?");

            return nullspace;
        }

        /**
         * Computes the null space using QRP decomposition. This is faster than using SVD but slower than QR.
         * Much more stable than QR though.
         *
         * @param A (Input) Matrix
         * @param totalSingular Number of singular values
         * @return Null space
         */
        public static DMatrixRMaj nullspaceQRP(DMatrixRMaj A, int totalSingular)
        {
            SolveNullSpaceQRP_DDRM solver = new SolveNullSpaceQRP_DDRM();

            DMatrixRMaj nullspace = new DMatrixRMaj(1, 1);

            if (!solver.process(A, totalSingular, nullspace))
                throw new SystemException("Solver failed. try SVD based method instead?");

            return nullspace;
        }

        /**
         * Computes the null space using SVD. Slowest bust most stable way to find the solution
         *
         * @param A (Input) Matrix
         * @param totalSingular Number of singular values
         * @return Null space
         */
        public static DMatrixRMaj nullspaceSVD(DMatrixRMaj A, int totalSingular)
        {
            SolveNullSpace<DMatrixRMaj> solver = new SolveNullSpaceSvd_DDRM();

            DMatrixRMaj nullspace = new DMatrixRMaj(1, 1);

            if (!solver.process(A, totalSingular, nullspace))
                throw new SystemException("Solver failed. try SVD based method instead?");

            return nullspace;
        }

        /**
         * <p>
         * The vector associated will the smallest singular value is returned as the null space
         * of the decomposed system.  A right null space is returned if 'isRight' is set to true,
         * and a left null space if false.
         * </p>
         *
         * @param svd A precomputed decomposition.  Not modified.
         * @param isRight true for right null space and false for left null space.  Right is more commonly used.
         * @param nullVector Optional storage for a vector for the null space.  Modified.
         * @return Vector in V associated with smallest singular value..
         */
        public static DMatrixRMaj nullVector(SingularValueDecomposition_F64<DMatrixRMaj> svd,
                                              bool isRight,
                                               DMatrixRMaj nullVector )
        {
            int N = svd.numberOfSingularValues();
            double[] s = svd.getSingularValues();

            DMatrixRMaj A = isRight ? svd.getV(null, true) : svd.getU(null, false);

            if (isRight)
            {
                if (A.numRows != svd.numCols())
                {
                    throw new ArgumentException("Can't compute the null space using a compact SVD for a matrix of this size.");
                }

                if (nullVector == null)
                {
                    nullVector = new DMatrixRMaj(svd.numCols(), 1);
                }
                else
                {
                    nullVector.reshape(svd.numCols(), 1);
                }
            }
            else
            {
                if (A.numCols != svd.numRows())
                {
                    throw new ArgumentException("Can't compute the null space using a compact SVD for a matrix of this size.");
                }

                if (nullVector == null)
                {
                    nullVector = new DMatrixRMaj(svd.numRows(), 1);
                }
                else
                {
                    nullVector.reshape(svd.numRows(), 1);
                }
            }

            int smallestIndex = -1;

            if (isRight && svd.numCols() > svd.numRows())
                smallestIndex = svd.numCols() - 1;
            else if (!isRight && svd.numCols() < svd.numRows())
                smallestIndex = svd.numRows() - 1;
            else
            {
                // find the smallest singular value
                double smallestValue = Double.MaxValue;

                for (int i = 0; i < N; i++)
                {
                    if (s[i] < smallestValue)
                    {
                        smallestValue = s[i];
                        smallestIndex = i;
                    }
                }
            }

            // extract the null space
            if (isRight)
                SpecializedOps_DDRM.subvector(A, smallestIndex, 0, A.numRows, true, 0, nullVector);
            else
                SpecializedOps_DDRM.subvector(A, 0, smallestIndex, A.numRows, false, 0, nullVector);

            return nullVector;
        }

        /**
         * Returns a reasonable threshold for singular values.<br><br>
         *
         * tol = max (size (A)) * largest sigma * eps;
         *
         * @param svd A precomputed decomposition.  Not modified.
         * @return threshold for singular values
         */
        public static double singularThreshold<T>(SingularValueDecomposition_F64<T> svd)
            where T : Matrix
        {
            return singularThreshold(svd, UtilEjml.EPS);
        }

        public static double singularThreshold<T>(SingularValueDecomposition_F64<T> svd, double tolerance)
            where T:Matrix
        {

            double[] w = svd.getSingularValues();

            int N = svd.numberOfSingularValues();

            return singularThreshold(w, N, tolerance);
        }

        private static double singularThreshold(double[] w, int N)
        {
            return singularThreshold(w, N, UtilEjml.EPS);
        }

        private static double singularThreshold(double[] w, int N, double tolerance)
        {
            double largest = 0;
            for (int j = 0; j < N; j++)
            {
                if (w[j] > largest)
                    largest = w[j];
            }

            return N * largest * tolerance;
        }

        /**
         * Extracts the rank of a matrix using a preexisting decomposition and default threshold.
         *
         * @param svd A precomputed decomposition.  Not modified.
         * @return The rank of the decomposed matrix.
         * @see #singularThreshold(SingularValueDecomposition_F64)
         */
        public static int rank<T>(SingularValueDecomposition_F64<T> svd)
            where T:Matrix
        {
            double threshold = singularThreshold(svd);
            return rank(svd, threshold);
        }

        /**
         * Extracts the rank of a matrix using a preexisting decomposition.
         *
         * @param svd A precomputed decomposition.  Not modified.
         * @param threshold Tolerance used to determine of a singular value is singular.
         * @return The rank of the decomposed matrix.
         * @see #singularThreshold(SingularValueDecomposition_F64)
         */
        public static int rank<T>(SingularValueDecomposition_F64<T> svd, double threshold)
            where T : Matrix
        {
            int numRank = 0;

            double[] w = svd.getSingularValues();

            int N = svd.numberOfSingularValues();

            for (int j = 0; j < N; j++)
            {
                if (w[j] > threshold)
                    numRank++;
            }

            return numRank;
        }

        /**
         * Extracts the nullity of a matrix using a preexisting decomposition and default threshold.
         *
         * @param svd A precomputed decomposition.  Not modified.
         * @return The nullity of the decomposed matrix.
         * @see #singularThreshold(SingularValueDecomposition_F64)
         */
        public static int nullity<T>(SingularValueDecomposition_F64<T> svd)
            where T : Matrix
        {
            double threshold = singularThreshold(svd);
            return nullity(svd, threshold);
        }

        /**
         * Extracts the nullity of a matrix using a preexisting decomposition.
         *
         * @param svd A precomputed decomposition.  Not modified.
         * @param threshold Tolerance used to determine of a singular value is singular.
         * @return The nullity of the decomposed matrix.
         * @see #singularThreshold(SingularValueDecomposition_F64)
         */
        public static int nullity<T>(SingularValueDecomposition_F64<T> svd, double threshold)
            where T : Matrix
        {
            int ret = 0;

            double[] w = svd.getSingularValues();

            int N = svd.numberOfSingularValues();

            int numCol = svd.numCols();

            for (int j = 0; j < N; j++)
            {
                if (w[j] <= threshold) ret++;
            }
            return ret + numCol - N;
        }

        /**
         * Returns the matrix's nullity
         *
         * @param A Matrix. Not modified.
         * @param threshold Tolerance used to determine of a singular value is singular.
         * @return nullity
         */
        //public static int nullity(DMatrixRMaj A, double threshold)
        //{
        //    SingularValueDecomposition_F64<DMatrixRMaj> svd = DecompositionFactory_DDRM.svd(A.numRows, A.numCols, false, true, true);

        //    if (svd.inputModified())
        //    {
        //        A = A.copy();
        //    }
        //    if (!svd.decompose(A))
        //    {
        //        throw new SystemException("SVD Failed!");
        //    }

        //    double[] sv = svd.getSingularValues();

        //    int count = 0;
        //    for (int i = 0; i < sv.Count(); i++)
        //    {
        //        if (sv[i] <= threshold)
        //        {
        //            count++;
        //        }
        //    }
        //    return count;
        //}

    }
}