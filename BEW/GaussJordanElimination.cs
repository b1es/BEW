using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    class GaussJordanElimination
    {
        public static bool gaussJordanElimination(double[,] A, out double[,] B)
        {
            int rows = (int)Math.Sqrt(A.Length);
            int cols = (int)Math.Sqrt(A.Length);
            bool isInvertible = true;
            B = new double[rows, cols];
            // TWORZENIE MACIERZY C
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if(i == j)
                    { B[i,j] = 1; }
                    else
                    { B[i,j] = 0; }
                }
            }
            double[,] C = new double[rows, 2 * cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    C[i, j] = A[i, j];
                    C[i, j + cols] = B[i, j];
                }
            }
            // tworzenie macierzy C


            for (int j = 0; j < cols; j++)
            {
                if (C[j, j] == 0.0)
                {
                    for (int k = j; k < rows; k++)  // Próba zamiany z jakimś wierszem
                    {
                        if (C[k, j] != 0.0)
                        {
                            double temp = 0.0;
                            for (int l = 0; l < cols; l++)
                            {
                                temp = C[k, l];
                                C[k, l] = C[j, l];
                                C[j, l] = temp;
                                temp = C[k, l + cols];
                                C[k, l + cols] = C[j, l + cols];
                                C[j, l + cols] = temp;
                            }
                            break;
                        }
                    }
                }
                if (C[j, j] == 0.0)
                {
                    for (int k = j; k < cols; k++)  // Próba zamiany z jakąś kolumną
                    {
                        if (C[j, k] != 0.0)
                        {
                            double temp = 0.0;
                            for (int l = 0; l < rows; l++)
                            {
                                temp = C[l, k];
                                C[l, k] = C[l, j];
                                C[l, j] = temp;
                                temp = C[l, k + cols];
                                C[l, k + cols] = C[l, j + cols];
                                C[l, j + cols] = temp;
                            }
                            break;
                        }
                    }
                }
                double div = C[j, j];   // Dzielnik
                if (div == 0.0)
                {
                    isInvertible = false;
                    break;
                }
                for (int k = 0; k < 2*cols; k++)    // Jedynka na przekątnej
                { C[j, k] /= div; }
                for (int l = 0; l < rows; l++)  // Zera w kolumnie
                {
                    if (l != j && C[l,j] != 0.0)
                    {
                        double qt = C[l, j];
                        for (int k = j; k < 2 * cols; k++)  // Sprawdź dla k = l
                        { C[l, k] -= C[j, k] * qt; }
                    }
                }
            }
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                { B[i, j] = C[i, j + cols]; }
            }
            return isInvertible;
        }
        public static void gaussJordanDet(double[,] Arr, out double B)
        {
            B = 1.0;
            int rows = (int)Math.Sqrt(Arr.Length);
            int cols = (int)Math.Sqrt(Arr.Length);
            double[,] A = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                { A[i,j] = Arr[i,j]; }
            }

            for (int j = 0; j < cols; j++)
            {
                if (A[j, j] == 0.0)
                {
                    for (int k = j; k < rows; k++)  // Próba zamiany z jakimś wierszem
                    {
                        if (A[k, j] != 0.0)
                        {
                            double temp = 0.0;
                            for (int l = 0; l < cols; l++)
                            {
                                temp = A[k, l];
                                A[k, l] = A[j, l];
                                A[j, l] = temp;
                            }
                            break;
                        }
                    }
                }
                if (A[j, j] == 0.0)
                {
                    for (int k = j; k < cols; k++)  // Próba zamiany z jakąś kolumną
                    {
                        if (A[j, k] != 0.0)
                        {
                            double temp = 0.0;
                            for (int l = 0; l < rows; l++)
                            {
                                temp = A[l, k];
                                A[l, k] = A[l, j];
                                A[l, j] = temp;
                            }
                            break;
                        }
                    }
                }
                for (int l = 0; l < rows; l++)  // Zera w kolumnie
                {
                    if (l != j && A[l, j] != 0.0)
                    {
                        double qt = A[l, j] / A[j,j];
                        for (int k = j; k < cols; k++)  // Sprawdź dla k = l
                        { A[l, k] -= A[j, k] * qt; }
                    }
                }
            }
            for (int i = 0; i < rows; i++)
            {
                B *= A[i, i];
            }
        }
    }
}
