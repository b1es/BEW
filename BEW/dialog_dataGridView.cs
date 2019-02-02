using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BEW
{
    public partial class dialog_dataGridView : Form
    {
        public dialog_dataGridView(object boundaryNodeList)
        {
            InitializeComponent();
            BoundaryNodeList bNodeList = (BoundaryNodeList)boundaryNodeList;
            dataGridView.ColumnCount = 8;
            dataGridView.Columns[0].Name = "Node no.";
            dataGridView.Columns[1].Name = "ElementID";
            dataGridView.Columns[2].Name = "BoundaryID";
            dataGridView.Columns[3].Name = "Boundary conditions";
            dataGridView.Columns[4].Name = "T";
            dataGridView.Columns[5].Name = "q";
            dataGridView.Columns[6].Name = "Coordinates";
            dataGridView.Columns[7].Name = "NodeType";
            for (int i = 0; i < bNodeList.Length; i++ )
            {
                string nt = null;
                if (bNodeList[i].NodeType == 1)
                { nt = "Singular"; }
                if (bNodeList[i].NodeType == 2)
                { nt = "Dual"; }
                if (bNodeList[i].NodeType == 3)
                { nt = "Singular (p)"; }
                if (bNodeList[i].NodeType == 4)
                { nt = "Internal"; }
                dataGridView.Rows.Add(
                    bNodeList[i].NodeID.ToString(),
                    bNodeList[i].ElemID.ToString(),
                    bNodeList[i].BoundaryID.ToString(),
                    bNodeList[i].BC,
                    bNodeList[i].T.ToString(),
                    bNodeList[i].Q.ToString(),
                    bNodeList[i].Coordinates[0].ToString() + " ; " + bNodeList[i].Coordinates[1].ToString(),
                    nt
                    );
            }
        }
    }
}
