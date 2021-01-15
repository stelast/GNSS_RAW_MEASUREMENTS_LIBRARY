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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data
{

    public enum MatrixTypesClass
    {
        DMatrixRMaj,
        FMatrixRMaj,
        ZMatrixRMaj,
        CMatrixRMaj,
        DMatrixSparseCSC,
        FMatrixSparseCSC,
        Object,
        DMatrixSparseTriplet,
        FMatrixSparseTriplet
    }
    public enum Types
    {
        DDRM,
        FDRM,
        ZDRM,
        CDRM,
        DSCC,
        FSCC,
        ZSCC,
        CSCC,
        DTRIPLET,
        FTRIPLET,
        UNSPECIFIED
    }

    /**
     * Specifies that type of data structure a matrix is encoded with.
     *
     * @author Peter Abeles
     */
    public class MatrixType
    {
        readonly bool fijed;
        readonly bool dense;
        readonly bool real;
        readonly int bits;
        readonly MatrixTypesClass classType;

        public Types tipo { get; set; }

        public MatrixType(bool real, bool dense, int bits, MatrixTypesClass type)
        {
            this.real = real;
            this.fijed = false;
            this.dense = dense;
            this.bits = bits;
            this.classType = type;
        }

        public MatrixType(bool fijed, bool real, bool dense, int bits, MatrixTypesClass type) {
            this.real = real;
            this.fijed = fijed;
            this.dense = dense;
            this.bits = bits;
            this.classType = type;
        }

        public static MatrixType DDRM()
        {
            return new MatrixType(true, true, 64, MatrixTypesClass.DMatrixRMaj);
        }

        public static MatrixType FDRM()
        {
            return new MatrixType(true, true, 32, MatrixTypesClass.FMatrixRMaj);
        }

        public static MatrixType ZDRM()
        {
            return new MatrixType(false, true, 64, MatrixTypesClass.ZMatrixRMaj);
        }

        public static MatrixType CDRM()
        {
            return new MatrixType(false, true, 32, MatrixTypesClass.CMatrixRMaj);
        }

        public static MatrixType DSCC()
        {
            return new MatrixType(true, false, 64, MatrixTypesClass.DMatrixSparseCSC);
        }

        public static MatrixType FSCC()
        {
            return new MatrixType(true, false, 32, MatrixTypesClass.FMatrixSparseCSC);
        }

        public static MatrixType ZSCC()
        {
            return new MatrixType(true, false, 64, MatrixTypesClass.Object);
        }

        public static MatrixType CSCC()
        {
            return new MatrixType(true, false, 32, MatrixTypesClass.Object);
        }

        public static MatrixType DTRIPLET()
        {
            return new MatrixType(false, false, 64, MatrixTypesClass.DMatrixSparseTriplet);
        }

        public static MatrixType FTRIPLET()
        {
            return new MatrixType(false, false, 64, MatrixTypesClass.FMatrixSparseTriplet);
        }

        public static MatrixType UNSPECIFIED()
        {
            return new MatrixType(false, false, 0, MatrixTypesClass.Object);
        }

        public static MatrixType lookup(MatrixTypesClass type)
        {
            if (type == MatrixTypesClass.DMatrixRMaj)
                return DDRM();
            else if(type == MatrixTypesClass.FMatrixRMaj )
                return FDRM();
            else if(type == MatrixTypesClass.ZMatrixRMaj )
                return ZDRM();
            else if(type == MatrixTypesClass.CMatrixRMaj )
                return CDRM();
            else if(type == MatrixTypesClass.DMatrixSparseCSC )
                return DSCC();
            else if(type == MatrixTypesClass.FMatrixSparseCSC )
                return FSCC();
            else
                throw new ArgumentException("Unknown class");
        }


        public bool isReal()
        {
            return real;
        }

        public bool isFixed()
        {
            return fijed;
        }

        public bool isDense()
        {
            return dense;
        }

        public int getBits()
        {
            return bits;
        }

        public MatrixTypesClass getClassType()
        {
            return classType;
        }

        /**
         * Looks up the default matrix type for the specified features
         */
        public static MatrixType lookup(bool dense, bool real, int bits)
        {
            if (dense)
            {
                if (real)
                {
                    if (bits == 64)
                    {
                        return DDRM();
                    }
                    else
                    {
                        return FDRM();
                    }
                }
                else
                {
                    if (bits == 64)
                    {
                        return ZDRM();
                    }
                    else
                    {
                        return CDRM();
                    }
                }
            }
            else
            {
                if (real)
                {
                    if (bits == 64)
                    {
                        return DSCC();
                    }
                    else
                    {
                        return FSCC();
                    }
                }
                else
                {
                    throw new ArgumentException("Complex sparse not yet supported");
                }
            }
        }


        public Matrix create(int rows, int cols)
        {
            switch (tipo)
            {
                case Types.DDRM: return new DMatrixRMaj(rows, cols);
                //case Types.FDRM: return new FMatrixRMaj(rows, cols);
                case Types.ZDRM: return new ZMatrixRMaj(rows, cols);
                //case Types.CDRM: return new CMatrixRMaj(rows, cols);
                case Types.DSCC: return new DMatrixSparseCSC(rows, cols);
                //case Types.FSCC: return new FMatrixSparseCSC(rows, cols);
                //case Types.ZSCC: return new ZMatrixSparseCSC(rows,cols);
                //case Types.CSCC: return new CMatrixSparseCSC(rows,cols);
                default:
                    throw new SystemException("Unknown Matrix Type " + this);
            }

        }
    }
} 
