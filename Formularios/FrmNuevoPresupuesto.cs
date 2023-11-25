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
            CargarProductos();
            LimpiarCampos();
            lblNroPresupuesto.Text += ProximoPresupuesto().ToString();
        }


        /////////////////////////////////////////////////////////////////////
        //
        // EVENTO IMPORTANTE 1: QUE CARGUE EL COMBO.
        //
        /////////////////////////////////////////////////////////////////////


        private void CargarProductos()
        {
            SqlConnection cnn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=carpinteria_db;Integrated Security=True;");
            cnn.Open();
        
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = "SP_CONSULTAR_PRODUCTOS";
            cmd.CommandType = CommandType.StoredProcedure;

            DataTable tablaProductos = new DataTable();
            tablaProductos.Load(cmd.ExecuteReader());

            cboProductos.DataSource = tablaProductos;
            cboProductos.ValueMember = "id_producto";
            cboProductos.DisplayMember = "n_producto";
            cboProductos.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProductos.SelectedIndex = -1;

            cnn.Close();
        }

        private void LimpiarCampos()
        {
            txtFecha.Text = DateTime.Today.ToString("dd/MM/yy");
            txtCliente.Text = "CONSUMIDOR FINAL";
            txtDescuento.Text = "0";
            txtCantidad.Text = "1";
            this.ActiveControl = txtCliente;
        }

        
        /////////////////////////////////////////////////////////////////////
        //
        // EVENTO IMPORTANTE 2: OBTENER EL NUMERO DEL ULTIMO PRESUPUESTO.
        //
        /////////////////////////////////////////////////////////////////////


        public int ProximoPresupuesto()
        {
            SqlConnection cnn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=carpinteria_db;Integrated Security=True;");
            cnn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = "SP_PROXIMO_ID";
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter param  = new SqlParameter();
            param.ParameterName = "@next";
            param.DbType = DbType.Int32;
            param.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();

            cnn.Close();

            int proximoId;
            proximoId = Convert.ToInt32(param.Value);
            return proximoId;
        }


        private void CalcularTotal()
        {
            double total = nuevo.CalcularTotal();
            txtSubTotal.Text = total.ToString();

            // SI EL DESCUENTO ES DISTINTO DE 0 O VACIO.
            if (txtDescuento.Text != "")
            {
                double dto = (total * Convert.ToInt32(txtDescuento.Text)) / 100;
                txtTotal.Text = (total - dto).ToString();
            }
        }

        /////////////////////////////////////////////////////////////////////
        //
        // EVENTO IMPORTANTE 3: BOTON AGREGAR DETALLLE A LA GRILLA.
        // 1) VALIDACIONES DE BOTON AGREGAR.
        // 2) VALIDACION DE QUE NO SE PUEDA CARGAR DOS VECES LA MISMA CARGA. 
        // 3) AGREGAR DETALLE.
        // 4) BOTON QUITAR DE LA GRILLA.
        //
        /////////////////////////////////////////////////////////////////////


        // 1) VALIDACIONES DE BOTON AGREGAR.
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // VALIDACION QUE EL TXTCLIENTE NO ESTE VACIO.
            if (txtCliente.Text == "")
            {
                MessageBox.Show("Debe indicar el NOMBRE DEL CLIENTE", "CONTROL",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // VALIDACION QUE EL COMBO NO ESTE SIN TEXTO.
            
            if (cboProductos.Text.Equals(string.Empty))
            {
                MessageBox.Show("Debe seleccionar un PRODUCTO", "CONTROL", 
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // VALIDACION QUE EL CAMPO CANTIDAD NO ESTE VACIO O UNA LETRA O CERO
            
            if (txtCantidad.Text == "0"  ||  
                !int.TryParse(txtCantidad.Text, out _) ||
                (txtCantidad.Text == string.Empty))
            {
                MessageBox.Show("Debe ingresar una CANTIDAD válida", "CONTROL", 
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // 2) VALIDACION DE QUE NO SE PUEDA CARGAR DOS VECES LA MISMA CARGA.

            foreach (DataGridViewRow row in dgvDetalles.Rows)
            {
                if (row.Cells["ColProducto"].Value.ToString().Equals(cboProductos.Text))
                {
                    MessageBox.Show("Ese PRODUCTO ya se encuentra cargado en el DETALLE", "CONTROL",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            // 3) AGREGAR DETALLE.
            // Lo seleccionado en el combo, va a ser un item de la fila.
            DataRowView item = (DataRowView)cboProductos.SelectedItem; // Castea el objeto elegido en el como a ITEM
            int cod = Convert.ToInt32(item.Row.ItemArray[0]); // columna 0 en T_Productos es CODIGO
            string nom = item.Row.ItemArray[1].ToString();     // columna 1 en T_productos es NOMBRE
            double pre = Convert.ToDouble(item.Row.ItemArray[2]); // columna 3 en T_producto es PRECIO
           
            // Creo el producto
            Producto p = new Producto(cod, nom, pre);

            // Tengo la cantidad DEK TXTCANTIDAD.TEXT
            int cant = Convert.ToInt32(txtCantidad.Text);

            // Con el producto y la cantidad puedo crear el detalle de presupuesto.
            DetallePresupuesto detalle = new DetallePresupuesto(p, cant);

            nuevo.AgregarDetalle(detalle);

            // Agrego a la DataGridView 
            //dgvDetalles.Rows.Add(new object[] { item.Row.ItemArray[0], item.Row.ItemArray[1], item.Row.ItemArray[2], txtCantidad.Text });

            dgvDetalles.Rows.Add(new object[] { cod, nom, pre, cant });

            CalcularTotal();       
        }


        // 4) BOTON QUITAR DE LA GRILLA.
        // TOCANDO EN LA GRILLA, YENDO EN PROPIEDADES >>> EVENTOS >>> CELL CONTENT CLICK
        private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            {
                if(dgvDetalles.CurrentCell.ColumnIndex == 4)
                {
                    nuevo.QuitarDetalle(dgvDetalles.CurrentRow.Index); // ESTE SERIE EL INDICE
                    // QUITA CON METODO DE PRESUPUESTO QUITAR DETALLE.
                    dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);
                    // REMUEVE DE LA GRILLA.
                    CalcularTotal();
                }
            }
        }

        /////////////////////////////////////////////////////////////////////
        //
        // EVENTO IMPORTANTE 4: BOTON ACEPTAR QUE LLAMA AL METODO GUARDAR MAESTRO.
        // 1) VALIDACIONES DE BOTON ACEPTAR.
        // 2) VALIDACION DE QUE QUE LA GRILLA TENGA AL MENOS DETALLE. 
        // 3) LLAMA AL METODO GUARDAR
        //
        /////////////////////////////////////////////////////////////////////
        

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            //1)
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

            //2) IMPORTANTE: GRILLA CON AL MENOS 1 DETALLE
            if(dgvDetalles.Rows.Count == 0)
            {
                MessageBox.Show("Debe ingresar al menos UN DETALLE", "Control",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            GuardarMaestro();
        }

        

        /////////////////////////////////////////////////////////////////////
        //
        // EVENTO IMPORTANTE 5: METODO CONFIRMAR QUE CONTIENE LA TRANSACCION.
        //
        /////////////////////////////////////////////////////////////////////


        public bool Confirmar()
        {
            bool resultado = true;
            SqlConnection cnn = new SqlConnection();
            SqlTransaction transaccion = null;
            // DEBEN IR AFUERA DEL TRY PORQUE TAMBIEN HAY cnn y transaccion en CATCH Y FINALLY
            

            /////////////////////////////////////////////////////////////////////
            // 
            //  BLOQUE TRY/ CATCH /FINALLY
            //  BOOL RESULTADO ES TRUE.
            //  SI SALE POR TRY SIGUE EN TRUE, HACE EL COMMIT PORQUE ESTA TODO OK.
            //  SI SALE POR CATCH HACE EL ROLLBACK Y RESULTADO ES FALSE.
            //  Y AL FINAL DEBE DEVOLVER RESULTADO.
            //
            /////////////////////////////////////////////////////////////////////

            try
            {
                cnn.ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=carpinteria_db;Integrated Security=True";
                cnn.Open();

                transaccion = cnn.BeginTransaction();

                SqlCommand cmdMaestro = new SqlCommand();
                cmdMaestro.Connection = cnn;
                cmdMaestro.Transaction = transaccion;
                cmdMaestro.CommandText = "SP_INSERTAR_MAESTRO"; // 3 ENTRADA 1 SALIDA
                cmdMaestro.CommandType = CommandType.StoredProcedure;
                
                cmdMaestro.Parameters.AddWithValue("@cliente", nuevo.Cliente);
                cmdMaestro.Parameters.AddWithValue("@dto", nuevo.Descuento);
                cmdMaestro.Parameters.AddWithValue("total", nuevo.CalcularTotal() - nuevo.Descuento);

                // CREO UN PARAMETRO PARA RECIBIR EL PARAMETRO DE SALIDA NRO PRESUPUESTO.
                SqlParameter param = new SqlParameter("@presupuesto_nro", SqlDbType.Int);
                param.Direction = ParameterDirection.Output;
                cmdMaestro.Parameters.Add(param);
                cmdMaestro.ExecuteNonQuery();

                int presupuestoNro = Convert.ToInt32(param.Value); // ALMACENO VALOR PARAM SALIDA


                // PONGO nroDetalle en 1!!!!
                int numeroDetalle = 1;
               
                //RECORRE CADA DETALLE
                foreach(DetallePresupuesto det in nuevo.Detalles)
                {
                    SqlCommand cmdDetalle = new SqlCommand();
                    cmdDetalle.Connection = cnn;
                    cmdDetalle.Transaction = transaccion;
                    cmdDetalle.CommandText = "SP_INSERTAR_DETALLE"; //USA PARAM SALIDA ANTERIOR
                    cmdDetalle.CommandType = CommandType.StoredProcedure;
                    cmdDetalle.Parameters.AddWithValue("@presupuesto_nro", presupuestoNro);
                    cmdDetalle.Parameters.AddWithValue("@detalle", numeroDetalle);
                    cmdDetalle.Parameters.AddWithValue("id_producto", det.Producto.ProductoNro);
                    cmdDetalle.Parameters.AddWithValue("@cantidad",det.Cantidad);
                    cmdDetalle.ExecuteNonQuery();

                    numeroDetalle++;
                }
                // SI ESTA TODO OK, DENTRO DEL TRY QUE HAGA EL COMMIT.
                transaccion.Commit();
            }

            catch (Exception)
            {
                transaccion.Rollback();
                resultado = false;
            }

            finally
            {
                if(cnn != null && cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                } 
                    
            }
            return resultado;
        }


        /////////////////////////////////////////////////////////////////////
        //
        // EVENTO IMPORTANTE 6: METODO GUARDAR MAESTRO con METODO CONFIRMAR.
        //
        /////////////////////////////////////////////////////////////////////

        private void GuardarMaestro()
        {
            nuevo.Fecha = Convert.ToDateTime(txtFecha.Text);
            nuevo.Cliente = txtCliente.Text;
            nuevo.Descuento = Convert.ToDouble(txtDescuento.Text);
            
            if (Confirmar())
            {
                MessageBox.Show("Nuevo PRESUPUESTO Confirmado.", 
                "INFORME",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
            else
            {
                MessageBox.Show("ERROR. No se pudo registrar el presupuesto", 
                "ERROR",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Seguro desea CANCELAR la carga?", "Control",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                this.Dispose();
        }

        /////////////////////////////////////////////////////////////////////
        // Propiedades VISUALES FORMULARIO:
        // StartPosition: CenterParent
        //  Maximize Box: False
        // MinimizeBox: True
        // FormerBorderStyle: FixedSingle
        /////////////////////////////////////////////////////////////////////

    }
}
