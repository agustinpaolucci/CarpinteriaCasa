using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarpinteriaCasa.Formularios
{
    public partial class FrmNuevoPresupuesto : Form
    {
        public FrmNuevoPresupuesto()
        {
            InitializeComponent();
        }

        private void FrmNuevoPresupuesto_Load(object sender, EventArgs e)
        {
            lblNroPresupuesto.Text += ProximoPresupuesto();
        }

       
        public int ProximoPresupuesto()
        {
            SqlConnection conexion = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=carpinteria_db;Integrated Security=True");
            conexion.Open();
            
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = "SP_PROXIMO_ID";

            SqlParameter parametro = new SqlParameter("@next", SqlDbType.Int); //se llama next y tipo INT
            parametro.Direction = ParameterDirection.Output; // direccion: salida
            comando.Parameters.Add(parametro);
            
            comando.ExecuteNonQuery(); 
            conexion.Close();

            return Convert.ToInt32(parametro.Value);
        }
    }
}
