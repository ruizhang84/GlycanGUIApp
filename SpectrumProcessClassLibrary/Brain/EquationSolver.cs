using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;


namespace PrecursorIonClassLibrary.Brain
{
    public class EquationSolver
    {
        private static double CubeRoot(double x)
        {
            if (x < 0)
                return -Math.Pow(-x, 1d / 3d);
            else
                return Math.Pow(x, 1d / 3d);
        }

        //ax + b
        public static List<Complex> Root(double a, double b)
        {
            return new List<Complex>() { new Complex(-a / b, 0.0) };
        }
        //ax^2 + bx +c
        public static List<Complex> Root(double a, double b, double c)
        {
            Complex q = Complex.Sqrt(b * b - 4 * a * c);
            return new List<Complex>() { (-b + q) / (2.0 * a), (-b - q) / (2.0 * a) };
        }
        //ax^3 + bx^2 + cx + d Cardano's method
        public static List<Complex> Root(double a, double b, double c, double d)
        {
            double p = (3.0 * a * c - b * b) / (3.0 * a * a);
            double q = (27 * a * a * d - 9 * a * b * c + 2 * b * b * b) / (27 * a * a * a);
            double middle = Math.Sqrt(Math.Pow((q / 2), 2) + Math.Pow((p / 3), 3));
            Complex w = new Complex(-0.5, Math.Sqrt(3) / 2.0);

            Complex x1 = new Complex(CubeRoot(-q/2 + middle) + CubeRoot(-q/2 - middle), 0);
            Complex x2 = w * CubeRoot(-q / 2 + middle) + w * w * CubeRoot(-q / 2 - middle);
            Complex x3 = w * w * CubeRoot(-q / 2 + middle) + w * CubeRoot(-q / 2 - middle);
            return new List<Complex>() { x1 - b / (3.0 * a), (x2 - b / (3.0 * a)), (x3 - b / (3.0 * a))};
        }

        //ax^4 + bx^3 + cx^2 + dx + e
        public static List<Complex> Root(double a, double b, double c, double d, double e)
        {
            double p = (8 * a * c - 3 * b * b) / (8 * a * a);
            double q = (b * b * b - 4 * a * b * c + 8 * a * a * d) / (8 * a * a * a);

            double delta0 = c * c - 3 * b * d + 12 * a * e;
            double delta1 = 2 * c * c * c - 9 * b * c * d + 27 * b * b * e + 27 * a * d * d - 72 * a * c * e;

            Complex Q = Complex.Pow((delta1 + Complex.Sqrt(delta1 * delta1 - 4 * Math.Pow(delta0, 3)))/2.0, 1.0/3);
            Complex S = 0.5 * Complex.Sqrt(-2.0 * p / 3 + (Q + delta0 / Q) / (3 * a));
            return  new List<Complex>()
            {
                -0.25 * b / a - S + 0.5 * Complex.Sqrt(-4 * S * S - 2 * p + q / S),
                -0.25 * b / a - S - 0.5 * Complex.Sqrt(-4 * S * S - 2 * p + q / S),
                -0.25 * b / a + S + 0.5 * Complex.Sqrt(-4 * S * S - 2 * p - q / S),
                -0.25 * b / a + S - 0.5 * Complex.Sqrt(-4 * S * S - 2 * p - q / S)
            };

        }


    }
}
