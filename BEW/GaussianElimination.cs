using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    class GaussianElimination
    {
        public static double[] gaussianElimination(ref double[,] A, ref double[] F)
        {
            // n - stopień aproksymacji

            int dim = F.Length;

            double[,] C = new double[dim, dim + 1];

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                { C[i, j] = A[i, j]; }
                C[i, dim] = F[i];
            }

            // Forward elimination
            double qt;
            for (int k = 0; k < dim - 1; k++)
            {
                for (int i = k + 1; i < dim; i++)
                {
                    qt = C[i, k] / C[k, k];
                    for (int j = k + 1; j < dim + 1; j++)
                    { C[i, j] -= qt * C[k, j]; }
                    C[i, k] = 0.0;
                }
            }

            // Back-substitution
            double sum = 0.0;
            double[] Y = new double[dim];
            Y[dim - 1] = C[dim - 1, dim] / C[dim - 1, dim - 1];
            for (int k = dim - 2; k >= 0; k--)
            {
                sum = 0.0;
                for (int j = k + 1; j < dim; j++)
                    sum += C[k, j] * Y[j];

                Y[k] = (C[k, dim] - sum) / C[k, k];
            }

            return Y;
        }
    }
}
