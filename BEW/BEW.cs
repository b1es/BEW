using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BEW
{
    public partial class BEW : Form
    {
        public BEW()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            constValue.MMEiternalPointList.Clear();
            constValue.dataMME = null;
            MME.path = this.openFileDialog1.FileName; // Ścieżka dostępu do pliku z geometrią
            Reader.GetInternalTemperaturesMME(MME.path, constValue.MMEiternalPointList);
            label1.Text = this.openFileDialog1.SafeFileName;
            if (constValue.MMEiternalPointList.Count < 1)
            { this.toolStripStatusLabel1.Text = "Podaj sensory"; }
            else
            {
                this.richTextBox1.Text = constValue.MMEiternalPointList.ToString();
                this.toolStripStatusLabel1.Text = "Podaj kolejny sensor lub oblicz";
            }
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        { this.openFileDialog1.ShowDialog(); }

        private void buttonEvolution_Click(object sender, EventArgs e)
        {
            this.toolStripProgressBar1.Value = 50;
            //this.toolStripProgressBar1.Step = 10;
            //this.toolStripProgressBar1.PerformStep();
            constValue.dataMME = null;
            BoundaryElement.Instances = 0;
            if (constValue.MMEiternalPointList.Count > 0)
            {
                this.richTextBox1.Text = null;
                if (constValue.MMEiternalPointList.Count < 1)
                { this.richTextBox1.Text = "Dodaj węzły wewnętrzne."; }
                else
                {
                    MME.nb = int.Parse(this.comboBoxNb.Text); // Ilość punktów Gaussa elementy brzegowe
                    if (this.textBoxLam.Text.IndexOf(".") >= 0)
                    { this.textBoxLam.Text = this.textBoxLam.Text.Replace(".", ","); }
                    MME.lam = double.Parse(this.textBoxLam.Text);
                    BoundaryElement.ElemType = this.comboBoxElemType.Text;
                    MME.epsilon = double.Parse(this.textBoxEpsilon.Text);
                    int shit;
                    if (int.TryParse(this.textBoxPrecision.Text, out shit))
                    { MME.precision = int.Parse(this.textBoxPrecision.Text); }
                    else
                    { MME.precision = -1; }
                    this.toolStripProgressBar1.Value = 70;
                    constValue.dataMME = MME.EvaluationMME();
                    this.toolStripProgressBar1.Value = 100;
                    if (constValue.dataMME.Error == null)
                    { this.richTextBox1.Text += "Rozwiązanie końcowe:\n" + constValue.dataMME.BoundaryNodeList.ToString(); }
                    else
                    { this.richTextBox1.Text += constValue.dataMME.Error; }
                    this.toolStripProgressBar1.Value = 0;
                    this.button_MMETable.Enabled = true;
                }
            }
        }

        private void buttonInerTemp_Click(object sender, EventArgs e)
        {
            double x1 = 0.0, x2 = 0.0, T = 0.0;
            x1 = double.Parse(textBoxX1.Text);
            x2 = double.Parse(textBoxX2.Text);
            T = double.Parse(textBoxTemp.Text);
            constValue.MMEiternalPointList.Add(new InternalPoint(x1, x2, T));

            /* //Do zadania pd
            constValue.MMEiternalPointList.Add(new InternalPoint(0.667, 2, 66.992));
            constValue.MMEiternalPointList.Add(new InternalPoint(1.333, 2, 82.341));
            constValue.MMEiternalPointList.Add(new InternalPoint(2.000, 2, 87.500));
            constValue.MMEiternalPointList.Add(new InternalPoint(2.667, 2, 82.341));
            constValue.MMEiternalPointList.Add(new InternalPoint(3.333, 2, 66.992));
            */
            /*//Do zadania g3.txt
            constValue.iternalPointList.Add(new InternalPoint(1.0, 0.5, 150.0));
            constValue.iternalPointList.Add(new InternalPoint(1.0, 1.0, 150.0));
            constValue.iternalPointList.Add(new InternalPoint(1.0, 1.5, 150.0));
            constValue.iternalPointList.Add(new InternalPoint(1.0, 2.0, 150.0));
            constValue.iternalPointList.Add(new InternalPoint(1.0, 2.5, 150.0));
            constValue.iternalPointList.Add(new InternalPoint(1.0, 3.0, 150.0));
            constValue.iternalPointList.Add(new InternalPoint(1.0, 3.5, 150.0));
            */
            /* //Do zadania g4.txt
            constValue.iternalPointList.Add(new InternalPoint(1.0, 1.0, 150.0));
            constValue.iternalPointList.Add(new InternalPoint(2.0, 1.0, 100.0));
            constValue.iternalPointList.Add(new InternalPoint(3.0, 1.0, 50.0));
            */
            richTextBox1.Text = constValue.MMEiternalPointList.ToString();
            if (constValue.MMEiternalPointList.Count < 1)
            { this.toolStripStatusLabel1.Text = "Podaj sensory"; }
            else
            { this.toolStripStatusLabel1.Text = "Podaj kolejny sensor lub oblicz"; }
        }

        private void buttonSClear_Click(object sender, EventArgs e)
        {
            if (constValue.MMEiternalPointList.Count > 0)
            { constValue.MMEiternalPointList.RemoveAt(constValue.MMEiternalPointList.Count - 1); }
            richTextBox1.Text = constValue.MMEiternalPointList.ToString();
        }

        private void buttonSShow_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = constValue.MMEiternalPointList.ToString();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            if (constValue.MEBiternalPointList.Count > 0)
            { constValue.MEBiternalPointList.Clear(); }
            MEB.path = this.openFileDialog2.FileName; // Ścieżka dostępu do pliku z geometrią
            Reader.GetInternalPoints(MEB.path, constValue.MEBiternalPointList);
            this.labelMEBpath.Text = this.openFileDialog2.SafeFileName;
            this.toolStripStatusLabel1.Text = "Gotowy";
        }

        private void buttonMEBread_Click(object sender, EventArgs e)
        { this.openFileDialog2.ShowDialog(); }

        private void buttonMEBevolution_Click(object sender, EventArgs e)
        {
            this.toolStripProgressBar1.Step = 10;
            this.toolStripProgressBar1.PerformStep();
            constValue.dataMEB = null;
            BoundaryElement.Instances = 0;
            this.richTextBox2.Text = null;
            MEB.nb = int.Parse(this.comboBoxMEBnb.Text); // Ilość punktów Gaussa elementy brzegowe
            if (this.textBoxMEBlambda.Text.IndexOf(".") >= 0)
            { this.textBoxMEBlambda.Text = this.textBoxMEBlambda.Text.Replace(".", ","); }
            MEB.lam = double.Parse(this.textBoxMEBlambda.Text);
            BoundaryElement.ElemType = this.comboBoxMEBelemType.Text;
            constValue.dataMEB = MEB.EvaluationMEB();
            if (constValue.MEBiternalPointList.Count > 0)
            {
                constValue.MEBiternalPointList.InternalTemperaturs(MEB.nb,
                   constValue.dataMEB.ElementList,
                   constValue.dataMEB.BoundaryNodeList,
                   MEB.lam);
                constValue.dataMEB.IntenralPointList = constValue.MEBiternalPointList;
                this.buttonMEBtextIntTemp.Enabled = true;
            }
            this.toolStripProgressBar1.Step = 100;
            this.toolStripProgressBar1.PerformStep();
            this.richTextBox2.Text += "Rozwiązanie końcowe:\n" + constValue.dataMEB.BoundaryNodeList.ToString();
            this.toolStripProgressBar1.Value = 0;
            this.buttonMEBintTemp.Enabled = true;
            this.labelMEBx2.Enabled = true;
            this.textBoxMEBx2.Enabled = true;
            this.labelMEBx1.Enabled = true;
            this.textBoxMEBx1.Enabled = true;
            this.labelMEBintTem.Enabled = true;
            this.buttonMEBtextSolution.Enabled = true;
            this.button_MEBTable.Enabled = true;
            this.button_MEBMakeTask.Enabled = true;
        }

        private void buttonMEBintTemp_Click(object sender, EventArgs e)
        {
            double x1 = 0.0, x2 = 0.0;
            x1 = double.Parse(this.textBoxMEBx1.Text);
            x2 = double.Parse(this.textBoxMEBx2.Text);
            constValue.MEBiternalPointList.Add(new InternalPoint(x1, x2));

            constValue.MEBiternalPointList.InternalTemperaturs(MEB.nb, 
                                   constValue.dataMEB.ElementList, 
                                   constValue.dataMEB.BoundaryNodeList, MEB.lam);   // Wycznaczenie temperatury w punktach wewnętrznych

            constValue.dataMEB.IntenralPointList = constValue.MEBiternalPointList;
            constValue.dataMEB.binarySerialize(); // Zapis obiektu data do pliku binarnego
            richTextBox2.Text = constValue.dataMEB.IntenralPointList.ToString();
            this.buttonMEBtextIntTemp.Enabled = true;
        }

        private void buttonMEBtextSolution_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = constValue.dataMEB.BoundaryNodeList.ToString();
        }

        private void buttonMEBtextIntTemp_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = constValue.dataMEB.IntenralPointList.ToString();
        }

        private void button_MMETable_Click(object sender, EventArgs e)
        {
            dialog_dataGridView dialogTable = new dialog_dataGridView(constValue.dataMME.BoundaryNodeList);
            dialogTable.Show();
        }

        private void button_MEBTable_Click(object sender, EventArgs e)
        {
            dialog_dataGridView dialogTable = new dialog_dataGridView(constValue.dataMEB.BoundaryNodeList);
            dialogTable.Show();
        }

        private void button_MEBMakeTask_Click(object sender, EventArgs e)
        {
            FileInfo thesourceFile = new FileInfo(MEB.path);
            StreamReader reader = thesourceFile.OpenText();
            string text = reader.ReadToEnd();
            reader.Close();
            string[] s = { "\r\n" };
            string[] Lines = text.Split(s, StringSplitOptions.None);
            int index = text.IndexOf("# endBoundaryConditions");
            string newText = text.Substring(0, index);
            newText += "# endBoundaryConditions\r\n\r\n# startInternalTemperatures\r\n//x1\tx2\tT\r\n";
            foreach(InternalPoint ip in constValue.dataMEB.IntenralPointList)
            { newText += ip.x1.ToString() + "\t" + ip.x2.ToString() + "\t" + ip.Temperature.ToString() + "\r\n"; }
            newText += "\n# endInternalTemperatures";
            dialog_MEBChangeBC dialogWin = new dialog_MEBChangeBC(ref newText);
            if (dialogWin.ShowDialog() == DialogResult.OK)
            {
                newText = dialogWin.FileText;
                newText = newText.Replace("\n", "\r\n");
                this.saveFileDialog_MEBMakeTask.ShowDialog();
                string path = this.saveFileDialog_MEBMakeTask.FileName;
                FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fileStream);
                writer.Write(newText);
                writer.Close();
                fileStream.Close();
            }
        }

        private void button_MMETable1_Click(object sender, EventArgs e)
        {
            dialog_dataGridView dialogTable = new dialog_dataGridView(new BoundaryNodeList(constValue.MEBiternalPointList));
            dialogTable.Show();
        }
    }
}
