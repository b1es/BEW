using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    [Serializable]
    class Boundary : System.Collections.CollectionBase
    {
        #region Pola składowe
        string boundaryType = null;
        private int boundaryNumber = 0;   // Numer brzegu
        #endregion

        #region Konstruktory
        public Boundary()   // Konstruktor bezargumentowy
        { }
        #endregion

        #region Metody
        public void Add(BoundaryElement element)    // Dodaj element
        { List.Add(element); }
        public override string ToString()
        {
            string s = null;
            s += "Boundary no.: " + boundaryNumber.ToString() + "\n";
            foreach (BoundaryElement be in List)
            { s += be.ToString() + "\n"; }
            s += "\n";
            return s;
        }
        #endregion

        #region Mechanizm indeksowania
        public BoundaryElement this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    // Obsługa niepoprawnych indeksów
                }
                return (BoundaryElement)List[index];
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

        #region Właściwości
        public string BoundaryType
        {
            get { return boundaryType; }
            set { boundaryType = value; }
        }
        public int BoundaryNumber
        {
            get { return boundaryNumber; }
            set { boundaryNumber = value; }
        }
        #endregion
    }
}
