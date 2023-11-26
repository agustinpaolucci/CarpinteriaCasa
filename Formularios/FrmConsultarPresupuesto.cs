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
    public partial class FrmConsultarPresupuesto : Form
    {
        public FrmConsultarPresupuesto()
        {
            InitializeComponent();
        }

        private void FrmConsultarPresupuesto_Load(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("¿Seguro que desea cerra la pantalla","CERRAR",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes) 
            {
                this.Close();            
            }
        }
    }
}
