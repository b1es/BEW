using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    class InitialTask
    {
        #region Pola składowe
        private int n;  // Ilość zmiennych decyzyjnych
        private int m;  // Ilość ograniczeń
        private double[,] c;
        private double[] p;
        private double[,] a;
        private double[] b;
        #endregion

        #region Konstruktory
        public InitialTask(int n, int m, double[,] C, double[] p, double[,] A, double[] b)
        {
            this.n = n;
            this.m = m;
            this.c = C;
            MakeSymetric();
            IsNonnegative();
            this.p = p;
            this.a = A;
            this.b = b;
        }
        #endregion

        #region Metody
        private void MakeSymetric()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (c[i, j] != c[j, i])
                    {
                        double temp = (c[i, j] + c[j, i]) / 2;
                        c[i, j] = temp;
                        c[j, i] = temp;
                    }
                }
            }
        }

        private void IsNonnegative()
        {
            double det;
            GaussJordanElimination.gaussJordanDet(c, out det);
            if(det <= 0.0)
            { throw new System.Exception("Macierz C nie jest nieujemnie określona!!!"); }
            for (int i = 0; i < (int)Math.Sqrt(c.Length); i++)
            {
                if (c[i, i] <= 0.0)
                { throw new System.Exception("Macierz C nie jest nieujemnie określona!!!"); }
            }
        }
        #endregion

        #region Właściowości
        public int N
        {
            get { return n; }
            set { n = value; }
        }
        public int M
        {
            get { return m; }
            set { m = value; }
        }
        public double[,] C
        {
            get { return c; }
            set { c = value; }
        }
        public double[] P
        {
            get { return p; }
            set { p = value; }
        }
        public double[,] A
        {
            get { return a; }
            set { a = value; }
        }
        public double[] B
        {
            get { return b; }
            set { b = value; }
        }
        #endregion
    }

    class SubstituteTask
    {
        #region Pola składowe
        private int n;  // Ilość zmiennych decyzyjnych
        private int m;  // Ilość ogranizceń wraz z warunkami brzegowymi
        private int q;  // Ilość ograniczeń q = m - n
        private int r;  // Ilość zmiennych sztucznych typu v
        double[,] a;    // Macierz ograniczeń
        double[] b;     // Wektor wyrazów wolnych ograniczeń
        double[,] c;    // Macierz formy kwadratowej
        double[] p;     // Wektor współczynników funkcji liniowej
        double[,] xdp;  // Jednostkowa macierz dla zmiennych x'
        double[,] vd;   // Macierz współczynników dla zmiennych sztucznych typu v
        double[,] ydp;  // Jednostkowa macierz dla zmiennych y'
        double[,] wd;   // Jednostkowa macierz dla zmiennych w
        double[,] md;   // Współczynniki lewej strony ograniczeń zadania zastępczego
        double[] bd;    // Współczynniki prawej strony ograniczeń zadania zastępczego
        string[] zd;    // Wektor zmiennych
        string[] baseVariable;  // Wektor zmiennych bazowych
        double[] baseValues;    // Wektor wartości bazowych
        double[] fc;    // Wektor funkcji celu
        #endregion

        #region Konstruktory
        public SubstituteTask(InitialTask iTask)
        {
            this.n = iTask.N;
            this.m = iTask.N + iTask.M;
            this.q = iTask.M;
            this.r = 0;
            a = new double[q, n];
            c = new double[n, n];
            p = new double[n];
            for (int i = 0; i < q; i++)
            {
                for (int j = 0; j < n; j++)
                { a[i,j] = iTask.A[i,j]; }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                { c[i, j] = iTask.C[i, j]; }
            }
            for (int j = 0; j < n; j++)
            { p[j] = iTask.P[j]; }
            xdp = new double[q, q];
            for (int i = 0; i < q; i++)
            {
                for (int j = 0; j < q; j++)
                {
                    if (i == j)
                    { xdp[i, j] = 1.0; }
                    else
                    { xdp[i, j] = 0.0; }
                }
            }
            for (int i = 0; i < q; i++ )
            {
                if (iTask.B[i] < 0.0)
                {
                    r++;
                    for (int j = 0; j < n; j++)
                    { a[i, j] *= -1; }
                    for (int j = 0; j < q; j++)
                    { xdp[i, j] *= -1; }
                }
            }
            ydp = new double[n, n];
            wd = new double[n, n];
            vd = new double[q,r];
            b = new double[q];
            for (int i = 0; i < q; i++)
            { b[i] = Math.Abs(iTask.B[i]); }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        ydp[i, j] = 1.0;
                        wd[i, j] = 1.0;
                    }
                    else
                    {
                        ydp[i, j] = 0.0;
                        wd[i, j] = 0.0;
                    }
                }
            }
            int ctr = 0;
            for (int i = 0; i < q; i++)
            {
                for (int j = 0; j < r; j++)
                { vd[i, j] = 0.0; }
                if (iTask.B[i] < 0.0)
                { vd[i, ctr++] = 1.0; }
            }

            md = new double[q + n, 3 * n + 2 * q + r];
            bd = new double[q + n];
            for (int i = 0; i < q+n; i++)   // Zerowanie M i B
            {
                bd[i] = 0.0;
                for (int j = 0; j < (3*n+2*q+r); j++)
                { md[i, j] = 0.0; }
            }
            for (int i = 0; i < q; i++)
            {
                bd[i] = Math.Abs(iTask.B[i]);
                for (int j = 0; j < n; j++)
                { md[i, j] = a[i, j]; }
            }
            for (int i = 0; i < n; i++)
            {
                bd[i + q] = iTask.P[i];
                for (int j = 0; j < n; j++)
                { md[i + q, j] = 2 * c[i, j]; }
            }
            for (int i = 0; i < q; i++)
            {
                for (int j = 0; j < q; j++)
                { md[i, j+n] = xdp[i, j]; }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < q; j++)
                { md[i+q, j + n+q] = iTask.A[j, i]; }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                { md[i + q, j + n + 2*q] = -ydp[i, j]; }
            }
            for (int i = 0; i < q; i++)
            {
                for (int j = 0; j < r; j++)
                { md[i, j + 2 * n + 2 * q] = vd[i, j]; }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                { md[i+q, j + 2 * n + 2 * q + r] = wd[i, j]; }
            }

            zd = new string[3 * n + 2 * q + r];
            ctr = 0;
            for (int i = 0; i < n; i++)
            { zd[ctr++] = "x" + (i + 1).ToString(); }
            for (int i = 0; i < q; i++)
            { zd[ctr++] = "x" + (i + 1).ToString() + "'"; }
            for (int i = 0; i < q; i++)
            { zd[ctr++] = "y" + (i + 1).ToString(); }
            for (int i = 0; i < n; i++)
            { zd[ctr++] = "y" + (i + 1).ToString() + "'"; }
            for (int i = 0; i < r; i++)
            { zd[ctr++] = "v" + (i + 1).ToString(); }
            for (int i = 0; i < n; i++)
            { zd[ctr++] = "w" + (i + 1).ToString(); }
            baseValues = new double[q + n];
            baseVariable = new string[q + n];
            ctr = 0;
            for (int i = 0; i < q; i++)
            {
                for (int k = 0; k < r; k++)
                {
                    if (vd[i, k] == 1)
                    { baseVariable[i] = zd[n + 2*q + n + ctr++]; }
                }
                if (baseVariable[i] == null)
                { baseVariable[i] = zd[n + i]; }
            }
            for (int i = 0; i < n; i++)
            { baseVariable[i+q] = zd[n + q + m + r + i]; }
            fc = new double[zd.Length];
            for (int i = 0; i < zd.Length; i++)
            {
                if (zd[i].IndexOf("v") >= 0 || zd[i].IndexOf("w") >= 0)
                { fc[i] = 1.0; }
                else
                { fc[i] = 0.0; }
            }
            for (int l = 0; l < q + n; l++)
            {
                for (int k = 0; k < zd.Length; k++)
                {
                    if (baseVariable[l] == zd[k])
                    { baseValues[l] = fc[k]; }
                }
            }
        }
        #endregion

        #region Właściwości
        public int N
        {
            get { return n; }
        }
        public int Q
        {
            get { return q; }
        }
        public double[,] M // Współczynniki lewej strony ograniczeń zadania zastępczego
        {
            get { return md; }
            set { md = value; }
        }
        public double[] B  // Współczynniki prawej strony ograniczeń zadania zastępczego
        {
            get { return bd; }
            set { bd = value; }
        }
        public string[] Z  // Wektor zmiennych
        {
            get { return zd; }
            set { zd = value; }
        }
        public string[] bVariable  // Wektor zmiennych bazowych
        {
            get { return baseVariable; }
            set { baseVariable = value; }
        }
        public double[] bValues // Wektor wartości bazowych
        {
            get { return baseValues; }
            set { baseValues = value; }
        }
        public double[] FC // Wektor funkcji celu
        {
            get { return fc; }
            set { fc = value; }
        }
        #endregion
    }
}
