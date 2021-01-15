using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.data;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple
{
	public class SimpleSVD<T> where T : SimpleBase<T,Matrix>
	{

		//private SingularValueDecomposition svd;
		//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods of the current type:
		private T U_Conflict;
		//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods of the current type:
		private T W_Conflict;
		//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods of the current type:
		private T V_Conflict;

		private Matrix mat;
		internal readonly bool is64;

		// tolerance for singular values
		internal double tol;

		//public SimpleSVD(Matrix mat, bool compact)
		//{
		//	this.mat = mat;
		//	this.is64 = mat is DMatrixRMaj;
		//	if (is64)
		//	{
		//		DMatrixRMaj m = (DMatrixRMaj)mat;
		//		svd = DecompositionFactory_DDRM.svd(m.numRows, m.numCols, true, true, compact);
		//	}

		//	if (!svd.decompose(mat))
		//	{
		//		throw new Exception("Decomposition failed");
		//	}
		//	U_Conflict = (T)SimpleMatrix.wrap(svd.getU(null, false));
		//	W_Conflict = (T)SimpleMatrix.wrap(svd.getW(null));
		//	V_Conflict = (T)SimpleMatrix.wrap(svd.getV(null, false));

		//	// order singular values from largest to smallest
		//	if (is64)
		//	{
		//		SingularOps_DDRM.descendingOrder((DMatrixRMaj)U_Conflict.mat, false, (DMatrixRMaj)W_Conflict.mat, (DMatrixRMaj)V_Conflict.mat, false);
		//		tol = SingularOps_DDRM.singularThreshold((SingularValueDecomposition_F64)svd);
		//	}
		//}

		/// <summary>
		/// <para>
		/// Returns the orthogonal 'U' matrix.
		/// </para>
		/// </summary>
		/// <returns> An orthogonal m by m matrix.
		/// 
		public virtual T U
		{
			get
			{
				return U;
			}
		}

		/// <summary>
		/// Returns a diagonal matrix with the singular values.  The singular values are ordered
		/// from largest to smallest.
		/// </summary>
		/// <returns> Diagonal matrix with singular values along the diagonal. </returns>
		public virtual T W
		{
			get
			{
				return W;
			}
		}

		/// <summary>
		/// <para>
		/// Returns the orthogonal 'V' matrix.
		/// </para>
		/// </summary>
		/// <returns> An orthogonal n by n matrix. </returns>
		public virtual T V
		{
			get
			{
				return V;
			}
		}

		/// <summary>
		/// <para>
		/// Computes the quality of the computed decomposition.  A value close to or less than 1e-15
		/// is considered to be within machine precision.
		/// </para>
		/// 
		/// <para>
		/// This function must be called before the original matrix has been modified or else it will
		/// produce meaningless results.
		/// </para>
		/// </summary>
		/// <returns> Quality of the decomposition. </returns>
		//public virtual double quality()
		//{
		//	if (is64)
		//	{
		//		return DecompositionFactory_DDRM.quality((DMatrixRMaj)mat, (DMatrixRMaj)U.Matrix, (DMatrixRMaj)W.Matrix, (DMatrixRMaj)V.transpose().Matrix);
		//	}
		//	else
		//	{
		//		return DecompositionFactory_DDRM.quality((DMatrixRMaj)mat, (DMatrixRMaj)U.Matrix, (DMatrixRMaj)W.Matrix, (DMatrixRMaj)V.transpose().Matrix);
		//		//return DecompositionFactory_FDRM.quality((FMatrixRMaj)mat, (FMatrixRMaj)U.Matrix, (FMatrixRMaj)W.Matrix, (FMatrixRMaj)V.transpose().Matrix);
		//	}
		//}

		/// <summary>
		/// Computes the null space from an SVD.  For more information see <seealso cref="SingularOps_DDRM.nullSpace"/>.
		/// </summary>
		/// <returns> Null space vector. </returns>
		//public virtual SimpleMatrix nullSpace()
		//{
		//	// TODO take advantage of the singular values being ordered already
		//	if (is64)
		//	{
		//		return SimpleMatrix.wrap(SingularOps_DDRM.nullSpace((SingularValueDecomposition_F64)svd, null, tol));
		//	}
		//	else
		//	{
		//		return SimpleMatrix.wrap(SingularOps_FDRM.nullSpace((SingularValueDecomposition_F32)svd, null, (float)tol));
		//		//return SimpleMatrix.wrap(SingularOps_FDRM.nullSpace((SingularValueDecomposition_F32)svd, null, (float)tol));
		//	}
		//}

		/// <summary>
		/// Returns the specified singular value.
		/// </summary>
		/// <param name="index"> Which singular value is to be returned. </param>
		/// <returns> A singular value. </returns>
		//public virtual double getSingleValue(int index)
		//{
		//	return W.get(index, index);
		//}

		///// <summary>
		///// Returns an array of all the singular values
		///// </summary>
		//public virtual double[] SingularValues
		//{
		//	get
		//	{
		//		double[] ret = new double[W.numCols()];

		//		for (int i = 0; i < ret.Length; i++)
		//		{
		//			ret[i] = getSingleValue(i);
		//		}
		//		return ret;
		//	}
		//}

		/// <summary>
		/// Returns the rank of the decomposed matrix.
		/// </summary>
		/// <returns> The matrix's rank </returns>
		/// @see SingularOps_DDRM#rank(SingularValueDecomposition_F64, double)
		//public virtual int rank()
		//{
		//	if (is64)
		//	{
		//		return SingularOps_DDRM.rank((SingularValueDecomposition_F64)svd, tol);
		//	}
		//	else
		//	{
		//		return SingularOps_FDRM.rank((SingularValueDecomposition_F32)svd, (float)tol);
		//	}
		//}


		///**
		// * The nullity of the decomposed matrix.
		// *
		// * @return The matrix's nullity
		// * @see SingularOps_DDRM#nullity(SingularValueDecomposition_F64, double)
		// */
		//public int nullity()
		//{
		//	if (is64) { return SingularOps_DDRM.nullity((SingularValueDecomposition_F64)svd, 10.0 * UtilEjml.EPS); } else { return SingularOps_FDRM.nullity((SingularValueDecomposition_F32)svd, 5.0f * UtilEjml.F_EPS); }
		//}

		///**
		// * Returns the underlying decomposition that this is a wrapper around.
		// *
		// * @return SingularValueDecomposition
		// */
		//public SingularValueDecomposition SVD {return svd; }
	}
}