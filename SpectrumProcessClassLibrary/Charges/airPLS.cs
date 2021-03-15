using System;
using System.Collections.Generic;
using System.Text;

namespace PrecursorIonClassLibrary.Charges
{
    
    public class AirPLS
    {
        public double Lambda { get; set; } = 100;
        public int MaxIter { get; set; } = 15;
        double[,] D = null;
        double[,] lambda_D_TD = null; // lambda * D.T * D

        public AirPLS(double lambda_ = 100)
        {
            Lambda = lambda_;
        }

        void InitD(double[,] X)
        {
            int length = X.GetLength(1);
            if (D != null && D.GetLength(1) == length-1)
                return;
            D = new double[length-1, length];
            for (int i = 0; i < length - 1; i++)
            {
                D[i, i] = -1;
                D[i, i + 1] = 1;
            }
            lambda_D_TD = Matrix.Multiply(
                Matrix.Multiply(Matrix.Transpose(D), D),
                Lambda);
        }

        public double[,] WhittakerSmooth(double[,] X, double[,] W)
        {
            InitD(X);
            //double[,] A = Matrix.Add(W, lambda_D_TD);
            //double[,] B = Matrix.Multiply(X, W);
            //double[] b = new double[B.GetLength(1)];
            //for(int i = 0; i < b.Length; i++)
            //{
            //    b[i] = B[0, i];
            //}
            //return MatrixSolver.Jacobi(A, b);
            double[,] W_i = Matrix.Inverse(Matrix.Add(W, lambda_D_TD));
            return Matrix.Multiply(X, Matrix.Multiply(W, W_i));

        }

        public double[,] Correction(double[,] X)
        {
            int length = X.GetLength(1);
            double[,] z = new double[1, length];
            double[,] W = new double[length, length];
            for (int i = 0; i < length; i++)
            {
                W[i, i] = 1;
            }

            for (int i = 1; i <= MaxIter; i++)
            {
                double diff = 0;
                double sum = 0;
                z = WhittakerSmooth(X, W);
                // eval
                for (int j = 0; j < z.GetLength(1); j++)
                {
                    if (z[0, j] > X[0, j])
                    {
                        diff += Math.Abs(X[0, j] - z[0, j]);
                    }
                    sum += Math.Abs(X[0, j]);
                }
                if (diff < 0.001 * sum)
                    break;
                // update
                double maxW = 0;
                for (int j = 0; j < length; j++)
                {
                    if (z[0, j] <= X[0, j])
                    {
                        W[j, j] = 0;
                    }
                    else
                    {
                        W[j, j] = Math.Exp(i * (z[0, j] - X[0, j]) / diff);
                        maxW = Math.Max(maxW, W[j, j]);
                    }
                }
                W[0, 0] = maxW;
                W[length - 1, length - 1] = maxW;
            }
            return z;
        }

    }
}
