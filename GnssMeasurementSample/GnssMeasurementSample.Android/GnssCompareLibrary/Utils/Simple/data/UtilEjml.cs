using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.linsol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data
{
    /**
     * Various functions that are useful but don't have a clear location that they belong in.
     *
     * @author Peter Abeles
     */
    public class UtilEjml
    {
        public static double EPS = Math.Pow(2, -52);
        public static float F_EPS = (float)Math.Pow(2, -21);

        public static double PI = Math.PI;
        public static double PI2 = 2.0 * Math.PI;
        public static double PId2 = Math.PI / 2.0;

        public static float F_PI = (float)Math.PI;
        public static float F_PI2 = (float)(2.0 * Math.PI);
        public static float F_PId2 = (float)(Math.PI / 2.0);

        // tolerances for unit tests
        public static float TEST_F32 = 5e-4f;
        public static double TEST_F64 = 1e-8;
        public static float TESTP_F32 = 1e-6f;
        public static double TESTP_F64 = 1e-12;
        public static float TEST_F32_SQ = (float)Math.Sqrt(TEST_F32);
        public static double TEST_F64_SQ = Math.Sqrt(TEST_F64);

        // The maximize size it will do inverse on
        public static int maxInverseSize = 5;

        public static int[] ZERO_LENGTH_I32 = new int[0];
        public static float[] ZERO_LENGTH_F32 = new float[0];
        public static double[] ZERO_LENGTH_F64 = new double[0];

        public static void checkSameInstance(Object a, Object b)
        {
            if (a == b)
                throw new ArgumentException("Can't pass in the same instance");
        }

        /**
         * If the input matrix is null a new matrix is created and returned. If it exists it will be reshaped and returned.
         *
         * @param a (Input/Output) matrix which is to be checked. Can be null.
         * @param rows Desired number of rows
         * @param cols Desired number of cols
         * @return modified matrix or new matrix
         */

        public static DMatrixRMaj reshapeOrDeclare(DMatrixRMaj? a, int rows, int cols)
        {
            if (a == null)
                return new DMatrixRMaj(rows, cols);
            else if (a.numRows != rows || a.numCols != cols)
                a.reshape(rows, cols);
            return a;
        }

        /**
         * If the input matrix is null a new matrix is created and returned. If it exists it will be reshaped and returned.
         *
         * @param a (Input/Output) matrix which is to be checked. Can be null.
         * @param rows Desired number of rows
         * @param cols Desired number of cols
         * @return modified matrix or new matrix
         */
    /*    public static FMatrixRMaj reshapeOrDeclare(FMatrixRMaj? a, int rows, int cols)
        {
            if (a == null)
                return new FMatrixRMaj(rows, cols);
            else if (a.numRows != rows || a.numCols != cols)
                a.reshape(rows, cols);
            return a;
        }
    */
        /**
         * If the input matrix is null a new matrix is created and returned. If it exists it will be reshaped and returned.
         *
         * @param a (Input/Output) matrix which is to be checked. Can be null.
         * @param rows Desired number of rows
         * @param cols Desired number of cols
         * @return modified matrix or new matrix
         */
        public static BMatrixRMaj reshapeOrDeclare(BMatrixRMaj? a, int rows, int cols)
        {
            if (a == null)
                return new BMatrixRMaj(rows, cols);
            else if (a.numRows != rows || a.numCols != cols)
                a.reshape(rows, cols);
            return a;
        }

        /**
         * If the input matrix is null a new matrix is created and returned. If it exists it will be reshaped and returned.
         *
         * @param a (Input/Output) matrix which is to be checked. Can be null.
         * @param rows Desired number of rows
         * @param cols Desired number of cols
         * @return modified matrix or new matrix
         */
        public static ZMatrixRMaj reshapeOrDeclare(ZMatrixRMaj? a, int rows, int cols)
        {
            if (a == null)
                return new ZMatrixRMaj(rows, cols);
            else if (a.numRows != rows || a.numCols != cols)
                a.reshape(rows, cols);
            return a;
        }

        /**
         * If the input matrix is null a new matrix is created and returned. If it exists it will be reshaped and returned.
         *
         * @param a (Input/Output) matrix which is to be checked. Can be null.
         * @param rows Desired number of rows
         * @param cols Desired number of cols
         * @return modified matrix or new matrix
         */
        /*
        public static CMatrixRMaj reshapeOrDeclare(CMatrixRMaj? a, int rows, int cols)
        {
            if (a == null)
                return new CMatrixRMaj(rows, cols);
            else if (a.numRows != rows || a.numCols != cols)
                a.reshape(rows, cols);
            return a;
        }*/

        /**
         * If the input matrix is null a new matrix is created and returned. If it exists it will be reshaped and returned.
         *
         * @param target (Input/Output) matrix which is to be checked. Can be null.
         * @param reference (Input) Refernece matrix who's shape will be matched
         * @return modified matrix or new matrix
         */
        public static T reshapeOrDeclare<T>(T target, T reference) 
            where T:ReshapeMatrix
        {
            if (target == null)
                return reference.createLike<T>();
            else if (target.NumRows != reference.NumRows || target.NumCols != reference.NumCols)
                target.reshape(reference.NumRows, reference.NumCols);
            return target;
        }

        public static T reshapeOrDeclare<T>(T target, MatrixSparse reference)
            where T : MatrixSparse
        {
            if (target == null)
                return reference.createLike<T>();
            else
                target.reshape(reference.NumRows, reference.NumCols, reference.NonZeroLength);
            return target;
        }

        /**
         * If the input matrix is null a new matrix is created and returned. If it exists it will be reshaped and returned.
         *
         * @param target (Input/Output) matrix which is to be checked. Can be null.
         * @param reference (Input) Refernece matrix who's shape will be matched
         * @return modified matrix or new matrix
         */
        public static T reshapeOrDeclare<T>(T target, T reference, int rows, int cols)
            where T : ReshapeMatrix
        {
            if (target == null)
                return reference.createLike<T>();
            else if (target.NumRows != rows || target.NumCols != cols)
                target.reshape(rows, cols);
            return target;
        }

        public static DMatrixSparseCSC reshapeOrDeclare( DMatrixSparseCSC? target, int rows, int cols, int nz_length)
        {
            if (target == null)
                return new DMatrixSparseCSC(rows, cols, nz_length);
            else
                target.reshape(rows, cols, nz_length);
            return target;
        }
        /*
        public static FMatrixSparseCSC reshapeOrDeclare( FMatrixSparseCSC? target, int rows, int cols, int nz_length)
        {
            if (target == null)
                return new FMatrixSparseCSC(rows, cols, nz_length);
            else
                target.reshape(rows, cols, nz_length);
            return target;
        }*/

        public static void checkSameShape(Matrix a, Matrix b, bool allowedSameInstance)
        {
            if (a.NumRows != b.NumRows || a.NumCols != b.NumCols)
            {
                throw new MatrixDimensionException("Must be same shape. " + a.NumRows + "x" + a.NumCols + " vs " + b.NumRows + "x" + b.NumCols);
            }
            if (!allowedSameInstance && a == b)
                throw new ArgumentException("Must not be the same instance");
        }

        public static void checkSameShape(Matrix a, Matrix b, Matrix c)
        {
            if (a.NumRows != b.NumRows || a.NumCols != b.NumCols)
            {
                throw new MatrixDimensionException("Must be same shape. " + a.NumRows + "x" + a.NumCols + " vs " + b.NumRows + "x" + b.NumCols);
            }
            if (a.NumRows != c.NumRows || a.NumCols != c.NumCols)
            {
                throw new ArgumentException("Must be same shape. " + a.NumRows + "x" + a.NumCols + " vs " + c.NumRows + "x" + c.NumCols);
            }
        }

        /**
         * Wraps a linear solver of any type with a safe solver the ensures inputs are not modified
         */
        /*
        public static  LinearSolver<Matrix, Matrix> safe(LinearSolver<Matrix, Matrix> solver)
        {
            if (solver.modifiesA() || solver.modifiesB())
            {
                if (solver is LinearSolverDense<Matrix>) {
                    return new LinearSolverSafe<ReshapeMatrix>((LinearSolverDense)solver);
                } else if (solver is LinearSolverSparse<Matrix, Matrix>) {
                    return new LinearSolverSparseSafe<ReshapeMatrix>((LinearSolverSparse)solver);
                } else
                {
                    throw new ArgumentException("Unknown solver type");
                }
            }
            else
            {
                return solver;
            }
        }
        */
        /*
        public static <D extends ReshapeMatrix> LinearSolverDense<D> safe(LinearSolverDense<D> solver)
        {
            if (solver.modifiesA() || solver.modifiesB())
            {
                return new LinearSolverSafe<>(solver);
            }
            else
            {
                return solver;
            }
        }
        */
        public static void checkTooLarge(int rows, int cols)
        {
            if ((rows * cols) != ((long)rows * cols))
                throw new ArgumentException("Matrix size exceeds the size of an integer");
        }

        public static void checkTooLargeComplex(int rows, int cols)
        {
            if ((2 * rows * cols) != ((long)rows * cols * 2))
                throw new ArgumentException("Matrix size exceeds the size of an integer");
        }

        public static bool isUncountable(double val)
        {
            return Double.IsNaN(val) || Double.IsInfinity(val);
        }

        public static bool isUncountable(float val)
        {
            return float.IsNaN(val) || float.IsInfinity(val);
        }

        public static bool isIdentical(double a, double b, double tol)
        {
            // if either is negative or positive infinity the result will be positive infinity
            // if either is NaN the result will be NaN
            double diff = Math.Abs(a - b);

            // diff = NaN == false
            // diff = infinity == false
            if (tol >= diff)
                return true;

            if (Double.IsNaN(a))
            {
                return Double.IsNaN(b);
            }
            else
                return Double.IsInfinity(a) && a == b;
        }

        public static bool isIdentical(float a, float b, float tol)
        {
            // if either is negative or positive infinity the result will be positive infinity
            // if either is NaN the result will be NaN
            double diff = Math.Abs(a - b);

            // diff = NaN == false
            // diff = infinity == false
            if (tol >= diff)
                return true;

            if (float.IsNaN(a))
            {
                return float.IsNaN(b);
            }
            else
                return float.IsInfinity(a) && a == b;
        }

        public static void memset(double[] data, double val, int length)
        {
            for (int i = 0; i < length; i++)
            {
                data[i] = val;
            }
        }

        public static void memset(int[] data, int val, int length)
        {
            for (int i = 0; i < length; i++)
            {
                data[i] = val;
            }
        }
/*
        public static <T> void setnull(T[] array) 
        {
            for (int i = 0; i < array.Count(); i++)
            {
                array[i] = null;
            }
        }
*/
        public static double max(double[] array, int start, int length)
        {
            double max = array[start];
            int end = start + length;

            for (int i = start + 1; i < end; i++)
            {
                double v = array[i];
                if (v > max)
                {
                    max = v;
                }
            }

            return max;
        }

        public static float max(float[] array, int start, int length)
        {
            float max = array[start];
            int end = start + length;

            for (int i = start + 1; i < end; i++)
            {
                float v = array[i];
                if (v > max)
                {
                    max = v;
                }
            }

            return max;
        }

        /**
         * Give a string of numbers it returns a DenseMatrix
         */
        //@SuppressWarnings("StringSplitter")
        public static DMatrixRMaj parse_DDRM(String s, int numColumns)
        {
            String[] vals = s.Split("(\\s)+");

            // there is the possibility the first element could be empty
            int start = String.IsNullOrEmpty(vals[0]) ? 1 : 0;

            // covert it from string to doubles
            int numRows = (vals.Count() - start) / numColumns;

            DMatrixRMaj ret = new DMatrixRMaj(numRows, numColumns);

            int index = start;
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    ret.set(i, j, Double.Parse(vals[index++]));
                }
            }

            return ret;
        }

        /**
         * Give a string of numbers it returns a DenseMatrix
         */
        /*
        @SuppressWarnings("StringSplitter")
        public static FMatrixRMaj parse_FDRM(String s, int numColumns)
        {
            String[] vals = s.split("(\\s)+");

            // there is the possibility the first element could be empty
            int start = vals[0].isEmpty() ? 1 : 0;

            // covert it from string to doubles
            int numRows = (vals.Count() - start) / numColumns;

            FMatrixRMaj ret = new FMatrixRMaj(numRows, numColumns);

            int index = start;
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    ret.set(i, j, float.parsefloat(vals[index++]));
                }
            }

            return ret;
        }*/
        /*
        public static int[] sortByIndex(double[] data, int size)
        {
            int[] idx = new int[size];
            for (int i = 0; i < size; i++)
            {
                idx[i] = i;
            }

            Array.Sort(idx, Comparator.comparingDouble(o->data[o]));

            return idx;
        }

        public static DMatrixSparseCSC parse_DSCC(String s, int numColumns)
        {
            DMatrixRMaj tmp = parse_DDRM(s, numColumns);

            return DConvertMatrixStruct.convert(tmp, (DMatrixSparseCSC)null, 0);
        }
        */ 
        /*
        public static FMatrixSparseCSC parse_FSCC(String s, int numColumns)
        {
            FMatrixRMaj tmp = parse_FDRM(s, numColumns);

            return FConvertMatrixStruct.convert(tmp, (FMatrixSparseCSC)null, 0);
        }*/

        public static int[] shuffled(int N, Random rand)
        {
            return shuffled(N, N, rand);
        }

        public static int[] shuffled(int N, int shuffleUpTo, Random rand)
        {
            int[] l = new int[N];
            for (int i = 0; i < N; i++)
            {
                l[i] = i;
            }
            shuffle(l, N, 0, shuffleUpTo, rand);
            return l;
        }

        public static int[] shuffledSorted(int N, int shuffleUpTo, Random rand)
        {
            int[] l = new int[N];
            for (int i = 0; i < N; i++)
            {
                l[i] = i;
            }
            shuffle(l, N, 0, shuffleUpTo, rand);
            Array.Sort(l, 0, shuffleUpTo);
            return l;
        }

        public static void shuffle(int[] list, int N, int start, int end, Random rand)
        {
            int range = end - start;
            for (int i = 0; i < range; i++)
            {
                int selected = rand.Next(N - i) + i + start;
                int v = list[i];
                list[i] = list[selected];
                list[selected] = v;
            }
        }

        public static int[] pivotVector(int[] pivots, int length,  IGrowArray? storage )
        {
            if (storage == null) storage = new IGrowArray();
            storage.reshape(length);
            System.Array.Copy(pivots, 0, storage.data, 0, length);
            return storage.data;
        }

        public static int permutationSign(int[] p, int N, int[] work)
        {
            System.Array.Copy(p, 0, work, 0, N);
            p = work;
            int cnt = 0;
            for (int i = 0; i < N; ++i)
            {
                while (i != p[i])
                {
                    ++cnt;
                    int tmp = p[i];
                    p[i] = p[p[i]];
                    p[tmp] = tmp;
                }
            }
            return cnt % 2 == 0 ? 1 : -1;
        }

        public static double[] randomVector_F64(Random rand, int length)
        {
            double[] d = new double[length];
            for (int i = 0; i < length; i++)
            {
                d[i] = rand.NextDouble();
            }
            return d;
        }

        public static float[] randomVector_F32(Random rand, int length)
        {
            float[] d = new float[length];
            for (int i = 0; i < length; i++)
            {
                d[i] = (float)rand.NextDouble();
            }
            return d;
        }

        public static String stringShapes(Matrix A, Matrix B, Matrix C)
        {
            return "( " + A.NumRows + "x" + A.NumCols + " ) " +
                    "( " + B.NumRows + "x" + B.NumCols + " ) " +
                    "( " + C.NumRows + "x" + C.NumCols + " )";
        }

        public static String stringShapes(Matrix A, Matrix B)
        {
            return "( " + A.NumRows + "x" + A.NumCols + " ) " +
                    "( " + B.NumRows + "x" + B.NumCols + " )";
        }

        /**
         * Fixed length fancy formatting for doubles. If possible decimal notation is used. If all the significant digits
         * can't be shown then it will switch to exponential notation.  If not all the space is needed then it will
         * be filled in to ensure it has the specified length.
         *
         * @param value value being formatted
         * @param format default format before exponential
         * @param length Maximum number of characters it can take.
         * @param significant Number of significant decimal digits to show at a minimum.
         * @return formatted string
         */
        public static String fancyStringF(double value, NumberFormatInfo format, int length, int significant)
        {

            String formatted = fancyString(value, format, length, significant);

            int n = length - formatted.Count();
            if (n > 0)
            {
                StringBuilder builder = new StringBuilder(n);
                for (int i = 0; i < n; i++)
                {
                    builder.Append(' ');
                }
                return formatted + builder.ToString();
            }
            else
            {
                return formatted;
            }
        }

        public static String fancyString(double value, NumberFormatInfo format, int length, int significant)
        {

            return fancyString(value, format, true, length, significant);
        }

        public static String fancyString(double value, NumberFormatInfo format, bool hasSpace, int length, int significant)
        {

            String formatted;
            
            // see if the number is negative. Including negative zero
            bool isNegative = value < 0;

            if (value == 0)
            {
                formatted = isNegative ? "-0" : hasSpace ? " 0" : "0";
            }
            else
            {
                int digits = length - 1;
                String extraSpace = isNegative ? "" : hasSpace ? " " : "";
                double vabs = Math.Abs(value);
                int a = (int)Math.Floor(Math.Log10(vabs));
                if (a >= 0 && a < digits)
                {
                    format.CurrencyDecimalDigits = digits - 2 - a;
                    //format.setMaximumFractionDigits(digits - 2 - a);
                    formatted = extraSpace + value.ToString(format);
                }
                else if (a < 0 && digits + a > significant)
                {
                    format.CurrencyDecimalDigits = digits - 1;
                    //format.setMaximumFractionDigits(digits - 1);
                    formatted = extraSpace + value.ToString(format);
                }
                else
                {
                    int exp = (int)Math.Log10(Math.Abs(a)) + 1;
                    // see if there is room for all the requested significant digits
                    significant = Math.Min(significant, digits - significant - exp);
                    if (significant > 0)
                    {
                        formatted = extraSpace + String.Format("%." + significant + "E", value);
                    }
                    else // I give up. time to break the length
                        formatted = extraSpace + String.Format("%.0E", value);
                }
            }
            return formatted;
        }

        /**
         * Resizes the array to ensure that it is at least of length desired and returns its internal array
         */
        public static int[] adjust( IGrowArray? gwork, int desired)
        {
            if (gwork == null) gwork = new IGrowArray();
            gwork.reshape(desired);
            return gwork.data;
        }

        public static int[] adjust( IGrowArray? gwork, int desired, int zeroToM)
        {
            int[] w = adjust(gwork, desired);
            Arrays.Fill(w, 0, zeroToM, 0);
            return w;
        }

        public static int[] adjustClear( IGrowArray? gwork, int desired)
        {
            return adjust(gwork, desired, desired);
        }

        public static int[] adjustFill( IGrowArray? gwork, int desired, int value)
        {
            int[] w = adjust(gwork, desired);
            Arrays.Fill(w, 0, desired, value);
            return w;
        }

        /**
         * Resizes the array to ensure that it is at least of length desired and returns its internal array
         */
        public static double[] adjust( DGrowArray? gwork, int desired)
        {
            if (gwork == null) gwork = new DGrowArray();
            gwork.reshape(desired);
            return gwork.data;
        }

        /**
         * Resizes the array to ensure that it is at least of length desired and returns its internal array
         */
        /*
        public static float[] adjust( FGrowArray? gwork, int desired)
        {
            if (gwork == null) gwork = new FGrowArray();
            gwork.reshape(desired);
            return gwork.data;
        }*/

        /**
         * Returns true if any of the matrix arguments has 
         */
        public static bool hasNullableArgument(MethodInfo func)
        {
            ParameterInfo[] annotations = func.GetParameters();
            if (annotations.Count() == 0)
                return false;

            return false;
            /*
            //Class <?>[] types = func.GetParameters();
            for (int i = 0; i < annotations.Count(); i++)
            {
                var type = annotations[i].ParameterType;
                
                Annotation[] argumentAnnotations = annotations[i].ParameterType;
                if (argumentAnnotations.Count() == 0)
                    continue;
                if (!Matrix.class.isAssignableFrom(types[i]))
                continue;
                Annotation last = argumentAnnotations[argumentAnnotations.Count() - 1];
                if (last.toString().contains("Nullable"))
                    return true;
            }
            return false;*/
        }
        /*
        public static GrowArray<FGrowArray> checkDeclare_F32( GrowArray<FGrowArray>? workspace )
        {
            if (workspace == null) {
                return new GrowArray<>(FGrowArray::new);
            }
            return workspace;
        }*/

        public static List<DGrowArray> checkDeclare_F64( List<DGrowArray>? workspace )
        {
            if (workspace == null)
                return new List<DGrowArray>();
            return workspace;
        }

        /**
         * Checks to see if a matrix of this size will exceed the maximum possible value an integer can store, which is
         * the max possible array size in Java.
         */
        public static bool exceedsMaxMatrixSize(int numRows, int numCols)
        {
            if (numRows == 0 || numCols == 0)
                return false;
            return numCols > int.MaxValue / numRows;
        }

        public static void printTime(String message, Process timer)
        {
            printTime("Processing... ", message, timer);
        }

        public static void printTime(String pre, String message, Process timer)
        {/*
            System.out.printf(pre);
            long time0 = System.nanoTime();
            timer.process();
            long time1 = System.nanoTime();
            System.out.println(message + " " + ((time1 - time0) * 1e-6) + " (ms)");*/
        }

        public interface Process
        {
            void process();
        }

        /**
         * Intended for checking preconditions. Throws an exception if the two values are not equal.
         */
        public static void assertEq(int valA, int valB)
        {
            assertEq(valA, valB, "");
        }

        /**
         * Intended for checking preconditions. Throws an exception if the two values are not equal.
         */
        public static void assertEq(int valA, int valB, String message)
        {
            if (valA != valB)
                throw new ArgumentException(valA + " != " + valB + " " + message);
        }

        /**
         * Intended for checking preconditions. Throws an exception if the input is not true
         */
        public static void assertTrue(bool value, String message)
        {
            if (!value)
                throw new ArgumentException(message);
        }

        /**
         * Intended for checking preconditions. Throws an exception if the input is not true
         */
        public static void assertTrue(bool value)
        {
            if (!value)
                throw new ArgumentException("Expected true");
        }

        /**
         * Intended for checking matrix shape preconditions. Throws an exception if the two values are not equal.
         */
        public static void assertShape(int valA, int valB, String message)
        {
            if (valA != valB)
                throw new MatrixDimensionException(valA + " != " + valB + " " + message);
        }

        /**
         * Intended for checking matrix shape preconditions. Throws an exception if the input is not true
         */
        public static void assertShape(bool value, String message)
        {
            if (!value)
                throw new MatrixDimensionException(message);
        }

        /**
         * Checks the size of inputs to the standard size function. Throws exception if B is incorrect. Reshapes X.
         *
         * @param numRowsA Number of rows in A matrix
         * @param numColsA Number of columns in A matrix
         */
        public static void checkReshapeSolve(int numRowsA, int numColsA, ReshapeMatrix B, ReshapeMatrix X)
        {
            if (B.NumRows != numRowsA)
                throw new ArgumentException("Unexpected number of rows in B based on shape of A. Found=" +
                        B.NumRows + " Expected=" + numRowsA);
            X.reshape(numColsA, B.NumCols);
        }
    }

}