using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    [Serializable]
    class BoundaryNodeList
    {
        private BoundaryNode[] nList;

        #region Konstruktory
        public BoundaryNodeList()   // Konstruktor
        { }

        public BoundaryNodeList(BoundaryNode node)  // Konstruktor do tworzenia listy jednoelementowej
        {
            nList = new BoundaryNode[1];
            nList[0] = node;
        }

        public BoundaryNodeList(BoundaryList boundaryList)   // Konstruktor
        {
            switch (BoundaryElement.ElemType)
            {
                case "Constant":
                    {
                        int ctr = 0;
                        foreach (Boundary boundary in boundaryList)
                        { ctr += boundary.Count; }

                        nList = new BoundaryNode[ctr];

                        ctr = 0;
                        foreach (Boundary boundary in boundaryList)
                        {
                            foreach (BoundaryElement bElement in boundary)
                            { nList[ctr] = new BoundaryNode(bElement, bElement.Node[0], 1, ctr++); }
                        }
                        break;
                    }
                case "Linear":
                    {
                        int R = 0;
                        foreach (Boundary boundary in boundaryList)
                        {
                            if ((!AuxiliaryFunctions.Parallelism(boundary[0], boundary[boundary.Count - 1]))
                            && (boundary[0].BC != boundary[boundary.Count - 1].BC))
                            { R += 2; }
                            else
                            { R++; }
                            for (int i = 1; i < boundary.Count; i++)
                            {
                                if ((!AuxiliaryFunctions.Parallelism(boundary[i], boundary[i - 1])) && (boundary[i].BC != boundary[i - 1].BC))
                                { R += 2; }
                                else
                                { R++; }
                            }
                        }

                        nList = new BoundaryNode[R];
                        int ctr = 0;
                        foreach (Boundary boundary in boundaryList)
                        {
                            bool isFirstDouble = false;
                            if ((!AuxiliaryFunctions.Parallelism(boundary[0], boundary[boundary.Count - 1]))
                            && (boundary[0].BC != boundary[boundary.Count - 1].BC))
                            {
                                nList[ctr] = new BoundaryNode(boundary[0], boundary[0].XP, 2, ctr);
                                isFirstDouble = true;
                            }
                            else
                            { nList[ctr] = new BoundaryNode(boundary[0], boundary[0].XP, 1, ctr); }
                            ctr++;
                            for (int i = 1; i < boundary.Count; i++)
                            {
                                if ((!AuxiliaryFunctions.Parallelism(boundary[i], boundary[i - 1])) && (boundary[i].BC != boundary[i - 1].BC))
                                {
                                    nList[ctr] = new BoundaryNode(boundary[i - 1], boundary[i - 1].XK, 2, ctr);
                                    ctr++;
                                    nList[ctr] = new BoundaryNode(boundary[i], boundary[i].XP, 2, ctr);
                                }
                                else
                                { nList[ctr] = new BoundaryNode(boundary[i], boundary[i].XP, 1, ctr); }
                                ctr++;
                            }
                            if (isFirstDouble)
                            { nList[ctr++] = new BoundaryNode(boundary[boundary.Count - 1], boundary[boundary.Count - 1].XK, 2, ctr - 1); }
                        }
                        break;
                    }
                case "Parabolic":
                    {
                        int R = 0;
                        foreach (Boundary boundary in boundaryList)
                        {
                            if ((!AuxiliaryFunctions.Parallelism(boundary[0], boundary[boundary.Count - 1]))
                            && (boundary[0].BC != boundary[boundary.Count - 1].BC))
                            { R += 2; }
                            else
                            { R++; }
                            R++;
                            for (int i = 1; i < boundary.Count; i++)
                            {
                                if ((!AuxiliaryFunctions.Parallelism(boundary[i], boundary[i - 1])) && (boundary[i].BC != boundary[i - 1].BC))
                                { R += 2; }
                                else
                                { R++; }
                                R++;
                            }
                        }

                        nList = new BoundaryNode[R];

                        int ctr = 0;
                        foreach (Boundary boundary in boundaryList)
                        {
                            bool isFirstDouble = false;
                            if ((!AuxiliaryFunctions.Parallelism(boundary[0], boundary[boundary.Count - 1]))
                            && (boundary[0].BC != boundary[boundary.Count - 1].BC))
                            {
                                nList[ctr] = new BoundaryNode(boundary[0], boundary[0].XP, 2, ctr);
                                isFirstDouble = true;
                            }
                            else
                            { nList[ctr] = new BoundaryNode(boundary[0], boundary[0].XP, 1, ctr); }
                            ctr++;
                            nList[ctr] = new BoundaryNode(boundary[0], boundary[0].Node[1], 3, ctr);
                            ctr++;
                            for (int i = 1; i < boundary.Count; i++)
                            {
                                if ((!AuxiliaryFunctions.Parallelism(boundary[i], boundary[i - 1])) && (boundary[i].BC != boundary[i - 1].BC))
                                {
                                    nList[ctr] = new BoundaryNode(boundary[i - 1], boundary[i - 1].XK, 2, ctr);
                                    ctr++;
                                    nList[ctr] = new BoundaryNode(boundary[i], boundary[i].XP, 2, ctr);
                                }
                                else
                                { nList[ctr] = new BoundaryNode(boundary[i], boundary[i].XP, 1, ctr); }
                                ctr++;
                                nList[ctr] = new BoundaryNode(boundary[i], boundary[i].Node[1], 3, ctr);
                                ctr++;
                            }
                            if (isFirstDouble)
                            { nList[ctr++] = new BoundaryNode(boundary[boundary.Count - 1], boundary[boundary.Count - 1].XK, 2, ctr - 1); }
                        }
                        break;
                    }
                default:
                    throw new System.Exception("Niepoprawny rodzaj elementu w konstruktorze NodeList!!!");
            }
        }

        public BoundaryNodeList(InternalPointList ipList)   // Konstruktor dla punktów wewnętrznych
        {
            int ctr = ipList.Count;

            nList = new BoundaryNode[ctr];

            ctr = 0;
            foreach (InternalPoint ip in ipList)
            { nList[ctr] = new BoundaryNode(ip.Coordinates, 4, ctr++); }
        }
        #endregion

        #region Mechanizm indeksowania
        public BoundaryNode this[int index]
        {
            get
            { return nList[index]; }
        }
        #endregion

        #region Właściwości
        public int Length
        { get { return nList.Length; } }
        #endregion

        #region Metody
        public void assignSolution(double[] YArray) // Przypisywanie rozwiązania do listy węzłów
        {
            for (int i = 0; i < YArray.Length; i++)
            {
                if (this.nList[i].BC == "Dirichlet" || this.nList[i].BC == "Identify")
                { this.nList[i].Q = YArray[i]; }
                else
                {
                    if (this.nList[i].BC == "Neumann")
                    { this.nList[i].T = YArray[i]; }
                    else
                    {
                        if (this.nList[i].BC == "Robin")
                        {
                            this.nList[i].T = YArray[i];
                            this.nList[i].Q = this.nList[i].A * (YArray[i] - this.nList[i].Tot);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            string s = null;
            for (int i = 0; i < this.nList.Length; i++)
            {
                s += this.nList[i].ToString();
                s += "\n\n";
            }
            return s;
        }
        #endregion
    }
}
