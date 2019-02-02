using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    class GaussianQuadrature
    {
        private static double[][] et = new double[6][];
        private static double[][] w = new double[6][];
        private static double[,] GauusianPoint(int n)   // Zwraca tablicę z wagami oraz odciętymi punktów Gaussa dla różnych wartości n
        {
            et[0] = new double[2];
            w[0] = new double[2];
            et[0][0] = -0.577350269189626;
            et[0][1] = 0.577350269189626;
            w[0][0] = 1.0;
            w[0][1] = 1.0;

            et[1] = new double[3];
            w[1] = new double[3];
            et[1][0] = -0.774596669241483;
            et[1][1] = 0.0;
            et[1][2] = 0.774596669241483;
            w[1][0] = 0.555555555555556;
            w[1][1] = 0.888888888888889;
            w[1][2] = 0.555555555555555;

            et[2] = new double[4];
            w[2] = new double[4];
            et[2][0] = -0.861136311594053;
            et[2][1] = -0.339981043584856;
            et[2][2] = 0.339981043584856;
            et[2][3] = 0.861136311594053;
            w[2][0] = 0.347854845137454;
            w[2][1] = 0.652145154862546;
            w[2][2] = 0.652145154862546;
            w[2][3] = 0.347854845137454;

            et[3] = new double[5];
            w[3] = new double[5];
            et[3][0] = -0.906179845938664;
            et[3][1] = -0.538469310105683;
            et[3][2] = 0.000000000000000;
            et[3][3] = 0.538469310105683;
            et[3][4] = 0.906179845938664;
            w[3][0] = 0.236926885056189;
            w[3][1] = 0.478628670499366;
            w[3][2] = 0.568888888888888;
            w[3][3] = 0.478628670499366;
            w[3][4] = 0.236926885056189;

            et[4] = new double[6];
            w[4] = new double[6];
            et[4][0] = -0.932469514203152;
            et[4][1] = -0.661209386466265;
            et[4][2] = -0.238619186083197;
            et[4][3] = 0.238619186083197;
            et[4][4] = 0.661209386466265;
            et[4][5] = 0.932469514203152;
            w[4][0] = 0.171324492379170;
            w[4][1] = 0.360761573048139;
            w[4][2] = 0.467913934572691;
            w[4][3] = 0.467913934572691;
            w[4][4] = 0.360761573048139;
            w[4][5] = 0.171324492379170;

            et[5] = new double[7];
            w[5] = new double[7];
            et[5][0] = -0.949107912342759;
            et[5][1] = -0.741531185599394;
            et[5][2] = -0.405845151377397;
            et[5][3] = 0.000000000000000;
            et[5][4] = 0.405845151377397;
            et[5][5] = 0.741531185599394;
            et[5][6] = 0.949107912342759;
            w[5][0] = 0.129484966168870;
            w[5][1] = 0.279705391489277;
            w[5][2] = 0.381830050505119;
            w[5][3] = 0.417959183673469;
            w[5][4] = 0.381830050505119;
            w[5][5] = 0.279705391489277;
            w[5][6] = 0.129484966168870;

            double[,] toReturn = new double[2, n];
            for (int j = 0; j < n; j++)
            { toReturn[0, j] = et[n - 2][j]; }
            for (int j = 0; j < n; j++)
            { toReturn[1, j] = w[n - 2][j]; }

            return toReturn;
        }

        public static double GIntegralConstant(int n, BoundaryElement elem, BoundaryNode node, double lam) // Oblicza całkę G
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy po który odbywa się całkowanie
            //  node - pkt obserwacji
            //  lam - współczynnik przewodzenia ciepła
            double G = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];
            double d = Math.Sqrt(Math.Pow(l[0], 2) + Math.Pow(l[1], 2));  // Długość elementu

            double function = 0.0;  // Zmienna pomocnicza
            double a = 0.0, b = 0.0;    // Zmienna pomocnicza

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = node.Coordinates;

            double[] x = new double[2]; // współrzędna środka elementu brzegowego
            x[0] = elem.Node[0][0];
            x[1] = elem.Node[0][1];

            double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

            if (ksi[0] == x[0] && ksi[1] == x[1])  // Jeżeli i = j
            { G = (d / (2 * Math.PI * lam)) * (1 + Math.Log(2 / d, Math.E)); }
            else // Jeżeli i jest różne od j
            {
                for (int i = 0; i < n; i++)
                {
                    a = Math.Pow(x[0] + (l[0] / 2) * GP[0, i] - ksi[0], 2);
                    b = Math.Pow(x[1] + (l[1] / 2) * GP[0, i] - ksi[1], 2);
                    function = Math.Log((1 / Math.Sqrt(a + b)), Math.E);
                    G += function * GP[1, i];
                }

                G *= d / (4 * Math.PI * lam);
            }

            return G;
        }
        public static double GIntegralConstant(int n, BoundaryElement elem, InternalPoint iPoint, double lam) // Oblicza całkę G
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy po który odbywa się całkowanie
            //  iPoint - pkt obserwacji
            //  lam - współczynnik przewodzenia ciepła
            double G = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];
            double d = Math.Sqrt(Math.Pow(l[0], 2) + Math.Pow(l[1], 2));  // Długość elementu

            double function = 0.0;  // Zmienna pomocnicza
            double a = 0.0, b = 0.0;    // Zmienna pomocnicza

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = iPoint.Coordinates;

            double[] x = new double[2]; // współrzędna środka elementu brzegowego
            x[0] = elem.Node[0][0];
            x[1] = elem.Node[0][1];

            double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

            if (ksi[0] == x[0] && ksi[1] == x[1])  // Jeżeli i = j
            { G = (d / (2 * Math.PI * lam)) * (1 + Math.Log(2 / d, Math.E)); }
            else // Jeżeli i jest różne od j
            {
                for (int i = 0; i < n; i++)
                {
                    a = Math.Pow(x[0] + (l[0] / 2) * GP[0, i] - ksi[0], 2);
                    b = Math.Pow(x[1] + (l[1] / 2) * GP[0, i] - ksi[1], 2);
                    function = Math.Log((1 / Math.Sqrt(a + b)), Math.E);
                    G += function * GP[1, i];
                }

                G *= d / (4 * Math.PI * lam);
            }

            return G;
        }

        public static double GIntegralLinear(int n, BoundaryElement elem, BoundaryNode node, double lam, string pk) // Oblicza całkę G
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy po który odbywa się całkowanie
            //  elemWithKsi - element zawierający pkt obserwacji
            //  lam - współczynnik przewodzenia ciepła
            //  pk - Gp czy Gk
            double G = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];
            double d = Math.Sqrt(Math.Pow(l[0], 2) + Math.Pow(l[1], 2));  // Długość elementu

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = node.Coordinates;

            double[] xp = new double[2]; // współrzędna początku elementu brzegowego
            xp = elem.XP;
            double[] xk = new double[2]; // współrzędna końca elementu brzegowego
            xk = elem.XK;

            if ((ksi[0] != xp[0] || ksi[1] != xp[1]) && (ksi[0] != xk[0] || ksi[1] != xk[1]))
            {
                double function = 0.0;  // Zmienna pomocnicza
                double a = 0.0, b = 0.0;    // Zmienna pomocnicza
                double naw = 0.0;   // Zmienna pomocnicza

                double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

                for (int i = 0; i < n; i++)
                {
                    a = Math.Pow(((xp[0] + xk[0]) / 2) + (l[0] / 2) * GP[0, i] - ksi[0], 2);
                    b = Math.Pow(((xp[1] + xk[1]) / 2) + (l[1] / 2) * GP[0, i] - ksi[1], 2);
                    if (pk == "p")
                    { naw = 1 - GP[0, i]; }
                    else
                    { naw = 1 + GP[0, i]; }
                    function = naw * Math.Log((1 / Math.Sqrt(a + b)), Math.E);
                    G += function * GP[1, i];
                }

                G *= d / (8 * Math.PI * lam);
            }
            else
            {
                if (ksi[0] == xp[0] && ksi[1] == xp[1])
                {
                    if (pk == "p")
                    { G = ((d * (3 - 2 * Math.Log(d, Math.E))) / (8 * Math.PI * lam)); }
                    else
                    { G = ((d * (1 - 2 * Math.Log(d, Math.E))) / (8 * Math.PI * lam)); }
                }
                if (ksi[0] == xk[0] && ksi[1] == xk[1])
                {
                    if (pk == "p")
                    { G = ((d * (1 - 2 * Math.Log(d, Math.E))) / (8 * Math.PI * lam)); }
                    else
                    { G = ((d * (3 - 2 * Math.Log(d, Math.E))) / (8 * Math.PI * lam)); }
                }
            }

            return G;
        }
        public static double GIntegralLinear(int n, BoundaryElement elem, InternalPoint iPoint, double lam, string pk) // Oblicza całkę G
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy po który odbywa się całkowanie
            //  elemWithKsi - element zawierający pkt obserwacji
            //  lam - współczynnik przewodzenia ciepła
            //  pk - Gp czy Gk
            double G = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];
            double d = Math.Sqrt(Math.Pow(l[0], 2) + Math.Pow(l[1], 2));  // Długość elementu

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = iPoint.Coordinates;

            double[] xp = new double[2]; // współrzędna początku elementu brzegowego
            xp = elem.XP;
            double[] xk = new double[2]; // współrzędna końca elementu brzegowego
            xk = elem.XK;

            if ((ksi[0] != xp[0] || ksi[1] != xp[1]) && (ksi[0] != xk[0] || ksi[1] != xk[1]))
            {
                double function = 0.0;  // Zmienna pomocnicza
                double a = 0.0, b = 0.0;    // Zmienna pomocnicza
                double naw = 0.0;   // Zmienna pomocnicza

                double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

                for (int i = 0; i < n; i++)
                {
                    a = Math.Pow(((xp[0] + xk[0]) / 2) + (l[0] / 2) * GP[0, i] - ksi[0], 2);
                    b = Math.Pow(((xp[1] + xk[1]) / 2) + (l[1] / 2) * GP[0, i] - ksi[1], 2);
                    if (pk == "p")
                    { naw = 1 - GP[0, i]; }
                    else
                    { naw = 1 + GP[0, i]; }
                    function = naw * Math.Log((1 / Math.Sqrt(a + b)), Math.E);
                    G += function * GP[1, i];
                }

                G *= d / (8 * Math.PI * lam);
            }
            else
            {
                if (ksi[0] == xp[0] && ksi[1] == xp[1])
                {
                    if (pk == "p")
                    { G = ((d * (3 - 2 * Math.Log(d, Math.E))) / (8 * Math.PI * lam)); }
                    else
                    { G = ((d * (1 - 2 * Math.Log(d, Math.E))) / (8 * Math.PI * lam)); }
                }
                if (ksi[0] == xk[0] && ksi[1] == xk[1])
                {
                    if (pk == "p")
                    { G = ((d * (1 - 2 * Math.Log(d, Math.E))) / (8 * Math.PI * lam)); }
                    else
                    { G = ((d * (3 - 2 * Math.Log(d, Math.E))) / (8 * Math.PI * lam)); }
                }
            }

            return G;
        }

        public static double GIntegralParabolic(int n, BoundaryElement elem, BoundaryNode node, double lam, string psk) // Oblicza całkę G
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy po który odbywa się całkowanie
            //  elemWithKsi - element zawierający pkt obserwacji
            //  lam - współczynnik przewodzenia ciepła
            //  pk - Gp, Gs, czy Gk
            double G = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];
            double d = Math.Sqrt(Math.Pow(l[0], 2) + Math.Pow(l[1], 2));  // Długość elementu

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = node.Coordinates;

            double[] xp = new double[2]; // współrzędna początku elementu brzegowego
            xp = elem.XP;
            double[] xs = new double[2]; // współrzędna środka elementu brzegowego
            xs = elem.Node[1];
            double[] xk = new double[2]; // współrzędna końca elementu brzegowego
            xk = elem.XK;

            if ((ksi[0] != xp[0] || ksi[1] != xp[1]) && (ksi[0] != xs[0] || ksi[1] != xs[1]) && (ksi[0] != xk[0] || ksi[1] != xk[1]))
            {
                double function = 0.0;  // Zmienna pomocnicza
                double a = 0.0, b = 0.0;    // Zmienna pomocnicza
                double naw = 0.0;   // Zmienna pomocnicza
                double con = 0.0;   // Zmienna pomocnicza

                double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

                for (int i = 0; i < n; i++)
                {
                    a = Math.Pow(((xp[0] + xk[0]) / 2) + (l[0] / 2) * GP[0, i] - ksi[0], 2);
                    b = Math.Pow(((xp[1] + xk[1]) / 2) + (l[1] / 2) * GP[0, i] - ksi[1], 2);
                    if (psk == "p")
                    {
                        naw = GP[0, i] * (GP[0, i] - 1);
                        con = d / (8 * Math.PI * lam);
                    }
                    if (psk == "s")
                    {
                        naw = (1 + GP[0, i]) * (1 - GP[0, i]);
                        con = d / (4 * Math.PI * lam);
                    }
                    if (psk == "k")
                    {
                        naw = GP[0, i] * (1 + GP[0, i]);
                        con = d / (8 * Math.PI * lam);
                    }
                    function = naw * Math.Log((1 / Math.Sqrt(a + b)), Math.E);
                    G += function * GP[1, i];
                }

                G *= con;
            }
            else
            {
                if (ksi[0] == xp[0] && ksi[1] == xp[1])
                {
                    if (psk == "p")
                    { G = ((d * (17 - 6 * Math.Log(d, Math.E))) / (72 * Math.PI * lam)); }
                    if (psk == "s")
                    { G = ((d * (5 - 6 * Math.Log(d, Math.E))) / (18 * Math.PI * lam)); }
                    if (psk == "k")
                    { G = ((d * (-1 - 6 * Math.Log(d, Math.E))) / (72 * Math.PI * lam)); }
                }
                if (ksi[0] == xs[0] && ksi[1] == xs[1])
                {
                    if (psk == "p")
                    { G = ((d * (1 + 3 * Math.Log(2, Math.E) - 3 * Math.Log(d, Math.E))) / (36 * Math.PI * lam)); }
                    if (psk == "s")
                    { G = ((d * (4 + 3 * Math.Log(2, Math.E) - 3 * Math.Log(d, Math.E))) / (9 * Math.PI * lam)); }
                    if (psk == "k")
                    { G = ((d * (1 + 3 * Math.Log(2, Math.E) - 3 * Math.Log(d, Math.E))) / (36 * Math.PI * lam)); }
                }
                if (ksi[0] == xk[0] && ksi[1] == xk[1])
                {
                    if (psk == "p")
                    { G = ((d * (-1 - 6 * Math.Log(d, Math.E))) / (72 * Math.PI * lam)); }
                    if (psk == "s")
                    { G = ((d * (5 - 6 * Math.Log(d, Math.E))) / (18 * Math.PI * lam)); }
                    if (psk == "k")
                    { G = ((d * (17 - 6 * Math.Log(d, Math.E))) / (72 * Math.PI * lam)); }
                }
            }

            return G;
        }
        public static double GIntegralParabolic(int n, BoundaryElement elem, InternalPoint iPoint, double lam, string psk) // Oblicza całkę G
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy po który odbywa się całkowanie
            //  elemWithKsi - element zawierający pkt obserwacji
            //  lam - współczynnik przewodzenia ciepła
            //  pk - Gp, Gs, czy Gk
            double G = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];
            double d = Math.Sqrt(Math.Pow(l[0], 2) + Math.Pow(l[1], 2));  // Długość elementu

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = iPoint.Coordinates;

            double[] xp = new double[2]; // współrzędna początku elementu brzegowego
            xp = elem.XP;
            double[] xs = new double[2]; // współrzędna środka elementu brzegowego
            xs = elem.Node[1];
            double[] xk = new double[2]; // współrzędna końca elementu brzegowego
            xk = elem.XK;

            if ((ksi[0] != xp[0] || ksi[1] != xp[1]) && (ksi[0] != xs[0] || ksi[1] != xs[1]) && (ksi[0] != xk[0] || ksi[1] != xk[1]))
            {
                double function = 0.0;  // Zmienna pomocnicza
                double a = 0.0, b = 0.0;    // Zmienna pomocnicza
                double naw = 0.0;   // Zmienna pomocnicza
                double con = 0.0;   // Zmienna pomocnicza

                double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

                for (int i = 0; i < n; i++)
                {
                    a = Math.Pow(((xp[0] + xk[0]) / 2) + (l[0] / 2) * GP[0, i] - ksi[0], 2);
                    b = Math.Pow(((xp[1] + xk[1]) / 2) + (l[1] / 2) * GP[0, i] - ksi[1], 2);
                    if (psk == "p")
                    {
                        naw = GP[0, i] * (GP[0, i] - 1);
                        con = d / (8 * Math.PI * lam);
                    }
                    if (psk == "s")
                    {
                        naw = (1 + GP[0, i]) * (1 - GP[0, i]);
                        con = d / (4 * Math.PI * lam);
                    }
                    if (psk == "k")
                    {
                        naw = GP[0, i] * (1 + GP[0, i]);
                        con = d / (8 * Math.PI * lam);
                    }
                    function = naw * Math.Log((1 / Math.Sqrt(a + b)), Math.E);
                    G += function * GP[1, i];
                }

                G *= con;
            }
            else
            {
                if (ksi[0] == xp[0] && ksi[1] == xp[1])
                {
                    if (psk == "p")
                    { G = ((d * (17 - 6 * Math.Log(d, Math.E))) / (72 * Math.PI * lam)); }
                    if (psk == "s")
                    { G = ((d * (5 - 6 * Math.Log(d, Math.E))) / (18 * Math.PI * lam)); }
                    if (psk == "k")
                    { G = ((d * (-1 - 6 * Math.Log(d, Math.E))) / (72 * Math.PI * lam)); }
                }
                if (ksi[0] == xs[0] && ksi[1] == xs[1])
                {
                    if (psk == "p")
                    { G = ((d * (1 + 3 * Math.Log(2, Math.E) - 3 * Math.Log(d, Math.E))) / (36 * Math.PI * lam)); }
                    if (psk == "s")
                    { G = ((d * (4 + 3 * Math.Log(2, Math.E) - 3 * Math.Log(d, Math.E))) / (9 * Math.PI * lam)); }
                    if (psk == "k")
                    { G = ((d * (1 + 3 * Math.Log(2, Math.E) - 3 * Math.Log(d, Math.E))) / (36 * Math.PI * lam)); }
                }
                if (ksi[0] == xk[0] && ksi[1] == xk[1])
                {
                    if (psk == "p")
                    { G = ((d * (-1 - 6 * Math.Log(d, Math.E))) / (72 * Math.PI * lam)); }
                    if (psk == "s")
                    { G = ((d * (5 - 6 * Math.Log(d, Math.E))) / (18 * Math.PI * lam)); }
                    if (psk == "k")
                    { G = ((d * (17 - 6 * Math.Log(d, Math.E))) / (72 * Math.PI * lam)); }
                }
            }

            return G;
        }

        public static double HIntegralConstant(int n, BoundaryElement elem, BoundaryNode node) // Oblicza całkę H z daszkiem
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy
            //  node - współrzędna punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            double H = 0.0;

            double function = 0.0;  // Zmienna pomocnicza
            double a = 0.0, b = 0.0;    // Zmienna pomocnicza

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = node.Coordinates;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];

            // Tuataj należy zmienić pierwszy indeks przy Node, aby wybierał inne wezły z elementu
            double[] x = new double[2]; // współrzędna środka elementu brzegowego
            x[0] = elem.Node[0][0];
            x[1] = elem.Node[0][1];

            double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

            if (ksi[0] == x[0] && ksi[1] == x[1])
            { H = 0.0; }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    a = x[0] + (l[0] / 2) * GP[0, i] - ksi[0];
                    b = x[1] + (l[1] / 2) * GP[0, i] - ksi[1];
                    function = (a * l[1] - b * l[0]) / (Math.Pow(a, 2) + Math.Pow(b, 2));
                    H += function * GP[1, i];
                }
                H *= 1 / (4 * Math.PI);
            }

            return H;
        }
        public static double HIntegralConstant(int n, BoundaryElement elem, InternalPoint iPoint) // Oblicza całkę H z daszkiem
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy
            //  node - współrzędna punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            double H = 0.0;

            double function = 0.0;  // Zmienna pomocnicza
            double a = 0.0, b = 0.0;    // Zmienna pomocnicza

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = iPoint.Coordinates;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];

            // Tuataj należy zmienić pierwszy indeks przy Node, aby wybierał inne wezły z elementu
            double[] x = new double[2]; // współrzędna środka elementu brzegowego
            x[0] = elem.Node[0][0];
            x[1] = elem.Node[0][1];

            double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

            if (ksi[0] == x[0] && ksi[1] == x[1])
            { H = 0.0; }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    a = x[0] + (l[0] / 2) * GP[0, i] - ksi[0];
                    b = x[1] + (l[1] / 2) * GP[0, i] - ksi[1];
                    function = (a * l[1] - b * l[0]) / (Math.Pow(a, 2) + Math.Pow(b, 2));
                    H += function * GP[1, i];
                }
                H *= 1 / (4 * Math.PI);
            }

            return H;
        }

        public static double HIntegralLinear(int n, BoundaryElement elem, BoundaryNode node, string pk) // Oblicza całkę H z daszkiem
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy
            //  ksi - współrzędna punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            //  lam - współczynnik przewodzenia ciepła
            //  pk - Gp czy Gk
            double H = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = node.Coordinates;

            double[] xp = new double[2]; // współrzędna początku elementu brzegowego
            xp = elem.XP;
            double[] xk = new double[2]; // współrzędna końca elementu brzegowego
            xk = elem.XK;

            if ((ksi[0] != xp[0] || ksi[1] != xp[1]) && (ksi[0] != xk[0] || ksi[1] != xk[1]))
            {
                double function = 0.0;  // Zmienna pomocnicza
                double a = 0.0, b = 0.0;    // Zmienna pomocnicza
                double naw = 0.0;   // Zmienna pomocnicza

                double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

                for (int i = 0; i < n; i++)
                {
                    a = ((xp[0] + xk[0]) / 2) + (l[0] / 2) * GP[0, i] - ksi[0];
                    b = ((xp[1] + xk[1]) / 2) + (l[1] / 2) * GP[0, i] - ksi[1];
                    if (pk == "p")
                    { naw = 1 - GP[0, i]; }
                    else
                    { naw = 1 + GP[0, i]; }
                    function = naw * ((a * l[1] - b * l[0]) / (Math.Pow(a, 2) + Math.Pow(b, 2)));
                    H += function * GP[1, i];
                }
                H *= (1 / (8 * Math.PI));
            }
            else
            {
                if (ksi[0] == xp[0] && ksi[1] == xp[1])
                { H = 0.0; }
                if (ksi[0] == xk[0] && ksi[1] == xk[1])
                { H = 0.0; }
            }

            return H;
        }
        public static double HIntegralLinear(int n, BoundaryElement elem, InternalPoint iPoint, string pk) // Oblicza całkę H z daszkiem
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy
            //  ksi - współrzędna punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            //  lam - współczynnik przewodzenia ciepła
            //  pk - Gp czy Gk
            double H = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = iPoint.Coordinates;

            double[] xp = new double[2]; // współrzędna początku elementu brzegowego
            xp = elem.XP;
            double[] xk = new double[2]; // współrzędna końca elementu brzegowego
            xk = elem.XK;

            if ((ksi[0] != xp[0] || ksi[1] != xp[1]) && (ksi[0] != xk[0] || ksi[1] != xk[1]))
            {
                double function = 0.0;  // Zmienna pomocnicza
                double a = 0.0, b = 0.0;    // Zmienna pomocnicza
                double naw = 0.0;   // Zmienna pomocnicza

                double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

                for (int i = 0; i < n; i++)
                {
                    a = ((xp[0] + xk[0]) / 2) + (l[0] / 2) * GP[0, i] - ksi[0];
                    b = ((xp[1] + xk[1]) / 2) + (l[1] / 2) * GP[0, i] - ksi[1];
                    if (pk == "p")
                    { naw = 1 - GP[0, i]; }
                    else
                    { naw = 1 + GP[0, i]; }
                    function = naw * ((a * l[1] - b * l[0]) / (Math.Pow(a, 2) + Math.Pow(b, 2)));
                    H += function * GP[1, i];
                }
                H *= (1 / (8 * Math.PI));
            }
            else
            {
                if (ksi[0] == xp[0] && ksi[1] == xp[1])
                { H = 0.0; }
                if (ksi[0] == xk[0] && ksi[1] == xk[1])
                { H = 0.0; }
            }

            return H;
        }

        public static double HIntegralParabolic(int n, BoundaryElement elem, BoundaryNode node, string psk) // Oblicza całkę H z daszkiem
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy
            //  ksi - współrzędna punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            //  lam - współczynnik przewodzenia ciepła
            //  pk - Hp, Hs, czy Hk
            double H = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = node.Coordinates;

            double[] xp = new double[2]; // współrzędna początku elementu brzegowego
            xp = elem.XP;
            double[] xs = new double[2]; // współrzędna środka elementu brzegowego
            xs = elem.Node[1];
            double[] xk = new double[2]; // współrzędna końca elementu brzegowego
            xk = elem.XK;

            if ((ksi[0] != xp[0] || ksi[1] != xp[1]) && (ksi[0] != xs[0] || ksi[1] != xs[1]) && (ksi[0] != xk[0] || ksi[1] != xk[1]))
            {
                double function = 0.0;  // Zmienna pomocnicza
                double a = 0.0, b = 0.0;    // Zmienna pomocnicza
                double naw = 0.0;   // Zmienna pomocnicza
                double con = 0.0;

                double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

                for (int i = 0; i < n; i++)
                {
                    a = ((xp[0] + xk[0]) / 2) + (l[0] / 2) * GP[0, i] - ksi[0];
                    b = ((xp[1] + xk[1]) / 2) + (l[1] / 2) * GP[0, i] - ksi[1];
                    if (psk == "p")
                    {
                        naw = GP[0, i] * (GP[0, i] - 1);
                        con = 1 / (8 * Math.PI);
                    }
                    if (psk == "s")
                    {
                        naw = (1 + GP[0, i]) * (1 - GP[0, i]);
                        con = 1 / (4 * Math.PI);
                    }
                    if (psk == "k")
                    {
                        naw = GP[0, i] * (1 + GP[0, i]);
                        con = 1 / (8 * Math.PI);
                    }
                    function = naw * ((a * l[1] - b * l[0]) / (Math.Pow(a, 2) + Math.Pow(b, 2)));
                    H += function * GP[1, i];
                }
                H *= con;
            }
            else
            {
                if (ksi[0] == xp[0] && ksi[1] == xp[1])
                { H = 0.0; }
                if (ksi[0] == xs[0] && ksi[1] == xs[1])
                { H = 0.0; }
                if (ksi[0] == xk[0] && ksi[1] == xk[1])
                { H = 0.0; }
            }

            return H;
        }
        public static double HIntegralParabolic(int n, BoundaryElement elem, InternalPoint iPoint, string psk) // Oblicza całkę H z daszkiem
        {
            //  n - ilość punktów Gaussa
            //  elem - element brzegowy
            //  ksi - współrzędna punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            //  lam - współczynnik przewodzenia ciepła
            //  pk - Hp, Hs, czy Hk
            double H = 0.0;

            double[] l = new double[2]; // tablica z długościami elementu względem x1 oraz x2
            l[0] = elem.XK[0] - elem.XP[0];
            l[1] = elem.XK[1] - elem.XP[1];

            double[] ksi = new double[2];   // Współrzędne punktu obserwacji (punktu w którym przyłożono punktowe źródło ciepła)
            ksi = iPoint.Coordinates;

            double[] xp = new double[2]; // współrzędna początku elementu brzegowego
            xp = elem.XP;
            double[] xs = new double[2]; // współrzędna środka elementu brzegowego
            xs = elem.Node[1];
            double[] xk = new double[2]; // współrzędna końca elementu brzegowego
            xk = elem.XK;

            if ((ksi[0] != xp[0] || ksi[1] != xp[1]) && (ksi[0] != xs[0] || ksi[1] != xs[1]) && (ksi[0] != xk[0] || ksi[1] != xk[1]))
            {
                double function = 0.0;  // Zmienna pomocnicza
                double a = 0.0, b = 0.0;    // Zmienna pomocnicza
                double naw = 0.0;   // Zmienna pomocnicza
                double con = 0.0;

                double[,] GP = GauusianPoint(n);    // Tablica z wagami i odciętymi pkt. Gaussa

                for (int i = 0; i < n; i++)
                {
                    a = ((xp[0] + xk[0]) / 2) + (l[0] / 2) * GP[0, i] - ksi[0];
                    b = ((xp[1] + xk[1]) / 2) + (l[1] / 2) * GP[0, i] - ksi[1];
                    if (psk == "p")
                    {
                        naw = GP[0, i] * (GP[0, i] - 1);
                        con = 1 / (8 * Math.PI);
                    }
                    if (psk == "s")
                    {
                        naw = (1 + GP[0, i]) * (1 - GP[0, i]);
                        con = 1 / (4 * Math.PI);
                    }
                    if (psk == "k")
                    {
                        naw = GP[0, i] * (1 + GP[0, i]);
                        con = 1 / (8 * Math.PI);
                    }
                    function = naw * ((a * l[1] - b * l[0]) / (Math.Pow(a, 2) + Math.Pow(b, 2)));
                    H += function * GP[1, i];
                }
                H *= con;
            }
            else
            {
                if (ksi[0] == xp[0] && ksi[1] == xp[1])
                { H = 0.0; }
                if (ksi[0] == xs[0] && ksi[1] == xs[1])
                { H = 0.0; }
                if (ksi[0] == xk[0] && ksi[1] == xk[1])
                { H = 0.0; }
            }

            return H;
        }
    }
}
