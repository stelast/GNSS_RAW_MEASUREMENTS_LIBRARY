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
    public class SimpleOperations_ZDRM : SimpleOperations<ZMatrixRMaj>
    {
        public void set(ZMatrixRMaj A, int row, int column, /**/double value)
        {
            A.set(row, column, (double)value, 0);
        }


        public void set(ZMatrixRMaj A, int row, int column, /**/double real, /**/double imaginary)
        {
            A.set(row, column, (double)real, (double)imaginary);
        }


        public /**/double get(ZMatrixRMaj A, int row, int column)
        {
            return (double)A.getReal(row, column);
        }


        public void get(ZMatrixRMaj A, int row, int column, Complex_F64 value)
        {
            int index = A.getIndex(row, column);
            value.real = A.data[index];
            value.imaginary = A.data[index + 1];
        }


        public void fill(ZMatrixRMaj A, /**/double value)
        {
            CommonOps_ZDRM.fill(A, (double)value, 0);
        }


        public void transpose(ZMatrixRMaj input, ZMatrixRMaj output)
        {
            CommonOps_ZDRM.transpose(input, output);
        }


        public void mult(ZMatrixRMaj A, ZMatrixRMaj B, ZMatrixRMaj output)
        {
            CommonOps_ZDRM.mult(A, B, output);
        }


        public void multTransA(ZMatrixRMaj A, ZMatrixRMaj B, ZMatrixRMaj output)
        {
            CommonOps_ZDRM.multTransA(A, B, output);
        }


        public void kron(ZMatrixRMaj A, ZMatrixRMaj B, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.kron(A,B,output);
            throw new NotSupportedException();
        }


        public void plus(ZMatrixRMaj A, ZMatrixRMaj B, ZMatrixRMaj output)
        {
            CommonOps_ZDRM.add(A, B, output);
        }


        public void minus(ZMatrixRMaj A, ZMatrixRMaj B, ZMatrixRMaj output)
        {
            CommonOps_ZDRM.subtract(A, B, output);
        }


        public void minus(ZMatrixRMaj A, /**/double b, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.subtract(A, (double)b, output);
            throw new NotSupportedException();
        }


        public void plus(ZMatrixRMaj A, /**/double b, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.add(A, (double)b, output);
            throw new NotSupportedException();
        }


        public void plus(ZMatrixRMaj A, /**/double beta, ZMatrixRMaj b, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.add(A, (double)beta, b, output);
            throw new NotSupportedException();
        }


        public void plus( /**/double alpha, ZMatrixRMaj A, /**/double beta, ZMatrixRMaj b, ZMatrixRMaj output)
        {
            throw new NotSupportedException();
        }


        public /**/double dot(ZMatrixRMaj A, ZMatrixRMaj v)
        {
            //        return VectorVectorMult_DDRM.innerProd(A, v);
            throw new NotSupportedException();
        }


        public void scale(ZMatrixRMaj A, /**/double val, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.scale( (double)val, 0,A,output);
            throw new NotSupportedException();
        }


        public void divide(ZMatrixRMaj A, /**/double val, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.divide( A, (double)val,output);
            throw new NotSupportedException();
        }


        public bool invert(ZMatrixRMaj A, ZMatrixRMaj output)
        {
            return CommonOps_ZDRM.invert(A, output);
        }


        public void setIdentity(ZMatrixRMaj A)
        {
            CommonOps_ZDRM.setIdentity(A);
        }


        public void pseudoInverse(ZMatrixRMaj A, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.pinv(A,output);
            throw new NotSupportedException();
        }


        public bool solve(ZMatrixRMaj A, ZMatrixRMaj X, ZMatrixRMaj B)
        {
            return CommonOps_ZDRM.solve(A, B, X);
        }


        public void zero(ZMatrixRMaj A)
        {
            A.zero();
        }


        public /**/double normF(ZMatrixRMaj A)
        {
            return NormOps_ZDRM.normF(A);
        }


        public /**/double conditionP2(ZMatrixRMaj A)
        {
            //        return NormOps_ZDRM.conditionP2(A);
            throw new NotSupportedException();
        }


        public /**/double determinant(ZMatrixRMaj A)
        {
            return CommonOps_ZDRM.det(A).real;
        }


        public /**/double trace(ZMatrixRMaj A)
        {
            //        return CommonOps_ZDRM.trace(A);
            throw new NotSupportedException();
        }


        public void setRow(ZMatrixRMaj A, int row, int startColumn, /**/double[] values )
        {
            for (int i = 0; i < values.Count(); i++)
            {
                A.set(row, startColumn + i, (double)values[i], 0);
            }
        }


        public void setColumn(ZMatrixRMaj A, int column, int startRow,  /**/double[] values )
        {
            for (int i = 0; i < values.Count(); i++)
            {
                A.set(startRow + i, column, (double)values[i], 0);
            }
        }


        public void extract(ZMatrixRMaj src, int srcY0, int srcY1, int srcX0, int srcX1, ZMatrixRMaj dst, int dstY0, int dstX0)
        {
            CommonOps_ZDRM.extract(src, srcY0, srcY1, srcX0, srcX1, dst, dstY0, dstX0);
        }


        public ZMatrixRMaj diag(ZMatrixRMaj A)
        {
            ZMatrixRMaj output;
            if (MatrixFeatures_ZDRM.isVector(A))
            {
                int N = Math.Max(A.numCols, A.numRows);
                output = new ZMatrixRMaj(N, N);
                CommonOps_ZDRM.diag(output, N, A.data);
            }
            else
            {
                int N = Math.Min(A.numCols, A.numRows);
                output = new ZMatrixRMaj(N, 1);
                CommonOps_ZDRM.extractDiag(A, output);
            }
            return output;
        }


        public bool hasUncountable(ZMatrixRMaj M)
        {
            return MatrixFeatures_ZDRM.hasUncountable(M);
        }


        public void changeSign(ZMatrixRMaj a)
        {
            //        CommonOps_ZDRM.changeSign(a);
            throw new NotSupportedException();
        }


        public /**/double elementMaxAbs(ZMatrixRMaj A)
        {
            return CommonOps_ZDRM.elementMaxAbs(A);
        }


        public /**/double elementMinAbs(ZMatrixRMaj A)
        {
            return CommonOps_ZDRM.elementMinAbs(A);
        }


        public /**/double elementSum(ZMatrixRMaj A)
        {
            //        return CommonOps_ZDRM.elementSum(A);
            throw new NotSupportedException();
        }


        public void elementMult(ZMatrixRMaj A, ZMatrixRMaj B, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.elementMult(A,B,output);
            throw new NotSupportedException();
        }


        public void elementDiv(ZMatrixRMaj A, ZMatrixRMaj B, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.elementDiv(A,B,output);
            throw new NotSupportedException();
        }


        public void elementPower(ZMatrixRMaj A, ZMatrixRMaj B, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.elementPower(A,B,output);
            throw new NotSupportedException();
        }


        public void elementPower(ZMatrixRMaj A, /**/double b, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.elementPower(A, (double)b, output);
            throw new NotSupportedException();
        }


        public void elementExp(ZMatrixRMaj A, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.elementExp(A,output);
            throw new NotSupportedException();
        }


        public void elementLog(ZMatrixRMaj A, ZMatrixRMaj output)
        {
            //        CommonOps_ZDRM.elementLog(A,output);
            throw new NotSupportedException();
        }


        public bool isIdentical(ZMatrixRMaj A, ZMatrixRMaj B, /**/double tol)
        {
            return MatrixFeatures_ZDRM.isIdentical(A, B, (double)tol);
        }

    }
}