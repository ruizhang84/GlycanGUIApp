using System;
using System.Collections.Generic;
using System.Text;

namespace PrecursorIonClassLibrary.Charges
{
    public class MatrixSolver
    {
        public static double Normalize(double[] vector)
        {
            double norm = 0;
            foreach(double x in vector)
            {
                norm += x * x;
            }
            return Math.Sqrt(norm);
        }

        public static double[] Jacobi(double[,]a, double[]b, double maxSteps = 500, double epsilon = 1e-3)
        {
            double[] X = new double[b.Length];
            double[] converge = new double[b.Length];
            for (int step = 0; step < maxSteps; step++)
            {
                
                for (int i = 0; i < b.Length; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        if (i != j)
                            sum += a[i, j] * X[j];
                    }
                    double x_new = (b[i] - sum) / a[i, i];
                    converge[i] = Math.Abs(x_new - X[i]);
                    X[i] = x_new;
                }
                if (Normalize(converge) < epsilon)
                    break;
            }
            return X;
        }

        //https://en.wikipedia.org/wiki/Tridiagonal_matrix_algorithm
        public static double[] Thomas(double[] a, double[] b, double[] c, double[] d)
        {
            int n = b.Length;
            double[] x = new double[n];

            double[] C = new double[c.Length];
            C[0] = c[0] / b[0];
            for(int i = 1; i < n-1; i++)
            {
                C[i] = c[i] / (b[i] - a[i-1] * C[i - 1]);
            }

            double[] D = new double[d.Length];
            D[0] = d[0] / b[0];
            for(int i = 1; i < n; i++)
            {
                D[i] = (d[i] - a[i-1] * D[i - 1]) / (b[i] - a[i-1] * C[i - 1]);
            }

            x[n - 1] = D[n - 1];
            for(int i = n-2; i >= 0; i--)
            {
                x[i] = D[i] - C[i] * x[i + 1];
            }
            return x;
        }

    }
}
