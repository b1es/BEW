using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    class MME
    {
        #region Pola składowe
        public static string path = null; // Ścieżka dostępu do pliku z geometrią
        public static int nb = 7; // Ilość punktów Gaussa elementy brzegowe
        public static double lam = 1.0;
        public static double epsilon = 0.75;
        public static int precision = -1;  // - 1 oznacza brak zaokrąglenia liczb
        public static string whichBC = "dirichlet";
        #endregion

        public static Data EvaluationMME()
        {
            // Utworzenie obiektu data
            Data data = new Data();

            BoundaryList boundaryList = new BoundaryList();
            Reader.GetGeometry(@path, ref boundaryList);    // Odczytywanie listy elementów brzegowych
            Reader.GetBoundaryConditionsMME(@path, ref boundaryList);    // Odczytywanie warunków brzegowych
            BoundaryNodeList bNodeList = new BoundaryNodeList(boundaryList);    // Utworzenie listy węzłów brzegowych
            ElementList elementList = new ElementList(boundaryList);    // Utworzenie listy elementów brzegowych

            InternalPointList iPointList = constValue.MMEiternalPointList;
            BoundaryNodeList nodeList = new BoundaryNodeList(iPointList);   // Wyznaczanie listy węzłów dla elementów brzegowych

            double[,] G = Matrixs.GMatrix(nb, ref elementList, ref bNodeList, lam); // Wycznaczenie macierzy G
            double[,] H = Matrixs.HMatrix(nb, ref elementList, ref bNodeList);  // Wycznaczenie macierzy H
            double[,] B;
            
            double[,] A1 = Matrixs.A1MatrixMME(ref G, ref H, ref bNodeList);    // Wycznaczenie macierzy A1
            if (!GaussJordanElimination.gaussJordanElimination(A1, out B))
            {
                data.Error = "Macierz A1 jest nieodwracalna.\n\n" 
                    + AuxiliaryFunctions.PrintArray(A1, (int)Math.Sqrt(A1.Length), (int)Math.Sqrt(A1.Length));
                data.binarySerialize(); // Zapis obiektu data do pliku binarnego
                return data;
            }
            
            double[,] A2 = Matrixs.A2MatrixMME(ref G, ref H, ref bNodeList);    // Wycznaczenie macierzy A2

            double[,] Hw;
            double[,] Gw;
            if (BoundaryElement.ElemType == "Constant")
            {
                Hw = Matrixs.HdMatrix(nb, ref elementList, ref nodeList);
                Gw = Matrixs.GdMatrix(nb, ref elementList, ref nodeList, lam);
            }
            else
            {
                Hw = Matrixs.HMatrixForInternalPoints(nb, ref elementList, ref nodeList, ref bNodeList);
                Gw = Matrixs.GMatrixForInternalPoints(nb, ref elementList, ref nodeList, ref bNodeList, lam);
            }
            data.BoundaryList = boundaryList;
            data.ElementList = elementList;
            data.BoundaryNodeList = bNodeList;
            data.IntenralPointList = iPointList;
            data.G = G;
            data.Gw = Gw;
            data.H = H;
            data.Hw = Hw;
            data.A1 = A1;
            data.B = B;
            data.A2 = A2;
            data.R = Matrixs.RMatrix(Gw, Hw, data.B, data.BoundaryNodeList, data.IntenralPointList);
            data.Dw = Matrixs.DwMatrix(Gw, Hw, data.U, data.BoundaryNodeList, data.IntenralPointList);
            data.Dw1 = Matrixs.Dw1Matrix(Gw, Hw, data.BoundaryNodeList, data.IntenralPointList);
            data.W = Matrixs.WMatrix(data.Hw, data.Gw, data.U, data.BoundaryNodeList, data.IntenralPointList);
            data.P = Matrixs.PMatrix(data.BoundaryNodeList);
            data.E = Matrixs.EMatrix(Gw, Hw, data.P, data.BoundaryNodeList, data.IntenralPointList);
            data.Z = Matrixs.ZMatrix(data.Dw, data.P, data.E, data.BoundaryNodeList, data.IntenralPointList);
            data.Fd = Matrixs.FdMatrix(data.Z, data.IntenralPointList);
            //double[] fff = { 11.327, 21.561, 25, 21.561, 11.327 };
            //data.Fd = fff;
            data.J = Matrixs.JMatrix(data.BoundaryNodeList);
            data.S = Matrixs.SMatrix(data.U, data.P, data.BoundaryNodeList);
            data.Pd = Matrixs.PdMatrix(data.U, data.J, data.S, data.BoundaryNodeList);
            data.C = Matrixs.CMatrix(data.U, data.BoundaryNodeList);

            // WOLFE
            int n = (int)Math.Sqrt(data.C.Length);  // Ilość zmiennych decyzyjnych
            int m = (int)((data.W.Length/n)*2); // Ilość ograniczeń
            double[,] A = new double[m, n];
            double[] b = new double[m];
            double[,] C;
            double[] p;
            if (precision == -1)
            {
                for (int i = 0; i < m / 2; i++)
                {
                    for (int j = 0; j < n; j++)
                    { A[i, j] = data.W[i, j]; }
                    b[i] = data.Fd[i] + epsilon;
                }
                for (int i = 0; i < m / 2; i++)
                {
                    for (int j = 0; j < n; j++)
                    { A[i + m / 2, j] = -data.W[i, j]; }
                    b[i + m / 2] = epsilon - data.Fd[i];
                }
                C = new double[n, n];
                p = new double[n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    { C[i, j] = data.C[i, j]; }
                    p[i] = data.Pd[i];
                }
            }
            else
            {
                for (int i = 0; i < m / 2; i++)
                {
                    for (int j = 0; j < n; j++)
                    { A[i, j] = Math.Round(data.W[i, j], precision); }
                    b[i] = Math.Round(data.Fd[i] + epsilon, precision);
                }
                for (int i = 0; i < m / 2; i++)
                {
                    for (int j = 0; j < n; j++)
                    { A[i + m / 2, j] = Math.Round(-data.W[i, j], precision); }
                    b[i + m / 2] = Math.Round(epsilon - data.Fd[i], precision);
                }
                C = new double[n, n];
                p = new double[n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    { C[i, j] = Math.Round(data.C[i, j], precision); }
                    p[i] = Math.Round(data.Pd[i], precision);
                }
            }
            InitialTask iTask = new InitialTask(n, m, C, p, A, b);
            SubstituteTask sTask = new SubstituteTask(iTask);
            Wolfe wolfe = new Wolfe(sTask);
            wolfe.Evaluation();
            data.Error = wolfe.Error;
            if (data.Error == null)
            {
                AssignSolution(wolfe, ref data);
                MEB.EvaluationMEB(data);
            }
            // wolfe

            data.binarySerialize(); // Zapis obiektu data do pliku binarnego
            return data;
        }
        private static void AssignSolution(Wolfe wolfe, ref Data data)  // Przypisywanie rozwiązania
        {
            int ctr = 0;
            for (int i = 0; i < data.BoundaryNodeList.Length; i++)
            {
                if (data.BoundaryNodeList[i].BC == "Identify")
                {
                    if (MME.whichBC == "dirichlet")
                    { data.BoundaryNodeList[i].T = wolfe.Solution[ctr++]; }
                    else
                    { data.BoundaryNodeList[i].Q = wolfe.Solution[ctr++]; }
                }
            }
            ctr = 0;
            foreach (Boundary boundary in data.BoundaryList) // Tutaj zamiana warunków brzegowych z Identify na Dirichlet
            {
                foreach (BoundaryElement boundaryElement in boundary)
                {
                    if (boundaryElement.BC == "Identify")
                    {
                        if (MME.whichBC == "dirichlet")
                        { boundaryElement.T = wolfe.Solution[ctr++]; }
                        else
                        { boundaryElement.Q = wolfe.Solution[ctr++]; }
                    }
                }
            }
        }
        private static InternalPointList GetInternalPoints()    // Pętla do wczytywania punktów wewnętrznych
        {
            InternalPointList iPointList = new InternalPointList();
            Console.WriteLine("Wyznaczyć temperatury w pkt. wew. ? (t / n):");
            /*string tn = Console.ReadLine();
            if (tn == "t" || tn == "T")
            { Console.WriteLine("Aby zakończyć, zamiast wsp. podaj \"n\":"); }
            double x1 = 0.0, x2 = 0.0, temp = 0.0;
            while (tn != "n" && tn != "N")
            {
                Console.WriteLine("x1:");
                tn = Console.ReadLine();
                if (tn != "n" && tn != "N")
                { x1 = double.Parse(tn); }
                else
                { break; }
                Console.WriteLine("x2:");
                tn = Console.ReadLine();
                if (tn != "n" && tn != "N")
                { x2 = double.Parse(tn); }
                else
                { break; }
                Console.WriteLine("T:");
                tn = Console.ReadLine();
                if (tn != "n" && tn != "N")
                { temp = double.Parse(tn); }
                iPointList.Add(new InternalPoint(x1, x2, temp));
            }*/
            //iPointList.Add(new InternalPoint(1, 1, 150));
            //iPointList.Add(new InternalPoint(2, 1, 100));
            //iPointList.Add(new InternalPoint(3, 1, 50));
            //iPointList.Add(new InternalPoint(1, 3, 150));
            //iPointList.Add(new InternalPoint(2, 3, 100));
            //iPointList.Add(new InternalPoint(3, 3, 50));
            //iPointList.Add(new InternalPoint(1, 2, 150));
            //iPointList.Add(new InternalPoint(3, 2, 50));
            //iPointList.Add(new InternalPoint(2, 2, 100));
            iPointList.Add(new InternalPoint(1, 1, 68.138944464657598));
            iPointList.Add(new InternalPoint(1, 2, 62.636409610026632));
            iPointList.Add(new InternalPoint(1, 3, 67.126922825109702));
            return iPointList;
        }
    }
}
