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
	public class AutomaticSimpleMatrixConvert 
	{
		internal MatrixType commonType;

		public virtual void specify0<T>(SimpleBase<T, DMatrixRMaj> a, params SimpleBase<T, DMatrixRMaj>[] inputs) 
			where T:SimpleBase<T, DMatrixRMaj>
		{
			SimpleBase<T, DMatrixRMaj>[] array = new SimpleBase<T, DMatrixRMaj>[inputs.Length + 1];
			Array.Copy(inputs, 0, array, 0, inputs.Length);
			array[inputs.Length] = a;
			specify(inputs);
		}

		public virtual void specify<T,W>(params SimpleBase<T,W>[] inputs) where T : SimpleBase<T,W>
			where W : Matrix
		{
			bool dense = false;
			bool real = true;
			int bits = 32;

			foreach (SimpleBase<T,W> s in inputs)
			{
				MatrixType t = s.mat.Type;
				if (t.isDense())
				{
					dense = true;
				}
				if (!t.isReal())
				{
					real = false;
				}
				if (t.getBits() == 64)
				{
					bits = 64;
				}
			}

		//	commonType = MatrixType.lookup(dense, real, bits);
		}

		public virtual T convert<T,W>(SimpleBase<T,W> matrix) where T : SimpleBase<T,W>
			where W : Matrix
		{
			if (matrix.getType == commonType)
			{
				return (T)matrix;
			}

			if (!matrix.getType.isDense() && commonType.isDense())
			{
				Console.Error.WriteLine("\n***** WARNING *****\n");
				Console.Error.WriteLine("Converting a sparse to dense matrix automatically.");
				Console.Error.WriteLine("Current auto convert code isn't that smart and this might have been available");
			}

			Matrix m = ConvertMatrixType.convert(matrix.mat, commonType);
			if (m == null)
			{
				throw new System.ArgumentException("Conversion from " + matrix.getType + " to " + commonType + " not possible");
			}

			return (T)matrix.wrapMatrix(m);
		}
	}
}