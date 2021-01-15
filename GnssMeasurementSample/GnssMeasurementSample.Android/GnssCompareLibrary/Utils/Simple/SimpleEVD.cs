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
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple
{
	public class SimpleEVD<T> where T : SimpleBase<T,Matrix>
	{
		//private EigenDecomposition eig;

		//internal Matrix mat;

		//public SimpleEVD(Matrix mat)
		//{
		//	this.mat = mat;

		//	switch (mat.Type)
		//	{
		//		case DDRM:
		//			eig = DecompositionFactory_DDRM.eig(mat.NumCols, true);
		//			break;
		//		case FDRM:
		//			eig = DecompositionFactory_FDRM.eig(mat.NumCols, true);
		//			break;
		//		default:
		//			throw new System.ArgumentException("Matrix type not yet supported. " + mat.Type);
		//	}

		//	if (!eig.decompose(mat))
		//	{
		//		throw new Exception("Eigenvalue Decomposition failed");
		//	}
		//}

		/// <summary>
		/// Returns a list of all the eigenvalues
		/// </summary>
		//public virtual IList<Complex_F64> Eigenvalues
		//{
		//	get
		//	{
		//		IList<Complex_F64> ret = new List<Complex_F64>();

		//		if (mat.Type.Bits == 64)
		//		{
		//			EigenDecomposition_F64 d = (EigenDecomposition_F64)eig;
		//			for (int i = 0; i < eig.NumberOfEigenvalues; i++)
		//			{
		//				ret.Add(d.getEigenvalue(i));
		//			}
		//		}
		//		else
		//		{
		//			/*
		//			EigenDecomposition_F32 d = (EigenDecomposition_F32)eig;
		//			for (int i = 0; i < eig.NumberOfEigenvalues; i++)
		//			{
		//				Complex_F32 c = d.getEigenvalue(i);
		//				ret.Add(new Complex_F64(c.real, c.imaginary));
		//			}*/
		//		}

		//		return ret;
		//	}
		//}

		/// <summary>
		/// Returns the number of eigenvalues/eigenvectors.  This is the matrix's dimension.
		/// </summary>
		/// <returns> number of eigenvalues/eigenvectors. </returns>
		//public virtual int NumberOfEigenvalues
		//{
		//	get
		//	{
		//		return eig.NumberOfEigenvalues;
		//	}
		//}

		/// <summary>
		/// <para>
		/// Returns an eigenvalue as a complex number.  For symmetric matrices the returned eigenvalue will always be a real
		/// number, which means the imaginary component will be equal to zero.
		/// </para>
		/// 
		/// <para>
		/// NOTE: The order of the eigenvalues is dependent upon the decomposition algorithm used.  This means that they may
		/// or may not be ordered by magnitude.  For example the QR algorithm will returns results that are partially
		/// ordered by magnitude, but this behavior should not be relied upon.
		/// </para>
		/// </summary>
		/// <param name="index"> Index of the eigenvalue eigenvector pair.
		/// 
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: * return An eigenvalue. */ public Complex_F64 getEigenvalue(int index)
		//public virtual Complex_F64 getEigenvalue(int index)
		//{
		//	if (mat.Type.Bits == 64)
		//	{
		//		return ((EigenDecomposition_F64)eig).getEigenvalue(index);
		//	}
		//	else
		//	{
		//		Complex_F64 c = ((EigenDecomposition_F64)eig).getEigenvalue(index);
		//		return new Complex_F64(c.real, c.imaginary);
		//	}
		//}

		/// <summary>
		/// <para>
		/// Used to retrieve real valued eigenvectors.  If an eigenvector is associated with a complex eigenvalue
		/// then null is returned instead.
		/// </para>
		/// </summary>
		/// <param name="index"> Index of the eigenvalue eigenvector pair. </param>
		/// <returns> If the associated eigenvalue is real then an eigenvector is returned, null otherwise. </returns>
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: public @Nullable T getEigenVector(int index)
		//public virtual T getEigenVector(int index)
		//{
		//	Matrix v = eig.getEigenVector(index);
		//	if (v == null)
		//	{
		//		return null;
		//	}
		//	return (T)SimpleMatrix.wrap(v);
		//}

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
		//	if (mat.Type.Bits == 64)
		//	{
		//		return DecompositionFactory_DDRM.quality((DMatrixRMaj)mat, (EigenDecomposition_F64)eig);
		//	}
		//	else
		//	{
		//		return DecompositionFactory_DDRM.quality((DMatrixRMaj)mat, (EigenDecomposition_F64)eig);
		//		//return DecompositionFactory_FDRM.quality((FMatrixRMaj)mat, (EigenDecomposition_F32)eig);
		//	}
		//}

		///// <summary>
		///// Returns the underlying decomposition that this is a wrapper around.
		///// </summary>
		///// <returns> EigenDecomposition </returns>
		//public virtual EigenDecomposition EVD
		//{
		//	get
		//	{
		//		return eig;
		//	}
		//}

		/// <summary>
		/// Returns the index of the eigenvalue which has the largest magnitude.
		/// </summary>
		/// <returns> index of the largest magnitude eigen value. </returns>
		//public virtual int IndexMax
		//{
		//	get
		//	{
		//		int indexMax = 0;
		//		double max = getEigenvalue(0).Magnitude2;

		//		//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
		//		//ORIGINAL LINE: final int N = getNumberOfEigenvalues();
		//		int N = NumberOfEigenvalues;
		//		for (int i = 1; i < N; i++)
		//		{
		//			double m = getEigenvalue(i).Magnitude2;
		//			if (m > max)
		//			{
		//				max = m;
		//				indexMax = i;
		//			}
		//		}

		//		return indexMax;
		//	}
		//}

		///// <summary>
		///// Returns the index of the eigenvalue which has the smallest magnitude.
		///// </summary>
		///// <returns> index of the smallest magnitude eigen value. </returns>
		//public virtual int IndexMin
		//{
		//	get
		//	{
		//		int indexMin = 0;
		//		double min = getEigenvalue(0).Magnitude2;

		//		//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
		//		//ORIGINAL LINE: final int N = getNumberOfEigenvalues();
		//		int N = NumberOfEigenvalues;
		//		for (int i = 1; i < N; i++)
		//		{
		//			double m = getEigenvalue(i).Magnitude2;
		//			if (m < min)
		//			{
		//				min = m;
		//				indexMin = i;
		//			}
		//		}

		//		return indexMin;
		//	}
		//}
	}
}