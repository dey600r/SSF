using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using Timer = System.Windows.Threading.DispatcherTimer;

namespace SSF
{
    public partial class vOnline : Window
    {
        MainWindow vPrin;
        FachadaSocket fachada;
        private string Serv_Client = "";
        private string Ip = "";
        private string jug1;
        private string jug2;
        DispatcherTimer t1;
        private int boton = 0;

        public vOnline(MainWindow vp)
        {
            InitializeComponent();
            vPrin = vp;
        }

        public string getJug1()
        {
            return jug1;
        }

        public string getJug2()
        {
            return jug2;
        }

        public string getServClient()
        {
            return this.Serv_Client;
        }
        
        public string getIp()
        {
            return this.Ip;
        }

        public MainWindow getPrincipal()
        {
            return vPrin;
        }


        private void conexionHabilitada()
        {
            t1 = new DispatcherTimer();
            t1.Tick += new EventHandler(tikTimer);
            t1.Interval = TimeSpan.FromMilliseconds(1000);
            t1.Start();
        }

        private void tikTimer(object o, EventArgs e)
        {
            if (fachada.getPersonajeContrincante() != -1)
            {
                // Crear Jugador Contrincante
                jug2 = fachada.getNombreContrincante();
                fachada.setJugador(-1);
                t1.Stop();
            }
        }

        private void btCrear_Click(object sender, RoutedEventArgs e)
        {
            //Creamos la conexion como servidor
            jug1 = textBox1.Text;
            Serv_Client = "Servidor";
            fachada = new FachadaSocket(jug1);
            fachada.CrearServidor(0);
            conexionHabilitada();
            fachada.EnviarTexto("#N " + jug1);//Envio mi nombre para que lo sepa el contrincante
            fachada.setJugador(-1);
            System.Threading.Thread.Sleep(1000);
            jug2 = fachada.getNombreContrincante();
            btCrear.IsEnabled = false;
            btUnirse.IsEnabled = false;
            btSiguiente.Visibility = Visibility.Visible;
            lblMonigote.Content = "Primero debe pulsar el servidor ... ";
            boton = 1;
        }

        private void btUnirse_Click(object sender, RoutedEventArgs e)
        {
            //Me uno a la partida creada por el servidor
            jug1 = textBox1.Text;
            Serv_Client = "Cliente";
            fachada = new FachadaSocket(textBox1.Text, this.textBox2.Text);
            fachada.CrearCliente(0);
            conexionHabilitada();
            fachada.EnviarTexto("#N " + jug1);//Envio mi nombre para que lo sepa el contrincante
            fachada.setJugador(-1);
            Ip = this.textBox2.Text;
            System.Threading.Thread.Sleep(1000); 
            jug2 = fachada.getNombreContrincante();
            btCrear.IsEnabled = false;
            btUnirse.IsEnabled = false;
            btSiguiente.Visibility = Visibility.Visible;
            lblMonigote.Content = "Primero debe pulsar el servidor ... ";
            boton = 1;
        }

        private void btSiguiente_Click(object sender, RoutedEventArgs e)
        {
            //Cerramos la conexion para ir a otra ventana
            System.Threading.Thread.Sleep(1000);
            fachada.CerrarConexion();
            ElegirJugador vElegirJugador = new ElegirJugador(this);
            vElegirJugador.Show();
            this.Close();
        }

        private void btAtras_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            vPrin.Show();
            if (boton == 1) fachada.CerrarConexion(); //Si he conectado cierro la conexion
        }
    }
}
