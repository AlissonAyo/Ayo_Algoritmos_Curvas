using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ayo_Alisson_CURVAS.Formularios;

namespace Ayo_Alisson_CURVAS
{
    public partial class CurvasHome : Form
    {
        public CurvasHome()
        {
            InitializeComponent();
        }

        private void bézierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmBezier frmBezier = new FrmBezier();
            frmBezier.MdiParent = this;
            frmBezier.Show();
        }

        private void bSplineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmBSpline frmBSpline = new FrmBSpline();
            frmBSpline.MdiParent = this;
            frmBSpline.Show();
        }
    }
}
