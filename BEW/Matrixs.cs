using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    class Matrixs
    {
        #region Metody

        public static double[,] GdMatrix(int n, ref ElementList elemList, ref BoundaryNodeList nodeList, double lam)    // Zwraca tablicę G z daszkiem
        {
            double[,] G = new double[nodeList.Length, elemList.Length];
            for (int i = 0; i < nodeList.Length; i++)
            {
                for (int j = 0; j < elemList.Length; j++)
                { G[i, j] = GaussianQuadrature.GIntegralConstant(n, elemList[j], nodeList[i], lam); }
            }
            return G;
        }

        public static double[,] GpkMatrix(int n, ref ElementList elemList, ref BoundaryNodeList nodeList, double lam, string pk)   // Zwraca tablicę Gp lub Gk
        {
            // pk - Zwrac albo Gp, albo Gk
            double[,] G = new double[nodeList.Length, elemList.Length];
            for (int i = 0; i < nodeList.Length; i++)
            {
                for (int j = 0; j < elemList.Length; j++)
                { G[i, j] = GaussianQuadrature.GIntegralLinear(n, elemList[j], nodeList[i], lam, pk); }
            }
            return G;
        }

        public static double[,] GpskMatrix(int n, ref ElementList elemList, ref BoundaryNodeList nodeList, double lam, string psk)   // Zwraca tablicę Gp, Gs lub Gk
        {
            // pk - Zwrac albo Gp, albo Gs, albo Gk
            double[,] G = new double[nodeList.Length, elemList.Length];
            for (int i = 0; i < nodeList.Length; i++)
            {
                for (int j = 0; j < elemList.Length; j++)
                { G[i, j] = GaussianQuadrature.GIntegralParabolic(n, elemList[j], nodeList[i], lam, psk); }
            }
            return G;
        }

        public static double[,] GMatrix(int n, ref ElementList elemList, ref BoundaryNodeList nodeList, double lam)   // Zwraca tablicę G
        {
            // n - stopień aproksymacji
            // elemList - lista z elementami brzegowymi
            double[,] G;
            switch (BoundaryElement.ElemType)
            {
                case "Constant":
                    {
                        double[,] Gd = GdMatrix(n, ref elemList, ref nodeList, lam);
                        G = Gd;
                        break;
                    }
                case "Linear":
                    {
                        double[,] Gp = GpkMatrix(n, ref elemList, ref nodeList, lam, "p");
                        double[,] Gk = GpkMatrix(n, ref elemList, ref nodeList, lam, "k");

                        int ctr = 1;    // Zmienna pomocnicza
                        G = new double[nodeList.Length, nodeList.Length];

                        for (int i = 0; i < nodeList.Length; i++)
                        {
                            ctr = 1;
                            if (nodeList[0].NodeType == 2)
                            {
                                G[i, nodeList.Length - 1] = Gk[i, elemList.Length - 1];
                                G[i, 0] = Gp[i, 0];
                            }
                            else
                            { G[i, 0] = Gk[i, elemList.Length - 1] + Gp[i, 0]; }
                            for (int j = 1; j < elemList.Length; j++)
                            {
                                if (nodeList[ctr].NodeType == 2)
                                {
                                    G[i, ctr++] = Gk[i, j - 1];
                                    G[i, ctr] = Gp[i, j];
                                }
                                else
                                { G[i, ctr] = Gk[i, j - 1] + Gp[i, j]; }
                                ctr++;
                            }
                        }
                        break;
                    }
                case "Parabolic":
                    {
                        double[,] Gp = GpskMatrix(n, ref elemList, ref nodeList, lam, "p");
                        double[,] Gs = GpskMatrix(n, ref elemList, ref nodeList, lam, "s");
                        double[,] Gk = GpskMatrix(n, ref elemList, ref nodeList, lam, "k");

                        int ctr;    // Zmienna pomocnicza
                        G = new double[nodeList.Length, nodeList.Length];

                        for (int i = 0; i < nodeList.Length; i++)
                        {
                            ctr = 2;
                            if (nodeList[0].NodeType == 2)
                            {
                                G[i, nodeList.Length - 1] = Gk[i, elemList.Length - 1];
                                G[i, 0] = Gp[i, 0];
                            }
                            else
                            { G[i, 0] = Gk[i, elemList.Length - 1] + Gp[i, 0]; }
                            G[i, 1] = Gs[i, 0];
                            for (int j = 1; j < elemList.Length; j++)
                            {
                                if (nodeList[ctr].NodeType == 2)
                                {
                                    G[i, ctr++] = Gk[i, j - 1];
                                    G[i, ctr] = Gp[i, j];
                                }
                                else
                                { G[i, ctr] = Gk[i, j - 1] + Gp[i, j]; }
                                ctr++;
                                G[i, ctr] = Gs[i, j];
                                ctr++;
                            }
                        }
                        break;
                    }
                default:
                    {
                        G = new double[elemList.Length, elemList.Length];
                        throw new System.Exception("Niepoprawny rodzaj elementu podczas tworzenia tablicy G!!!");
                    }
            }
            return G;
        }

        public static double[,] HdMatrix(int n, ref ElementList elemList, ref BoundaryNodeList nodeList)    // Zwraca tablicę H z daszkiem
        {
            double[,] H = new double[nodeList.Length, elemList.Length];

            for (int j = 0; j < elemList.Length; j++)
            {
                for (int i = 0; i < nodeList.Length; i++)
                { H[i, j] = GaussianQuadrature.HIntegralConstant(n, elemList[j], nodeList[i]); }
            }
            return H;
        }

        public static double[,] HpkMatrix(int n, ref ElementList elemList, ref BoundaryNodeList nodeList, string pk)   // Zwraca tablicę Hp lub Hk
        {
            // pk - Zwrac albo Hp, albo Hk
            double[,] H = new double[nodeList.Length, elemList.Length];
            for (int i = 0; i < nodeList.Length; i++)
            {
                for (int j = 0; j < elemList.Length; j++)
                { H[i, j] = GaussianQuadrature.HIntegralLinear(n, elemList[j], nodeList[i], pk); }
            }
            return H;
        }

        public static double[,] HpskMatrix(int n, ref ElementList elemList, ref BoundaryNodeList nodeList, string psk)   // Zwraca tablicę Hp, Hs lub Hk
        {
            // psk - Zwrac albo Hp, albo Hs, albo Hk
            double[,] H = new double[nodeList.Length, elemList.Length];
            for (int i = 0; i < nodeList.Length; i++)
            {
                for (int j = 0; j < elemList.Length; j++)
                { H[i, j] = GaussianQuadrature.HIntegralParabolic(n, elemList[j], nodeList[i], psk); }
            }
            return H;
        }

        public static double[,] HMatrix(int n, ref ElementList elemList, ref BoundaryNodeList nodeList)   // Zwraca tablicę H
        {
            // n - stopień aproksymacji
            // elemList - lista z elementami brzegowymi
            double[,] H;
            switch (BoundaryElement.ElemType)
            {
                case "Constant":
                    {
                        double[,] Hd = HdMatrix(n, ref elemList, ref nodeList);

                        H = new double[nodeList.Length, nodeList.Length];

                        for (int i = 0; i < elemList.Length; i++)
                        {
                            for (int j = 0; j < elemList.Length; j++)
                            {
                                if (i == j)  // Jeżeli i = j
                                { H[i, j] = Hd[i, j] - 0.5; }
                                else
                                { H[i, j] = Hd[i, j]; }
                            }
                        }

                        break;
                    }
                case "Linear":
                    {
                        double[,] Hp = HpkMatrix(n, ref elemList, ref nodeList, "p");
                        double[,] Hk = HpkMatrix(n, ref elemList, ref nodeList, "k");

                        int ctr = 1;    // Zmienna pomocnicza
                        H = new double[nodeList.Length, nodeList.Length];

                        for (int i = 0; i < nodeList.Length; i++)
                        {
                            ctr = 1;
                            if (nodeList[0].NodeType == 2)
                            {
                                H[i, nodeList.Length - 1] = Hk[i, elemList.Length - 1];
                                H[i, 0] = Hp[i, 0];
                            }
                            else
                            { H[i, 0] = Hk[i, elemList.Length - 1] + Hp[i, 0]; }
                            for (int j = 1; j < elemList.Length; j++)
                            {
                                if (nodeList[ctr].NodeType == 2)
                                {
                                    H[i, ctr++] = Hk[i, j - 1];
                                    H[i, ctr] = Hp[i, j];
                                }
                                else
                                { H[i, ctr] = Hk[i, j - 1] + Hp[i, j]; }
                                ctr++;
                            }
                        }
                        double sum = 0.0;
                        for (int i = 0; i < nodeList.Length; i++)
                        {
                            for (int j = 0; j < nodeList.Length; j++)
                            {
                                if (i != j)
                                { sum += H[i, j]; }
                            }
                            H[i, i] = -sum;
                            sum = 0.0;
                        }
                        break;
                    }
                case "Parabolic":
                    {
                        double[,] Hp = HpskMatrix(n, ref elemList, ref nodeList, "p");
                        double[,] Hs = HpskMatrix(n, ref elemList, ref nodeList, "s");
                        double[,] Hk = HpskMatrix(n, ref elemList, ref nodeList, "k");

                        int ctr;    // Zmienna pomocnicza
                        H = new double[nodeList.Length, nodeList.Length];

                        for (int i = 0; i < nodeList.Length; i++)
                        {
                            ctr = 2;
                            if (nodeList[0].NodeType == 2)
                            {
                                H[i, nodeList.Length - 1] = Hk[i, elemList.Length - 1];
                                H[i, 0] = Hp[i, 0];
                            }
                            else
                            { H[i, 0] = Hk[i, elemList.Length - 1] + Hp[i, 0]; }
                            H[i, 1] = Hs[i, 0];
                            for (int j = 1; j < elemList.Length; j++)
                            {
                                if (nodeList[ctr].NodeType == 2)
                                {
                                    H[i, ctr++] = Hk[i, j - 1];
                                    H[i, ctr] = Hp[i, j];
                                }
                                else
                                { H[i, ctr] = Hk[i, j - 1] + Hp[i, j]; }
                                ctr++;
                                H[i, ctr] = Hs[i, j];
                                ctr++;
                            }
                        }
                        double sum = 0.0;
                        for (int i = 0; i < nodeList.Length; i++)
                        {
                            for (int j = 0; j < nodeList.Length; j++)
                            {
                                if (i != j)
                                { sum += H[i, j]; }
                            }
                            H[i, i] = -sum;
                            sum = 0.0;
                        }
                        break;
                    }
                default:
                    {
                        H = new double[elemList.Length, elemList.Length];
                        throw new System.Exception("Niepoprawny rodzaj elementu podczas tworzenia tablicy H!!!");
                    }
            }
            return H;
        }

        public static double[,] GMatrixForInternalPoints(int n, ref ElementList elemList, ref BoundaryNodeList internalNodeList, ref BoundaryNodeList boundaryNodeList, double lam)   // Zwraca tablicę G
        {
            // n - stopień aproksymacji
            // elemList - lista z elementami brzegowymi
            double[,] G;
            switch (BoundaryElement.ElemType)
            {
                case "Linear":
                    {
                        double[,] Gp = GpkMatrix(n, ref elemList, ref internalNodeList, lam, "p");
                        double[,] Gk = GpkMatrix(n, ref elemList, ref internalNodeList, lam, "k");

                        int ctr = 1;    // Zmienna pomocnicza
                        G = new double[internalNodeList.Length, boundaryNodeList.Length];

                        for (int i = 0; i < internalNodeList.Length; i++)
                        {
                            ctr = 1;
                            if (boundaryNodeList[0].NodeType == 2)
                            {
                                G[i, boundaryNodeList.Length - 1] = Gk[i, elemList.Length - 1];
                                G[i, 0] = Gp[i, 0];
                            }
                            else
                            { G[i, 0] = Gk[i, elemList.Length - 1] + Gp[i, 0]; }
                            for (int j = 1; j < elemList.Length; j++)
                            {
                                if (boundaryNodeList[ctr].NodeType == 2)
                                {
                                    G[i, ctr++] = Gk[i, j - 1];
                                    G[i, ctr] = Gp[i, j];
                                }
                                else
                                { G[i, ctr] = Gk[i, j - 1] + Gp[i, j]; }
                                ctr++;
                            }
                        }
                        break;
                    }
                case "Parabolic":
                    {
                        double[,] Gp = GpskMatrix(n, ref elemList, ref internalNodeList, lam, "p");
                        double[,] Gs = GpskMatrix(n, ref elemList, ref internalNodeList, lam, "s");
                        double[,] Gk = GpskMatrix(n, ref elemList, ref internalNodeList, lam, "k");

                        int ctr;    // Zmienna pomocnicza
                        G = new double[internalNodeList.Length, boundaryNodeList.Length];

                        for (int i = 0; i < internalNodeList.Length; i++)
                        {
                            ctr = 2;
                            if (boundaryNodeList[0].NodeType == 2)
                            {
                                G[i, boundaryNodeList.Length - 1] = Gk[i, elemList.Length - 1];
                                G[i, 0] = Gp[i, 0];
                            }
                            else
                            { G[i, 0] = Gk[i, elemList.Length - 1] + Gp[i, 0]; }
                            G[i, 1] = Gs[i, 0];
                            for (int j = 1; j < elemList.Length; j++)
                            {
                                if (boundaryNodeList[ctr].NodeType == 2)
                                {
                                    G[i, ctr++] = Gk[i, j - 1];
                                    G[i, ctr] = Gp[i, j];
                                }
                                else
                                { G[i, ctr] = Gk[i, j - 1] + Gp[i, j]; }
                                ctr++;
                                G[i, ctr] = Gs[i, j];
                                ctr++;
                            }
                        }
                        break;
                    }
                default:
                    {
                        G = new double[elemList.Length, elemList.Length];
                        throw new System.Exception("Niepoprawny rodzaj elementu podczas tworzenia tablicy G!!!");
                    }
            }
            return G;
        }
        public static double[,] HMatrixForInternalPoints(int n, ref ElementList elemList, ref BoundaryNodeList internalNodeList, ref BoundaryNodeList boundaryNodeList)   // Zwraca tablicę H
        {
            // n - stopień aproksymacji
            // elemList - lista z elementami brzegowymi
            double[,] H;
            switch (BoundaryElement.ElemType)
            {
                case "Linear":
                    {
                        double[,] Hp = HpkMatrix(n, ref elemList, ref internalNodeList, "p");
                        double[,] Hk = HpkMatrix(n, ref elemList, ref internalNodeList, "k");

                        int ctr = 1;    // Zmienna pomocnicza
                        H = new double[internalNodeList.Length, boundaryNodeList.Length];

                        for (int i = 0; i < internalNodeList.Length; i++)
                        {
                            ctr = 1;
                            if (boundaryNodeList[0].NodeType == 2)
                            {
                                H[i, boundaryNodeList.Length - 1] = Hk[i, elemList.Length - 1];
                                H[i, 0] = Hp[i, 0];
                            }
                            else
                            { H[i, 0] = Hk[i, elemList.Length - 1] + Hp[i, 0]; }
                            for (int j = 1; j < elemList.Length; j++)
                            {
                                if (boundaryNodeList[ctr].NodeType == 2)
                                {
                                    H[i, ctr++] = Hk[i, j - 1];
                                    H[i, ctr] = Hp[i, j];
                                }
                                else
                                { H[i, ctr] = Hk[i, j - 1] + Hp[i, j]; }
                                ctr++;
                            }
                        }
                        break;
                    }
                case "Parabolic":
                    {
                        H = new double[internalNodeList.Length, boundaryNodeList.Length];
                        double[,] Hp = HpskMatrix(n, ref elemList, ref internalNodeList, "p");
                        double[,] Hs = HpskMatrix(n, ref elemList, ref internalNodeList, "s");
                        double[,] Hk = HpskMatrix(n, ref elemList, ref internalNodeList, "k");

                        int ctr;    // Zmienna pomocnicza

                        for (int i = 0; i < internalNodeList.Length; i++)
                        {
                            ctr = 2;
                            if (boundaryNodeList[0].NodeType == 2)
                            {
                                H[i, boundaryNodeList.Length - 1] = Hk[i, elemList.Length - 1];
                                H[i, 0] = Hp[i, 0];
                            }
                            else
                            { H[i, 0] = Hk[i, elemList.Length - 1] + Hp[i, 0]; }
                            H[i, 1] = Hs[i, 0];
                            for (int j = 1; j < elemList.Length; j++)
                            {
                                if (boundaryNodeList[ctr].NodeType == 2)
                                {
                                    H[i, ctr++] = Hk[i, j - 1];
                                    H[i, ctr] = Hp[i, j];
                                }
                                else
                                { H[i, ctr] = Hk[i, j - 1] + Hp[i, j]; }
                                ctr++;
                                H[i, ctr] = Hs[i, j];
                                ctr++;
                            }
                        }
                        break;
                    }
                default:
                    {
                        H = new double[elemList.Length, elemList.Length];
                        throw new System.Exception("Niepoprawny rodzaj elementu podczas tworzenia tablicy H dla punktów wewnętrznych!!!");
                    }
            }
            return H;
        }

        public static double[,] A1Matrix(ref double[,] G, ref double[,] H, ref BoundaryNodeList nodeList)   // Zwraca tablicę A1
        {
            int dim = nodeList.Length;
            double[,] A = new double[dim, dim];

            for (int j = 0; j < dim; j++)
            {
                if (nodeList[j].BC == "Dirichlet")
                {
                    for (int i = 0; i < dim; i++)
                    { A[i, j] = G[i, j]; }
                }
                else
                {
                    if (nodeList[j].BC == "Neumann")
                    {
                        for (int i = 0; i < dim; i++)
                        { A[i, j] = -H[i, j]; }
                    }
                    else
                    {
                        if (nodeList[j].BC == "Robin")
                        {
                            for (int i = 0; i < dim; i++)
                            { A[i, j] = G[i, j] * nodeList[j].A - H[i, j]; }
                        }
                    }
                }
            }
            return A;
        }
        public static double[,] A1MatrixMME(ref double[,] G, ref double[,] H, ref BoundaryNodeList nodeList)   // Zwraca tablicę A1
        {
            int dim = nodeList.Length;
            double[,] A = new double[dim, dim];

            for (int j = 0; j < dim; j++)
            {
                if (nodeList[j].BC == "Dirichlet")
                {
                    for (int i = 0; i < dim; i++)
                    { A[i, j] = G[i, j]; }
                }
                else
                {
                    if (nodeList[j].BC == "Neumann")
                    {
                        for (int i = 0; i < dim; i++)
                        { A[i, j] = -H[i, j]; }
                    }
                    else
                    {
                        if (nodeList[j].BC == "Robin")
                        {
                            for (int i = 0; i < dim; i++)
                            { A[i, j] = G[i, j] * nodeList[j].A - H[i, j]; }
                        }
                        else
                        {
                            for (int i = 0; i < dim; i++)   // Zkaładam, że identyfikuję pierwszy WB
                            { A[i, j] = G[i, j]; }
                        }
                    }
                }
            }
            return A;
        }
        public static double[,] A2Matrix(ref double[,] G, ref double[,] H, ref BoundaryNodeList nodeList)   // Zwraca tablicę A2
        {
            int dim = nodeList.Length;
            double[,] A = new double[dim, dim];

            for (int j = 0; j < dim; j++)
            {
                if (nodeList[j].BC == "Dirichlet")
                {
                    for (int i = 0; i < dim; i++)
                    { A[i, j] = H[i, j]; }
                }
                else
                {
                    if (nodeList[j].BC == "Neumann")
                    {
                        for (int i = 0; i < dim; i++)
                        { A[i, j] = -G[i, j]; }
                    }
                    else
                    {
                        if (nodeList[j].BC == "Robin")
                        {
                            for (int i = 0; i < dim; i++)
                            { A[i, j] = G[i, j] * nodeList[j].A; }
                        }
                    }
                }
            }
            return A;
        }
        public static double[,] A2MatrixMME(ref double[,] G, ref double[,] H, ref BoundaryNodeList nodeList)   // Zwraca tablicę A2
        {
            int dim = nodeList.Length;
            double[,] A = new double[dim, dim];

            for (int j = 0; j < dim; j++)
            {
                if (nodeList[j].BC == "Dirichlet")
                {
                    for (int i = 0; i < dim; i++)
                    { A[i, j] = H[i, j]; }
                }
                else
                {
                    if (nodeList[j].BC == "Neumann")
                    {
                        for (int i = 0; i < dim; i++)
                        { A[i, j] = -G[i, j]; }
                    }
                    else
                    {
                        if (nodeList[j].BC == "Robin")
                        {
                            for (int i = 0; i < dim; i++)
                            { A[i, j] = G[i, j] * nodeList[j].A; }
                        }
                        else
                        {
                            for (int i = 0; i < dim; i++)   // Zkaładam, że identyfikuję pierwszy WB
                            { A[i, j] = H[i, j]; }
                        }
                    }
                }
            }
            return A;
        }

        public static double[] FMatrix(ref double[,] G, ref double[,] H, ref BoundaryNodeList nodeList)    // Zwraca tablicę F
        {
            int dim = nodeList.Length;

            double[] F = new double[dim];

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    if (nodeList[j].BC == "Dirichlet")
                    { F[i] += H[i, j] * nodeList[j].T; }
                    else
                    {
                        if (nodeList[j].BC == "Neumann")
                        { F[i] += -G[i, j] * nodeList[j].Q; }
                        else
                        {
                            if (nodeList[j].BC == "Robin")
                            { F[i] += nodeList[j].A * G[i, j] * nodeList[j].Tot; }
                        }
                    }
                }
            }

            return F;
        }

        public static double[] XMatrix(ref BoundaryNodeList nodeList)   // Zwraca ablicę X
        {
            int dim = nodeList.Length;

            double[] X = new double[dim];

            for (int i = 0; i < dim; i++)
            {
                if (nodeList[i].BC == "Dirichlet")
                { X[i] = nodeList[i].T; }
                else
                {
                    if (nodeList[i].BC == "Neumann")
                    { X[i] = nodeList[i].Q; }
                    else
                    {
                        if (nodeList[i].BC == "Robin")
                        { X[i] = nodeList[i].Tot; }
                    }
                }
            }

            return X;
        }

        public static double[,] RMatrix(double[,] Gw, double[,] Hw, double[,] B, BoundaryNodeList bNodeList, InternalPointList iPointList)
        {
            int N = bNodeList.Length;    // N - ilość elementów brzegowych
            int M = iPointList.Count;   // M - ilość sensorów
            double[,] R = new double[M,N];
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    double sum1 = 0.0, sum2 = 0.0, sum3 = 0.0, sum4 = 0.0;
                    for (int k = 0; k < N; k++)
                    {
                        if (bNodeList[k].BC == "Identify" && MME.whichBC == "dirichlet")
                        { sum1 += Gw[i, k] * B[k, j]; }
                        if (bNodeList[k].BC == "Identify" && MME.whichBC == "neumann")
                        { sum1 += Hw[i, k] * B[k, j]; }
                        if (bNodeList[k].BC == "Dirichlet")
                        { sum2 += Gw[i, k] * B[k, j]; }
                        if (bNodeList[k].BC == "Neumann")
                        { sum3 += Hw[i, k] * B[k, j]; }
                        if (bNodeList[k].BC == "Robin")
                        { sum4 += (Hw[i, k] - Gw[i, k] * bNodeList[k].A) * B[k, j]; }
                    }
                    R[i, j] = -sum1 - sum2 + sum3 + sum4;
                }
            }
            return R;
        }
        public static double[,] DwMatrix(double[,] Gw, double[,] Hw, double[,] U, BoundaryNodeList bNodeList, InternalPointList iPointList)
        {
            int N = bNodeList.Length;    // N - ilość elementów brzegowych
            int M = iPointList.Count;   // M - ilość sensorów
            double[,] D = new double[M, N];
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    double sum1 = 0.0, sum2 = 0.0, sum3 = 0.0, sum4 = 0.0;
                    for (int k = 0; k < N; k++)
                    {
                        if (bNodeList[k].BC == "Identify" && MME.whichBC == "dirichlet")
                        { sum1 += Gw[i, k] * U[k, j]; }
                        if (bNodeList[k].BC == "Identify" && MME.whichBC == "neumann")
                        { sum1 += Hw[i, k] * U[k, j]; }
                        if (bNodeList[k].BC == "Dirichlet")
                        { sum2 += Gw[i, k] * U[k, j]; }
                        if (bNodeList[k].BC == "Neumann")
                        { sum3 += Hw[i, k] * U[k, j]; }
                        if (bNodeList[k].BC == "Robin")
                        { sum4 += (Hw[i, k] - Gw[i, k] * bNodeList[k].A) * U[k, j]; }
                    }
                    D[i, j] = -sum1 - sum2 + sum3 + sum4;
                }
            }
            return D;
        }
        public static double[,] Dw1Matrix(double[,] Gw, double[,] Hw, BoundaryNodeList bNodeList, InternalPointList iPointList)
        {
            int N1 = 0; // N1 - ilość elementów brzegowych, na których identyfikuję
            for (int j = 0; j < bNodeList.Length; j++)
            {
                if (bNodeList[j].BC == "Identify")
                { N1++; }
            }
            double[,] D = new double[iPointList.Count, N1];
            for (int i = 0; i < iPointList.Count; i++)
            {
                int ctr = 0;
                for (int j = 0; j < bNodeList.Length; j++)
                {
                    if (bNodeList[j].BC == "Identify" && MME.whichBC == "dirichlet")
                    { D[i, ctr++] = Hw[i, j]; }
                    if (bNodeList[j].BC == "Identify" && MME.whichBC == "neumann")
                    { D[i, ctr++] = -Gw[i, j]; }
                }
            }
            return D;
        }
        public static double[,] WMatrix(double[,] Hw, double[,] Gw, double[,] U, BoundaryNodeList bNodeList, InternalPointList iPointList)
        {
            int N = bNodeList.Length;
            int N1 = 0; // N1 - ilość elementów brzegowych, na których identyfikuję
            for (int j = 0; j < N; j++)
            {
                if (bNodeList[j].BC == "Identify")
                { N1++; }
            }
            int M = iPointList.Count;   // M - ilość sensorów
            double[,] W = new double[M, N];
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    double sum1 = 0.0, sum2 = 0.0, sum3 = 0.0, sum4 = 0.0;
                    for (int k = 0; k < N; k++)
                    {
                        if (bNodeList[k].BC == "Identify" && MME.whichBC == "dirichlet")
                        { sum1 += Gw[i, k] * U[j, k]; }
                        if (bNodeList[k].BC == "Identify" && MME.whichBC == "neumann")
                        { sum1 += Hw[i, k] * U[j, k]; }
                        if (bNodeList[k].BC == "Dirichlet")
                        { sum2 += Gw[i, k] * U[j ,k]; }
                        if (bNodeList[k].BC == "Neumann")
                        { sum3 += Hw[i, k] * U[j, k]; }
                        if (bNodeList[k].BC == "Robin")
                        { sum4 += (Hw[i, k] - Gw[i, k] * bNodeList[k].A) * U[k, j]; }
                    }
                    if (MME.whichBC == "dirichlet")
                    { W[i, j] = Hw[i, j] - sum1 - sum2 + sum3 + sum4; }
                    else
                    { W[i, j] = -Gw[i, j] + sum1 - sum2 + sum3 + sum4; }
                }
            }

            double[,] WI = new double[M, N1];
            int ctr = 0;
            for (int j = 0; j < N; j++)
            {
                if (bNodeList[j].BC == "Identify")
                {
                    for (int i = 0; i < M; i++)
                    { WI[i, ctr] = W[i, j]; }
                    ctr++;
                }
            }
            return WI;
        }
        public static double[] PMatrix(BoundaryNodeList bNodeList)
        {
            int N = bNodeList.Length;
            int N1 = 0; // N1 - ilość elementów brzegowych, na których identyfikuję
            for (int j = 0; j < N; j++)
            {
                if (bNodeList[j].BC == "Identify")
                { N1++; }
            }
            double[] P = new double[N - N1];
            int ctr = 0;
            for ( int j = 0; j < N; j++)
            {
                if (bNodeList[j].BC == "Dirichlet")
                { P[ctr++] = bNodeList[j].T; }
                if (bNodeList[j].BC == "Neumann")
                { P[ctr++] = bNodeList[j].Q; }
                if (bNodeList[j].BC == "Robin")
                { P[ctr++] = bNodeList[j].Tot; }
            }
            return P;
        }
        public static double[] JMatrix(BoundaryNodeList bNodeList)  // Zmienić tutaj przyjmowaną wartość dla WB Robina
        {
            int N = bNodeList.Length;
            int N1 = 0; // N1 - ilość elementów brzegowych, na których identyfikuję
            for (int j = 0; j < N; j++)
            {
                if (bNodeList[j].BC == "Identify")
                { N1++; }
            }
            double[] P = new double[N - N1];
            int ctr = 0;
            for (int j = 0; j < N; j++)
            {
                if (bNodeList[j].BC == "Dirichlet")
                { P[ctr++] = bNodeList[j].T; }
                if (bNodeList[j].BC == "Neumann")
                { P[ctr++] = bNodeList[j].Q; }
                if (bNodeList[j].BC == "Robin")
                { P[ctr++] = bNodeList[j].Tot; }
            }
            return P;
        }
        public static double[] EMatrix(double[,] Gw, double[,] Hw, double[] P, BoundaryNodeList bNodeList, InternalPointList iPointList)
        {
            int M = iPointList.Count;
            double[] E = new double[M];
            for (int i = 0; i < M; i++)
            {
                double sum1 = 0.0, sum2 = 0.0, sum3 = 0.0;
                int ctr = 0;
                for (int j = 0; j < bNodeList.Length; j++)
                {
                    if (bNodeList[j].BC == "Dirichlet")
                    { sum1 += Hw[i, j] * P[ctr++]; }
                    if (bNodeList[j].BC == "Neumann")
                    { sum2 += Gw[i, j] * P[ctr++]; }
                    if (bNodeList[j].BC == "Robin")
                    { sum3 += bNodeList[j].A * Gw[i, j] * P[ctr++]; }
                }
                E[i] = sum1 - sum2 + sum3;
            }
            return E;
        }
        public static double[] ZMatrix(double[,] Dw, double[] P, double[] E, BoundaryNodeList bNodeList, InternalPointList iPointList)
        {
            int M = iPointList.Count;
            double[] Z = new double[M];
            for (int i = 0; i < M; i++)
            {
                double sum1 = 0.0;
                int ctr = 0;
                for (int j = 0; j < bNodeList.Length; j++)
                {
                    if (bNodeList[j].BC != "Identify")
                    { sum1 += Dw[i, j] * P[ctr++]; }
                }
                E[i] = sum1 + E[i];
            }
            return E;
        }
        public static double[] FdMatrix(double[] Z, InternalPointList iPointList)
        {
            double[] F = new double[iPointList.Count];
            for (int i = 0; i < iPointList.Count; i++)
            { F[i] = iPointList[i].Temperature - Z[i]; }
            return F;
        }
        public static double[] SMatrix(double[,] U, double[] P, BoundaryNodeList bNodeList)
        {
            int N = bNodeList.Length;
            int N1 = 0; // N1 - ilość elementów brzegowych, na których identyfikuję
            for (int j = 0; j < N; j++)
            {
                if (bNodeList[j].BC == "Identify")
                { N1++; }
            }
            double[] S = new double[N1];
            int ctr = 0;
            for (int i = 0; i < N; i++)
            {
                if (bNodeList[i].BC == "Identify")
                {
                    double sum = 0.0;
                    int ctr1 = 0;
                    for (int j = 0; j < N; j++)
                    {
                        if (bNodeList[j].BC != "Identify")
                        { sum += U[i,j] * P[ctr1++]; }
                    }
                    S[ctr++] = sum;
                }
            }
            return S;
        }
        public static double[] PdMatrix(double[,] U, double[] J, double[] S, BoundaryNodeList bNodeList)
        {
            int N = bNodeList.Length;
            int N1 = 0; // N1 - ilość elementów brzegowych, na których identyfikuję
            for (int j = 0; j < N; j++)
            {
                if (bNodeList[j].BC == "Identify")
                { N1++; }
            }
            double[] Pd = new double[N1];
            int ctr = 0;
            for (int i = 0; i < N; i++)
            {
                if (bNodeList[i].BC == "Identify")
                {
                    double sum = 0.0;
                    int ctr1 = 0;
                    for (int j = 0; j < N; j++)
                    {
                        if (bNodeList[j].BC != "Identify")
                        { sum += J[ctr1++] * U[i,j]; }
                    }
                    Pd[ctr] = sum + S[ctr++];
                }
            }
            return Pd;
        }
        public static double[,] CMatrix(double[,] U, BoundaryNodeList bNodeList)
        {
            int N = bNodeList.Length;
            int N1 = 0; // N1 - ilość elementów brzegowych, na których identyfikuję
            for (int j = 0; j < N; j++)
            {
                if (bNodeList[j].BC == "Identify")
                { N1++; }
            }
            double[,] C = new double[N1, N1];
            int ctr1 = 0;
            for (int i = 0; i < N; i++)
            {
                if (bNodeList[i].BC == "Identify")
                {
                    int ctr2 = 0;
                    for (int j = 0; j < N; j++)
                    {
                        if (bNodeList[j].BC == "Identify")
                        { C[ctr1, ctr2++] = -U[i, j]; }
                    }
                    ctr1++;
                }
            }
            return C;
        }

        #endregion
    }
}