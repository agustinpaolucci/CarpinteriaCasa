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

        private Presupuesto nuevo;

        public FrmNuevoPresupuesto()
        {
            InitializeComponent();
            nuevo = new Presupuesto();
        }


        private void FrmNuevoPresupuesto_Load(object sender, EventArgs e)
        {
            lblNroPresupuesto.Text += ProximoPresupuesto();
            CargarProductos();
            txtFecha.Text = DateTime.Today.ToString("dd/MM/yy");
            txtCliente.Text = "CONSUMIDOR FINAL";
            txtDescuento.Text = "0";
            txtCantidad.Text = "1";
        }

       
        public int ProximoPresupuesto()
        {
            SqlConnection conexion = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=carpinteria_db;Integrated Security=True");
            conexion.Open();
            
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = "SP_PROXIMO_ID";

            SqlParameter parametro = new SqlParameter("@next", SqlDbType.Int); // le paso nombre del parametro y tipo 
            parametro.Direction = ParameterDirection.Output; 
            
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

        private void CalcularTotal()
        {
            double total = nuevo.CalcularTotal();
            txtSubTotal.Text = total.ToString();

            if (txtDescuento.Text != "")
            {
                double dto = (total * Convert.ToInt32(txtDescuento.Text)) / 100;
                txtTotal.Text = (total - dto).ToString();
            }
        }

        // AGREGAR A LA GRILLA
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // SI COMBO VACIO
            if(cboProductos.Text.Equals(string.Empty))
            {
                MessageBox.Show("Debe seleccionar un PRODUCTO", "CONTROL", 
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // SI CANTIDAD VACIA O UNA LETRA
            if (txtCantidad.Text == "" ||  !int.TryParse(txtCantidad.Text, out _))
            {
                MessageBox.Show("Debe ingresar una CANTIDAD válida", "CONTROL", 
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtCliente.Text == "")
            {
                MessageBox.Show("Debe indicar el NOMBRE DEL CLIENTE", "CONTROL",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // VALIDACION DE QUE NO SE PUEDA CARGAR DOS VECES LA MISMA CARGA.

            foreach (DataGridViewRow row in dgvDetalles.Rows)
            {
                if (row.Cells["ColProducto"].Value.ToString().Equals(cboProductos.Text))
                {
                    MessageBox.Show("Ese PRODUCTO ya se encuentra cargado en el DETALLE", "CONTROL",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }


            // El item seleccionado del combo, va a ser un item de la fila.
            DataRowView item = (DataRowView)cboProductos.SelectedItem;
            int prod = Convert.ToInt32(item.Row.ItemArray[0]); // id
            string nom = item.Row.ItemArray[1].ToString();     // nombre
            double pre = Convert.ToDouble(item.Row.ItemArray[2]); // precio
           
            // Creo el producto
            Producto p = new Producto(prod, nom, pre);

            // Tengo la cantidad
            int cant = Convert.ToInt32(txtCantidad.Text); // cantidad

            // Con el producto y la cantidad puedo crear el detalle de presupuesto.

            DetallePresupuesto detalle = new DetallePresupuesto(p, cant);


            nuevo.AgregarDetalle(detalle);

            // Agrego a la DataGridView 
            //dgvDetalles.Rows.Add(new object[] { item.Row.ItemArray[0], item.Row.ItemArray[1], item.Row.ItemArray[2], txtCantidad.Text });

            dgvDetalles.Rows.Add(new object[] { prod, nom, pre, cant });

            CalcularTotal();       
        }

        // BOTON QUITAR DE LA GRILLA.
        //TOCANDO EN LA GRILLA, YENDO EN PROPIEDADES >>> CELL CONTENT CLICK
        private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            {
                if(dgvDetalles.CurrentCell.ColumnIndex == 4)
                {
                    nuevo.QuitarDetalle(dgvDetalles.CurrentRow.Index);
                    //Quite del presupuesto
                    dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);
                    //Remueve de la grilla
                    CalcularTotal();
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("¿Seguro desea CANCELAR la carga?", "Control",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            this.Dispose();
        }


        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                MessageBox.Show("Debe ingresar un CLIENTE", "Control",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDescuento.Text))
            {
                MessageBox.Show("Debe ingresar un valor en DESCUENTO", "Control",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if(dgvDetalles.Rows.Count == 0)
            {
                MessageBox.Show("Debe ingresar al menos UN DETALLE", "Control",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            GuardarPresupuesto();
        }


        // METODO GUARDAR PRESUPUESTO.
        private void GuardarPresupuesto()
        {
            nuevo.Cliente = txtCliente.Text;
            nuevo.Descuento = Convert.ToDouble(txtDescuento.Text);
            nuevo.Fecha = Convert.ToDateTime(txtFecha.Text);

            if (Confirmar())
            {
                MessageBox.Show("Nuevo PRESUPUESTO Confirmado", "Informe",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
            else
            {
                MessageBox.Show("ERROR. No se pudo registrar el presupuesto", "ERROR",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool Confirmar()
        {
            bool resultado = true;
            SqlConnection conexion = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=carpinteria_db;Integrated Security=True");

            try
            {
                conexion.Open();
                SqlCommand comandoMaestro = new SqlCommand();
                comandoMaestro.Connection = conexion;
                comandoMaestro.CommandType = CommandType.StoredProcedure;
                comandoMaestro.CommandText = "SP_INSERTAR_MAESTRO";
                comandoMaestro.Parameters.AddWithValue("@cliente", nuevo.Cliente);
                comandoMaestro.Parameters.AddWithValue("@dto", nuevo.Descuento);
                comandoMaestro.Parameters.AddWithValue("total", nuevo.CalcularTotal() - nuevo.Descuento);

                // CREO UN PARAMETRO PARA RECIBIR EL PARAMETRO DE SALIDA NRO PRESUPUESTO.
                SqlParameter parametro = new SqlParameter("@presupuesto_nro", SqlDbType.Int);
                parametro.Direction = ParameterDirection.Output;
                comandoMaestro.Parameters.Add(parametro);
                comandoMaestro.ExecuteNonQuery();

                int presupuestoNro = Convert.ToInt32(parametro.Value);

                int nroDetalle = 1;
                
                foreach(DetallePresupuesto det in nuevo.Detalles)
                {
                    SqlCommand comandoDetalle = new SqlCommand();
                    comandoDetalle.Connection = conexion;
                    comandoDetalle.CommandType = CommandType.StoredProcedure;
                    comandoDetalle.CommandText = "SP_INSERTAR_DETALLE";
                    comandoDetalle.Parameters.AddWithValue("@presupuesto_nro", presupuestoNro);
                    comandoDetalle.Parameters.AddWithValue("@detalle", nroDetalle);
                    comandoDetalle.Parameters.AddWithValue("id_producto", det.Producto.ProductoNro);
                    comandoDetalle.Parameters.AddWithValue("@cantidad",det.Cantidad);
                    comandoDetalle.ExecuteNonQuery();

                    nroDetalle++;
                }
            }
            catch (Exception)
            {
                resultado = false;
            }
            finally
            {
                if(conexion != null && conexion.State == ConnectionState.Open)
                {

                } 
                  conexion.Close();  
            }
            return resultado;
        }
    }
}
