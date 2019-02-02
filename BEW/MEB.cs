using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    class MEB
    {
        public static string path = null; // Ścieżka dostępu do pliku z geometrią
        public static int nb = 7; // Ilość punktów Gaussa elementy brzegowe
        public static double lam = 1.0;

        public static Data EvaluationMEB()
        {
            // Utworzenie obiektu data
            Data data = new Data();

            BoundaryList boundaryList = new BoundaryList();
            Reader.GetGeometry(@path, ref boundaryList);    // Odczytywanie listy elementów brzegowych
            Reader.GetBoundaryConditions(@path, ref boundaryList);    // Odczytywanie warunków brzegowych
            BoundaryNodeList bNodeList = new BoundaryNodeList(boundaryList);    // Utworzenie listy węzłów brzegowych
            ElementList elementList = new ElementList(boundaryList);    // Utworzenie listy elementów brzegowych

            double[,] G = Matrixs.GMatrix(nb, ref elementList, ref bNodeList, lam); // Wycznaczenie macierzy G
            double[,] H = Matrixs.HMatrix(nb, ref elementList, ref bNodeList);  // Wycznaczenie macierzy H
            double[,] A = Matrixs.A1Matrix(ref G, ref H, ref bNodeList);    // Wycznaczenie macierzy A
            double[] F = Matrixs.FMatrix(ref G, ref H, ref bNodeList);  // Wycznaczenie wektora F
            double[] Y = GaussianElimination.gaussianElimination(ref A, ref F); // Wyznaczenie rozwiązania
            bNodeList.assignSolution(Y);    // Przypisanie rozwiązania do listy węzłów brzegowych

            data.BoundaryList = boundaryList;
            data.ElementList = elementList;
            data.BoundaryNodeList = bNodeList;
            data.G = G;
            data.H = H;
            data.A1 = A;
            data.F = F;
            data.Y = Y;
            data.binarySerialize(); // Zapis obiektu data do pliku binarnego

            return data;
        }

        public static void EvaluationMEB(Data task)
        {
            BoundaryList boundaryList = task.BoundaryList;
            foreach (Boundary boundary in boundaryList) // Tutaj zamiana warunków brzegowych z Identify na Dirichlet
            {
                foreach (BoundaryElement boundaryElement in boundary)
                {
                    if (boundaryElement.BC == "Identify")
                    {
                        if (MME.whichBC == "dirichlet")
                        { boundaryElement.BC = "Dirichlet"; }
                        else
                        { boundaryElement.BC = "Neumann"; }
                    }
                }
            }
            BoundaryNodeList bNodeList = new BoundaryNodeList(boundaryList);    // Utworzenie listy węzłów brzegowych
            ElementList elementList = new ElementList(boundaryList);    // Utworzenie listy elementów brzegowych

            double[,] G = task.G; // Wycznaczenie macierzy G
            double[,] H = task.H;  // Wycznaczenie macierzy H
            double[,] A = task.A1;    // Wycznaczenie macierzy A
            double[] F = Matrixs.FMatrix(ref G, ref H, ref bNodeList);  // Wycznaczenie wektora F
            double[] Y = GaussianElimination.gaussianElimination(ref A, ref F); // Wyznaczenie rozwiązania

            task.BoundaryNodeList.assignSolution(Y);    // Przypisanie rozwiązania do listy węzłów brzegowych
            task.F = F;
            task.Y = Y;
            task.binarySerialize(); // Zapis obiektu data do pliku binarnego
        }
    }
}
