using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarpinteriaCasa
{
    public class Presupuesto
    {
        public int PresupuestoNro { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public double CostoMO { get; set; }
        public double Descuento { get; set; }
        public DateTime fechaBaja { get; set; }
        
        // vamos a tener una lista de DetallePresupuesto.

        public List<DetallePresupuesto> Detalles { get; set; }


        public Presupuesto()
        {
            Detalles = new List<DetallePresupuesto>();
        }

        public void AgregarDetalle(DetallePresupuesto detalle)
        {
            Detalles.Add(detalle);
        }

        public void QuitarDetalle(int indice)
        {
            Detalles.RemoveAt(indice);
        }


        public double CalcularTotal()
        {
            double total = 0;

            for(int i = 0; i < Detalles.Count; i++)
            {
                    total += Detalles[i].CalcularSubTotal();
            }
            return total;
        }
            // Podemos hacerlo con un foreach tambien.
            //foreach (DetallePresupuesto item in Detalles)
            //{
            //    total += item.CalcularSubTotal();
            //}
            //return total;


        public double CalcularTotalConDescuento()
        {
            double final = this.CalcularTotal();
            if (Descuento > 0)
            {
                final -= final * Descuento / 100;
            }
            return final;
        }

    }
}
