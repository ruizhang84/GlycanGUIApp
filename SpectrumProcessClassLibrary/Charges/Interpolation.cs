using System;
using System.Collections.Generic;
using System.Text;

namespace PrecursorIonClassLibrary.Charges
{
    public class Interpolation
    {

        // Linear interpolation
        static public double Linear(double x, double x0, double y0, double x1, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        //Lagrange polynomials
        static public double Polynomial(double x, List<double> X, List<double> Y)
        {
            double sum = 0;
            for (int i = 0, n = X.Count; i < n; i++)
            {
                if (x - X[i] == 0)
                {
                    return Y[i];
                }
                double product = Y[i];
                for (int j = 0; j < n; j++)
                {
                    if (X[i] != X[j])
                    {
                        product *= (x - X[i]) / (X[i] - X[j]);
                    }
                }
                sum += product;
            }
            return sum;
        }


        private static double[] SplineParameter(List<double> X, List<double> Y)
        {
            int n = X.Count;
            double[] h = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                h[i] = X[i + 1] - X[i];
            }
            double[] a = new double[n - 1];
            double[] b = new double[n];
            double[] c = new double[n - 1];
            double[] d = new double[n];

            for (int i = 0; i < n - 2; i++)
            {
                a[i] = h[i];
            }

            b[0] = 1;
            for (int i = 1; i < n-1; i++)
            {
                b[i] = 2 * (h[i - 1] + h[i]);
            }
            b[n - 1] = 1;

            for (int i = 1; i < n - 1; i++)
            {
                c[i] = h[i];
            }

            for (int i = 1; i < n - 1; i++)
            {
                d[i] = (Y[i + 1] - Y[i]) / h[i] - (Y[i] - Y[i - 1]) / h[i - 1];
            }

            return  MatrixSolver.Thomas(a, b, c, d);
        }

        private static int SearchIndex(double v, List<double> X)
        {
            int start = 0, end = X.Count - 1;
            int index = start;
            while (start <= end)
            {
                int mid = (start + end) / 2;
                if (v == X[mid]) return mid;

                if (v < X[mid])
                {
                    end = mid-1;
                }
                else
                {
                    start = mid+1;
                    index = mid;
                }
            }

            return index;
        }

        //Cubic Spline interpolation
        public static List<double> Spline(List<double> x, List<double> X, List<double> Y)
        {
            int n = X.Count;
            double[] h = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                h[i] = X[i + 1] - X[i];
            }
            double[] m = SplineParameter(X, Y);

            List<double> y = new List<double>();

            foreach(double v in x)
            {
                int index = SearchIndex(v, X);
                if (X[index] == v)
                {
                    y.Add(Y[index]);
                    continue;
                }

                double a = Y[index];
                double b = (Y[index + 1] - Y[index]) / h[index] 
                    - h[index] * m[index] / 2 - h[index] * (m[index+1] - m[index]) / 6;
                double c = m[index] / 2;
                double d = (m[index + 1] - m[index]) / (6 * h[index]);
                y.Add(a + b * (v - X[index]) + c * Math.Pow(v - X[index], 2) + d * Math.Pow(v - X[index], 3));

            }



            return y;
        }

    }
}
