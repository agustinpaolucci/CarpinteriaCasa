using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpinteriaCasa
{
    public class Producto
    {
        // ATRIBUTOS
        private int productoNro;
        private string nombre;
        private double precio;
        private bool activo;


        // PROPERTIES
        public int ProductoNro { get => productoNro; set => productoNro = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public double Precio { get => precio; set => precio = value; }
        public bool Activo { get => activo; set => activo = value; }


        // CONSTRUCTOR CON PARAMETROS
        public Producto(int productoNro, string nombre, double precio)
        {
            ProductoNro = productoNro;
            Nombre = nombre;
            Precio = precio;
            activo = true;
            /* NO ponemos el atributo en el constructor y por defecto sale en true. Cuando se dé de baja el 
            producto transformamos el activo = false
            */
        }


        // MOSTRAR DATOS
        public string MostrarDatos()
        {
            return "Producto Nro: " + productoNro + "Nombre:" + nombre + "Precio: $" + precio;
        }


        // TO STRING()
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
