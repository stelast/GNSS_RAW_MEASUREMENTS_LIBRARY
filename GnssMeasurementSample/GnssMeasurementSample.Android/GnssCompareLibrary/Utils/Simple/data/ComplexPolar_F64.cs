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
	/// <seealso cref="Complex_F64"/> number in polar notation.<br>
	/// z = r*(cos(&theta;) + i*sin(&theta;))<br>
	/// where r and &theta; are polar coordinate parameters
	/// </para>
	/// 
	/// @author Peter Abeles
	/// </summary>
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @Data public class ComplexPolar_F64
	public class ComplexPolar_F64
	{
		public double r;
		public double theta;

		public ComplexPolar_F64(double r, double theta)
		{
			this.r = r;
			this.theta = theta;
		}

		public ComplexPolar_F64(Complex_F64 n)
		{
			ComplexMath_F64.convert(n, this);
		}

		public ComplexPolar_F64()
		{
		}

		public virtual Complex_F64 toStandard()
		{
			Complex_F64 ret = new Complex_F64();
			ComplexMath_F64.convert(this, ret);
			return ret;
		}

		public virtual void setTo(double r, double theta)
		{
			this.r = r;
			this.theta = theta;
		}

		public virtual ComplexPolar_F64 To
		{
			set
			{
				this.r = value.r;
				this.theta = value.theta;
			}
		}

		public override string ToString()
		{
			return "( r = " + r + " theta = " + theta + " )";
		}
	}
}