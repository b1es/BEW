using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BEW
{
    class Reader
    {
        public static void GetGeometry(string path, ref BoundaryList bList) // Wczytuje geometrię obszaru
        {
            FileInfo theSourceFile = new FileInfo(@path);
            StreamReader reader = theSourceFile.OpenText();
            string text = reader.ReadToEnd();
            reader.Close();
            string[] s = { "\r\n" };
            string[] Lines = text.Split(s, StringSplitOptions.None);

            for (int i = 0; i < Lines.Length; i++)   // Przechodzi przez wszystkie linie tekstu
            {
                if (Lines[i++].IndexOf("# startGeometry") >= 0)
                {
                    while (Lines[i].IndexOf("# endGeometry") < 0)
                    {
                        if (Lines[i].IndexOf("Brzeg") >= 0 && Lines[i].IndexOf("//") == -1)
                        {
                            Boundary boundary = new Boundary();
                            int num = 0;
                            int.TryParse(Lines[i].Substring(Lines[i].IndexOf("Brzeg")+5), out num);
                            boundary.BoundaryNumber = num;
                            i++;
                            List<double> pointList = new List<double>();
                            while (Lines[i].IndexOf("Brzeg") < 0 && Lines[i].IndexOf("# endGeometry") < 0)
                            {
                                MatchCollection theMatches = new Regex(@"\S+").Matches(Lines[i]);
                                if (theMatches.Count == 2 && Lines[i].IndexOf("//") == -1)
                                {
                                    double x1, x2;
                                    Double.TryParse(theMatches[0].ToString(), out x1);
                                    Double.TryParse(theMatches[1].ToString(), out x2);
                                    pointList.Add(x1);
                                    pointList.Add(x2);
                                }
                                i++;
                                if (i >= Lines.Length || Lines[i].IndexOf("# endGeometry") >= 0 || Lines[i].IndexOf("Brzeg") >= 0)
                                {
                                    if (Lines[i].IndexOf("Brzeg") >= 0)
                                    { i--; }
                                    break;
                                }
                            }
                            for (int j = 0; j < pointList.Count - 2; j += 2)
                            {
                                boundary.Add(new BoundaryElement(pointList[j], pointList[j + 1], pointList[j + 2], pointList[j + 3]));
                                boundary[boundary.Count - 1].BoundaryID = boundary.BoundaryNumber;
                            }
                            pointList.Clear();
                            if (boundary.Count != 0)
                            { bList.Add(boundary); }
                        }
                        if (i >= Lines.Length || Lines[i].IndexOf("# endGeometry") >= 0)
                        { break; }
                        i++;
                    }
                }
            }
        }

        public static void GetBoundaryConditions(string path, ref BoundaryList bList)   // Wczytuje warunki brzegowe
        {
            FileInfo thesourceFile = new FileInfo(@path);
            StreamReader reader = thesourceFile.OpenText();
            string text = reader.ReadToEnd();
            reader.Close();
            string[] s = { "\r\n" };
            string[] Lines = text.Split(s, StringSplitOptions.None);

            List<string> nazwa = new List<string>();
            List<double[]> wartosc = new List<double[]>();
            List<List<int>> elementy = new List<List<int>>();

            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i].IndexOf("# startBoundaryConditions") >= 0)
                {
                    i++;
                    while (Lines[i].IndexOf("# endBoundaryConditions") < 0)
                    {
                        if (Lines[i].IndexOf("Dirichlet") >= 0 && Lines[i].IndexOf("//") == -1)
                        {
                            nazwa.Add("Dirichlet");
                            double[] t = new double[1];
                            if (Lines[i].IndexOf("=") >= 0)
                            { t[0] = Double.Parse(Lines[i].Substring(Lines[i].IndexOf("=") + 1)); }
                            wartosc.Add(t);
                            List<int> ele = new List<int>();
                            Regex theReg = new Regex(@"\d+");
                            MatchCollection theMatches = theReg.Matches(Lines[++i]);
                            foreach (Match theMach in theMatches)
                            { ele.Add(int.Parse(theMach.ToString()) - 1); }
                            elementy.Add(ele);
                        }
                        if (Lines[i].IndexOf("Neumann") >= 0 && Lines[i].IndexOf("//") == -1)
                        {
                            nazwa.Add("Neumann");
                            double[] q = new double[1];
                            q[0] = Double.Parse(Lines[i].Substring(Lines[i].IndexOf("=") + 1));
                            wartosc.Add(q);
                            List<int> ele = new List<int>();
                            Regex theReg = new Regex(@"\d+");
                            MatchCollection theMatches = theReg.Matches(Lines[++i]);
                            foreach (Match theMach in theMatches)
                            { ele.Add(int.Parse(theMach.ToString()) - 1); }
                            elementy.Add(ele);
                        }
                        if (Lines[i].IndexOf("Robin") >= 0 && Lines[i].IndexOf("//") == -1)
                        {
                            nazwa.Add("Robin");
                            double[] aTot = new double[2];
                            int tmp1 = Lines[i].IndexOf("=");
                            int tmp2 = Lines[i].IndexOf("T");
                            aTot[0] = Double.Parse(Lines[i].Substring(tmp1 + 1, tmp2 - tmp1 - 2)); // Pobiera wartość wsp. wnikania
                            aTot[1] = Double.Parse(Lines[i].Substring(Lines[i].LastIndexOf("=") + 1)); // Pobiera wartość wsp. wnikania
                            wartosc.Add(aTot);
                            List<int> ele = new List<int>();
                            Regex theReg = new Regex(@"\d+");
                            MatchCollection theMatches = theReg.Matches(Lines[++i]);
                            foreach (Match theMach in theMatches)
                            { ele.Add(int.Parse(theMach.ToString()) - 1); }
                            elementy.Add(ele);
                        }
                        if (++i >= Lines.Length)
                        { break; }
                    }
                }
            }


            int elemNo = 0;
            foreach (Boundary boundary in bList)
            {
                foreach (BoundaryElement bElement in boundary)
                {
                    for (int i = 0; i < elementy.Count; i++ )
                    {
                        List<int> L = elementy[i];
                        for (int j = 0; j < L.Count; j++ )
                        {
                            if (elemNo == L[j])
                            {
                                bElement.BC = nazwa[i];
                                if (nazwa[i] == "Dirichlet")
                                { bElement.T = wartosc[i][0]; }
                                if (nazwa[i] == "Neumann")
                                { bElement.Q = wartosc[i][0]; }
                                if (nazwa[i] == "Robin")
                                {
                                    bElement.A = wartosc[i][0];
                                    bElement.Tot = wartosc[i][1];
                                }
                                break;
                            }
                        }
                    }
                    elemNo++;
                }
            }
        }
        public static void GetInternalPoints(string path, InternalPointList iPointList)   // Wczytuje listę punktów wew. do wyznaczenia w nich temperatur
        {
            FileInfo thesourceFile = new FileInfo(@path);
            StreamReader reader = thesourceFile.OpenText();
            string text = reader.ReadToEnd();
            reader.Close();
            string[] s = { "\r\n" };
            string[] Lines = text.Split(s, StringSplitOptions.None);

            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i].IndexOf("# startInternalTemperatures") >= 0)
                {
                    i++;
                    while (Lines[i].IndexOf("# endInternalTemperatures") < 0)
                    {
                        if (Lines[i].IndexOf("//") == -1)
                        {
                            Regex theReg = new Regex(@"\S+");
                            MatchCollection theMatches = theReg.Matches(Lines[i]);
                            if (theMatches.Count == 2)
                            {
                                double x1, x2;
                                x1 = double.Parse(theMatches[0].ToString());
                                x2 = double.Parse(theMatches[1].ToString());
                                iPointList.Add(new InternalPoint(x1, x2));
                            }
                        }
                        if (++i >= Lines.Length)
                        { break; }
                    }
                }
            }
        }
        public static void GetBoundaryConditionsMME(string path, ref BoundaryList bList)   // Wczytuje warunki brzegowe
        {
            FileInfo thesourceFile = new FileInfo(@path);
            StreamReader reader = thesourceFile.OpenText();
            string text = reader.ReadToEnd();
            reader.Close();
            string[] s = { "\r\n" };
            string[] Lines = text.Split(s, StringSplitOptions.None);

            List<string> nazwa = new List<string>();
            List<double[]> wartosc = new List<double[]>();
            List<List<int>> elementy = new List<List<int>>();

            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i].IndexOf("# startBoundaryConditions") >= 0)
                {
                    i++;
                    while (Lines[i].IndexOf("# endBoundaryConditions") < 0)
                    {
                        if (Lines[i].IndexOf("Dirichlet") >= 0 && Lines[i].IndexOf("//") == -1)
                        {
                            nazwa.Add("Dirichlet");
                            double[] t = new double[1];
                            t[0] = Double.Parse(Lines[i].Substring(Lines[i].IndexOf("=") + 1));
                            wartosc.Add(t);
                            List<int> ele = new List<int>();
                            Regex theReg = new Regex(@"\d+");
                            MatchCollection theMatches = theReg.Matches(Lines[++i]);
                            foreach (Match theMach in theMatches)
                            { ele.Add(int.Parse(theMach.ToString()) - 1); }
                            elementy.Add(ele);
                        }
                        if (Lines[i].IndexOf("Neumann") >= 0 && Lines[i].IndexOf("//") == -1)
                        {
                            nazwa.Add("Neumann");
                            double[] q = new double[1];
                            q[0] = Double.Parse(Lines[i].Substring(Lines[i].IndexOf("=") + 1));
                            wartosc.Add(q);
                            List<int> ele = new List<int>();
                            Regex theReg = new Regex(@"\d+");
                            MatchCollection theMatches = theReg.Matches(Lines[++i]);
                            foreach (Match theMach in theMatches)
                            { ele.Add(int.Parse(theMach.ToString()) - 1); }
                            elementy.Add(ele);
                        }
                        if (Lines[i].IndexOf("Robin") >= 0 && Lines[i].IndexOf("//") == -1)
                        {
                            nazwa.Add("Robin");
                            double[] aTot = new double[2];
                            int tmp1 = Lines[i].IndexOf("=");
                            int tmp2 = Lines[i].IndexOf("T");
                            aTot[0] = Double.Parse(Lines[i].Substring(tmp1 + 1, tmp2 - tmp1 - 2)); // Pobiera wartość wsp. wnikania
                            aTot[1] = Double.Parse(Lines[i].Substring(Lines[i].LastIndexOf("=") + 1)); // Pobiera wartość wsp. wnikania
                            wartosc.Add(aTot);
                            List<int> ele = new List<int>();
                            Regex theReg = new Regex(@"\d+");
                            MatchCollection theMatches = theReg.Matches(Lines[++i]);
                            foreach (Match theMach in theMatches)
                            { ele.Add(int.Parse(theMach.ToString()) - 1); }
                            elementy.Add(ele);
                        }
                        if (Lines[i].IndexOf("Identify") >= 0 && Lines[i].IndexOf("//") == -1)
                        {
                            nazwa.Add("Identify");
                            double[] t = new double[1]; //  Zakładam, że identyfikuje pierwszy warunek brzegowy
                            wartosc.Add(t);
                            List<int> ele = new List<int>();
                            Regex theReg = new Regex(@"\d+");
                            MatchCollection theMatches = theReg.Matches(Lines[++i]);
                            foreach (Match theMach in theMatches)
                            { ele.Add(int.Parse(theMach.ToString()) - 1); }
                            elementy.Add(ele);
                        }
                        if (++i >= Lines.Length)
                        { break; }
                    }
                }
            }


            int elemNo = 0;
            foreach (Boundary boundary in bList)
            {
                foreach (BoundaryElement bElement in boundary)
                {
                    for (int i = 0; i < elementy.Count; i++)
                    {
                        List<int> L = elementy[i];
                        for (int j = 0; j < L.Count; j++)
                        {
                            if (elemNo == L[j])
                            {
                                bElement.BC = nazwa[i];
                                if (nazwa[i] == "Dirichlet")
                                { bElement.T = wartosc[i][0]; }
                                if (nazwa[i] == "Neumann")
                                { bElement.Q = wartosc[i][0]; }
                                if (nazwa[i] == "Robin")
                                {
                                    bElement.A = wartosc[i][0];
                                    bElement.Tot = wartosc[i][1];
                                }
                                break;
                            }
                        }
                    }
                    elemNo++;
                }
            }
        }
        public static void GetInternalTemperaturesMME(string path, InternalPointList iPointList)   // Wczytuje temperatury z sensorów
        {
            FileInfo thesourceFile = new FileInfo(@path);
            StreamReader reader = thesourceFile.OpenText();
            string text = reader.ReadToEnd();
            reader.Close();
            string[] s = { "\r\n" };
            string[] Lines = text.Split(s, StringSplitOptions.None);

            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i].IndexOf("# startInternalTemperatures") >= 0)
                {
                    i++;
                    while (Lines[i].IndexOf("# endInternalTemperatures") < 0)
                    {
                        if (Lines[i].IndexOf("//") == -1)
                        {
                            Regex theReg = new Regex(@"\S+");
                            MatchCollection theMatches = theReg.Matches(Lines[i]);
                            if (theMatches.Count == 3)
                            {
                                double x1, x2, T;
                                x1 = double.Parse(theMatches[0].ToString());
                                x2 = double.Parse(theMatches[1].ToString());
                                T = double.Parse(theMatches[2].ToString());
                                iPointList.Add(new InternalPoint(x1, x2, T));
                            }
                        }
                        if (++i >= Lines.Length)
                        { break; }
                    }
                }
            }
        }

        public static void writeStringToFile(string path, string text)  // Zpisuje stringa do pliku tekstowego
        {
            //FileInfo theSourceFile = new FileInfo("wynik.txt");

            StreamWriter writer = new StreamWriter(@path, false);

            writer.Write(text);

            writer.Close();
        }
    }

    class constValue
    {
        public static Data dataMME = new Data();
        public static Data dataMEB = new Data();
        public static InternalPointList MEBiternalPointList = new InternalPointList();
        public static InternalPointList MMEiternalPointList = new InternalPointList();
    }

    class AuxiliaryFunctions
    {
        public static bool Parallelism(BoundaryElement elem1, BoundaryElement elem2) // Funkcja sprawdzająca równoległość elementów
        {
            // elem1 - element pierwszy
            // elem2 - element drugi
            double[] U = { (elem1.XK[0] - elem1.XP[0]), (elem1.XK[1] - elem1.XP[1]) };
            double[] V = { (elem2.XK[0] - elem2.XP[0]), (elem2.XK[1] - elem2.XP[1]) };
            double J = U[0] * V[1] - U[1] * V[0];
            
            if (J == 0)
            { return true; }
            else
            { return false; }
        }

        public static void binaryDeserialize(out Data data) // Deserializacja z pliku dat
        {
            FileStream fileStream = new FileStream("data.dat", FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            data = (Data)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
        }

        public static double[] MVMultiplication(ref double[,] X, ref double[] Y)    // Mnożenie macierzy przez wektor
        {
            double[] Z = new double[X.Length / Y.Length];
            double sum;
            for (int i = 0; i < X.Length / Y.Length; i++)
            {
                sum = 0.0;
                for (int j = 0; j < Y.Length; j++)
                { sum += X[i, j] * Y[j]; }
                Z[i] = sum;
            }

            return Z;
        }

        public static double[,] MMMultiplication(ref double[,] X, ref double[,] Y, int zRows, int zCols)    // Mnożenie macierzy przez wektor
        {
            double[,] Z = new double[zRows,zCols];
            int m = X.Length / zRows;
            double sum;
            for (int i = 0; i < zRows; i++)
            {
                for (int j = 0; j < zCols; j++)
                {
                    sum = 0.0;
                    for (int k = 0; k < m; k++)
                    { sum += X[i, k] * Y[k, j]; }
                    Z[i,j] = sum;
                }
            }

            return Z;
        }
        public static double[,] TransposeMatrix(ref double[,] M, int Mrows, int MCols)
        {
            double[,] R = new double[MCols, Mrows];
            for (int i = 0; i < Mrows; i++)
            {
                for (int j = 0; j < MCols; j++)
                { R[j, i] = M[i, j]; }
            }
            return R;
        }

        public static string PrintArray(double[,] arr, int rows, int cols)
        {
            string toReturn = null;
            double temp = 0.0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    temp = arr[i, j];
                    toReturn += Math.Round(temp, 4).ToString() + ";";
                }
                toReturn += "\n";
            }
            return toReturn;
        }
        public static string PrintArray(double[] arr)
        {
            string toReturn = null;
            int n = arr.Length;
            double temp = 0.0;
            for (int i = 0; i < n; i++)
            {
                temp = arr[i];
                toReturn += Math.Round(temp, 4).ToString() + "\n";
            }
            return toReturn;
        }
    }
}