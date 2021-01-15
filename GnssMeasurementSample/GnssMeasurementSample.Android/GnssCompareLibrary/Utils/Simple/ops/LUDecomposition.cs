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
    public interface LUDecomposition<T>: DecompositionInterface<T>
        where T:Matrix
    {
        /**
         * <p>
         * Returns the L matrix from the decomposition.  Should only
         * be called after {@link #decompose(org.ejml.data.Matrix)} has
         * been called.
         * </p>
         *
         * <p>
         * If parameter 'lower' is not null, then that matrix is used to store the L matrix.  Otherwise
         * a new matrix is created.
         * </p>
         *
         * @param lower Storage for T matrix. If null then a new matrix is returned.  Modified.
         * @return The L matrix.
         */
        T getLower( T lower );

        /**
         * <p>
         * Returns the U matrix from the decomposition.  Should only
         * be called after {@link #decompose(org.ejml.data.Matrix)}  has
         * been called.
         * </p>
         *
         * <p>
         * If parameter 'upper' is not null, then that matrix is used to store the U matrix.  Otherwise
         * a new matrix is created.
         * </p>
         *
         * @param upper Storage for U matrix. If null then a new matrix is returned. Modified.
         * @return The U matrix.
         */
        T getUpper( T upper );

        /**
         * <p>
         * For numerical stability there are often row interchanges.  This computes
         * a pivot matrix that will undo those changes.
         * </p>
         *
         * @param pivot Storage for the pivot matrix. If null then a new matrix is returned. Modified.
         * @return The pivot matrix.
         */
        T getRowPivot( T pivot );

        /**
         * Returns the row pivot vector
         *
         * @param pivot (Optional) Storage for pivot vector
         * @return The pivot vector
         */
        int[] getRowPivotV( IGrowArray pivot );

        /**
         * Returns true if the decomposition detected a singular matrix.  This check
         * will not work 100% of the time due to machine precision issues.
         *
         * @return True if the matrix is singular and false if it is not.
         */
        // TODO Remove?  If singular decomposition will fail.
        bool isSingular();
    }
}