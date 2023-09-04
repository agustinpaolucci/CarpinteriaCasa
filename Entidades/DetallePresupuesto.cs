using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpinteriaCasa
{
    public class DetallePresupuesto
    {
        // ATRIBUTOS
        private Producto producto;
        private int cantidad;


        // PROPERTIES
        public Producto Producto { get => producto; set => producto = value; }
        public int Cantidad { get => cantidad; set => cantidad = value; }


        // CONSTRUCTOR CON PARAMETROS
        public DetallePresupuesto(Producto producto, int cantidad)
        {
            Producto = producto;
            Cantidad = cantidad;
        }

        // METODOS DE CONTROL
        public double CalcularSubTotal()
        {
            double subtotal = 0;
            subtotal = Producto.Precio * cantidad;
            return subtotal;
        }
    }
}
