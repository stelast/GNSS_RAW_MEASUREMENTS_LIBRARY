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
using System.Net.Sockets;
using GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple.ops;

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple
{
	[Serializable]
	public abstract class SimpleBase<T,W> 
		where T : SimpleBase<T,W>
		where W : Matrix
	{

		internal const long serialVersionUID = 2342556642L;

		/// <summary>
		/// Internal matrix which this is a wrapper around.
		/// </summary>
		protected internal Matrix mat
        {
			get; set;
        }
		internal SimpleOperations<Matrix> ops;

		[NonSerialized]
		protected internal AutomaticSimpleMatrixConvert convertType = new AutomaticSimpleMatrixConvert();

		protected internal SimpleBase(int numRows, int numCols)
		{
			mat = new DMatrixRMaj(numRows, numCols);
			setMatrix(new DMatrixRMaj(numRows, numCols));
		}

		protected internal SimpleBase()
		{
		}

		//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		//ORIGINAL LINE: private void readObject(NetworkStream  in) throws java.io.IOException, ClassNotFoundException
		/*
		private void readObject(NetworkStream  @in)
		{
			@in.re
			@in.defaultReadObject();
			convertType = new AutomaticSimpleMatrixConvert();
		}*/

		/// <summary>
		/// Used internally for creating new instances of SimpleMatrix.  If SimpleMatrix is extended
		/// by another class this function should be overridden so that the returned matrices are
		/// of the correct type.
		/// </summary>
		/// <param name="numRows"> number of rows in the new matrix. </param>
		/// <param name="numCols"> number of columns in the new matrix. </param>
		/// <param name="type"> Type of matrix it should create </param>
		/// <returns> A new matrix. </returns>
		protected internal abstract T createMatrix(int numRows, int numCols, MatrixType type);

		protected internal abstract T wrapMatrix(Matrix m);

		/// <summary>
		/// <para>
		/// Returns a reference to the matrix that it uses internally.  This is useful
		/// when an operation is needed that is not provided by this class.
		/// </para>
		/// </summary>
		/// <returns> Reference to the internal DMatrixRMaj. </returns>
		public virtual InnerType getMatrix<InnerType>() where InnerType : W
		{
			return (InnerType)mat;
		}

		public virtual DMatrixRMaj DDRM
		{
			get
			{
				return (DMatrixRMaj)mat;
			}
		}

		/*
		public virtual FMatrixRMaj FDRM
		{
			get
			{
				return (FMatrixRMaj)mat;
			}
		}*/

		public virtual ZMatrixRMaj ZDRM
		{
			get
			{
				return (ZMatrixRMaj)mat;
			}
		}
		/*
		public virtual CMatrixRMaj CDRM
		{
			get
			{
				return (CMatrixRMaj)mat;
			}
		}*/

		public virtual DMatrixSparseCSC DSCC
		{
			get
			{
				return (DMatrixSparseCSC)mat;
			}
		}
		/*
		public virtual FMatrixSparseCSC FSCC
		{
			get
			{
				return (FMatrixSparseCSC)mat;
			}
		}*/

	//	protected internal static SimpleOperations<W> lookupOps<W>(MatrixType type)
	//		where W:MatrixType
	//	{
	//		switch (type)
	//		{
	//			case MatrixType.DDRM:
	//				return new SimpleOperations_DDRM();
	//			//case MatrixType.FDRM:
	//				//return new SimpleOperations_FDRM();
	//			case MatrixType.ZDRM:
	//				return new SimpleOperations_ZDRM();
	//			//case MatrixType.CDRM:
	//				//return new SimpleOperations_CDRM();
	//			case MatrixType.DSCC:
	//				return new SimpleOperations_DSCC();
	//			//case MatrixType.FSCC:
	//				//return new SimpleOperations_FSCC();
	//			default:
	//				throw new Exception("Unknown Matrix Type. " + type);
	//		}
	//	}

	//	/// <summary>
	//	/// <para>
	//	/// Returns the transpose of this matrix.<br>
	//	/// a<sup>T</sup>
	//	/// </para>
	//	/// </summary>
	//	/// <returns> A matrix that is n by m. </returns>
	//	/// <seealso cref= CommonOps_DDRM#transpose(DMatrixRMaj, DMatrixRMaj) </seealso>
		public virtual T transpose()
		{
			T ret = createMatrix(mat.NumCols, mat.NumRows, mat.Type);

			ops.transpose((Matrix)mat, (Matrix)ret.mat);

			return ret;
		}

	//	/// <summary>
	//	/// <para>
	//	/// Returns a matrix which is the result of matrix multiplication:<br>
	//	/// <br>
	//	/// c = a * b <br>
	//	/// <br>
	//	/// where c is the returned matrix, a is this matrix, and b is the passed in matrix.
	//	/// </para>
	//	/// </summary>
	//	/// <param name="B"> A matrix that is n by bn. Not modified. </param>
	//	/// <returns> The results of this operation. </returns>
	//	/// <seealso cref= CommonOps_DDRM#mult(DMatrix1Row, DMatrix1Row, DMatrix1Row) </seealso>
		public virtual T mult(T B)
		{
			convertType.specify<T,W>(this, B);

			// Look to see if there is a special function for handling this case
			//if (this.mat.Type.tipo != B.getType.tipo)
			//{
	//			System.Reflection.MethodInfo m = findAlternative("mult", mat, B.mat, convertType.commonType.ClassType);
	//			if (m != null)
	//			{
	//				T ret = wrapMatrix(convertType.commonType.create(1, 1));
	//				invoke(m, this.mat, B.mat, ret.mat);
	//				return ret;
	//			}
			//}

			// Otherwise convert into a common matrix type if necessary
			T A = convertType.convert(this);
			B = convertType.convert(B);

			T ret = A.createMatrix(mat.NumRows, B.mat.NumCols, A.getType);

			A.ops.mult((Matrix)A.mat, (Matrix)B.mat, (Matrix)ret.mat);

			return ret;
		}

	//	/// <summary>
	//	/// <para>
	//	/// Computes the Kronecker product between this matrix and the provided B matrix:<br>
	//	/// <br>
	//	/// C = kron(A,B)
	//	/// </para>
	//	/// 	
	//	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//	//ORIGINAL LINE: * * @param B The right matrix in the operation.Not modified. * return Kronecker product between this matrix and B. * @see CommonOps_DDRM#kron(DMatrixRMaj, DMatrixRMaj, DMatrixRMaj) */ public T kron(T B)
	//	//* * B The right matrix @in the operation.Not modified. * Kronecker product between this matrix and B. * CommonOps_DDRM#kron(DMatrixRMaj, DMatrixRMaj, DMatrixRMaj) * / 
	//	public virtual T kron(T B)
	//	{
	//		convertType.specify(this, B);
	//		T A = convertType.convert(this);
	//		B = convertType.convert(B);

	//		T ret = A.createMatrix(mat.NumRows * B.numRows(), mat.NumCols * B.numCols(), A.Type);

	//		A.ops.kron(A.mat, B.mat, ret.mat);

	//		return ret;
	//	}

	//	/// <summary>
	//	/// <para>
	//	/// Returns the result of matrix addition:<br>
	//	/// <br>
	//	/// c = a + b <br>
	//	/// <br>
	//	/// where c is the returned matrix, a is this matrix, and b is the passed in matrix.
	//	/// </para>
	//	/// </summary>
	//	/// <param name="B"> m by n matrix. Not modified. </param>
	//	/// <returns> The results of this operation. </returns>
	//	/// <seealso cref= CommonOps_DDRM#mult(DMatrix1Row, DMatrix1Row, DMatrix1Row) </seealso>
		public virtual T plus(T B)
		{
			convertType.specify(this, B);
			T A = convertType.convert(this);
			B = convertType.convert(B);

			T ret = A.createMatrix(mat.NumRows, mat.NumCols, A.getType);

			A.ops.plus((Matrix)A.mat, (Matrix)B.mat, (Matrix)ret.mat);

			return ret;
		}

	//	/// <summary>
	//	/// <para>
	//	/// Returns the result of matrix subtraction:<br>
	//	/// <br>
	//	/// c = a - b <br>
	//	/// <br>
	//	/// where c is the returned matrix, a is this matrix, and b is the passed in matrix.
	//	/// </para>
	//	/// </summary>
	//	/// <param name="B"> m by n matrix. Not modified. </param>
	//	/// <returns> The results of this operation. </returns>
	//	/// <seealso cref= CommonOps_DDRM#subtract(DMatrixD1, DMatrixD1, DMatrixD1) </seealso>
		public virtual T minus(T B)
		{
			convertType.specify(this, B);
			T A = convertType.convert(this);
			B = convertType.convert(B);
			T ret = A.createLike();			

			A.ops.minus((Matrix)A.mat, (Matrix)B.mat, (Matrix)ret.mat);
			return ret;
		}

	//	/// <summary>
	//	/// <para>
	//	/// Returns the result of matrix-double subtraction:<br>
	//	/// <br>
	//	/// c = a - b <br>
	//	/// <br>
	//	/// where c is the returned matrix, a is this matrix, and b is the passed in double.
	//	/// </para>
	//	/// </summary>
	//	/// <param name="b"> Value subtracted from each element </param>
	//	/// <returns> The results of this operation. </returns>
	//	/// <seealso cref= CommonOps_DDRM#subtract(DMatrixD1, double, DMatrixD1) </seealso>
	//	public virtual T minus(double b)
	//	{
	//		T ret = createLike();
	//		ops.minus(mat, b, ret.mat);
	//		return ret;
	//	}

	//	/// <summary>
	//	/// <para>
	//	/// Returns the result of scalar addition:<br>
	//	/// <br>
	//	/// c = a + b<br>
	//	/// <br>
	//	/// where c is the returned matrix, a is this matrix, and b is the passed in double.
	//	/// </para>
	//	/// </summary>
	//	/// <param name="b"> Value added to each element </param>
	//	/// <returns> A matrix that contains the results. </returns>
	//	/// <seealso cref= CommonOps_DDRM#add(DMatrixD1, double, DMatrixD1) </seealso>
		public virtual T plus(double b)
		{
			T ret = createLike();
			ops.plus((Matrix)mat, b, (Matrix)ret.mat);

			return ret;
		}

	//	/// <summary>
	//	/// <para>
	//	/// Performs a matrix addition and scale operation.<br>
	//	/// <br>
	//	/// c = a + &beta;*b <br>
	//	/// <br>
	//	/// where c is the returned matrix, a is this matrix, and b is the passed in matrix.
	//	/// </para>
	//	/// </summary>
	//	/// <param name="B"> m by n matrix. Not modified. </param>
	//	/// <returns> A matrix that contains the results. </returns>
	//	/// <seealso cref= CommonOps_DDRM#add(DMatrixD1, double, DMatrixD1, DMatrixD1) </seealso>
		public virtual T plus(double beta, T B)
		{
			convertType.specify(this, B);
			T A = convertType.convert(this);
			B = convertType.convert(B);

			T ret = A.createLike();
			A.ops.plus((Matrix)A.mat, beta, (Matrix)B.mat, (Matrix)ret.mat);
			return ret;
		}

	//	/// <summary>
	//	/// Computes the dot product (a.k.a. inner product) between this vector and vector 'v'.
	//	/// </summary>
	//	/// <param name="v"> The second vector in the dot product.  Not modified. </param>
	//	/// <returns> dot product </returns>
	//	public virtual double dot(T v)
	//	{
	//		convertType.specify(this, v);
	//		T A = convertType.convert(this);
	//		v = convertType.convert(v);

	//		if (!ArrayList)
	//		{
	//			throw new System.ArgumentException("'this' matrix is not a vector.");
	//		}
	//		else if (!v.Vector)
	//		{
	//			throw new System.ArgumentException("'v' matrix is not a vector.");
	//		}

	//		return A.ops.dot(A.mat, v.Matrix);
	//	}

	///// <summary>
	///// Returns true if this matrix is a vector.  A vector is defined as a matrix
	///// that has either one row or column.
	///// </summary>
	///// <returns> Returns true for vectors and false otherwise. </returns>
	//public virtual bool Vector
	//{
	//	get
	//	{
	//		return mat.NumRows == 1 || mat.NumCols == 1;
	//	}
	//}

	///// <summary>
	///// <para>
	///// Returns the result of scaling each element by 'val':<br>
	///// b<sub>i,j</sub> = val*a<sub>i,j</sub>
	///// </para>
	///// </summary>
	///// <param name="val"> The multiplication factor. </param>
	///// <returns> The scaled matrix. </returns>
	///// <seealso cref= CommonOps_DDRM#scale(double, DMatrixD1) </seealso>
	public virtual T scale(double val)
	{
		T ret = createLike();
		ops.scale((Matrix)mat, val, (Matrix)ret.mat);
		return ret;
	}

	///// <summary>
	///// <para>
	///// Returns the result of dividing each element by 'val':
	///// b<sub>i,j</sub> = a<sub>i,j</sub>/val
	///// </para>
	///// </summary>
	///// <param name="val"> Divisor. </param>
	///// <returns> Matrix with its elements divided by the specified value. </returns>
	///// <seealso cref= CommonOps_DDRM#divide(DMatrixD1, double) </seealso>
	public virtual T divide(double val)
	{
		T ret = createLike();
		ops.divide((Matrix)mat, val, (Matrix)ret.mat);
		return ret;
	}

	///// <summary>
	///// <para>
	///// Returns the inverse of this matrix.<br>
	///// <br>
	///// b = a<sup>-1</sup><br>
	///// </para>
	///// 
	///// <para>
	///// If the matrix could not be inverted then SingularMatrixException is thrown.  Even
	///// if no exception is thrown the matrix could still be singular or nearly singular.
	///// </para>
	///// 
	///// 
	// //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// //ORIGINAL LINE: * return The inverse of this matrix. * @see CommonOps_DDRM#invert(DMatrixRMaj, DMatrixRMaj) */ public T invert()
	// //* The inverse of this matrix. * CommonOps_DDRM#invert(DMatrixRMaj, DMatrixRMaj) * / 
	 public virtual T invert()
	 {
		T ret = createLike();

		if (!ops.invert((Matrix)mat, (Matrix)ret.mat))
		{
			throw new SingularMatrixException();
		}
		if (ops.hasUncountable((Matrix)ret.mat))
		{
			throw new SingularMatrixException("Solution contains uncountable numbers");
		}

		return ret;
	}

	///// <summary>
	///// <para>
	///// Computes the Moore-Penrose pseudo-inverse
	///// </para>
	///// </summary>
	///// <returns> inverse computed using the pseudo inverse. </returns>
	//public virtual T pseudoInverse()
	//{
	//	T ret = createLike();
	//	ops.pseudoInverse(mat, ret.mat);
	//	return ret;
	//}

	///// <summary>
	///// <para>
	///// Solves for X in the following equation:<br>
	///// <br>
	///// x = a<sup>-1</sup>b<br>
	///// <br>
	///// where 'a' is this matrix and 'b' is an n by p matrix.
	///// </para>
	///// 
	///// <para>
	///// If the system could not be solved then SingularMatrixException is thrown.  Even
	///// if no exception is thrown 'a' could still be singular or nearly singular.
	///// </para>
	///// </summary>
	///// <param name="B"> n by p matrix. Not modified. </param>
	///// <returns> The solution for 'x' that is n by p. </returns>
	///// <seealso cref= CommonOps_DDRM#solve(DMatrixRMaj, DMatrixRMaj, DMatrixRMaj) </seealso>
	//public virtual T solve(T B)
	//{
	//	convertType.specify(this, B);

	//	// Look to see if there is a special function for handling this case
	//	if (this.mat.Type != B.Type)
	//	{
	//		System.Reflection.MethodInfo m = findAlternative("solve", mat, B.mat, convertType.commonType.ClassType);
	//		if (m != null)
	//		{
	//			T ret = wrapMatrix(convertType.commonType.create(1, 1));
	//			invoke(m, this.mat, B.mat, ret.mat); // TODO handle boolean return from solve
	//			return ret;
	//		}
	//	}

	//	T A = convertType.convert(this);
	//	B = convertType.convert(B);

	//	T x = A.createMatrix(mat.NumCols, B.Matrix.NumCols, A.Type);

	//	if (!A.ops.solve(A.mat, x.mat, B.mat))
	//	{
	//		throw new SingularMatrixException();
	//	}
	//	if (A.ops.hasUncountable(x.mat))
	//	{
	//		throw new SingularMatrixException("Solution contains uncountable numbers");
	//	}

	//	return x;
	//}

	///// <summary>
	///// Sets the elements in this matrix to be equal to the elements in the passed in matrix.
	///// Both matrix must have the same dimension.
	///// </summary>
	///// <param name="a"> The matrix whose value this matrix is being set to. </param>
	//public virtual T To
	//{
	//	set
	//	{
	//		if (value.Type == Type)
	//		{
	//			mat.To = value.Matrix;
	//		}
	//		else
	//		{
	//			Matrix = value.mat.copy();
	//		}
	//	}
	//}

	///// <summary>
	///// <para>
	///// Sets all the elements in this matrix equal to the specified value.<br>
	///// <br>
	///// a<sub>ij</sub> = val<br>
	///// </para>
	///// </summary>
	///// <param name="val"> The value each element is set to. </param>
	///// <seealso cref= CommonOps_DDRM#fill(DMatrixD1, double) </seealso>
	public virtual void fill(double val)
	{
		try
		{
			ops.fill((Matrix)mat, val);
		}
		catch (ConvertToDenseException)
		{
			convertToDense();
			fill(val);
		}
	}
		/**
		 * Sets the elements in this matrix to be equal to the elements in the passed in matrix.
		 * Both matrix must have the same dimension.
		 *
		 * @param a The matrix whose value this matrix is being set to.
		 */
		public void set(T a)
		{
			mat = a.mat;
		}

		/**
		 * <p>
		 * Sets all the elements in this matrix equal to the specified value.<br>
		 * <br>
		 * a<sub>ij</sub> = val<br>
		 * </p>
		 *
		 * @see CommonOps_DDRM#fill(DMatrixD1, double)
		 *
		 * @param val The value each element is set to.
		 */
		public void set(double val)
		{
			if (bits() == 64)
			{
				CommonOps_DDRM.fill((DMatrixD1)mat, val);
			}
			else
			{
				CommonOps_DDRM.fill((DMatrixD1)mat, val);
				//CommonOps_FDRM.fill((FMatrixRMaj)mat, (float)val);
			}
		}

		///// <summary>
		///// Sets all the elements in the matrix equal to zero.
		///// </summary>
		///// <seealso cref= CommonOps_DDRM#fill(DMatrixD1, double) </seealso>
		public void zero()
	{
		fill(0);
	}

	///// <summary>
	///// <para>
	///// Computes the Frobenius normal of the matrix:<br>
	///// <br>
	///// normF = Sqrt{  &sum;<sub>i=1:m</sub> &sum;<sub>j=1:n</sub> { a<sub>ij</sub><sup>2</sup>}   }
	///// </para>
	///// </summary>
	///// <returns> The matrix's Frobenius normal. </returns>
	///// <seealso cref= NormOps_DDRM#normF(DMatrixD1) </seealso>
	//public double normF()
	//{
	//	return ops.normF(mat);
	//}

	///// <summary>
	///// <para>
	///// The condition p = 2 number of a matrix is used to measure the sensitivity of the linear
	///// system <b>Ax=b</b>.  A value near one indicates that it is a well conditioned matrix.
	///// </para>
	///// </summary>
	///// <returns> The condition number. </returns>
	///// <seealso cref= NormOps_DDRM#conditionP2(Matrix) </seealso>
	//public double conditionP2()
	//{
	//	return ops.conditionP2(mat);
	//}

	///// <summary>
	///// Computes the determinant of the matrix.
	///// </summary>
	///// <returns> The determinant. </returns>
	///// <seealso cref= CommonOps_DDRM#det(Matrix) </seealso>
	//public double determinant()
	//{
	//	double ret = ops.determinant(mat);
	//	if (UtilEjml.isUncountable(ret))
	//	{
	//		return 0;
	//	}
	//	return ret;
	//}

	///// <summary>
	///// <para>
	///// Computes the trace of the matrix.
	///// </para>
	///// </summary>
	///// <returns> The trace of the matrix. </returns>
	///// <seealso cref= CommonOps_DDRM#trace(DMatrix1Row) </seealso>
	//public double trace()
	//{
	//	return ops.trace(mat);
	//}

	///// <summary>
	///// <para>
	///// Reshapes the matrix to the specified number of rows and columns.  If the total number of elements
	///// is &le; number of elements it had before the data is saved.  Otherwise a new internal array is
	///// declared and the old data lost.
	///// </para>
	///// 
	///// <para>
	///// This is equivalent to calling A.mat.reshape(numRows,numCols,false).
	///// </para>
	///// </summary>
	///// <param name="numRows"> The new number of rows in the matrix. </param>
	///// <param name="numCols"> The new number of columns in the matrix. </param>
	///// <seealso cref= DMatrixRMaj#reshape(int, int, boolean) </seealso>
	//public void reshape(int numRows, int numCols)
	//{
	//	if (mat.Type.Fixed)
	//	{
	//		throw new System.ArgumentException("Can't reshape a fixed sized matrix");
	//	}
	//	else
	//	{
	//		((ReshapeMatrix)mat).reshape(numRows, numCols);
	//	}
	//}

	///// <summary>
	///// Assigns the element in the Matrix to the specified value.  Performs a bounds check to make sure
	///// the requested element is part of the matrix.
	///// </summary>
	///// <param name="row"> The row of the element. </param>
	///// <param name="col"> The column of the element. </param>
	///// <param name="value"> The element's new value. </param>
	public void set(int row, int col, double value)
	{
		ops.set((Matrix)mat, row, col, value);//ops es nulo
	}



	///// <summary>
	///// Assigns an element a value based on its index in the internal array..
	///// </summary>
	///// <param name="index"> The matrix element that is being assigned a value. </param>
	///// <param name="value"> The element's new value. </param>
	public virtual void set(int index, double value)
	{
		if (mat.Type.tipo == Types.DDRM)
		{
			((DMatrixD1)mat).set(index, value);
		}
		else if (mat.Type.tipo == Types.FDRM)
		{
				((DMatrixD1)mat).set(index, value);
				//((FMatrixRMaj)mat).set(index, (float)value);
		}
		else
		{
			throw new Exception("Not supported yet for this matrix type");
		}
	}

	///// <summary>
	///// Used to set the complex value of a matrix element.
	///// </summary>
	///// <param name="row"> The row of the element. </param>
	///// <param name="col"> The column of the element. </param>
	///// <param name="real"> Real component of assigned value </param>
	///// <param name="imaginary"> Imaginary component of assigned value </param>
	public virtual void set(int row, int col, double real, double imaginary)
	{
		if (imaginary == 0)
		{
			set(row, col, real);
		}
		else
		{
			ops.set((Matrix)mat, row, col, real, imaginary);
		}
	}

	///// <summary>
	///// <para>
	///// Assigns consecutive elements inside a row to the provided array.<br>
	///// <br>
	///// A(row,offset:(offset + values.length)) = values
	///// </para>
	///// </summary>
	///// <param name="row"> The row that the array is to be written to. </param>
	///// <param name="startColumn"> The initial column that the array is written to. </param>
	///// <param name="values"> Values which are to be written to the row in a matrix. </param>
	//public virtual void setRow(int row, int startColumn, params double[] values)
	//{
	//	ops.setRow(mat, row, startColumn, values);
	//}

	///// <summary>
	///// <para>
	///// Assigns consecutive elements inside a column to the provided array.<br>
	///// <br>
	///// A(offset:(offset + values.length),column) = values
	///// </para>
	///// </summary>
	///// <param name="column"> The column that the array is to be written to. </param>
	///// <param name="startRow"> The initial column that the array is written to. </param>
	///// <param name="values"> Values which are to be written to the row in a matrix. </param>
	//public virtual void setColumn(int column, int startRow, params double[] values)
	//{
	//	ops.setColumn(mat, column, startRow, values);
	//}

	///// <summary>
	///// Returns the value of the specified matrix element.  Performs a bounds check to make sure
	///// the requested element is part of the matrix.
	///// 
	///// NOTE: Complex matrices will throw an exception
	///// </summary>
	///// <param name="row"> The row of the element. </param>
	///// <param name="col"> The column of the element. </param>
	///// <returns> The value of the element. </returns>
	public virtual double get(int row, int col)
	{
		return ops.get((Matrix)mat, row, col);
	}

	///// <summary>
	///// Returns the value of the matrix at the specified index of the 1D row major array.
	///// </summary>
	///// <param name="index"> The element's index whose value is to be returned </param>
	///// <returns> The value of the specified element. </returns>
	///// <seealso cref= DMatrixRMaj#get(int) </seealso>
	public virtual double get(int index)
	{
		MatrixType type = mat.Type;

		if (type.isReal())
		{
			if (type.getBits() == 64)
			{
				return ((DMatrixD1)mat).data[index];
			}
			else
			{
				return ((DMatrixD1)mat).data[index];
				//return ((FMatrixRMaj)mat).data[index];
			}
		}
		else
		{
			throw new System.SystemException("Complex matrix. Call get(int,Complex64F) instead");
		}
	}

	///// <summary>
	///// Used to get the complex value of a matrix element.
	///// 
	///// 
	// //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// //ORIGINAL LINE: * @param row The row of the element. * @param col The column of the element. * @param output Storage for the value */ public void get(int row, int col, Complex_F64 output)
	// ///* row The row of the element. * col The column of the element. * output Storage for the value * / 
	//public virtual void get(int row, int col, Complex_F64 output)
	//{
	//	ops.get(mat, row, col, output);
	//}

	///// <summary>
	///// Returns the index in the matrix's array.
	///// </summary>
	///// <param name="row"> The row number. </param>
	///// <param name="col"> The column number. </param>
	///// <returns> The index of the specified element. </returns>
	///// <seealso cref= DMatrixRMaj#getIndex(int, int) </seealso>
	//public virtual int getIndex(int row, int col)
	//{
	//	return row * mat.NumCols + col;
	//}

	///// <summary>
	///// Creates a new iterator for traversing through a submatrix inside this matrix.  It can be traversed
	///// by row or by column.  Range of elements is inclusive, e.g. minRow = 0 and maxRow = 1 will include rows
	///// 0 and 1.  The iteration starts at (minRow,minCol) and ends at (maxRow,maxCol)
	///// </summary>
	///// <param name="rowMajor"> true means it will traverse through the submatrix by row first, false by columns. </param>
	///// <param name="minRow"> first row it will start at. </param>
	///// <param name="minCol"> first column it will start at. </param>
	///// <param name="maxRow"> last row it will stop at. </param>
	///// <param name="maxCol"> last column it will stop at. </param>
	///// <returns> A new MatrixIterator </returns>
	//public virtual DMatrixIterator iterator(bool rowMajor, int minRow, int minCol, int maxRow, int maxCol)
	//{
	//	return new DMatrixIterator((Matrix)mat, rowMajor, minRow, minCol, maxRow, maxCol);
	//}

	///// <summary>
	///// Creates and returns a matrix which is idential to this one.
	///// </summary>
	///// <returns> A new identical matrix. </returns>
	public virtual T copy()
	{
		T ret = createLike();
		ret.mat.To = this.mat;
		return ret;
	}

	///// <summary>
	///// Returns the number of rows in this matrix.
	///// </summary>
	///// <returns> number of rows. </returns>
	public virtual int numRows()
	{
		return mat.NumRows;
	}

	///// <summary>
	///// Returns the number of columns in this matrix.
	///// </summary>
	///// <returns> number of columns. </returns>
	public virtual int numCols()
	{
		return mat.NumCols;
	}

		///// <summary>
		///// Returns the number of elements in this matrix, which is equal to
		///// the number of rows times the number of columns.
		///// </summary>
		///// <returns> The number of elements in the matrix. </returns>
		//public virtual int NumElements
		//{
		//	get
		//	{
		//		return mat.NumCols * mat.NumRows;
		//	}
		//}

		///// <summary>
		///// Prints the matrix to standard out.
		///// </summary>
		//public virtual void print()
		//{
		//	mat.print();
		//}

		///// <summary>
		///// <para>
		///// Prints the matrix to standard out given a <seealso cref="java.io.PrintStream.printf"/> style floating point format,
		///// e.g. print("%f").
		///// </para>
		///// </summary>
		//public virtual void print(string format)
		//{
		//	ops.print(System.out, mat, format);
		//}

		///// <summary>
		///// <para>
		///// Converts the array into a string format for display purposes.
		///// The conversion is done using <seealso cref="MatrixIO.print(java.io.PrintStream, DMatrix)"/>.
		///// </para>
		///// </summary>
		///// <returns> String representation of the matrix. </returns>


		//public override string ToString()
		//{
		//	MemoryStream stream = new MemoryStream();
		//	PrintStream p = new PrintStream(stream);

		//	MatrixIO.print(p, mat);

		//	return stream.ToString();
		//}

		///// <summary>
		///// <para>
		///// Creates a new SimpleMatrix which is a submatrix of this matrix.
		///// </para>
		///// <para>
		///// s<sub>i-y0 , j-x0</sub> = o<sub>ij</sub> for all y0 &le; i &lt; y1 and x0 &le; j &lt; x1<br>
		///// <br>
		///// where 's<sub>ij</sub>' is an element in the submatrix and 'o<sub>ij</sub>' is an element in the
		///// original matrix.
		///// </para>
		///// 
		///// <para>
		///// If any of the inputs are set to SimpleMatrix.END then it will be set to the last row
		///// or column in the matrix.
		///// </para>
		///// </summary>
		///// <param name="y0"> Start row. </param>
		///// <param name="y1"> Stop row + 1. </param>
		///// <param name="x0"> Start column. </param>
		///// <param name="x1"> Stop column + 1. </param>
		///// <returns> The submatrix. </returns>
		//public virtual T extractMatrix(int y0, int y1, int x0, int x1)
		//{
		//	if (y0 == SimpleMatrix.END)
		//	{
		//		y0 = mat.NumRows;
		//	}
		//	if (y1 == SimpleMatrix.END)
		//	{
		//		y1 = mat.NumRows;
		//	}
		//	if (x0 == SimpleMatrix.END)
		//	{
		//		x0 = mat.NumCols;
		//	}
		//	if (x1 == SimpleMatrix.END)
		//	{
		//		x1 = mat.NumCols;
		//	}

		//	T ret = createMatrix(y1 - y0, x1 - x0, mat.Type);

		//	ops.extract(mat, y0, y1, x0, x1, ret.mat, 0, 0);

		//	return ret;
		//}

		///// <summary>
		///// <para>
		///// Extracts a row or column from this matrix. The returned vector will either be a row
		///// or column vector depending on the input type.
		///// </para>
		///// </summary>
		///// <param name="extractRow"> If true a row will be extracted. </param>
		///// <param name="element"> The row or column the vector is contained in. </param>
		///// <returns> Extracted vector. </returns>
		//public virtual T extractVector(bool extractRow, int element)
		//{
		//	if (extractRow)
		//	{
		//		return extractMatrix(element, element + 1, 0, SimpleMatrix.END);
		//	}
		//	else
		//	{
		//		return extractMatrix(0, SimpleMatrix.END, element, element + 1);
		//	}
		//}


		///// <summary>
		///// <para>
		///// If a vector then a square matrix is returned if a matrix then a vector of diagonal ements is returned
		///// </para>
		///// </summary>
		///// <returns> Diagonal elements inside a vector or a square matrix with the same diagonal elements. </returns>
		///// <seealso cref= CommonOps_DDRM#extractDiag(DMatrixRMaj, DMatrixRMaj) </seealso>
		public virtual T diag()
		{
			return wrapMatrix(ops.diag((Matrix)mat));
		}

	///// <summary>
	///// Checks to see if matrix 'a' is the same as this matrix within the specified
	///// tolerance.
	///// </summary>
	///// <param name="a"> The matrix it is being compared against. </param>
	///// <param name="tol"> How similar they must be to be equals. </param>
	///// <returns> If they are equal within tolerance of each other. </returns>
	public virtual bool isIdentical(T a, double tol)
	{
		if (a.getType.tipo != mat.Type.tipo)
		{
			return false;
		}
		return ops.isIdentical((Matrix)mat, (Matrix)a.mat, tol);
	}

	///// <summary>
	///// Checks to see if any of the elements in this matrix are either NaN or infinite.
	///// </summary>
	///// <returns> True of an element is NaN or infinite.  False otherwise. </returns>
	//public virtual bool hasUncountable()
	//{
	//	return ops.hasUncountable(mat);
	//}

	///// <summary>
	///// Computes a full Singular Value Decomposition (SVD) of this matrix with the
	///// 

	// //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// //ORIGINAL LINE: * eigenvalues ordered from largest to smallest. * * return SVD */ public SimpleSVD<T> svd()
	// ///* eigenvalues ordered from largest to smallest. ** SVD * / 
	//public virtual SimpleSVD<T> svd()
	//{
	//	return new SimpleSVD(mat, false);
	//}

	///// <summary>
	///// Computes the SVD in either  compact format or full format.
	///// </summary>
	///// <returns> SVD of this matrix. </returns>
	//public virtual SimpleSVD<T> svd(bool compact)
	//{
	//	return new SimpleSVD(mat, compact);
	//}

	///// <summary>
	///// Returns the Eigen Value Decomposition (EVD) of this matrix.
	///// </summary>
	//public virtual SimpleEVD<T> eig()
	//{
	//	return new SimpleEVD(mat);
	//}

	///// <summary>
	///// Copy matrix B into this matrix at location (insertRow, insertCol).
	///// </summary>
	///// <param name="insertRow"> First row the matrix is to be inserted into. </param>
	///// <param name="insertCol"> First column the matrix is to be inserted into. </param>
	///// <param name="B"> The matrix that is being inserted. </param>
	public virtual void insertIntoThis(int insertRow, int insertCol, T B)
	{
		convertType.specify(this, B);
		B = convertType.convert(B);

		// See if this type's need to be changed or not
		if (convertType.commonType.tipo == mat.Type.tipo)
		{
			insert(B.mat, mat, insertRow, insertCol);
		}
		else
		{
			T A = convertType.convert(this);
			A.insert(B.mat, A.mat, insertRow, insertCol);
			mat = A.mat;
		}
	}

	internal virtual void insert(Matrix src, Matrix dst, int destY0, int destX0)
	{
		ops.extract((Matrix)src, 0, src.NumRows, 0, src.NumCols, (Matrix)dst, destY0, destX0);
	}

	///// <summary>
	///// <para>
	///// Creates a new matrix that is a combination of this matrix and matrix B.  B is
	///// written into A at the specified location if needed the size of A is increased by
	///// growing it.  A is grown by padding the new area with zeros.
	///// </para>
	///// 
	///// <para>
	///// While useful when adding data to a matrix which will be solved for it is also much
	///// less efficient than predeclaring a matrix and inserting data into it.
	///// </para>
	///// 
	///// <para>
	///// If insertRow or insertCol is set to SimpleMatrix.END then it will be combined
	///// at the last row or column respectively.
	///// </para>
	///// <para>
	///// 
	///// </para>
	///// </summary>
	///// <param name="insertRow"> Row where matrix B is written in to. </param>
	///// <param name="insertCol"> Column where matrix B is written in to. </param>
	///// <param name="B"> The matrix that is written into A. </param>
	///// <returns> A new combined matrix. </returns>
	public virtual T combine(int insertRow, int insertCol, T B)
	{
		convertType.specify(this, B);
		T A = convertType.convert(this);
		B = convertType.convert(B);

		if (insertRow == SimpleMatrix<W>.END)
		{
			insertRow = mat.NumRows;
		}

		if (insertCol == SimpleMatrix<W>.END)
		{
			insertCol = mat.NumCols;
		}

		int maxRow = insertRow + B.numRows();
		int maxCol = insertCol + B.numCols();

		T ret;

		if (maxRow > mat.NumRows || maxCol > mat.NumCols)
		{
			int M = Math.Max(maxRow, mat.NumRows);
			int N = Math.Max(maxCol, mat.NumCols);

			ret = A.createMatrix(M, N, A.mat.Type);
			ret.insertIntoThis(0, 0, A);
		}
		else
		{
			ret = A.copy();
		}

		ret.insertIntoThis(insertRow, insertCol, B);
		return ret;
	}

	///// <summary>
	///// Returns the maximum absolute value of all the elements in this matrix.  This is
	///// equivalent the the infinite p-norm of the matrix.
	///// </summary>
	///// <returns> Largest absolute value of any element. </returns>
	//public virtual double elementMaxAbs()
	//{
	//	return ops.elementMaxAbs(mat);
	//}

	///// <summary>
	///// Returns the minimum absolute value of all the elements in this matrix.
	///// </summary>
	///// <returns> Smallest absolute value of any element. </returns>
	//public virtual double elementMinAbs()
	//{
	//	return ops.elementMinAbs(mat);
	//}

	///// <summary>
	///// Computes the sum of all the elements in the matrix.
	///// </summary>
	///// <returns> Sum of all the elements. </returns>
	public virtual double elementSum()
	{
		return ops.elementSum((Matrix)mat);
	}

	///// <summary>
	///// <para>
	///// Returns a matrix which is the result of an element by element multiplication of 'this' and 'b':
	///// c<sub>i,j</sub> = a<sub>i,j</sub>*b<sub>i,j</sub>
	///// </para>
	///// </summary>
	///// <param name="b"> A simple matrix. </param>
	///// <returns> The element by element multiplication of 'this' and 'b'. </returns>
	//public virtual T elementMult(T b)
	//{
	//	convertType.specify(this, b);
	//	T A = convertType.convert(this);
	//	b = convertType.convert(b);

	//	T c = A.createLike();
	//	A.ops.elementMult(A.mat, b.mat, c.mat);
	//	return c;
	//}

	///// <summary>
	///// <para>
	///// Returns a matrix which is the result of an element by element division of 'this' and 'b':
	///// c<sub>i,j</sub> = a<sub>i,j</sub>/b<sub>i,j</sub>
	///// </para>
	///// </summary>
	///// <param name="b"> A simple matrix. </param>
	///// <returns> The element by element division of 'this' and 'b'. </returns>
	//public virtual T elementDiv(T b)
	//{
	//	convertType.specify(this, b);
	//	T A = convertType.convert(this);
	//	b = convertType.convert(b);

	//	T c = A.createLike();
	//	A.ops.elementDiv(A.mat, b.mat, c.mat);
	//	return c;
	//}

	///// <summary>
	///// <para>
	///// Returns a matrix which is the result of an element by element power of 'this' and 'b':
	///// c<sub>i,j</sub> = a<sub>i,j</sub> ^ b<sub>i,j</sub>
	///// </para>
	///// </summary>
	///// <param name="b"> A simple matrix. </param>
	///// <returns> The element by element power of 'this' and 'b'. </returns>
	//public virtual T elementPower(T b)
	//{
	//	convertType.specify(this, b);
	//	T A = convertType.convert(this);
	//	b = convertType.convert(b);

	//	T c = A.createLike();
	//	A.ops.elementPower(A.mat, b.mat, c.mat);
	//	return c;
	//}

	///// <summary>
	///// <para>
	///// Returns a matrix which is the result of an element by element power of 'this' and 'b':
	///// c<sub>i,j</sub> = a<sub>i,j</sub> ^ b
	///// </para>
	///// </summary>
	///// <param name="b"> Scalar </param>
	///// <returns> The element by element power of 'this' and 'b'. </returns>
	//public virtual T elementPower(double b)
	//{
	//	T c = createLike();
	//	ops.elementPower(mat, b, c.mat);
	//	return c;
	//}

	///// <summary>
	///// <para>
	///// Returns a matrix which is the result of an element by element exp of 'this'
	///// c<sub>i,j</sub> = Math.exp(a<sub>i,j</sub>)
	///// </para>
	///// </summary>
	///// <returns> The element by element power of 'this' and 'b'. </returns>
	//public T elementExp()
	//{
	//	T c = createLike();
	//	ops.elementExp(mat, c.mat);
	//	return c;
	//}

	///// <summary>
	///// <para>
	///// Returns a matrix which is the result of an element by element exp of 'this'
	///// c<sub>i,j</sub> = Math.log(a<sub>i,j</sub>)
	///// </para>
	///// </summary>
	///// <returns> The element by element power of 'this' and 'b'. </returns>
	//public T elementLog()
	//{
	//	T c = createLike();
	//	ops.elementLog(mat, c.mat);
	//	return c;
	//}

	///// <summary>
	///// <para>
	///// Returns a new matrix whose elements are the negative of 'this' matrix's elements.<br>
	///// <br>
	///// b<sub>ij</sub> = -a<sub>ij</sub>
	///// </para>
	///// </summary>
	///// <returns> A matrix that is the negative of the original. </returns>
	//public T negative()
	//{
	//	T A = copy();
	//	ops.changeSign(A.mat);
	//	return A;
	//}

	///// <summary>
	///// <para>Allows you to perform an equation in-place on this matrix by specifying the right hand side.  For information on how to define an equation
	///// see <seealso cref="org.ejml.equation.Equation"/>.  The variable sequence alternates between variable and it's label String.
	///// This matrix is by default labeled as 'A', but is a string is the first object in 'variables' then it will take
	///// on that value.  The variable passed in can be any data type supported by Equation can be passed in.
	///// This includes matrices and scalars.</para>
	///// 
	///// Examples:<br>
	///// <pre>
	///// perform("A = A + B",matrix,"B");     // Matrix addition
	///// perform("A + B",matrix,"B");         // Matrix addition with implicit 'A = '
	///// perform("A(5,:) = B",matrix,"B");    // Insert a row defined by B into A
	///// perform("[A;A]");                    // stack A twice with implicit 'A = '
	///// perform("Q = B + 2","Q",matrix,"B"); // Specify the name of 'this' as Q
	///// 
	///// </pre>
	///// </summary>
	///// <param name="equation"> String representing the symbol equation </param>
	///// <param name="variables"> List of variable names and variables </param>
	//public void equation(string equation, object... variables)
	//{
	//	if (variables.length >= 25)
	//	{
	//		throw new System.ArgumentException("Too many variables!  At most 25");
	//	}

	//	if (!(mat is DMatrixRMaj))
	//	{
	//		return;
	//	}

	//	Equation eq = new Equation();

	//	string nameThis = "A";
	//	int offset = 0;
	//	if (variables.length > 0 && variables[0] is string)
	//	{
	//		nameThis = (string)variables[0];
	//		offset = 1;

	//		if (variables.length % 2 != 1)
	//		{
	//			throw new System.ArgumentException("Expected and odd length for variables");
	//		}
	//	}
	//	else
	//	{
	//		if (variables.length % 2 != 0)
	//		{
	//			throw new System.ArgumentException("Expected and even length for variables");
	//		}
	//	}
	//	eq.alias((Matrix)mat, nameThis);

	//	for (int i = offset; i < variables.length; i += 2)
	//	{
	//		if (!(variables[i + 1] is string))
	//		{
	//			throw new System.ArgumentException("String expected at variables index " + i);
	//		}
	//		object o = variables[i];
	//		string name = (string)variables[i + 1];

	//		if (o.Type.IsAssignableFrom(typeof(SimpleBase)))
	//		{
	//			eq.alias(((SimpleBase)o).DDRM, name);
	//		}
	//		else if (o is DMatrixRMaj)
	//		{
	//			eq.alias((Matrix)o, name);

	//		}
	//		else if (o is double?)
	//		{
	//			eq.alias((double?)o, name);
	//		}
	//		else if (o is int?)
	//		{
	//			eq.alias((int?)o, name);
	//		}
	//		else
	//		{
	//			string type = o == null ? "null" : o.Type.Name;
	//			throw new System.ArgumentException("Variable type not supported by Equation! " + type);
	//		}
	//	}

	//	// see if the assignment is implicit
	//	if (!equation.contains("="))
	//	{
	//		equation = nameThis + " = " + equation;
	//	}

	//	eq.process(equation);
	//}

	///// <summary>
	///// <para>
	///// Saves this matrix to a file as a serialized binary object.
	///// </para>
	///// </summary>
	///// <seealso cref= MatrixIO#saveBin(DMatrix, String) </seealso>
	//public void saveToFileBinary(string fileName) throws IOException
	//{
	//	MatrixIO.saveBin((Matrix)mat, fileName);
	//}

	///// <summary>
	///// <para>
	///// Loads a new matrix from a serialized binary file.
	///// </para>
	///// </summary>
	///// <param name="fileName"> File which is to be loaded. </param>
	///// <returns> The matrix. </returns>
	///// <seealso cref= MatrixIO#loadBin(String) </seealso>
	//public static SimpleMatrix loadBinary(string fileName) throws IOException
	//{
	//	DMatrix mat = MatrixIO.loadBin(fileName);

	//		// see if its a DMatrixRMaj
	//		if (mat is DMatrixRMaj)
	//		{
	//		return SimpleMatrix.wrap((Matrix)mat);
	//	}
	//		else
	//		{
	//		// if not convert it into one and wrap it
	//		return SimpleMatrix.wrap(new DMatrixRMaj(mat));
	//	}
	//}

	///// <summary>
	///// <para>
	///// Saves this matrix to a file in a CSV format.  For the file format see <seealso cref="MatrixIO"/>.
	///// </para>
	///// </summary>
	///// <seealso cref= MatrixIO#saveBin(DMatrix, String) </seealso>
	//public void saveToFileCSV(string fileName) throws IOException
	//{
	//	MatrixIO.saveDenseCSV((Matrix)mat, fileName);
	//}

	///// <summary>
	///// <para>
	///// Loads a new matrix from a CSV file.  For the file format see <seealso cref="MatrixIO"/>.
	///// </para>
	///// </summary>
	///// <param name="fileName"> File which is to be loaded. </param>
	///// <returns> The matrix. </returns>
	///// <seealso cref= MatrixIO#loadCSV(String, boolean) </seealso>
	//public T loadCSV(string fileName) throws IOException
	//{
	//	DMatrix mat = MatrixIO.loadCSV(fileName, true);

	//	T ret = createMatrix(1, 1, mat.Type);

	//	ret.Matrix = mat;

	//		return ret;
	//}

	///// <summary>
	///// Returns true of the specified matrix element is valid element inside this matrix.
	///// </summary>
	///// <param name="row"> Row index. </param>
	///// <param name="col"> Column index. </param>
	///// <returns> true if it is a valid element in the matrix. </returns>
	//public bool isInBounds(int row, int col)
	//{
	//	return row >= 0 && col >= 0 && row < mat.NumRows && col < mat.NumCols;
	//}

	///// <summary>
	///// Prints the number of rows and column in this matrix.
	///// </summary>
	//public void printDimensions()
	//{
	//	Console.WriteLine("[rows = " + numRows() + " , cols = " + numCols() + " ]");


	//}

	///// <summary>
	///// Size of internal array elements.  32 or 64 bits
	///// </summary>
	public virtual int bits()
	{
		return mat.Type.getBits();
	}

	///// <summary>
	///// <para>Concatinates all the matrices together along their columns.  If the rows do not match the upper elements
	///// are set to zero.</para>
	///// 
	///// A = [ this, m[0] , ... , m[n-1] ]
	///// </summary>
	///// <param name="matrices"> Set of matrices </param>
	///// <returns> Resulting matrix </returns>
	//public virtual T concatColumns(params SimpleBase[] matrices)
	//{
	//	convertType.specify0(this, matrices);
	//	T A = convertType.convert(this);

	//	int numCols = A.numCols();
	//	int numRows = A.numRows();
	//	for (int i = 0; i < matrices.Length; i++)
	//	{
	//		numRows = Math.Max(numRows, matrices[i].numRows());
	//		numCols += matrices[i].numCols();
	//	}

	//	SimpleMatrix combined = SimpleMatrix.wrap(convertType.commonType.create(numRows, numCols));

	//	A.ops.extract(A.mat, 0, A.numRows(), 0, A.numCols(), combined.mat, 0, 0);
	//	int col = A.numCols();
	//	for (int i = 0; i < matrices.Length; i++)
	//	{
	//		Matrix m = convertType.convert(matrices[i]).mat;
	//		int cols = m.NumCols;
	//		int rows = m.NumRows;
	//		A.ops.extract(m, 0, rows, 0, cols, combined.mat, 0, col);
	//		col += cols;
	//	}

	//	return (T)combined;
	//}

	///// <summary>
	///// <para>Concatinates all the matrices together along their columns.  If the rows do not match the upper elements
	///// are set to zero.</para>
	///// 
	///// A = [ this; m[0] ; ... ; m[n-1] ]
	///// </summary>
	///// <param name="matrices"> Set of matrices </param>
	///// <returns> Resulting matrix </returns>
	//public virtual T concatRows(params SimpleBase[] matrices)
	//{
	//	convertType.specify0(this, matrices);
	//	T A = convertType.convert(this);

	//	int numCols = A.numCols();
	//	int numRows = A.numRows();
	//	for (int i = 0; i < matrices.Length; i++)
	//	{
	//		numRows += matrices[i].numRows();
	//		numCols = Math.Max(numCols, matrices[i].numCols());
	//	}

	//	SimpleMatrix combined = SimpleMatrix.wrap(convertType.commonType.create(numRows, numCols));

	//	A.ops.extract(A.mat, 0, A.numRows(), 0, A.numCols(), combined.mat, 0, 0);
	//	int row = A.numRows();
	//	for (int i = 0; i < matrices.Length; i++)
	//	{
	//		Matrix m = convertType.convert(matrices[i]).mat;
	//		int cols = m.NumCols;
	//		int rows = m.NumRows;
	//		A.ops.extract(m, 0, rows, 0, cols, combined.mat, row, 0);
	//		row += rows;
	//	}

	//	return (T)combined;
	//}

	///// <summary>
	///// Extracts the specified rows from the matrix.
	///// </summary>
	///// <param name="begin"> First row.  Inclusive. </param>
	///// <param name="end"> Last row + 1. </param>
	///// <returns> Submatrix that contains the specified rows. </returns>
	//public virtual T rows(int begin, int end)
	//{
	//	return extractMatrix(begin, end, 0, SimpleMatrix.END);
	//}

	///// <summary>
	///// Extracts the specified rows from the matrix.
	///// </summary>
	///// <param name="begin"> First row.  Inclusive. </param>
	///// <param name="end"> Last row + 1. </param>
	///// <returns> Submatrix that contains the specified rows. </returns>
	//public virtual T cols(int begin, int end)
	//{
	//	return extractMatrix(0, SimpleMatrix.END, begin, end);
	//}


	///// <summary>
	///// Returns the type of matrix is is wrapping.
	///// </summary>
	public virtual MatrixType getType
	{
		get
		{
			return mat.Type;
		}
	}

		///// <summary>
		///// Creates a matrix that is the same type and shape
		///// </summary>
		///// <returns> New matrix </returns>
		public virtual T createLike()
		{
			return createMatrix(numRows(), numCols(), getType);
		}

		virtual protected void setMatrix(Matrix mat)
		{
			this.mat = mat;
			var tt = mat.Type;
			//this.ops = lookupOps(tt.tipo);

			switch (tt.tipo)
			{
				case Types.DDRM:
					this.ops =new SimpleOperations_DDRM();
					break;
				//case FDRM: return new SimpleOperations_FDRM();
				//case Types.ZDRM:
					//return (new SimpleOperations_ZDRM<ZMatrixRMaj>()).toGeneryc();
					//case CDRM: return new SimpleOperations_CDRM();
					//case DSCC: return new SimpleOperations_SPARSE();
			}
		}
		//protected SimpleOperations<W> lookupOps(Types type)
		//{
		//	switch (type)
		//	{
		//		case Types.DDRM: 
		//			return (new SimpleOperations_DDRM()).toGeneryc<W>();
		//		//case FDRM: return new SimpleOperations_FDRM();
		//		case Types.ZDRM:
		//			return (new SimpleOperations_ZDRM<ZMatrixRMaj>()).toGeneryc();
		//		//case CDRM: return new SimpleOperations_CDRM();
		//		//case DSCC: return new SimpleOperations_SPARSE();
		//	}
		//	throw new SystemException("Unknown Matrix Type");
		//}

		//protected internal virtual Matrix Matrix
		//{
		//	set
		//	{
		//		this.mat = value;
		//		this.ops = lookupOps(value.Type);
		//	}
		//}

		////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		////ORIGINAL LINE: @Nullable Method findAlternative(String method, Object... arguments)
		//internal virtual System.Reflection.MethodInfo findAlternative(string method, params object[] arguments)
		//{
		//	System.Reflection.MethodInfo[] methods = ops.Type.GetMethods();
		//	for (int methodIdx = 0; methodIdx < methods.Length; methodIdx++)
		//	{
		//		if (!methods[methodIdx].Name.Equals(method))
		//		{
		//			continue;
		//		}

		//		Type[] paramTypes = methods[methodIdx].ParameterTypes;
		//		if (paramTypes.Length != arguments.Length)
		//		{
		//			continue;
		//		}

		//		// look for an exact match only
		//		bool match = true;
		//		for (int j = 0; j < paramTypes.Length; j++)
		//		{
		//			if (arguments[j] is Type)
		//			{
		//				if (paramTypes[j] != arguments[j])
		//				{
		//					match = false;
		//					break;
		//				}
		//			}
		//			else if (paramTypes[j] != arguments[j].Type)
		//			{
		//				match = false;
		//				break;
		//			}
		//		}
		//		if (match)
		//		{
		//			return methods[methodIdx];
		//		}
		//	}
		//	return null;
		//}

		//public virtual void invoke(System.Reflection.MethodInfo m, params object[] inputs)
		//{
		//	try
		//	{
		//		m.invoke(ops, inputs);
		//	}
		//	catch (Exception e) when (e is IllegalAccessException || e is InvocationTargetException)
		//	{
		//		throw new Exception(e);
		//	}
		//}

		///// <summary>
		///// Switches from a dense to sparse matrix
		///// </summary>
		//public virtual void convertToSparse()
		//{
		//	switch (mat.Type)
		//	{
		//		case MatrixType.DDRM:
		//			{
		//				DMatrixSparseCSC m = new DMatrixSparseCSC(mat.NumRows, mat.NumCols);
		//				DConvertMatrixStruct.convert((Matrix)mat, m, 0);
		//				Matrix = m;
		//			}
		//			break;
		//		case MatrixType.FDRM:
		//			{
		//				FMatrixSparseCSC m = new FMatrixSparseCSC(mat.NumRows, mat.NumCols);
		//				FConvertMatrixStruct.convert((FMatrixRMaj)mat, m, 0);
		//				Matrix = m;
		//			}
		//			break;

		//		case MatrixType.DSCC:
		//		case MatrixType.FSCC:
		//			break;
		//		default:
		//			throw new Exception("Conversion not supported!");
		//	}
		//}

		//	/// <summary>
		//	/// Switches from a sparse to dense matrix
		//	/// </summary>
		public virtual void convertToDense()
		{
			switch (mat.Type.tipo)
			{
				case Types.DSCC:
					{
						DMatrix m = new DMatrixRMaj(mat.NumRows, mat.NumCols);
						DConvertMatrixStruct.convert((DMatrix)mat, m);
						mat = m;
					}
					break;
				case Types.FSCC:
					{
						//FMatrix m = new FMatrixRMaj(mat.NumRows, mat.NumCols);
						//FConvertMatrixStruct.convert((FMatrix)mat, m);


						//mat = m;
					}
					break;
				case Types.DDRM:
				case Types.FDRM:
				case Types.ZDRM:
				case Types.CDRM:
					break;
				default:
					throw new Exception("Not a sparse matrix!");
			}
		}
	}
}