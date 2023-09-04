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
            CargarProductos();
            txtFecha.Text = DateTime.Today.ToShortDateString();
            txtCliente.Text = "CONSUMIDOR FINAL";
            txtDescuento.Text = "0";
            txtCantidad.Text = "1";
            txtSubTotal.Text = "0";
            txtTotal.Text = "0";
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

        private void CargarProductos()
        {
            SqlConnection conexion = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=carpinteria_db;Integrated Security=True");
            SqlCommand comando = new SqlCommand();
            conexion.Open();
            comando.Connection = conexion;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = "SP_CONSULTAR_PRODUCTOS";
            DataTable tabla = new DataTable();
            tabla.Load(comando.ExecuteReader());
            conexion.Close();
            cboProductos.DataSource = tabla;
            cboProductos.ValueMember = "id_producto"; // ojo los nombres, CHEQUEAR
            cboProductos.DisplayMember = "n_producto"; // ojo los nombres, CHEQUEAR
            cboProductos.DropDownStyle = ComboBoxStyle.DropDownList;
        }
    }
}
