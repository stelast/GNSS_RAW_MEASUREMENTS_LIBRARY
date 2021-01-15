using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public class ConvertMatrixType
    {/**
     * Converts a matrix of one data type into another data type. If no conversion is known then an exception
     * is thrown.
     *
     * @return The converted matrix
     */
        //@SuppressWarnings("MissingCasesInEnumSwitch")
    public static Matrix convert(Matrix matrix, MatrixType desired)
        {
            Matrix m = null;

            switch (matrix.Type.tipo)
            {
                case Types.DDRM:
                    {
                        switch (desired.tipo)
                        {
                            case Types.DDRM:
                                {
                                    m = matrix.copy<Matrix>();
                                }
                                break;
                            case Types.FDRM:
                                {
                                    //m = new FMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((DMatrixRMaj)matrix, (FMatrixRMaj)m);
                                }
                                break;

                            case Types.ZDRM:
                                {
                                    m = new ZMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    ConvertMatrixData.convert((DMatrixRMaj)matrix, (ZMatrixRMaj)m);
                                }
                                break;

                            case Types.CDRM:
                                {
                                    //m = new CMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((DMatrixRMaj)matrix, (CMatrixRMaj)m);
                                }
                                break;

                            case Types.DSCC:
                                {
                                    m = new DMatrixSparseCSC(matrix.NumRows, matrix.NumCols);
                                    DConvertMatrixStruct.convert((DMatrixRMaj)matrix, (DMatrixSparseCSC)m);
                                }
                                break;

                            case Types.FSCC:
                                {
                                    //m = new FMatrixSparseCSC(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((DMatrixRMaj)matrix, (FMatrixSparseCSC)m);
                                }
                                break;
                        }
                    }
                    break;

                case Types.FDRM:
                    {
                        switch (desired.tipo)
                        {
                            case Types.FDRM:
                                {
                                    m = matrix.copy<Matrix>();
                                }
                                break;
                            case Types.DDRM:
                                {
                                    //m = new DMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((FMatrixRMaj)matrix, (DMatrixRMaj)m);
                                }
                                break;

                            case Types.ZDRM:
                                {
                                    //m = new ZMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((FMatrixRMaj)matrix, (ZMatrixRMaj)m);
                                }
                                break;

                            case Types.CDRM:
                                {
                                    //m = new CMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((FMatrixRMaj)matrix, (CMatrixRMaj)m);
                                }
                                break;

                            case Types.DSCC:
                                {
                                    //m = new DMatrixSparseCSC(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((FMatrixRMaj)matrix, (DMatrixSparseCSC)m);
                                }
                                break;

                            case Types.FSCC:
                                {
                                    //m = new FMatrixSparseCSC(matrix.NumRows, matrix.NumCols);
                                    //FConvertMatrixStruct.convert((FMatrixRMaj)matrix, (FMatrixSparseCSC)m);
                                }
                                break;
                        }
                    }
                    break;

                case Types.ZDRM:
                    {
                        switch (desired.tipo)
                        {
                            case Types.CDRM:
                                {
                                    //m = new CMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((ZMatrixRMaj)matrix, (CMatrixRMaj)m);
                                }
                                break;
                        }
                    }
                    break;

                case Types.CDRM:
                    {
                        switch (desired.tipo)
                        {
                            case Types.ZDRM:
                                {
                                    //m = new ZMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((CMatrixRMaj)matrix, (ZMatrixRMaj)m);
                                }
                                break;
                        }
                    }
                    break;

                case Types.DSCC:
                    {
                        switch (desired.tipo)
                        {
                            case Types.DDRM:
                                {
                                    m = new DMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    DConvertMatrixStruct.convert((DMatrixSparseCSC)matrix, (DMatrixRMaj)m);
                                }
                                break;

                            case Types.FDRM:
                                {
                                    //m = new FMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((DMatrixSparseCSC)matrix, (FMatrixRMaj)m);
                                }
                                break;

                            case Types.ZDRM:
                                {
                                    m = new ZMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    ConvertMatrixData.convert((DMatrixSparseCSC)matrix, (ZMatrixRMaj)m);
                                }
                                break;

                            case Types.CDRM:
                                {
                                    //m = new CMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((DMatrixSparseCSC)matrix, (CMatrixRMaj)m);
                                }
                                break;

                            case Types.FSCC:
                                {
                                    //m = new FMatrixSparseCSC(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((DMatrixSparseCSC)matrix, (FMatrixSparseCSC)m);
                                }
                                break;
                        }
                    }
                    break;

                case Types.FSCC:
                    {
                        switch (desired.tipo)
                        {
                            case Types.DDRM:
                                {
                                    //m = new DMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((FMatrixSparseCSC)matrix, (DMatrixRMaj)m);
                                }
                                break;

                            case Types.FDRM:
                                {
                                    //m = new FMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //FConvertMatrixStruct.convert((FMatrixSparseCSC)matrix, (FMatrixRMaj)m);
                                }
                                break;

                            case Types.ZDRM:
                                {
                                    //m = new ZMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((FMatrixSparseCSC)matrix, (ZMatrixRMaj)m);
                                }
                                break;

                            case Types.CDRM:
                                {
                                    //m = new CMatrixRMaj(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((FMatrixSparseCSC)matrix, (CMatrixRMaj)m);
                                }
                                break;

                            case Types.DSCC:
                                {
                                    //m = new DMatrixSparseCSC(matrix.NumRows, matrix.NumCols);
                                    //ConvertMatrixData.convert((FMatrixSparseCSC)matrix, (DMatrixSparseCSC)m);
                                }
                                break;
                        }
                    }
                    break;
            }

            if (m == null)
            {
                throw new ArgumentException("Unknown " + matrix.Type + " " + desired);
            }

            return m;
        }
    }
}