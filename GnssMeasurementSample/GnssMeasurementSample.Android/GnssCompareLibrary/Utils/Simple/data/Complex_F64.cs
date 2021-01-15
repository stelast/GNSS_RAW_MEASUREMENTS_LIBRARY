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
	/// Represents a complex number using 64bit floating point numbers.  A complex number is composed of
	/// a real and imaginary components.
	/// </para>
	/// </summary>
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @Data public class Complex_F64
	public class Complex_F64
	{
		public double real;
		public double imaginary;

		public Complex_F64(double real, double imaginary)
		{
			this.real = real;
			this.imaginary = imaginary;
		}

		public Complex_F64()
		{
		}

		public double getMagnitude()
		{
			return Math.Sqrt(real * real + imaginary * imaginary);
		}

		public double getMagnitude2()
		{
			return real * real + imaginary * imaginary;
		}

		public virtual double Magnitude
		{
			get
			{
				return Math.Sqrt(real * real + imaginary * imaginary);
			}
		}

		public virtual double Magnitude2
		{
			get
			{
				return real * real + imaginary * imaginary;
			}
		}

		public virtual void setTo(double real, double imaginary)
		{
			this.real = real;
			this.imaginary = imaginary;
		}

		public virtual Complex_F64 To
		{
			set
			{
				this.real = value.real;
				this.imaginary = value.imaginary;
			}
		}

		public virtual bool isReal()
		{
			return imaginary == 0.0;
		}

		public virtual Complex_F64 plus(Complex_F64 a)
		{
			Complex_F64 ret = new Complex_F64();
			ComplexMath_F64.plus(this, a, ret);
			return ret;
		}

		public virtual Complex_F64 minus(Complex_F64 a)
		{
			Complex_F64 ret = new Complex_F64();
			ComplexMath_F64.minus(this, a, ret);
			return ret;
		}

		public virtual Complex_F64 times(Complex_F64 a)
		{
			Complex_F64 ret = new Complex_F64();
			ComplexMath_F64.multiply(this, a, ret);
			return ret;
		}

		public virtual Complex_F64 divide(Complex_F64 a)
		{
			Complex_F64 ret = new Complex_F64();
			ComplexMath_F64.divide(this, a, ret);
			return ret;
		}

		public override string ToString()
		{
			if (imaginary == 0)
			{
				return "" + real;
			}
			else
			{
				return real + " " + imaginary + "i";
			}
		}
	}
}