using System;
using System.Collections.Generic;
using System.Text;

namespace PrecursorIonClassLibrary.Charges
{
    public class Matrix
    {
        public static double [,] Transpose (double[,] x)
        {
            int m = x.GetLength(0);
            int n = x.GetLength(1);
            double[,] y = new double[n, m];
            for(int i=0; i<n; i++)
            {
                for(int j=0; j<m; j++)
                {
                    y[i, j] = x[j, i];
                }
            }
            return y;
        }

        public static double[,] Add(double[,] x, double[,]y)
        {
            int m = x.GetLength(0);
            int n = x.GetLength(1);
            int k = y.GetLength(0);
            int l = y.GetLength(1);

            if (m != k || n != l)
            {
                throw new Exception("dimention is not correct!");
            }

            double[,] z = new double[m, n];
            for (int i=0; i < m; i++)
            {
                for(int j=0; j < n; j++)
                {
                    z[i, j] = x[i, j] + y[i, j];
                }
            }
            return z;
        }

        public static double[,] Multiply(double[,] x, double[,] y)
        {
            int m = x.GetLength(0);
            int n = x.GetLength(1);
            int k = y.GetLength(0);
            int l = y.GetLength(1);

            if (n != k)
            {
                throw new Exception("dimention is not correct!");
            }

            double[,] z = new double[m, l];
            for(int i=0; i<m; i++)
            {
                for(int j=0; j<l; j++)
                {
                    for(int p=0; p<n; p++)
                    {
                        z[i, j] += x[i, p] * y[p, j];
                    }
                }
            }

            return z;
        }

        public static double[,] Multiply(double[,] x, double a)
        {
            int m = x.GetLength(0);
            int n = x.GetLength(1);
            double[,] y = new double[m, n];
            for(int i = 0; i < m; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    y[i, j] = x[i, j] * a;
                }
            }
            return y;
        }

        public static double[,] Inverse(double[,] x)
        {
            int m = x.GetLength(0);
            int n = x.GetLength(1);

            if (m != n)
            {
                throw new Exception("dimention is not correct!");
            }

            // init augumented matrix
            double[,] y = new double[m, 2 * m];
            for(int i = 0; i < m; i++)
            {
                for(int j = 0; j < m; j++)
                {
                    y[i, j] = x[i, j];
                    if (i == j)
                    {
                        y[i, j + m] = 1;
                    }
                    else
                    {
                        y[i, j + m] = 0;
                    }
                }
            }

            // elimination
            for (int k = 0; k < m-1; k++)
            {
                for(int i = k; i < m-1; i++)
                {
                    // not zero, so to elminate
                    if (Math.Abs(y[i+1, k]) > 1e-8 )
                    {
                        double temp = y[i + 1, k];
                        //multiple to substract
                        for (int l=0; l < m; l++)
                        {
                            if (l >= k)
                                y[i + 1, l] = y[i + 1, l] * y[k, k] - y[k, l] * temp;
                            y[i + 1, l+m] = y[i + 1, l+m] * y[k, k] - y[k, l+m] * temp;
                        }
                    }
                }

                if (Math.Abs(y[m-1-k, m-1-k]) > 1e-8)
                {
                    for(int i = k; i < m-1; i++)
                    {
                        if(Math.Abs(y[m - i - 2, m - 1 - k]) > 1e-8)
                        {
                            double temp = y[m - i - 2, m - k - 1];
                            for (int l = m - 1; l >= 0; l--)
                            {
                                if (l <= m - k - 1)
                                    y[m - i - 2, l] = y[m - i - 2, l] * y[m - 1 - k, m - 1 - k]
                                        - y[m - k - 1, l] * temp;
                                y[m - i - 2, l+m] = y[m - i - 2, l+m] * y[m - 1 - k, m - 1 - k] 
                                    - y[m - k - 1, l+m] * temp;      
                            }
                        }
                    }
                }
            } 

            // assign
            double[,] z = new double[m, m];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {

                    z[i, j] = y[i, j + m] * 1.0/ y[i, i];
                }
            }

            return z;
        }

    }
}
