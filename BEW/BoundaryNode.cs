using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    [Serializable]
    class BoundaryNode
    {
        #region Pola składowe
        private int nodeID; // Numer węzła;
        private int elemID; // Numer elementu, do którego należy węzeł
        private int boundaryID; // Numer brzegu, do którego należy węzeł
        private string whichBC = null;  // Rodzaj założonego warunku brzegowego
        private double t = 0.0, q = 0.0, a = 0.0, tot = 0.0;    // Temperatura, strumień ciepła, współczynnik wnikania, temp. otoczenia
        private double[] node;    // Tablica z wsp. węzła
        private int nodeType;
        // nodeType = 1 - węzeł pojedynczy
        // nodeType = 2 - węzeł podwójny
        // nodeType = 3 - węzeł węzeł środkowy elementu kwadratowego
        // nodeType = 4 - węzeł wewnętrzny
        #endregion

        #region Konstruktory
        public BoundaryNode(double[] nodeCoordinate, int nType, int ID) // Konstruktor do tworzenia węzłów wewnętrznych
        {
            this.nodeID = ID;
            this.nodeType = nType;
            this.node = nodeCoordinate;
        }
        public BoundaryNode(BoundaryElement elem, double[] nodeCoordinate, int nType, int ID)   // Konstruktor do tworzenia węzłów brzegowych
        {
            nodeID = ID;
            nodeType = nType;
            this.elemID = elem.ElemID;
            this.boundaryID = elem.BoundaryID;
            this.whichBC = elem.BC;
            this.node = nodeCoordinate;
            if (whichBC == "Dirichlet")
            { t = elem.T; }
            if (whichBC == "Neumann")
            { q = elem.Q; }
            if (whichBC == "Robin")
            { a = elem.A; tot = elem.Tot; }
        }
        #endregion

        #region Metody
        public override string ToString()
        {
            string s = null;

            s += ("Node: " + (nodeID + 1).ToString() + "\n");
            s += ("\tElementID: " + (elemID + 1).ToString() + "\n");
            s += ("\tBoundaryID: " + boundaryID.ToString() + "\n");
            s += ("\tBoundary conditions: " + whichBC + "\n");
            s += ("\tT = " + t.ToString() + "\n");
            s += ("\tq = " + q.ToString() + "\n");
            if (whichBC == "Robin")
            {
                s += ("\talpha = " + a.ToString() + "\n");
                s += ("\tTot = " + tot.ToString() + "\n");
            }
            s += ("\tCoordinates = [" + node[0].ToString() + " ; " + node[1].ToString() + "]\n");
            if (nodeType == 2)
            { s += ("\tNodeType: dual\n"); }
            else
            { s += ("\tNodeType: singular\n"); }

            return s;
        }
        #endregion

        #region Właściwości
        public int NodeID   // Numer węzeła
        {
            get { return nodeID; }
            set { nodeID = value; }
        }
        public int ElemID   // Numer elementu brzegowego do którego należy węzeł
        {
            get { return elemID; }
            set { elemID = value; }
        }
        public int BoundaryID   // Numer brzegu do którego należy węzeł
        {
            get { return boundaryID; }
            set { boundaryID = value; }
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
        public double[] Coordinates  // Tablica z współrzędnymi węzła
        {
            get { return node; }
            set { node = value; }
        }
        public int NodeType // Rodzaj elementu (podwójny czy pojedynczy)
        { get { return nodeType; } }
        #endregion
    }
}
