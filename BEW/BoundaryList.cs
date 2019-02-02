using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    [Serializable]
    class BoundaryList : System.Collections.CollectionBase
    {
        #region Pola składowe
        #endregion

        #region Mechanizm indeksowania
        public Boundary this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    // Obsługa niepoprawnych indeksów
                }
                return (Boundary)List[index];
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

        #region Metody
        public void Add(Boundary boundary)    // Dodaj brzeg
        { List.Add(boundary); }
        public override string ToString()
        {
            string s = null;
            foreach (Boundary b in List)
            { s += b.ToString(); }
            return s;
        }
        #endregion
    }

    [Serializable]
    class ElementList
    {
        #region Pola składowe
        private BoundaryElement[] elementList;
        #endregion

        #region Konstruktory
        public ElementList(BoundaryList bList)
        {
            int ctr = 0;
            foreach (Boundary b in bList)
            { ctr += b.Count; }

            elementList = new BoundaryElement[ctr];

            ctr = 0;
            foreach (Boundary b in bList)
            {
                foreach (BoundaryElement be in b)
                { elementList[ctr] = be; ctr++; }
            }
        }
        #endregion

        #region Mechanizm indeksowania
        public BoundaryElement this[int index]
        {
            get
            { return elementList[index]; }
        }
        #endregion

        #region Właściwości
        public int Length
        { get { return elementList.Length; } }
        #endregion

        #region Metody
        public override string ToString()
        {
            string s = null;
            for (int i = 0; i < this.elementList.Length; i++)
            {
                s += this.elementList[i].ToString();
                s += "\n\n";
            }
            return s;
        }
        #endregion
    }
}
