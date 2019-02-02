using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEW
{
    [Serializable]
    class InternalPoint
    {
        #region Pola składowe
        double[] coordinates = new double[2];
        double temperature;
        #endregion

        #region Konstruktory
        public InternalPoint(double x1, double x2)
        {
            coordinates[0] = x1;
            coordinates[1] = x2;
        }
        public InternalPoint(double x1, double x2, double temp)
        {
            coordinates[0] = x1;
            coordinates[1] = x2;
            temperature = temp;
        }
        public InternalPoint(double[] x)
        { coordinates = x; }
        #endregion

        #region Metody
        public override string ToString()
        { return ("T(" + x1.ToString() + "," + x2.ToString() + ") = " + temperature.ToString()); }
        #endregion

        #region Właściwości
        public double[] Coordinates
        {
            get { return coordinates; }
            set { coordinates = value; }
        }
        public double x1
        {
            get { return coordinates[0]; }
            set { coordinates[0] = value; }
        }
        public double x2
        {
            get { return coordinates[1]; }
            set { coordinates[1] = value; }
        }
        public double Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }
        #endregion
    }
}
