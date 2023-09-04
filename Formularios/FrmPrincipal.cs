using CarpinteriaCasa.Formularios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarpinteriaCasa.Formularios
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmNuevoPresupuesto nuevo = new FrmNuevoPresupuesto();
            nuevo.ShowDialog();
            // a diferencia de nuevo.SHOW SOLO, SHOWDIALOG no te deja volver atras.
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
