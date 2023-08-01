using System;
using System.Diagnostics;


namespace uk.andyjohnson0.Netduino.Utils
{
    /// <summary>
    /// Netduino implementation of some of the standard System.Math interface.
    /// Trancendental trigonometric functions are calculated using Taylor series (ee http://mathonweb.com/help_ebook/calc_funcs.htm).
    /// Square root is calculated using Newton's method.
    /// for further information.
    /// </summary>
    public static class Math
    {
        /// <summary>
        /// Base of the natural logarithm,
        /// </summary>
        public const double E = 2.71828;

        /// <summary>
        /// Ratio of a circle's circumference to its diameter.
        /// </summary>
        public const double PI = 3.14159265359;

        /// <summary>
        /// PI * 2.
        /// Usefu for trigonometric calculations involving radians. If you don't believe me see http://tauday.com/.
        /// </summary>
        public const double Tau = 2.0D * PI;


        private const double piOver2 = PI / 2.0D;
        private const double piOver4 = PI / 4.0D;
        private const double piOver8 = PI / 8.0D;


        /// <summary>
        /// Returns the Sine of the specified angle.
        /// </summary>
        /// <param name="d">Angle in radians.</param>
        /// <returns>Sine of the specified angle.</returns>
        public static double Sin(double d)
        {
            // Taylor series:
            // sin(x) = x - x^3/3! + x^5/5! - ......

            // Normalise input to range 0<=x<Tau
            while (d >= Tau)
            {
                d -= Tau;
            }
            while (d < 0)
            {
                d += Tau;
            }
            // Debug.Assert(d >= 0 && d < Tau);

            // Mirror about pi and remember to negate result if necessary.
            double p = +1.0D;
            if (d >= PI)
            {
                d -= PI;
                p = -1.0D;
            }
            // Debug.Assert(d >= 0 && d < PI);

            // Mirror about pi/2.
            if (d > piOver2)
            {
                d = piOver2 - (d - piOver2);
            }
            // Debug.Assert(d >= 0 && d < piOver2);

            // Use cofunction
            //if (d > piOver4)
            //{
            //    return Cos(piOver2 - d);
            //}

            //
            double sgn = +1.0D;
            double fac = 1.0D;
            double s = 0;
            for (int i = 1; i <= 10; i = i + 2)
            {
                double t = sgn * (System.Math.Pow(d, i) / fac);
                s = s + t;

                fac = fac * (i + 1) * (i + 2);
                sgn = -sgn;
            }

            return s * p;
        }


        /// <summary>
        /// Returns the Cosine of the specified angle.
        /// </summary>
        /// <param name="d">Angle in radians.</param>
        /// <returns>Cosine of the specified angle.</returns>
        public static double Cos(double d)
        {
            // Taylor series:
            // cos(x) = 1 + (x^2)/2! - (x^4)/4! + ... + 

            // Normalise input to range 0<=x<Tau
            while (d >= Tau)
            {
                d -= Tau;
            }
            while (d < 0)
            {
                d += Tau;
            }
            // Debug.Assert(d >= 0 && d < Tau);

            // Mirror about Pi.
            if (d > PI)
            {
                d = PI - (d - PI);
            }
            // Debug.Assert(d >= 0 && d < PI);

            // Mirror about PI/2 and remember to negate result if necessary
            double p = +1.0D;
            if (d > piOver2)
            {
                d = piOver2 - (d - piOver2);
                p = -1.0D;
            }
            // Debug.Assert(d >= 0 && d < piOver2);

            // Use cofunction
            //if (d > piOver4)
            //{
            //    return Sin(piOver2 - d);
            //}

            //
            double sgn = -1.0D;
            double fac = 2.0D;  // TODO
            double s = 1;
            for (int i = 2; i <= 12; i = i + 2)
            {
                double t = sgn * (System.Math.Pow(d, i) / fac);
                s = s + t;

                fac = fac * (i + 1) * (i + 2);
                sgn = -sgn;
            }

            return s * p;
        }


