using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data.decomposition;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops
{
    public interface SingularValueDecomposition<T> : DecompositionInterface<T>
        where T:Matrix
    {
        /**
         * The number of singular values in the matrix. This is equal to the length of the smallest side.
         *
         * @return Number of singular values in the matrix.
         */
        int numberOfSingularValues();

        /**
         * If true then compact matrices are returned.
         *
         * @return true if results use compact notation.
         */
        bool isCompact();

        /**
         * <p>
         * Returns the orthogonal 'U' matrix.
         * </p>
         * <p>
         * Internally the SVD algorithm might compute U transposed or it might not.  To avoid an
         * unnecessary double transpose the option is provided to select if the transpose is returned.
         * </p>
         *
         * @param U Optional storage for U. If null a new instance or internally maintained matrix is returned.  Modified.
         * @param transposed If the returned U is transposed.
         * @return An orthogonal matrix.
         */
        T getU( T U , bool transposed);

        /**
         * <p>
         * Returns the orthogonal 'V' matrix.
         * </p>
         *
         * <p>
         * Internally the SVD algorithm might compute V transposed or it might not.  To avoid an
         * unnecessary double transpose the option is provided to select if the transpose is returned.
         * </p>
         *
         * @param V Optional storage for v. If null a new instance or internally maintained matrix is returned.  Modified.
         * @param transposed If the returned V is transposed.
         * @return An orthogonal matrix.
         */
        T getV( T V , bool transposed);

        /**
         * Returns a diagonal matrix with the singular values.  Order of the singular values
         * is not guaranteed.
         *
         * @param W Optional storage for W. If null a new instance or internally maintained matrix is returned.  Modified.
         * @return Diagonal matrix with singular values along the diagonal.
         */
        T getW( T W );

        /**
         * Number of rows in the decomposed matrix.
         * @return Number of rows in the decomposed matrix.
         */
        int numRows();

        /**
         * Number of columns in the decomposed matrix.
         * @return Number of columns in the decomposed matrix.
         */
        int numCols();
    }
}