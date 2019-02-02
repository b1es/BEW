using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BEW
{
    [Serializable]
    class Data
    {
        #region Pola składowe
        private string error;
        private BoundaryList boundaryList;
        private ElementList elementList;
        private BoundaryNodeList bNodeList;
        private InternalPointList iPointList;
        private double[,] g;
        private double[,] gw;
        private double[,] h;
        private double[,] hw;
        private double[,] a1;
        private double[,] a2;
        private double[] f;
        private double[] y;
        private double[] x;
        private double[,] b;
        private double[,] u;
        private double[,] r;
        private double[,] dw;
        private double[,] dw1;
        private double[,] w;
        private double[] p;
        private double[] e;
        private double[] z;
        private double[] fd;
        private double[] j;
        private double[] s;
        private double[] pd;
        private double[,] c;
        #endregion

        #region Konstruktory
        public Data()
        { }
        public Data(ref BoundaryList bl, ref ElementList el, ref BoundaryNodeList bnl, ref InternalPointList ipl, ref double[,] Garr,
            ref double[,] Harr, ref double[,] A1arr, ref double[,] A2arr, ref double[] Farr, ref double[] Yarr, ref double[] Xarr)
        {
            this.boundaryList = bl;
            this.elementList = el;
            this.bNodeList = bnl;
            this.iPointList = ipl;
            this.g = Garr;
            this.h = Harr;
            this.a1 = A1arr;
            this.a2 = A2arr;
            this.f = Farr;
            this.y = Yarr;
            this.x = Xarr;
            if (!GaussJordanElimination.gaussJordanElimination(A1, out this.b))
            { throw new System.Exception("Macierz A1 jest nieodwracalna!!!"); }
            else
            { u = AuxiliaryFunctions.MMMultiplication(ref b, ref a2, (int)Math.Sqrt(b.Length), (int)Math.Sqrt(b.Length)); }
        }
        #endregion

        #region Metody
        public void binarySerialize()    // Serializacja do pliku dat
        {
            FileStream fileStream = new FileStream("data.dat", FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, this);
            fileStream.Close();
        }
        #endregion

        #region Właściwości
        public string Error
        {
            get { return error; }
            set { error = value; }
        }
        public BoundaryList BoundaryList   // Lista brzegów
        {
            get { return boundaryList; }
            set { boundaryList = value; }
        }
        public ElementList ElementList // Lista elementów brzegowych
        {
            get { return elementList; }
            set { elementList = value; }
        }
        public BoundaryNodeList BoundaryNodeList   // Lista węzłów brzegowych
        {
            get { return bNodeList; }
            set { bNodeList = value; }
        }
        public InternalPointList IntenralPointList // Lista punktów wewnętrznych z wyznaczoną temperaturą
        {
            get { return iPointList; }
            set { iPointList = value; }
        }
        public double[,] G
        {
            get { return g; }
            set { g = value; }
        }
        public double[,] Gw
        {
            get { return gw; }
            set { gw = value; }
        }
        public double[,] H
        {
            get { return h; }
            set { h = value; }
        }
        public double[,] Hw
        {
            get { return hw; }
            set { hw = value; }
        }
        public double[,] A1
        {
            get { return a1; }
            set
            {
                a1 = value;
                //if (!GaussJordanElimination.gaussJordanElimination(value, out this.b))
                //{ throw new System.Exception("Macierz A1 jest nieodwracalna!!!"); }
            }
        }
        public double[,] A2
        {
            get { return a2; }
            set
            {
                a2 = value;
                if (b != null)
                { u = AuxiliaryFunctions.MMMultiplication(ref b, ref a2, (int)Math.Sqrt(b.Length), (int)Math.Sqrt(b.Length)); }
            }
        }
        public double[] F
        {
            get { return f; }
            set { f = value; }
        }
        public double[] Y
        {
            get { return y; }
            set { y = value; }
        }
        public double[] X
        {
            get { return x; }
            set { x = value; }
        }
        public double[,] B
        {
            get { return b; }
            set { b = value; }
        }
        public double[,] U
        {
            get { return u; }
            set { u = value; }
        }
        public double[,] R
        {
            get { return r; }
            set { r = value; }
        }
        public double[,] Dw
        {
            get { return dw; }
            set { dw = value; }
        }
        public double[,] Dw1
        {
            get { return dw1; }
            set { dw1 = value; }
        }
        public double[,] W
        {
            get { return w; }
            set { w = value; }
        }
        public double[] P
        {
            get { return p; }
            set { p = value; }
        }
        public double[] E
        {
            get { return e; }
            set { e = value; }
        }
        public double[] Z
        {
            get { return z; }
            set { z = value; }
        }
        public double[] Fd
        {
            get { return fd; }
            set { fd = value; }
        }
        public double[] S
        {
            get { return s; }
            set { s = value; }
        }
        public double[] J
        {
            get { return j; }
            set { j = value; }
        }
        public double[] Pd
        {
            get { return pd; }
            set { pd = value; }
        }
        public double[,] C
        {
            get { return c; }
            set { c = value; }
        }
        #endregion
    }
}