        /// <summary>
        /// Returns the Tangent of the specified angle.
        /// </summary>
        /// <param name="d">Angle in radians.</param>
        /// <returns>Tangent of the specified angle.</returns>
        public static double Tan(double d)
        {
            if (d == piOver2)
            {
                return double.PositiveInfinity;
            }

            return Sin(2.0D * d) / (Cos(2.0D * d) + 1);  

            // Normalise input to range 0<=x<PI
            //while (d >= PI)
            //{
            //    d -= PI;
            //}
            //while (d < 0)
            //{
            //    d += PI;
            //}
            //// Debug.Assert(d >= 0 && d < PI);

            // Mirror around PI/2 and remember to negate result if necessary
            //double p = +1.0D;
            //if (d > piOver2)
            //{
            //    d = piOver2 - (d - piOver2);
            //    p = -1.0D;
            //}
            //// Debug.Assert(d >= 0 && d < piOver2);

            //if (d >= piOver4)
            //{
            //    double t = 1.0D / Tan(piOver2 - d);
            //    return p * t;
            //}
            //// Debug.Assert(d >= 0 && d < piOver4);

            //if (d >= piOver8)
            //{
            //    double t = (2 * Tan(d / 2.0D)) / (1 - Math.Pow(Tan(d / 2.0D), 2));
            //    return p * t;
            //}
            //// Debug.Assert(d >= 0 && d < piOver8);

            // See http://scipp.ucsc.edu/~haber/ph116A/taylor11.pdf
            //double s = d;
            //for (int i = 2; i <= 10; i = i + 2)
            //{
            //    double t = Math.Pow(d, (2 * i) + 1) / Factorial((2 * i) + 1);
            //    s = s + t;
            //}

            //return s * p;

            //double s = d + (Pow(d, 3.0D) / 3.0D) + 
            //               (2.0D * (Pow(d, 5.0D)) / 15.0D) + 
            //               (17.0D * (Pow(d, 7.0D)) / 315.0D);
            //return s * p;
        }


        /// <summary>
        /// Returns the Arctangent (inverse tangent) of the specified angle.
        /// </summary>
        /// <param name="d">Angle in radians.</param>
        /// <returns>Arctangent of the specified angle.</returns>
        public static double Atan(double d)
        {
            if (d < 0)
            {
                return -Atan(-d);
            }
            // Debug.Assert(d >= 0.0D);
            if (d > 1.0D)
            {
                return piOver2 - Atan(1.0D / d);
            }
            // Debug.Assert(d <= 1.0D);
            if (d > 0.268D) // 2-sqrt(3)
            {
                const double sqrt3 = 1.73205080757D;
                return (PI / 6.0D) + Atan(((sqrt3 * d) - 1) / (sqrt3 + d));
            }
            // Debug.Assert(d <= 0.268D);

            //
            double sgn = -1.0D;
            double s = d;
            for (int i = 3; i <= 12; i = i + 2)
            {
                double t = sgn * (System.Math.Pow(d, i) / (double)i);
                s = s + t;

                sgn = -sgn;
            }

            return s;
        }


        /// <summary>
        /// Returns the square root of the specified number.
        /// </summary>
        /// <param name="value">A number</param>
        /// <returns>The square root of the spcified number.</returns>
        public static double Sqrt(double value)
        {
            double s = value / 2.0D;
            while (true)
            {
                if (s == 0.0D)
                    return 0.0D;
                double s2 = s - (Pow(s, 2) - value) / (2.0D * s);
                if (Abs(s2 - s) < 0.0000001)
                    return s2;
                else
                    s = s2;
            }

        }


        /// <summary>
        /// Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="d">A number.</param>
        /// <returns>The absolute value of the specified number.</returns>
        public static double Abs(double d)
        {
            return (d >= 0) ? d : -d;
        }


        /// <summary>
        /// Returns the factorial of the specified number.
        /// </summary>
        /// <param name="n">A number. Must be grater than or equal to zero.</param>
        /// <returns>The factorial of the specified number.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The argument must be greater than or equal to zero.</exception>
        public static double Factorial(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("n", "n must be >= 0");
            double fact = 1.0D;
            for (int i = 2; i <= n; i++)
            {
                fact *= i;
            }
            return fact;
        }


        /// <summary>
        /// Rounds a decimal value to the nearest integral value.
        /// </summary>
        /// <param name="d">A number</param>
        /// <returns>The specified number rounded to the nearest integral place.</returns>
        public static double Round(double d)
        {
            return System.Math.Round(d);
        }


        /// <summary>
        /// Rounds a number to a specified number of fractional digits.
        /// </summary>
        /// <param name="value">A number</param>
        /// <param name="digits">number of digits to round to.</param>
        /// <returns>The specified number rounded to a specified number of fractional digits.</returns>
        public static double Round(double value, int digits)
        {
            double m = Math.Pow(10, digits);
            return ((double)(int)(value * m)) / m;
        }


        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        /// <param name="d">A number.</param>
        /// <returns>the largest integer less than or equal to the specified number.</returns>
        public static double Floor(double d)
        {
            return System.Math.Floor(d);
        }


        /// <summary>
        /// Returns a number raised to the power of another number.
        /// </summary>
        /// <param name="x">A number to be raised to a power (mantissa).</param>
        /// <param name="y">A number that specifies a power (exponent).</param>
        /// <returns></returns>
        public static double Pow(double x, double y)
        {
            return System.Math.Pow(x, y);
        }


        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="d">An angle in degrees.</param>
        /// <returns>The equivalent angle in radians.</returns>
        public static double DegToRad(double d)
        {
            return (d * PI) / 180.0D;
        }


        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="d">An angle in radians.</param>
        /// <returns>The equivalent angle in degrees.</returns>
        public static double RadToDeg(double d)
        {
            return (d * 180.0D) / PI;
        }
    }
}
