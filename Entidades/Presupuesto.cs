using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarpinteriaCasa
{
    class Presupuesto
    {
        // PROPERTIES
        public int PresupuestoNro { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public double CostoMO { get; set; }
        public double Descuento { get; set; }
        public DateTime FechaBaja { get; set; }
        public List<DetallePresupuesto> Detalles { get; set; }
        // Usamos una lista que es mas piola que una array. Porque es DINAMICA. El array es ESTATICO.
        

        public Presupuesto()
        { 
            Detalles = new List<DetallePresupuesto>(); 
            // inicializo la lista con el constructor!.
        }

        // METODOS DE CONTROL
        public void AgregarDetalle(DetallePresupuesto objetoDetallePresupuesto) //Pasandole por parametro un objeto DetallePresupuesto llamado detalle en este caso.
        {
            Detalles.Add(objetoDetallePresupuesto);
        }

        public void QuitarDetalle(int indice) // el int pasado como parametro indica EL INDICE (POSICION DE LA LISTA) A QUITAR EN LA LISTA!!!!
        {
            Detalles.RemoveAt(indice); // OJO REMOVE AT, NO REMOVE SOLO.
        }

        /*
        USAMOS REMOTE AT porque REMOVE SOLO quita el PRIMER INDICE(POSICION) de la lista. 
        REMOVE AT espera un INT que es el INDICE (INDEX) indicado.
        */

        //CALCULAR TOTAL CON CICLO FOR EACH
        public double CalcularTotal()
        {
            double total = 0; 
            foreach (DetallePresupuesto item in Detalles)
            {
                total = total + item.CalcularSubTotal();
                // idem a: total += item.CalcularSubTotal();
            }
            return total;
        }

        public void Confirmar()
        {

        }



    }
}
