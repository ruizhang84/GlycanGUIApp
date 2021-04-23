using GlycanGUI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycanGUI.Algorithm.CurveFitting
{
    public class PolynomialFitting : ICurveFitting
    {
        public double[] parameter;
        public int Order {get; set;}
        public PolynomialFitting(int order=3)
        {
            Order = order;
        }

        public void Fit(List<double> x, List<double> y)
        {
            parameter = new double[Order + 1];
            double[,] X = new double[x.Count,Order+1];
            for(int i=0; i < x.Count; i++)
            {
                for(int j=0; j<=Order; j++)
                {
                    X[i, j] = Math.Pow(x[i], j);
                }
            }

            double[,] Y = new double[y.Count, 1];
            for(int i = 0; i < y.Count; i++)
            {
                Y[i, 0] = y[i];
            }

            double[,] z = 
                Matrix.Multiply(
                    Matrix.Multiply(
                        Matrix.Inverse(Matrix.Multiply(Matrix.Transpose(X), X)),
                        Matrix.Transpose(X) 
                    ), 
                    Y);
            for(int i = 0; i < z.GetLength(0); i++)
            {
                parameter[i] = z[i, 0];
            }
        }

        public double GlucoseUnit(double rentention)
        {
            double unit = 0.0;
            for(int i=0; i <= Order; i++)
            {
                unit += parameter[i] * Math.Pow(rentention, i);
            }
            return unit;
        }

        public double[] Parameter()
        { return parameter;  }
    } 
}
