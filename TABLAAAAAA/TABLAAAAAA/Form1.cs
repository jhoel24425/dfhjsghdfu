using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.ActiveDirectory;
using System.Text.RegularExpressions;

namespace TABLAAAAAA
{
    public partial class Form1 : Form
    {
        int turno = 0;
        bool movExtra = false;
        PictureBox seleccionado = null;

        List<PictureBox> azules = new List<PictureBox>();
        List<PictureBox> rojas = new List<PictureBox>();

        public Form1()
        {
            InitializeComponent();
            cargarLista();
        }

        private void cargarLista()
        {
            azules.Add(azul1);
            azules.Add(azul2);
            azules.Add(azul3);
            azules.Add(azul4);
            azules.Add(azul5);
            azules.Add(azul6);
            azules.Add(azul7);
            azules.Add(azul8);
            azules.Add(azul9);
            azules.Add(azul10);
            azules.Add(azul11);
            azules.Add(azul12);

            rojas.Add(roja1);
            rojas.Add(roja2);
            rojas.Add(roja3);
            rojas.Add(roja4);
            rojas.Add(roja5);
            rojas.Add(roja6);
            rojas.Add(roja7);
            rojas.Add(roja8);
            rojas.Add(roja9);
            rojas.Add(roja10);
            rojas.Add(roja11);
            rojas.Add(roja12);
        }


        public void seleccion(object objeto)
        {
            if (!movExtra)
            {
                if (seleccionado != null)
                {

                    seleccionado.BackColor = Color.Black;

                }
                seleccionado = (PictureBox)objeto; seleccionado.BackColor = Color.Lime;

            }
        }


        private void cuadroClick(object sender, MouseEventArgs e)
        {
            movimiento((PictureBox)sender);
        }

        private void movimiento(PictureBox cuadro)
        {
            if (seleccionado != null)
            {
                string color = seleccionado.Name.Substring(0, 4); // "roja" o "azul"
                Point origen = seleccionado.Location;
                Point destino = cuadro.Location;

                Console.WriteLine($"Movimiento iniciado: Origen: {origen}, Destino: {destino}");

                // Verificación de si el movimiento es simple
                if (validacion(seleccionado, cuadro, color))
                {
                    seleccionado.Location = destino;
                    ifqueen(color); // Verificar si se convierte en reina
                    turno++;
                    seleccionado.BackColor = Color.Black;
                    seleccionado = null;
                    movExtra = false;
                }
                // Verificación de si el movimiento implica captura
                else if (validacionCaptura(seleccionado, cuadro, color))
                {
                    capturarPieza(origen, destino, color); // Captura de pieza enemiga
                    seleccionado.Location = destino;
                    ifqueen(color); // Verificar si se convierte en reina
                    turno++;
                    seleccionado.BackColor = Color.Black;
                    seleccionado = null;
                    movExtra = false;
                }
                else
                {
                    Console.WriteLine("Movimiento inválido.");
                    MessageBox.Show("Movimiento inválido.");
                }
            }
        }

        private bool movimientosExtras(string color)
        {
            List<PictureBox> bandoContrario = color == "roja" ? azules : rojas;
            List<Point> posiciones = new List<Point>();
            int sigPosicion = color == "roja" ? -100 : 100;

            posiciones.Add(new Point(seleccionado.Location.X + 100, seleccionado.Location.Y + sigPosicion));
            posiciones.Add(new Point(seleccionado.Location.X - 100, seleccionado.Location.Y + sigPosicion));
            if (seleccionado.Tag == "queen")
            {
                posiciones.Add(new Point(seleccionado.Location.X + 100, seleccionado.Location.Y - sigPosicion));
                posiciones.Add(new Point(seleccionado.Location.X - 100, seleccionado.Location.Y - sigPosicion));
            }

            bool resultado = false;
            for (int i = 0; i < posiciones.Count; i++)
            {
                if (posiciones[i].X >= 50 && posiciones[i].X <= 400 && posiciones[i].Y >= 50 && posiciones[i].Y <= 400)
                {
                    if (!ocupado(posiciones[i], rojas) && !ocupado(posiciones[i], azules))
                    {

                        Point puntoMedio = new Point(promedio(posiciones[i].X, seleccionado.Location.X), promedio(posiciones[i].Y, seleccionado.Location.Y));
                        if (ocupado(puntoMedio, bandoContrario))
                        {
                            resultado = true;
                        }


                    }
                }
            }
            return resultado;


        }

        private bool ocupado(Point punto, List<PictureBox> bando)
        {
            for (int i = 0; i < bando.Count; i++)
            {
                if (punto == bando[i].Location)
                {
                    return true;
                }
            }
            return false;
        }

        private int promedio(int n1, int n2)
        {
            int resultado = n1 + n2;
            resultado = resultado / 2;
            return resultado;
        }

