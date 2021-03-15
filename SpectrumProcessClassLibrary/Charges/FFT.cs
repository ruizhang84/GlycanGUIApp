using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PrecursorIonClassLibrary.Charges
{
    public class FFT
    {
        public static Complex[] Transform(Complex[] x)
        {
            int N = x.Length;
            Complex[] X = new Complex[N];
            if (N == 1)
            {
                X[0] = x[0];
                return X;
            }

            Complex[] d, D, e, E;
            e = new Complex[N / 2];
            d = new Complex[N / 2];

            for (int k = 0; k < N / 2; k++)
            {
                e[k] = x[2 * k];
                d[k] = x[2 * k + 1];
            }
            D = Transform(d);
            E = Transform(e);

            for (int k = 0; k < N / 2; k++)
            {
                D[k] *= Complex.FromPolarCoordinates(1, -2 * Math.PI * k / N);
            }

            for (int k = 0; k < N / 2; k++)
            {
                X[k] = E[k] + D[k];
                X[k + N / 2] = E[k] - D[k];
            }
            return X;
        }
    }
}
