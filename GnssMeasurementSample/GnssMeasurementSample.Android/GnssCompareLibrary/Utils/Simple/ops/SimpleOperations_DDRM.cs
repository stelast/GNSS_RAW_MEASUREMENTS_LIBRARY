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
    public class SimpleOperations_DDRM : SimpleOperations<Matrix>
    {
        public void changeSign(Matrix a)
        {
            CommonOps_DDRM.changeSign((DMatrixRMaj)a);
        }

        public double conditionP2(Matrix A)
        {
            //return NormOps_DDRM.conditionP2(A);
            return 0;
        }


        public double determinant(Matrix A)
        {
            return CommonOps_DDRM.det((DMatrixRMaj)A);
        }

        public Matrix diag(Matrix A)
        {
            throw new NotImplementedException();
        }

        public void divide(Matrix A, double val, Matrix output)
        {
            CommonOps_DDRM.divide((DMatrixRMaj)A, (double)val, (DMatrixRMaj)output);
        }

        public double dot(Matrix A, Matrix v)
        {
            return VectorVectorMult_DDRM.innerProd((DMatrixRMaj)A, (DMatrixRMaj)v);
        }

        public void elementDiv(Matrix A, Matrix B, Matrix output)
        {
            CommonOps_DDRM.elementDiv((DMatrixRMaj)A, (DMatrixRMaj)B, (DMatrixRMaj)output);
        }

        public void elementExp(Matrix A, Matrix output)
        {
            CommonOps_DDRM.elementExp((DMatrixRMaj)A, (DMatrixRMaj)output);
        }

        public void elementLog(Matrix A, Matrix output)
        {
            CommonOps_DDRM.elementLog((DMatrixRMaj)A, (DMatrixRMaj)output);
        }

        public double elementMaxAbs(Matrix A)
        {
            return CommonOps_DDRM.elementMaxAbs((DMatrixRMaj)A);
        }

        public double elementMinAbs(Matrix A)
        {
            return CommonOps_DDRM.elementMinAbs((DMatrixRMaj)A);
        }

        public void elementMult(Matrix A, Matrix B, Matrix output)
        {
            CommonOps_DDRM.elementMult((DMatrixRMaj)A, (DMatrixRMaj)B, (DMatrixRMaj)output);
        }

        public void elementPower(Matrix A, Matrix B, Matrix output)
        {
            CommonOps_DDRM.elementPower((DMatrixRMaj)A, (DMatrixRMaj)B, (DMatrixRMaj)output);
        }

        public void elementPower(Matrix A, double b, Matrix output)
        {
            CommonOps_DDRM.elementPower((DMatrixRMaj)A, (double)b, (DMatrixRMaj)output);
        }

        public double elementSum(Matrix A)
        {
            return CommonOps_DDRM.elementSum((DMatrixRMaj)A); 
        }

        public void extract(Matrix src, int srcY0, int srcY1, int srcX0, int srcX1, Matrix dst, int dstY0, int dstX0)
        {
            CommonOps_DDRM.extract((DMatrixRMaj)src, srcY0, srcY1, srcX0, srcX1, (DMatrixRMaj)dst, dstY0, dstX0);
        }

        public void fill(Matrix A, double value)
        {
            CommonOps_DDRM.fill((DMatrixRMaj)A, (double)value);
        }

        public double get(Matrix A, int row, int column)
        {
            return (double)((DMatrixRMaj)A).get(row, column);
        }

        public void get(Matrix A, int row, int column, Complex_F64 value)
        {
            value.real = ((DMatrixRMaj)A).get(row, column);
            value.imaginary = 0;
        }

        public bool hasUncountable(Matrix M)
        {
            return MatrixFeatures_DDRM.hasUncountable((DMatrixRMaj)M);
        }

        public bool invert(Matrix A, Matrix output)
        {
            return CommonOps_DDRM.invert((DMatrixRMaj)A, (DMatrixRMaj)output);
        }

        public bool isIdentical(Matrix A, Matrix B, double tol)
        {
            return MatrixFeatures_DDRM.isIdentical((DMatrixRMaj)A, (DMatrixRMaj)B, (double)tol);
        }

        public void kron(Matrix A, Matrix B, Matrix output)
        {
            CommonOps_DDRM.kron((DMatrixRMaj)A, (DMatrixRMaj)B, (DMatrixRMaj)output);
        }

        public void minus(Matrix A, Matrix B, Matrix output)
        {
            CommonOps_DDRM.subtract((DMatrixRMaj)A, (DMatrixRMaj)B, (DMatrixRMaj)output);
        }

        public void minus(Matrix A, double b, Matrix output)
        {
            CommonOps_DDRM.subtract((DMatrixRMaj)A, (double)b, (DMatrixRMaj)output);
        }

        public void mult(Matrix A, Matrix B, Matrix output)
        {
            CommonOps_DDRM.mult((DMatrixRMaj)A, (DMatrixRMaj)B, (DMatrixRMaj)output);
        }

        public void multTransA(Matrix A, Matrix B, Matrix output)
        {
            CommonOps_DDRM.multTransA((DMatrixRMaj)A, (DMatrixRMaj)B, (DMatrixRMaj)output);
        }

        public double normF(Matrix A)
        {
            return NormOps_DDRM.normF((DMatrixRMaj)A);
        }

        public void plus(Matrix A, Matrix B, Matrix output)
        {
            CommonOps_DDRM.add((DMatrixRMaj)A, (DMatrixRMaj)B, (DMatrixRMaj)output);
        }

        public void plus(Matrix A, double b, Matrix output)
        {
            CommonOps_DDRM.add((DMatrixRMaj)A, (double)b, (DMatrixRMaj)output);
        }

        public void plus(Matrix A, double beta, Matrix b, Matrix output)
        {
            CommonOps_DDRM.add((DMatrixRMaj)A, (double)beta, (DMatrixRMaj)b, (DMatrixRMaj)output);
        }

        public void plus(double alpha, Matrix A, double beta, Matrix b, Matrix output)
        {
            CommonOps_DDRM.add((double)alpha, (DMatrixRMaj)A, (double)beta, (DMatrixRMaj)b, (DMatrixRMaj)output);
        }

        public void pseudoInverse(Matrix A, Matrix output)
        {
            //CommonOps_DDRM.pinv(A, output);
        }

        public void scale(Matrix A, double val, Matrix output)
        {
            CommonOps_DDRM.scale((double)val, (DMatrixRMaj)A, (DMatrixRMaj)output);   
        }

        public void set(Matrix A, int row, int column, double value)
        {
            ((DMatrixRMaj)A).set(row, column, (double)value);
        }

        public void set(Matrix A, int row, int column, double real, double imaginary)
        {
            throw new ArgumentException("Does not support imaginary values");
        }

        public void setColumn(Matrix A, int column, int startRow, params double[] values)
        {
            for (int i = 0; i < values.Count(); i++)
            {
                ((DMatrixRMaj)A).set(startRow + i, column, (double)values[i]);
            }
        }

        public void setIdentity(Matrix A)
        {
            CommonOps_DDRM.setIdentity((DMatrixRMaj)A);
        }

        public void setRow(Matrix A, int row, int startColumn, params double[] values)
        {
            for (int i = 0; i < values.Count(); i++)
            {
                ((DMatrixRMaj)A).set(row, startColumn + i, (double)values[i]);
            }
        }

        public bool solve(Matrix A, Matrix X, Matrix B)
        {
            return CommonOps_DDRM.solve((DMatrixRMaj)A, (DMatrixRMaj)B, (DMatrixRMaj)X);
        }

        public double trace(Matrix A)
        {
            return CommonOps_DDRM.trace((DMatrixRMaj)A);
        }

        public void transpose(Matrix input, Matrix output)
        {
            CommonOps_DDRM.transpose((DMatrixRMaj)input, (DMatrixRMaj)output);
        }

        public void zero(Matrix A)
        {
            A.zero();
        }
    }
}