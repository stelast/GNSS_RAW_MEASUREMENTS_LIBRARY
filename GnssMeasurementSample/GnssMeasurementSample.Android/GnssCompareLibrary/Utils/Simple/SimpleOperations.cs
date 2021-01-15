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

namespace GnssMeasurementSample.Droid.GnssCompareLibrary.Utils.Simple
{

	/// <summary>
	/// High level interface for operations inside of SimpleMatrix for one matrix type.
	/// 
	/// @author Peter Abeles
	/// </summary>
	public interface SimpleOperations<T> 
		where T : Matrix
	{

		void set(T A, int row, int column, double value);

		void set(T A, int row, int column, double real, double imaginary);

		double get(T A, int row, int column);

		void get(T A, int row, int column, Complex_F64 value);

		void fill(T A, double value);

		void transpose(T input, T output);

		void mult(T A, T B, T output);

		void multTransA(T A, T B, T output);

		void kron(T A, T B, T output);

		void plus(T A, T B, T output);

		void minus(T A, T B, T output);

		void minus(T A, double b, T output);

		void plus(T A, double b, T output);

		void plus(T A, double beta, T b, T output);

		void plus(double alpha, T A, double beta, T b, T output);

		double dot(T A, T v);

		void scale(T A, double val, T output);

		void divide(T A, double val, T output);

		bool invert(T A, T output);

		void setIdentity(T A);

		void pseudoInverse(T A, T output);

		bool solve(T A, T X, T B);

		void zero(T A);

		double normF(T A);

		double conditionP2(T A);

		double determinant(T A);

		double trace(T A);

		void setRow(T A, int row, int startColumn, params double[] values);

		void setColumn(T A, int column, int startRow, params double[] values);

		void extract(T src, int srcY0, int srcY1, int srcX0, int srcX1, T dst, int dstY0, int dstX0);

		T diag(T A);

		bool hasUncountable(T M);

		void changeSign(T a);

		double elementMaxAbs(T A);

		double elementMinAbs(T A);

		double elementSum(T A);

		void elementMult(T A, T B, T output);

		void elementDiv(T A, T B, T output);

		void elementPower(T A, T B, T output);

		void elementPower(T A, double b, T output);

		void elementExp(T A, T output);

		void elementLog(T A, T output);

		bool isIdentical(T A, T B, double tol);

		//void print(PrintStream @out, Matrix mat, string format);
	}

}