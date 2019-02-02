using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    [Serializable]
    class BoundaryElement
    {
        #region Pola składowe
        private static int instances = 0;   // Statyczne pole składowe określające ilość wystąpień obiektów typu BoundaryElement w programie
        private int elemID; // Numer elementu
        private int boundaryID; // Numer brzegu
        private double[] xp = new double[2];    // Wsp. początku elementu
        private double[] xk = new double[2];    // Wsp. końca elementu
        private string whichBC = null;  // Rodzaj założonego warunku brzegowego
        private double t = 0.0, q = 0.0, a = 0.0, tot = 0.0;    // Temperatura, strumień ciepła, współczynnik wnikania, temp. otoczenia
        private static string elemType = null;   // Rodzaj elementu brzegowego
        private double[][] node;    // Tablica z wsp. węzłów należących do elementu brzegowego
        #endregion

        #region Konsruktory
        public BoundaryElement()    // Konstruktor domyślny
        { instances++; }
        public BoundaryElement(double[] xp, double[] xk)    // Konstruktor
        {
            elemID = instances;
            instances++;
            this.xp = xp;
            this.xk = xk;
            AssignNode();
        }
        public BoundaryElement(double xp1, double xp2, double xk1, double xk2)  // Konstruktor
        {
            elemID = instances;
            instances++;
            this.xp[0] = xp1;
            this.xp[1] = xp2;
            this.xk[0] = xk1;
            this.xk[1] = xk2;
            AssignNode();
        }
        #endregion

        #region Metody i obsługa interfejsu
        private void AssignNode()   // Tworzy tablicę z węzłami należącymi do elementu
        {
            if (elemType == "Constant")
            {
                node = new double[1][];
                node[0] = new double[2];
                node[0][0] = (xp[0] + xk[0]) / 2;
                node[0][1] = (xp[1] + xk[1]) / 2;
            }
            if (elemType == "Linear")
            {
                node = new double[2][];
                node[0] = new double[2];
                node[1] = new double[2];
                node[0][0] = xp[0];
                node[0][1] = xp[1];
                node[1][0] = xk[0];
                node[1][1] = xk[1];
            }
            if (elemType == "Parabolic")
            {
                node = new double[3][];
                node[0] = new double[2];
                node[1] = new double[2];
                node[2] = new double[2];
                node[0][0] = xp[0];
                node[0][1] = xp[1];
                node[1][0] = (xp[0] + xk[0]) / 2;
                node[1][1] = (xp[1] + xk[1]) / 2;
                node[2][0] = xk[0];
                node[2][1] = xk[1];
            }
        }
        public override string ToString()
        {
            string s = null;
            s += "Element " + (elemID + 1).ToString() + ":\n";
            s += "\tBoundaryID: " + boundaryID.ToString();
            s += "\n\txp = [ " + xp[0].ToString() + " ; " + xp[1].ToString() + " ]\txk = [ ";
            s += xk[0].ToString() + " ; " + xk[1].ToString() + " ]\t\n";
            s += "\tBoundary conditions: " + whichBC;
            s += "\n\tT = " + t.ToString();
            s += ";\tq = " + q.ToString() + ";";
            if (whichBC == "Robin")
            {
                s += ("\talpha = " + a.ToString() + ";\t");
                s += ("Tot = " + tot.ToString() + ";\n");
            }
            return s;
        }
        #endregion

        #region Właściwości
        public static int Instances // Ilość obiektów Element w programie
        {
            get { return instances; }
            set { instances = value; }
        }
        public static string ElemType   // Rodzaje przyjętego elementu brzegowego
        {
            get { return elemType; }
            set { elemType = value; }
        }
        public int ElemID   // Numer elementu brzegowego
        {
            get { return elemID; }
            set { elemID = value; }
        }
        public int BoundaryID   // Numer brzegu
        {
            get { return boundaryID; }
            set { boundaryID = value; }
        }
        public double[] XP  // Wsp. początku elementu brzegowego
        {
            get { return xp; }
            set { xp = value; }
        }
        public double[] XK  // Wsp. końca elementu brzegowego
        {
            get { return xk; }
            set { xk = value; }
        }
        public string BC    // Warunek brzegowy założony na elemencie
        {
            get { return whichBC; }
            set { whichBC = value; }
        }
        public double T // Temperatura WB I 
        {
            get { return t; }
            set { t = value; }
        }
        public double Q // Strumień ciepła WB II
        {
            get { return q; }
            set { q = value; }
        }
        public double A // Współczynnik wnikania WB III
        {
            get { return a; }
            set { a = value; }
        }
        public double Tot   // Temp. otoczenia WB III
        {
            get { return tot; }
            set { tot = value; }
        }
        public double[][] Node  // Tablica z współrzędnymi węzłów należacymi do elementu
        {
            get { return node; }
            set { node = value; }
        }
        #endregion
    }
}
