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
	/// <summary>
	/// <para>
	/// Describes a rectangular submatrix inside of a <seealso cref="DMatrixD1"/>.
	/// </para>
	/// 
	/// <para>
	/// Rows are row0 &le; i &lt; row1 and Columns are col0 &le; j &lt; col1
	/// </para>
	/// 
	/// @author Peter Abeles
	/// </summary>
	public class DSubmatrixD1 : Submatrix<DMatrixD1>
	{
		public DSubmatrixD1()
		{
		}

		public DSubmatrixD1(DMatrixD1 original)
		{
			set(original);
		}

		public DSubmatrixD1(DMatrixD1 original, int row0, int row1, int col0, int col1)
		{
			set(original, row0, row1, col0, col1);
		}

		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @SuppressWarnings("NullAway") public double get(int row, int col)
		public virtual double get(int row, int col)
		{
			return original.get(row + row0, col + col0);
		}

		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @SuppressWarnings("NullAway") public void set(int row, int col, double value)
		public virtual void set(int row, int col, double value)
		{
			original.set(row + row0, col + col0, value);
		}

		public virtual DMatrixRMaj extract()
		{
			DMatrixRMaj ret = new DMatrixRMaj(row1 - row0, col1 - col0);

			for (int i = 0; i < ret.numRows; i++)
			{
				for (int j = 0; j < ret.numCols; j++)
				{
					ret.set(i, j, get(i, j));
				}
			}

			return ret;
		}

		public override void print()
		{
			if (original == null)
			{
				throw new Exception("Uninitialized submatrix");
			}
			//MatrixIO.print(original, "%6.3f", row0, row1, col0, col1);
		}
	}
}