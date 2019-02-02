using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    [Serializable]
    class InternalPointList : System.Collections.CollectionBase
    {
        #region Metody
        public void Add(InternalPoint point)    // Dodaj element
        { List.Add(point); }
        public override string ToString()
        {
            string s = null;
            foreach (InternalPoint ip in List)
            { s += ip.ToString() + "\n"; }
            s += "\n";
            return s;
        }
        public void InternalTemperaturs(int nb, ElementList elemList, BoundaryNodeList boundaryNodeList, double lam)  // Wyznacza temperaturę w punkcie wewnętrznym
        {
            BoundaryNodeList nodeList = new BoundaryNodeList(this);
            if (BoundaryElement.ElemType == "Constant")
            {
                double[,] Hd = Matrixs.HdMatrix(nb, ref elemList, ref nodeList);
                double[,] G = Matrixs.GdMatrix(nb, ref elemList, ref nodeList, lam);

                int i = 0;
                foreach (InternalPoint ip in this)
                {
                    double sumHT = 0.0, sumGq = 0.0;
                    for (int j = 0; j < boundaryNodeList.Length; j++)
                    {
                        sumHT += (Hd[i, j] * boundaryNodeList[j].T);
                        sumGq += (G[i, j] * boundaryNodeList[j].Q);
                    }
                    this[i++].Temperature = sumHT - sumGq;
                }
            }
            else
            {
                double[,] H = Matrixs.HMatrixForInternalPoints(nb, ref elemList, ref nodeList, ref boundaryNodeList);
                double[,] G = Matrixs.GMatrixForInternalPoints(nb, ref elemList, ref nodeList, ref boundaryNodeList, lam);

                int i = 0;
                foreach (InternalPoint ip in this)
                {
                    double sumHT = 0.0, sumGq = 0.0;
                    for (int j = 0; j < boundaryNodeList.Length; j++)
                    {
                        sumHT += (H[i, j] * boundaryNodeList[j].T);
                        sumGq += (G[i, j] * boundaryNodeList[j].Q);
                    }
                    this[i++].Temperature = sumHT - sumGq;
                }
            }
        }
        #endregion

        #region Mechanizm indeksowania
        public InternalPoint this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    // Obsługa niepoprawnych indeksów
                }
                return (InternalPoint)List[index];
            }
            set
            {
                // Dodawanie elementów możliwe jest tylko poprzez metodę Add
                if (index >= this.Count)
                {
                    // Obsługa błędów
                }
                else
                { List[index] = value; }
            }
        }
        #endregion
    }
}
