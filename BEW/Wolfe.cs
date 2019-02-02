using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BEW
{
    class Wolfe
    {
        #region Pola składowe
        private int n;  // Ilość zmiennych bazowych
        private int q;  // Ilość ograniczeń
        private double[,] m;   // Współczynniki lewej strony ograniczeń zadania zastępczego
        private double[] b;    // Współczynniki prawej strony ograniczeń zadania zastępczego
        private string[] z;    // Wektor zmiennych
        private string[] bVariable;  // Wektor zmiennych bazowych
        private double[] bValues;    // Wektor wartości bazowych
        private double[] fc;    // Wektor funkcji celu
        private double[] ej;
        private double[] solution;
        private string error = null;
        #endregion

        #region Konstruktory
        public Wolfe(SubstituteTask sTask)
        {
            n = sTask.N;
            q = sTask.Q;
            m = sTask.M;
            b = sTask.B;
            z = sTask.Z;
            ej = new double[z.Length];
            bVariable = sTask.bVariable;
            bValues = sTask.bValues;
            this.fc = sTask.FC;
            SetEj();
        }
        #endregion

        #region Metody
        private void SetEj()
        {
            for (int i = 0; i < z.Length; i++)
            {
                double sum = 0.0;
                for (int j = 0; j < q+n; j++)
                { sum += bValues[j] * m[j, i]; }
                ej[i] = fc[i] - sum;
            }
        }
        private bool Optimum()
        {
            bool temp = true;
            for (int i = 0; i < ej.Length; i++)
            {
                if (ej[i] < 0.0)
                { return false; }
            }
            return temp;
        }
        private string ExitCriterion(string entryVariable)  // Kryterium wyjścia
        {
            int ctr = 0;
            for (int i = 0; i < fc.Length; i++)
            {
                if (z[i] == entryVariable)
                {
                    ctr = i;
                    break;
                }
            }
            List<double> list = new List<double>();
            List<int> list1 = new List<int>();
            for (int j = 0; j < q + n; j++)
            {
                if (m[j, ctr] > 0)
                {
                    list.Add(b[j] / m[j, ctr]);
                    list1.Add(j);
                }
            }
            if (list.Count == 0)
            { return "break"; }
            double min = list[0];
            ctr = list1[0];
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j] < min)
                {
                    min = list[j];
                    ctr = list1[j];
                }
            }
            return bVariable[ctr];
        }
        private string EntryCriterion() // Kryterium wejścia
        {
            List<double> e = new List<double>();
            List<string> zj = new List<string>();
            for (int i = 0; i < ej.Length; i++)
            { zj.Add(z[i]); }
        poczatek:
            e.Clear();
            for (int i = 0; i < z.Length; i++)
            {
                for(int k = 0; k < zj.Count; k++)
                {
                    if (z[i] == zj[k])
                    { e.Add(ej[i]); }
                }
            }
            double min = e[0];
            string str = zj[0];
            int ctr = 0;
            for (int i = 1; i < e.Count; i++)
            {
                if (e[i] < min)
                {
                    min = e[i];
                    str = zj[i];
                    ctr = i;
                }
            }
            string komp = null;
            Match theMatch = new Regex(@"\d").Match(str);
            if (str.IndexOf("x") >= 0 && str.IndexOf("'") < 0)
            {
                int no = int.Parse(theMatch.ToString());
                komp = "y" + no + "'";
            }
            if (str.IndexOf("x") >= 0 && str.IndexOf("'") >= 0)
            {
                int no = int.Parse(theMatch.ToString());
                komp = "y" + no;
            }
            if (str.IndexOf("y") >= 0 && str.IndexOf("'") < 0)
            {
                int no = int.Parse(theMatch.ToString());
                komp = "x" + no + "'";
            }
            if (str.IndexOf("y") >= 0 && str.IndexOf("'") >= 0)
            {
                int no = int.Parse(theMatch.ToString());
                komp = "x" + no;
            }
            for (int j = 0; j < q + n; j++)
            {
                if (komp == bVariable[j])
                {
                    if (ExitCriterion(str) != komp)
                    {
                        zj.Remove(str);
                        goto poczatek;
                    }
                }
            }
            return str;
        }
        private void ChangeBase(string entryVariable, string exitVariable)
        {
            double temp = 0;
            int baseCtr = 0;
            int mCtr = 0;
            for (int i = 0; i < z.Length; i++)
            {
                if (z[i] == entryVariable)
                {
                    baseCtr = i;
                    temp = fc[i];
                    break;
                }
            }
            for (int j = 0; j < n + q; j++)
            {
                if (bVariable[j] == exitVariable)
                {
                    mCtr = j;
                    bVariable[j] = entryVariable;
                    bValues[j] = temp;
                    break;
                }
            }
            double div = m[mCtr, baseCtr];   // Dzielnik
            for (int k = 0; k < z.Length; k++)    // Jedynka na przekątnej
            { m[mCtr, k] /= div; }
            b[mCtr] /= div;
            for (int l = 0; l < q+n; l++)  // Zera w kolumnie
            {
                if (l != mCtr && m[l, baseCtr] != 0.0)
                {
                    double qt = m[l, baseCtr];
                    for (int k = 0; k < z.Length; k++)  // Sprawdź dla k = l
                    { m[l, k] -= m[mCtr, k] * qt; }
                    b[l] -= b[mCtr] * qt;
                }
            }
        }
        public void Evaluation()
        {
            string entryVariable = null;
            string exitVariable = null;
            while (!Optimum())
            {
                entryVariable = EntryCriterion();
                exitVariable = ExitCriterion(entryVariable);
                if (entryVariable == exitVariable)
                {
                    error = "Zadanie jest sprzeczne";
                    break;
                }
                if (exitVariable == "break")
                {
                    error = "Nieograniczony zbiór rozwiązań dopuszczalnych w meotdzie Wolfe'a";
                    break;
                }
                ChangeBase(entryVariable, exitVariable);
                SetEj();
            }
            if (error == null)
            {
                solution = new double[n];
                for (int k = 0; k < n; k++)
                {
                    solution[k] = 0.0;
                    string str = "x" + (k + 1).ToString();
                    for (int i = 0; i < n + q; i++)
                    {
                        if (bVariable[i] == str)
                        {
                            solution[k] = b[i];
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Właściwości
        public double[,] M // Współczynniki lewej strony ograniczeń zadania zastępczego
        {
            get { return m; }
            set { m = value; }
        }
        public double[] B  // Współczynniki prawej strony ograniczeń zadania zastępczego
        {
            get { return b; }
            set { b = value; }
        }
        public string[] Z  // Wektor zmiennych
        {
            get { return z; }
            set { z = value; }
        }
        public string[] baseVariable  // Wektor zmiennych bazowych
        {
            get { return bVariable; }
            set { bVariable = value; }
        }
        public double[] baseValues // Wektor wartości bazowych
        {
            get { return bValues; }
            set { bValues = value; }
        }
        public double[] FC // Wektor funkcji celu
        {
            get { return fc; }
            set { fc = value; }
        }
        public double[] Solution    // Rozwiązanie
        { get { return solution; } }
        public string Error
        { get { return error; } }
        #endregion
    }
}
