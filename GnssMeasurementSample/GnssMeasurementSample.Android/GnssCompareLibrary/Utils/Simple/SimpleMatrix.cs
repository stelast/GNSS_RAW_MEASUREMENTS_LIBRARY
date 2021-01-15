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
	[Serializable]
	public class SimpleMatrix<W> : SimpleBase<SimpleMatrix<W>,W>
		where W : Matrix
	{

		/// <summary>
		/// A simplified way to reference the last row or column in the matrix for some functions.
		/// </summary>
		public static readonly int END = int.MaxValue;

		/// <summary>
		/// <para>
		/// Creates a new matrix which has the same value as the matrix encoded in the
		/// provided array.  The input matrix's format can either be row-major or
		/// column-major.
		/// </para>
		/// 
		/// <para>
		/// Note that 'data' is a variable argument type, so either 1D arrays or a set of numbers can be
		/// passed in:<br>
		/// SimpleMatrix a = new SimpleMatrix(2,2,true,new double[]{1,2,3,4});<br>
		/// SimpleMatrix b = new SimpleMatrix(2,2,true,1,2,3,4);<br>
		/// <br>
		/// Both are equivalent.
		/// </para>
		/// </summary>
		/// <param name="numRows"> The number of rows. </param>
		/// <param name="numCols"> The number of columns. </param>
		/// <param name="rowMajor"> If the array is encoded in a row-major or a column-major format. </param>
		/// <param name="data"> The formatted 1D array. Not modified. </param>
		/// <seealso cref= DMatrixRMaj#DMatrixRMaj(int, int, boolean, double...) </seealso>
		public SimpleMatrix(int numRows, int numCols, bool rowMajor, double[] data)
		{
			mat = new DMatrixRMaj(numRows, numCols, rowMajor, data);
		}
		/*
		public SimpleMatrix(int numRows, int numCols, bool rowMajor, float[] data)
		{
			mat = new FMatrixRMaj(numRows, numCols, rowMajor, data);
		}*/

		/// <summary>
		/// <para>
		/// Creates a matrix with the values and shape defined by the 2D array 'data'.
		/// It is assumed that 'data' has a row-major formatting:<br>
		/// <br>
		/// data[ row ][ column ]
		/// </para>
		/// </summary>
		/// <param name="data"> 2D array representation of the matrix. Not modified. </param>
		/// <seealso cref= DMatrixRMaj#DMatrixRMaj(double[][]) </seealso>
		public SimpleMatrix(double[][] data)
		{
			setMatrix(new DMatrixRMaj(data));
		}
		/*
		public SimpleMatrix(float[][] data)
		{
			mat = new FMatrixRMaj(data);
		}*/

		/// <summary>
		/// Creates a new matrix that is initially set to zero with the specified dimensions. This will wrap a
		/// <seealso cref="DMatrixRMaj"/>.
		/// </summary>
		/// <param name="numRows"> The number of rows in the matrix. </param>
		/// <param name="numCols"> The number of columns in the matrix. </param>
		public SimpleMatrix(int numRows, int numCols)
		{
			DMatrixRMaj dj = new DMatrixRMaj(numRows, numCols);
			setMatrix( dj );
		}

		public SimpleMatrix(int numRows, int numCols, MatrixTypesClass type) : this(numRows, numCols, MatrixType.lookup(type) /*MatrixType.lookup(type)*/)
		{
		}

		/// <summary>
		/// Create a simple matrix of the specified type
		/// </summary>
		/// <param name="numRows"> The number of rows in the matrix. </param>
		/// <param name="numCols"> The number of columns in the matrix. </param>
		/// <param name="type"> The matrix type </param>
		public SimpleMatrix(int numRows, int numCols, MatrixType type)
		{
			switch (type.tipo)
			{
				case Types.DDRM:
					mat = new DMatrixRMaj(numRows, numCols);
					break;
				case Types.FDRM:
					//mat = new FMatrixRMaj(numRows, numCols);
					break;
				case Types.ZDRM:
					mat = new ZMatrixRMaj(numRows, numCols);
					break;
				case Types.CDRM:
					//mat = new CMatrixRMaj(numRows, numCols);
					break;
				case Types.DSCC:
					mat = new DMatrixSparseCSC(numRows, numCols);
					break;
				case Types.FSCC:
					//mat = new FMatrixSparseCSC(numRows, numCols);
					break;
				default:
					throw new Exception("Unknown matrix type");
			}
		}

		/// <summary>
		/// Creats a new SimpleMatrix which is identical to the original.
		/// </summary>
		/// <param name="orig"> The matrix which is to be copied. Not modified. </param>
		public SimpleMatrix(SimpleMatrix<W> orig)
		{
			setMatrix(orig.mat.copy<Matrix>());
		}

		/// <summary>
		/// Creates a new SimpleMatrix which is a copy of the Matrix.
		/// </summary>
		/// <param name="orig"> The original matrix whose value is copied.  Not modified. </param>
		public SimpleMatrix(Matrix orig)
		{
			Matrix mat;
			if (orig is DMatrixRBlock)
			{
				DMatrixRMaj a = new DMatrixRMaj(orig.NumRows, orig.NumCols);
				DConvertMatrixStruct.convert((DMatrixRBlock)orig, a);
				mat = a;
			}
			//else if (orig is FMatrixRBlock)
			//{
				//FMatrixRMaj a = new FMatrixRMaj(orig.NumRows, orig.NumCols);
				//FConvertMatrixStruct.convert((FMatrixRBlock)orig, a);
				//mat = a;
			//}
			else
			{
				mat = orig.copy<Matrix>();
			}
			setMatrix(mat);
		}

		/// <summary>
		/// Constructor for internal library use only.  Nothing is configured and is intended for serialization.
		/// </summary>
		protected internal SimpleMatrix()
		{
		}

		/// <summary>
		/// Creates a new SimpleMatrix with the specified DMatrixRMaj used as its internal matrix.  This means
		/// that the reference is saved and calls made to the returned SimpleMatrix will modify the passed in DMatrixRMaj.
		/// </summary>
		/// <param name="internalMat"> The internal DMatrixRMaj of the returned SimpleMatrix. Will be modified. </param>
		public static SimpleMatrix<W> wrap(Matrix internalMat)
		{
			SimpleMatrix<W> ret = new SimpleMatrix<W>();
			ret.setMatrix(internalMat);
			return ret;
		}

		/// <summary>
		/// Creates a new identity matrix with the specified size.
		/// </summary>
		/// <param name="width"> The width and height of the matrix. </param>
		/// <returns> An identity matrix. </returns>
		/// <seealso cref= CommonOps_DDRM#identity(int) </seealso>
		public static SimpleMatrix<W> identity(int width)
		{
			return identity(width, MatrixTypesClass.DMatrixRMaj);
		}

		public static SimpleMatrix<W> identity(int width, MatrixTypesClass type)
		{
			SimpleMatrix<W> ret = new SimpleMatrix<W>(width, width, type);
			ret.ops.setIdentity((W)ret.mat);
			return ret;
		}

		/// <summary>
		/// <para>
		/// Creates a matrix where all but the diagonal elements are zero.  The values
		/// of the diagonal elements are specified by the parameter 'vals'.
		/// </para>
		/// 
		/// <para>
		/// To extract the diagonal elements from a matrix see <seealso cref="diag()"/>.
		/// </para>
		/// </summary>
		/// <param name="vals"> The values of the diagonal elements. </param>
		/// <returns> A diagonal matrix. </returns>
		/// <seealso cref= CommonOps_DDRM#diag(double...) </seealso>
		public static SimpleMatrix<W> diag(params double[] vals)
		{
			DMatrixRMaj m = CommonOps_DDRM.diag(vals);
			SimpleMatrix<W> ret = wrap(m);
			return ret;
		}

		/**
		 * <p>
		 * If a vector then a square matrix is returned if a matrix then a vector of diagonal ements is returned
		 * </p>
		 *
		 * @see CommonOps_DDRM#extractDiag(DMatrixRMaj, DMatrixRMaj)
		 * @return Diagonal elements inside a vector or a square matrix with the same diagonal elements.
		 */
		public T diag<T>()
			where T: SimpleMatrix<W>
		{
			T diag;
			if (bits() == 64)
			{
				if (MatrixFeatures_DDRM.isVector(mat))
				{
					int N = Math.Max(mat.NumCols, mat.NumRows);
					diag = (T)createMatrix(N, N, mat.Type);
					CommonOps_DDRM.diag((DMatrixRMaj)diag.mat, N, ((DMatrixRMaj)mat).data);
				}
				else
				{
					int N = Math.Min(mat.NumCols, mat.NumRows);
					diag = (T)createMatrix(N, 1, mat.Type);
					CommonOps_DDRM.extractDiag((DMatrixRMaj)mat, (DMatrixRMaj)diag.mat);
				}
			}
			else
			{

				if (MatrixFeatures_DDRM.isVector(mat))
				{
					int N = Math.Max(mat.NumCols, mat.NumRows);
					diag = (T)createMatrix(N, N, mat.Type);
					CommonOps_DDRM.diag((DMatrixRMaj)diag.mat, N, ((DMatrixRMaj)mat).data);
				}
				else
				{
					int N = Math.Min(mat.NumCols, mat.NumRows);
					diag = (T)createMatrix(N, 1, mat.Type);
					CommonOps_DDRM.extractDiag((DMatrixRMaj)mat, (DMatrixRMaj)diag.mat);
				}
				//if (MatrixFeatures_FDRM.isVector(mat))
				//{
				//	int N = Math.Max(mat.NumCols, mat.NumRows);
				//	diag = createMatrix(N, N, mat.Type);
				//	CommonOps_FDRM.diag((FMatrixRMaj)diag.mat, N, ((FMatrixRMaj)mat).data);
				//}
				//else
				//{
				//	int N = Math.min(mat.NumCols, mat.NumRows);
				//	diag = createMatrix(N, 1, mat.Type);
				//	CommonOps_FDRM.extractDiag((FMatrixRMaj)mat, (FMatrixRMaj)diag.mat);
				//}
			}

			return diag;
		}
		/// <summary>
		/// Creates a real valued diagonal matrix of the specified type
		/// </summary>
		/// 
		/*
		public static SimpleMatrix diag(Type type, params double[] vals)
		{
			SimpleMatrix M = new SimpleMatrix(vals.Length, vals.Length, type);
			for (int i = 0; i < vals.Length; i++)
			{
				M.set(i, i, vals[i]);
			}
			return M;
		}*/

		/// <summary>
		/// <para>
		/// Creates a new SimpleMatrix with random elements drawn from a uniform distribution from minValue to maxValue.
		/// </para>
		/// </summary>
		/// <param name="numRows"> The number of rows in the new matrix </param>
		/// <param name="numCols"> The number of columns in the new matrix </param>
		/// <param name="minValue"> Lower bound </param>
		/// <param name="maxValue"> Upper bound </param> </param>
		/// <param name="rand"> The random number generator that's used to fill the matrix.  <returns> The new random matrix. </returns>
		/// <seealso cref= RandomMatrices_DDRM#fillUniform(DMatrixRMaj, java.util.Random) </seealso>
		public static SimpleMatrix<W> random_DDRM(int numRows, int numCols, double minValue, double maxValue, Random rand)
		{
			SimpleMatrix<W> ret = new SimpleMatrix<W>(numRows, numCols);
			Java.Util.Random rd = new Java.Util.Random();
			RandomMatrices_DDRM.fillUniform((DMatrixRMaj)ret.mat, minValue, maxValue, rd);
			return ret;
		}

		//public static SimpleMatrix random_FDRM(int numRows, int numCols, float minValue, float maxValue, Random rand)
		//{
		//SimpleMatrix ret = new SimpleMatrix(numRows, numCols, typeof(FMatrixRMaj));
		//RandomMatrices_FDRM.fillUniform((FMatrixRMaj)ret.mat, minValue, maxValue, rand);
		//return ret;
		//}

		/**
		 * Returns the type of matrix is is wrapping.
		 */
		public MatrixType getType()
		{
			return mat.Type;
		}


		/**
		 * Returns the number of rows in this matrix.
		 *
		 * @return number of rows.
		 */
		public int numRows()
		{
			return mat.NumRows;
		}

		/**
		 * Returns the number of columns in this matrix.
		 *
		 * @return number of columns.
		 */
		public int numCols()
		{
			return mat.NumCols;
		}

		/**
		 * Returns the number of elements in this matrix, which is equal to
		 * the number of rows times the number of columns.
		 *
		 * @return The number of elements in the matrix.
		 */
		public int getNumElements()
		{
			return mat.NumCols * mat.NumRows;
		}


		/// <summary>
		/// <para>
		/// Creates a new vector which is drawn from a multivariate normal distribution with zero mean
		/// and the provided covariance.
		/// </para>
		/// </summary>
		/// <param name="covariance"> Covariance of the multivariate normal distribution </param>
		/// <returns> Vector randomly drawn from the distribution </returns>
		/// <seealso cref= CovarianceRandomDraw_DDRM </seealso>
		public static SimpleMatrix<W> randomNormal(SimpleMatrix<W> covariance, Random random)
		{

			SimpleMatrix<W> found = new SimpleMatrix<W>(covariance.numRows(), 1, covariance.getType());

			switch (found.getType().tipo)
			{
				case Types.DDRM:
					{
						CovarianceRandomDraw_DDRM draw = new CovarianceRandomDraw_DDRM(random, (DMatrixRMaj)covariance.mat);

						draw.next((DMatrixRMaj)found.mat);
					}
					break;

				//case MatrixType.FDRM:
				//	{
				//		CovarianceRandomDraw_FDRM draw = new CovarianceRandomDraw_FDRM(random, (FMatrixRMaj)covariance.Matrix);

				//		draw.next((FMatrixRMaj)found.mat);
				//	}
				//	break;

				default:
					throw new System.ArgumentException("Matrix type is currently not supported");
			}

			return found;
		}

		protected internal override SimpleMatrix<W> createMatrix(int numRows, int numCols, MatrixType type)
		{
			return new SimpleMatrix<W>(numRows, numCols, type);
		}

		protected internal override SimpleMatrix<W> wrapMatrix(Matrix m)
		{
			return new SimpleMatrix<W>(m);
		}
		// TODO should this function be added back?  It makes the code hard to read when its used
		//    /**
		//     * <p>
		//     * Performs one of the following matrix multiplication operations:<br>
		//     * <br>
		//     * c = a * b <br>
		//     * c = a<sup>T</sup> * b <br>
		//     * c = a * b <sup>T</sup><br>
		//     * c = a<sup>T</sup> * b <sup>T</sup><br>
		//     * <br>
		//     * where c is the returned matrix, a is this matrix, and b is the passed in matrix.
		//     * </p>
		//     *
		//     * @see CommonOps#mult(DMatrixRMaj, DMatrixRMaj, DMatrixRMaj)
		//     * @see CommonOps#multTransA(DMatrixRMaj, DMatrixRMaj, DMatrixRMaj)
		//     * @see CommonOps#multTransB(DMatrixRMaj, DMatrixRMaj, DMatrixRMaj)
		//     * @see CommonOps#multTransAB(DMatrixRMaj, DMatrixRMaj, DMatrixRMaj)
		//     *
		//     * @param tranA If true matrix A is transposed.
		//     * @param tranB If true matrix B is transposed.
		//     * @param b A matrix that is n by bn. Not modified.
		//     *
		//     * @return The results of this operation.
		//     */
		//    public SimpleMatrix mult( boolean tranA , boolean tranB , SimpleMatrix b) {
		//        SimpleMatrix ret;
		//
		//        if( tranA && tranB ) {
		//            ret = createMatrix(mat.numCols,b.mat.numRows);
		//            CommonOps.multTransAB(mat,b.mat,ret.mat);
		//        } else if( tranA ) {
		//            ret = createMatrix(mat.numCols,b.mat.numCols);
		//            CommonOps.multTransA(mat,b.mat,ret.mat);
		//        } else if( tranB ) {
		//            ret = createMatrix(mat.numRows,b.mat.numRows);
		//            CommonOps.multTransB(mat,b.mat,ret.mat);
		//        }  else  {
		//            ret = createMatrix(mat.numRows,b.mat.numCols);
		//            CommonOps.mult(mat,b.mat,ret.mat);
		//        }
		//
		//        return ret;
		//    }
	}
}