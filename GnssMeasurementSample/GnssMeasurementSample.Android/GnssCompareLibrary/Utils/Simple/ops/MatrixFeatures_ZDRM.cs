﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class MatrixFeatures_ZDRM
    {
        /**
         * Checks to see if the matrix is a vector or not.
         *
         * @param mat A matrix. Not modified.
         * @return True if it is a vector and false if it is not.
         */
        public static bool isVector(Matrix mat)
        {
            return (mat.NumCols == 1 || mat.NumRows == 1);
        }

        /**
         * <p>
         * Checks to see if the two matrices are the negative of each other:<br>
         * <br>
         * a<sub>ij</sub> = -b<sub>ij</sub>
         * </p>
         *
         * @param a First matrix.  Not modified.
         * @param b Second matrix.  Not modified.
         * @param tol Numerical tolerance.
         * @return True if they are the negative of each other within tolerance.
         */
        public static bool isNegative(ZMatrixD1 a, ZMatrixD1 b, double tol)
        {
            if (a.numRows != b.numRows || a.numCols != b.numCols)
                throw new ArgumentException("Matrix dimensions must match");

            int length = a.NumElements * 2;

            for (int i = 0; i < length; i++)
            {
                if (!(Math.Abs(a.data[i] + b.data[i]) <= tol))
                    return false;
            }

            return true;
        }

        /**
         * Checks to see if any element in the matrix is NaN.
         *
         * @param m A matrix. Not modified.
         * @return True if any element in the matrix is NaN.
         */
        public static bool hasNaN(ZMatrixD1 m)
        {
            int length = m.DataLength;

            for (int i = 0; i < length; i++)
            {
                if (Double.IsNaN(m.data[i]))
                    return true;
            }
            return false;
        }

        /**
         * Checks to see if any element in the matrix is NaN of Infinite.
         *
         * @param m A matrix. Not modified.
         * @return True if any element in the matrix is NaN of Infinite.
         */
        public static bool hasUncountable(ZMatrixD1 m)
        {
            int length = m.DataLength;

            for (int i = 0; i < length; i++)
            {
                double a = m.data[i];
                if (Double.IsNaN(a) || Double.IsInfinity(a))
                    return true;
            }
            return false;
        }

        /**
         * <p>
         * Checks to see if each element in the two matrices are equal:
         * a<sub>ij</sub> == b<sub>ij</sub>
         * <p>
         *
         * <p>
         * NOTE: If any of the elements are NaN then false is returned.  If two corresponding
         * elements are both positive or negative infinity then they are equal.
         * </p>
         *
         * @param a A matrix. Not modified.
         * @param b A matrix. Not modified.
         * @return true if identical and false otherwise.
         */
        public static bool isEquals(ZMatrixD1 a, ZMatrixD1 b)
        {
            if (a.numRows != b.numRows || a.numCols != b.numCols)
            {
                return false;
            }

             int length = a.DataLength;
            for (int i = 0; i < length; i++)
            {
                if (!(a.data[i] == b.data[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * <p>
         * Checks to see if each element in the two matrices are within tolerance of
         * each other: tol &ge; |a<sub>ij</sub> - b<sub>ij</sub>|.
         * <p>
         *
         * <p>
         * NOTE: If any of the elements are not countable then false is returned.<br>
         * NOTE: If a tolerance of zero is passed in this is equivalent to calling
         * {@link #isEquals(ZMatrixD1, ZMatrixD1)}
         * </p>
         *
         * @param a A matrix. Not modified.
         * @param b A matrix. Not modified.
         * @param tol How close to being identical each element needs to be.
         * @return true if equals and false otherwise.
         */
        public static bool isEquals(ZMatrixD1 a, ZMatrixD1 b, double tol)
        {
            if (a.numRows != b.numRows || a.numCols != b.numCols)
            {
                return false;
            }

            if (tol == 0.0)
                return isEquals(a, b);

             int length = a.DataLength;

            for (int i = 0; i < length; i++)
            {
                if (!(tol >= Math.Abs(a.data[i] - b.data[i])))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * <p>
         * Checks to see if each corresponding element in the two matrices are
         * within tolerance of each other or have the some symbolic meaning.  This
         * can handle NaN and Infinite numbers.
         * <p>
         *
         * <p>
         * If both elements are countable then the following equality test is used:<br>
         * |a<sub>ij</sub> - b<sub>ij</sub>| &le; tol.<br>
         * Otherwise both numbers must both be Double.NaN, Double.POSITIVE_INFINITY, or
         * Double.NEGATIVE_INFINITY to be identical.
         * </p>
         *
         * @param a A matrix. Not modified.
         * @param b A matrix. Not modified.
         * @param tol Tolerance for equality.
         * @return true if identical and false otherwise.
         */
        public static bool isIdentical(ZMatrixD1 a, ZMatrixD1 b, double tol)
        {
            if (a.numRows != b.numRows || a.numCols != b.numCols)
            {
                return false;
            }
            if (tol < 0)
                throw new ArgumentException("Tolerance must be greater than or equal to zero.");

             int length = a.DataLength;
            for (int i = 0; i < length; i++)
            {
                double valA = a.data[i];
                double valB = b.data[i];

                // if either is negative or positive infinity the result will be positive infinity
                // if either is NaN the result will be NaN
                double diff = Math.Abs(valA - valB);

                // diff = NaN == false
                // diff = infinity == false
                if (tol >= diff)
                    continue;

                if (Double.IsNaN(valA))
                {
                    return Double.IsNaN(valB);
                }
                else if (Double.IsInfinity(valA))
                {
                    return valA == valB;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * Checks to see if the provided matrix is within tolerance to an identity matrix.
         *
         * @param mat Matrix being examined.  Not modified.
         * @param tol Tolerance.
         * @return True if it is within tolerance to an identify matrix.
         */
        public static bool isIdentity(ZMatrix mat, double tol)
        {
            // see if the result is an identity matrix
            Complex_F64 c = new Complex_F64();
            for (int i = 0; i < mat.NumRows; i++)
            {
                for (int j = 0; j < mat.NumCols; j++)
                {
                    mat.get(i, j, c);
                    if (i == j)
                    {
                        if (!(Math.Abs(c.real - 1) <= tol))
                            return false;
                        if (!(Math.Abs(c.imaginary) <= tol))
                            return false;
                    }
                    else
                    {
                        if (!(Math.Abs(c.real) <= tol))
                            return false;
                        if (!(Math.Abs(c.imaginary) <= tol))
                            return false;
                    }
                }
            }

            return true;
        }

        /**
         * <p>Hermitian matrix is a square matrix with complex entries that are equal to its own conjugate transpose.</p>
         *
         * <p>a[i,j] = conj(a[j,i])</p>
         *
         * @param Q The matrix being tested. Not modified.
         * @param tol Tolerance.
         * @return True if it passes the test.
         */
        public static bool isHermitian(ZMatrixRMaj Q, double tol)
        {
            if (Q.numCols != Q.numRows)
                return false;

            Complex_F64 a = new Complex_F64();
            Complex_F64 b = new Complex_F64();

            for (int i = 0; i < Q.numCols; i++)
            {
                for (int j = i; j < Q.numCols; j++)
                {
                    Q.get(i, j, a);
                    Q.get(j, i, b);

                    if (Math.Abs(a.real - b.real) > tol)
                        return false;
                    if (Math.Abs(a.imaginary + b.imaginary) > tol)
                        return false;
                }
            }

            return true;
        }

        /**
         * <p>
         * Unitary matrices have the following properties:<br><br>
         * Q*Q<sup>H</sup> = I
         * </p>
         * <p>
         * This is the complex equivalent of orthogonal matrix.
         * </p>
         *
         * @param Q The matrix being tested. Not modified.
         * @param tol Tolerance.
         * @return True if it passes the test.
         */
        public static bool isUnitary(ZMatrixRMaj Q, double tol)
        {
            if (Q.numRows < Q.numCols)
            {
                throw new ArgumentException("The number of rows must be more than or equal to the number of columns");
            }

            Complex_F64 prod = new Complex_F64();

            ZMatrixRMaj[] u = CommonOps_ZDRM.columnsToVector(Q, null);

            for (int i = 0; i < u.Length; i++)
            {
                ZMatrixRMaj a = u[i];

                VectorVectorMult_ZDRM.innerProdH(a, a, prod);

                if (Math.Abs(prod.real - 1) > tol)
                    return false;
                if (Math.Abs(prod.imaginary) > tol)
                    return false;

                for (int j = i + 1; j < u.Length; j++)
                {
                    VectorVectorMult_ZDRM.innerProdH(a, u[j], prod);

                    if (!(prod.getMagnitude2() <= tol * tol))
                        return false;
                }
            }

            return true;
        }

        /**
         * <p>
         * Checks to see if the matrix is positive definite.
         * </p>
         * <p>
         * x<sup>T</sup> A x &gt; 0<br>
         * for all x where x is a non-zero vector and A is a hermitian matrix.
         * </p>
         *
         * @param A square hermitian matrix. Not modified.
         * @return True if it is positive definite and false if it is not.
         */
        public static bool isPositiveDefinite(ZMatrixRMaj A)
        {
            if (A.numCols != A.numRows)
                return false;

            CholeskyDecompositionInner_ZDRM chol = new CholeskyDecompositionInner_ZDRM(true);
            if (chol.inputModified())
                A = A.copy();

            return chol.decompose(A);
        }

        /**
         * <p>
         * Checks to see if a matrix is upper triangular or Hessenberg. A Hessenberg matrix of degree N
         * has the following property:<br>
         * <br>
         * a<sub>ij</sub> &le; 0 for all i &lt; j+N<br>
         * <br>
         * A triangular matrix is a Hessenberg matrix of degree 0.
         * </p>
         *
         * @param A Matrix being tested.  Not modified.
         * @param hessenberg The degree of being hessenberg.
         * @param tol How close to zero the lower left elements need to be.
         * @return If it is an upper triangular/hessenberg matrix or not.
         */
        public static bool isUpperTriangle(ZMatrixRMaj A, int hessenberg, double tol)
        {
            tol *= tol;
            for (int i = hessenberg + 1; i < A.numRows; i++)
            {
                int maxCol = Math.Min(i - hessenberg, A.numCols);
                for (int j = 0; j < maxCol; j++)
                {
                    int index = (i * A.numCols + j) * 2;

                    double real = A.data[index];
                    double imag = A.data[index + 1];
                    double mag = real * real + imag * imag;

                    if (!(mag <= tol))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * <p>
         * Checks to see if a matrix is lower triangular or Hessenberg. A Hessenberg matrix of degree N
         * has the following property:<br>
         * <br>
         * a<sub>ij</sub> &le; 0 for all i &lt; j+N<br>
         * <br>
         * A triangular matrix is a Hessenberg matrix of degree 0.
         * </p>
         *
         * @param A Matrix being tested.  Not modified.
         * @param hessenberg The degree of being hessenberg.
         * @param tol How close to zero the lower left elements need to be.
         * @return If it is an upper triangular/hessenberg matrix or not.
         */
        public static bool isLowerTriangle(ZMatrixRMaj A, int hessenberg, double tol)
        {
            tol *= tol;
            for (int i = 0; i < A.numRows - hessenberg - 1; i++)
            {
                for (int j = i + hessenberg + 1; j < A.numCols; j++)
                {
                    int index = (i * A.numCols + j) * 2;

                    double real = A.data[index];
                    double imag = A.data[index + 1];
                    double mag = real * real + imag * imag;

                    if (!(mag <= tol))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * Checks to see all the elements in the matrix are zeros
         *
         * @param m A matrix. Not modified.
         * @return True if all elements are zeros or false if not
         */
        public static bool isZeros(ZMatrixD1 m, double tol)
        {
            int length = m.NumElements * 2;

            for (int i = 0; i < length; i++)
            {
                if (Math.Abs(m.data[i]) > tol)
                    return false;
            }
            return true;
        }
    }
}