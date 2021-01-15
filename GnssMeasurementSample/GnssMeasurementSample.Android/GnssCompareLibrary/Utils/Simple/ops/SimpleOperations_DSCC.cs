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
    public class SimpleOperations_DSCC : SimpleSparseOperations<DMatrixSparseCSC, DMatrixRMaj>
    {
        // Workspace variables
        public IGrowArray gw = new IGrowArray();
        public DGrowArray gx = new DGrowArray();


        public void set(DMatrixSparseCSC A, int row, int column, /**/double value)
        {
            A.set(row, column, (double)value);
        }


        public void set(DMatrixSparseCSC A, int row, int column, /**/double real, /**/double imaginary)
        {
            throw new ConvertToImaginaryException();
        }


        public /**/double get(DMatrixSparseCSC A, int row, int column)
        {
            return A.get(row, column);
        }


        public void get(DMatrixSparseCSC A, int row, int column, /**/Complex_F64 value)
        {
            value.real = A.get(row, column);
            value.imaginary = 0;
        }


        public void fill(DMatrixSparseCSC A, /**/double value)
        {
            if (value == 0)
            {
                A.zero();
            }
            else
            {
                throw new ConvertToDenseException();
            }
        }


        //public void transpose(DMatrixSparseCSC input, DMatrixSparseCSC output)
        //{
        //    CommonOps_DSCC.transpose(input, output, gw);
        //}


        //public void mult(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        //{
        //    CommonOps_DSCC.mult(A, B, output);
        //}


        //public void multTransA(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        //{
        //    var At = new DMatrixSparseCSC(1, 1);
        //    CommonOps_DSCC.transpose(A, At, gw);
        //    CommonOps_DSCC.mult(At, B, output, gw, gx);
        //}


        //public void extractDiag(DMatrixSparseCSC input, DMatrixRMaj output)
        //{
        //    CommonOps_DSCC.extractDiag(input, output);
        //}


        //public void multTransA(DMatrixSparseCSC A, DMatrixRMaj B, DMatrixRMaj output)
        //{
        //    CommonOps_DSCC.multTransA(A, B, output, null);
        //}


        //public void mult(DMatrixSparseCSC A, DMatrixRMaj B, DMatrixRMaj output)
        //{
        //    CommonOps_DSCC.mult(A, B, output);
        //}


        //public void kron(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        //{
        //    //        CommonOps_DSCC.kron(A,B,output);
        //    throw new RuntimeException("Unsupported");
        //}


        //public void plus(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        //{
        //    CommonOps_DSCC.add(1, A, 1, B, output, null, null);
        //}


        //public void minus(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        //{
        //    CommonOps_DSCC.add(1, A, -1, B, output, null, null);
        //}


        public void minus(DMatrixSparseCSC A, /**/double b, DMatrixSparseCSC output)
        {
            throw new ConvertToDenseException();
        }


        public void plus(DMatrixSparseCSC A, /**/double b, DMatrixSparseCSC output)
        {
            throw new ConvertToDenseException();
        }


        //public void plus(DMatrixSparseCSC A, /**/double beta, DMatrixSparseCSC b, DMatrixSparseCSC output)
        //{
        //    CommonOps_DSCC.add(1, A, (double)beta, b, output, gw, gx);
        //}


        //public void plus( /**/double alpha, DMatrixSparseCSC A, /**/double beta, DMatrixSparseCSC b, DMatrixSparseCSC output)
        //{
        //    CommonOps_DSCC.add((double)alpha, A, (double)beta, b, output, gw, gx);
        //}


        //public /**/double dot(DMatrixSparseCSC A, DMatrixSparseCSC v)
        //{
        //    return CommonOps_DSCC.dotInnerColumns(A, 0, v, 0, gw, gx);
        //}


        //public void scale(DMatrixSparseCSC A, /**/double val, DMatrixSparseCSC output)
        //{
        //    CommonOps_DSCC.scale((double)val, A, output);
        //}


        //public void divide(DMatrixSparseCSC A, /**/double val, DMatrixSparseCSC output)
        //{
        //    CommonOps_DSCC.divide(A, (double)val, output);
        //}


        //public bool invert(DMatrixSparseCSC A, DMatrixSparseCSC output)
        //{
        //    return solve(A, output, CommonOps_DSCC.identity(A.numRows, A.numCols));
        //}


        //public void setIdentity(DMatrixSparseCSC A)
        //{
        //    CommonOps_DSCC.setIdentity(A);
        //}


        //public void pseudoInverse(DMatrixSparseCSC A, DMatrixSparseCSC output)
        //{
        //    throw new RuntimeException("Unsupported");
        //}


        //public bool solve(DMatrixSparseCSC A, DMatrixSparseCSC X, DMatrixSparseCSC B)
        //{
        //    return CommonOps_DSCC.solve(A, X, B);
        //}

        //public bool solve(DMatrixSparseCSC A, DMatrixRMaj X, DMatrixRMaj B)
        //{
        //    return CommonOps_DSCC.solve(A, X, B);
        //}


        //public void zero(DMatrixSparseCSC A)
        //{
        //    A.zero();
        //}


        //public /**/double normF(DMatrixSparseCSC A)
        //{
        //    return NormOps_DSCC.normF(A);
        //}


        //public /**/double conditionP2(DMatrixSparseCSC A)
        //{
        //    throw new RuntimeException("Unsupported");
        //}


        //public /**/double determinant(DMatrixSparseCSC A)
        //{
        //    return CommonOps_DSCC.det(A);
        //}


        //public /**/double trace(DMatrixSparseCSC A)
        //{
        //    return CommonOps_DSCC.trace(A);
        //}


        //public void setRow(DMatrixSparseCSC A, int row, int startColumn, /**/double values )
        //{
        //    // TODO Update with a more efficient algorithm
        //    for (int i = 0; i < values.length; i++)
        //    {
        //        A.set(row, startColumn + i, (double)values[i]);
        //    }
        //    // check to see if value are zero, if so ignore them

        //    // Do a pass through the matrix and see how many elements need to be added

        //    // see if the existing storage is enough

        //    // If it is enough 
        //    // starting from the tail, move a chunk, insert, move the next chunk, etc

        //    // If not enough, create new arrays and construct it
        //}


        //public void setColumn(DMatrixSparseCSC A, int column, int startRow,  /**/double values )
        //{
        //    // TODO Update with a more efficient algorithm
        //    for (int i = 0; i < values.length; i++)
        //    {
        //        A.set(startRow + i, column, (double)values[i]);
        //    }
        //}


        //public void extract(DMatrixSparseCSC src, int srcY0, int srcY1, int srcX0, int srcX1, DMatrixSparseCSC dst, int dstY0, int dstX0)
        //{
        //    CommonOps_DSCC.extract(src, srcY0, srcY1, srcX0, srcX1, dst, dstY0, dstX0);
        //}


        //public DMatrixSparseCSC diag(DMatrixSparseCSC A)
        //{
        //    DMatrixSparseCSC output;
        //    if (MatrixFeatures_DSCC.isVector(A))
        //    {
        //        int N = Math.max(A.numCols, A.numRows);
        //        output = new DMatrixSparseCSC(N, N);
        //        CommonOps_DSCC.diag(output, A.nz_values, 0, N);
        //    }
        //    else
        //    {
        //        int N = Math.min(A.numCols, A.numRows);
        //        output = new DMatrixSparseCSC(N, 1);
        //        CommonOps_DSCC.extractDiag(A, output);
        //    }
        //    return output;
        //}


        //public bool hasUncountable(DMatrixSparseCSC M)
        //{
        //    return MatrixFeatures_DSCC.hasUncountable(M);
        //}


        //public void changeSign(DMatrixSparseCSC a)
        //{
        //    CommonOps_DSCC.changeSign(a, a);
        //}


        //public /**/double elementMaxAbs(DMatrixSparseCSC A)
        //{
        //    return CommonOps_DSCC.elementMaxAbs(A);
        //}


        //public /**/double elementMinAbs(DMatrixSparseCSC A)
        //{
        //    return CommonOps_DSCC.elementMinAbs(A);
        //}


        //public /**/double elementSum(DMatrixSparseCSC A)
        //{
        //    return CommonOps_DSCC.elementSum(A);
        //}


        //public void elementMult(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        //{
        //    CommonOps_DSCC.elementMult(A, B, output, null, null);
        //}


        public void elementDiv(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        {
            throw new ConvertToDenseException();
        }


        public void elementPower(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        {
            throw new ConvertToDenseException();
        }


        public void elementPower(DMatrixSparseCSC A, /**/double b, DMatrixSparseCSC output)
        {
            throw new ConvertToDenseException();
        }


        public void elementExp(DMatrixSparseCSC A, DMatrixSparseCSC output)
        {
            throw new ConvertToDenseException();
        }


        public void elementLog(DMatrixSparseCSC A, DMatrixSparseCSC output)
        {
            throw new ConvertToDenseException();
        }


        //public bool isIdentical(DMatrixSparseCSC A, DMatrixSparseCSC B, /**/double tol)
        //{
        //    return MatrixFeatures_DSCC.isEqualsSort(A, B, (double)tol);
        //}

        public void setRow(DMatrixSparseCSC A, int row, int startColumn, params double[] values)
        {
            throw new NotImplementedException();
        }

        public void setColumn(DMatrixSparseCSC A, int column, int startRow, params double[] values)
        {
            throw new NotImplementedException();
        }

        public void extractDiag(DMatrixSparseCSC input, DMatrixRMaj output)
        {
            throw new NotImplementedException();
        }

        public void multTransA(DMatrixSparseCSC A, DMatrixRMaj B, DMatrixRMaj output)
        {
            throw new NotImplementedException();
        }

        public void mult(DMatrixSparseCSC A, DMatrixRMaj B, DMatrixRMaj output)
        {
            throw new NotImplementedException();
        }

        public void transpose(DMatrixSparseCSC input, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public void mult(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public void multTransA(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public void kron(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public void plus(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public void minus(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public void plus(DMatrixSparseCSC A, double beta, DMatrixSparseCSC b, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public void plus(double alpha, DMatrixSparseCSC A, double beta, DMatrixSparseCSC b, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public double dot(DMatrixSparseCSC A, DMatrixSparseCSC v)
        {
            throw new NotImplementedException();
        }

        public void scale(DMatrixSparseCSC A, double val, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public void divide(DMatrixSparseCSC A, double val, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public bool invert(DMatrixSparseCSC A, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public void setIdentity(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public void pseudoInverse(DMatrixSparseCSC A, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public bool solve(DMatrixSparseCSC A, DMatrixSparseCSC X, DMatrixSparseCSC B)
        {
            throw new NotImplementedException();
        }

        public void zero(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public double normF(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public double conditionP2(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public double determinant(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public double trace(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public void extract(DMatrixSparseCSC src, int srcY0, int srcY1, int srcX0, int srcX1, DMatrixSparseCSC dst, int dstY0, int dstX0)
        {
            throw new NotImplementedException();
        }

        public DMatrixSparseCSC diag(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public bool hasUncountable(DMatrixSparseCSC M)
        {
            throw new NotImplementedException();
        }

        public void changeSign(DMatrixSparseCSC a)
        {
            throw new NotImplementedException();
        }

        public double elementMaxAbs(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public double elementMinAbs(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public double elementSum(DMatrixSparseCSC A)
        {
            throw new NotImplementedException();
        }

        public void elementMult(DMatrixSparseCSC A, DMatrixSparseCSC B, DMatrixSparseCSC output)
        {
            throw new NotImplementedException();
        }

        public bool isIdentical(DMatrixSparseCSC A, DMatrixSparseCSC B, double tol)
        {
            throw new NotImplementedException();
        }
    }
}