        private bool validacion(PictureBox origen, PictureBox destino, string color)
        {
            Point puntoDestino = destino.Location;

            // Verificar límites del tablero
            if (puntoDestino.X < 50 || puntoDestino.X > 400 || puntoDestino.Y < 50 || puntoDestino.Y > 400)
            {
                return false; // Movimiento fuera de los límites
            }

            // Verificar si el destino está ocupado
            if (ocupado(puntoDestino, rojas) || ocupado(puntoDestino, azules))
            {
                return false; // Hay una pieza en el destino
            }

            return true; // Movimiento válido (simple)
        }

        private bool validacionCaptura(PictureBox origen, PictureBox destino, string color)
        {
            Point puntoOrigen = origen.Location;
            Point puntoDestino = destino.Location;

            // Calcular diferencia en Y para asegurarnos que el movimiento es de dos casillas
            int avance = color == "roja" ? puntoOrigen.Y - puntoDestino.Y : puntoDestino.Y - puntoOrigen.Y;
            int diferenciaX = Math.Abs(puntoOrigen.X - puntoDestino.X); // Diferencia en X

            Console.WriteLine($"Avance: {avance}, DiferenciaX: {diferenciaX}");

            // Verificamos si el movimiento es de dos casillas en diagonal (y=100, x=100)
            if (Math.Abs(avance) == 100 && diferenciaX == 100)
            {
                // Calcular el punto medio entre las dos piezas
                Point puntoMedio = new Point(promedio(puntoDestino.X, puntoOrigen.X), promedio(puntoDestino.Y, puntoOrigen.Y));
                Console.WriteLine($"Punto medio calculado: {puntoMedio}");

                // Determinar el bando contrario
                List<PictureBox> bandoContrario = color == "roja" ? azules : rojas;

                // Buscar si hay una pieza enemiga en el punto medio
                PictureBox piezaCapturada = null;
                foreach (var ficha in bandoContrario)
                {
                    if (ficha.Location == puntoMedio)
                    {
                        piezaCapturada = ficha;
                        break;
                    }
                }

                if (piezaCapturada != null)
                {
                    Console.WriteLine($"¡Pieza capturada en {puntoMedio}! ({piezaCapturada.Name})");

                    // Capturar la pieza: moverla fuera de pantalla y hacerla invisible
                    piezaCapturada.Location = new Point(-50, -50);
                    piezaCapturada.Visible = false;

                    // Eliminarla de la lista de piezas del bando contrario
                    bandoContrario.Remove(piezaCapturada);
                    Console.WriteLine("Pieza capturada y eliminada correctamente.");
                    return true; // Captura exitosa
                }
                else
                {
                    Console.WriteLine("No se encontró pieza para capturar.");
                }
            }
            return false;  // No se puede capturar
        }


        private void capturarPieza(Point origen, Point destino, string color)
        {
            // Calcular el punto medio entre las dos piezas
            Point puntoMedio = new Point(promedio(destino.X, origen.X), promedio(destino.Y, origen.Y));
            Console.WriteLine($"Intentando capturar en punto medio: {puntoMedio}");

            // Determinar el bando contrario
            List<PictureBox> bandoContrario = color == "roja" ? azules : rojas;

            // Buscar la pieza enemiga en el punto medio
            PictureBox piezaCapturada = bandoContrario.FirstOrDefault(ficha => ficha.Location == puntoMedio);

            if (piezaCapturada != null)
            {
                Console.WriteLine($"¡Pieza capturada en {puntoMedio}: {piezaCapturada.Name}");

                // Eliminar la pieza capturada: moverla fuera de pantalla
                piezaCapturada.Location = new Point(-50, -50);  // Mover fuera de la vista
                piezaCapturada.Visible = false;                  // Hacerla invisible
                bandoContrario.Remove(piezaCapturada);           // Eliminarla de la lista del bando contrario
            }
            else
            {
                Console.WriteLine("No se encontró pieza para capturar.");
            }
        }

        private void ifqueen(string color)
        {
            if (color == "azul" && seleccionado.Location.Y == 369)
            {
                seleccionado.BackgroundImage = Properties.Resources.azul_corona;
                seleccionado.Tag = "queen";

            }
            else if (color == "roja" && seleccionado.Location.Y == 50)
            {
                seleccionado.BackgroundImage = Properties.Resources.Proyecto_nuevo;
                seleccionado.Tag = "queen";

            }
        }


        private void seleccionRoja(object sender, MouseEventArgs e)
        {
            if (turno % 2 == 0)
            {

                seleccion(sender);
            }
            else
            {
                MessageBox.Show("turno del equipo azul");
            }
        }


        private void seleccionAzul(object sender, MouseEventArgs e)
        {
            if (turno % 2 == 1)
            {

                seleccion(sender);
            }
            else
            {
                MessageBox.Show("turno del equipo rojo");
            }

        }

       
    }
}
