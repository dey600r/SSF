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
using Timer = System.Windows.Threading.DispatcherTimer;
using System.Windows.Threading;
using System.Threading;

namespace SSF
{
    public partial class ElegirJugador : Window
    {
        //Imagenes de los personajes
        private string blanka = "Images/blanka.gif";
        private string cammy = "Images/cammy.gif";
        private string sagat = "Images/sagat.gif";
        private string dhalsim = "Images/dhalsim.gif";
        private string adonn = "Images/adonn.png";
        private string ken = "Images/ken.gif";
        private string ryu = "Images/ryu.gif";
        private string zangief = "Images/zangief.gif";
        //Variables para saber si se ha introducido correctamente el jugador
        private bool jug1 = false;
        private bool jug2 = false;
        private Jugador jugador1;
        private Jugador jugador2;
        //Ventanas necesarias para empezar el juego
        MainWindow vPrin;
        vOnline online;
        FachadaSocket fach;
        Juego escenario;
        private string Personaje1;
        private string Personaje2;
        //Timer para esperar la contestacion del contrincante
        DispatcherTimer t1;

        public ElegirJugador(MainWindow vp)
        {
            InitializeComponent();
            vPrin = vp;
            textNick2.IsEnabled = false;
        }

        public MainWindow getPrincipal()
        {
            return vPrin;
        }

        public ElegirJugador(vOnline line)
        {
            InitializeComponent();
            online = line;
            vPrin = line.getPrincipal();
            lblEligeLuchador.Content = "Elige tu jugador ...";
            textNick1.Text = online.getJug1();
            if (online.getServClient() == "Servidor")
            {
                fach = new FachadaSocket(online.getJug1());
                fach.CrearServidor(1); //Creamos conexion en otro puerto
            }
            else
            {
                System.Threading.Thread.Sleep(1000);
                fach = new FachadaSocket(online.getJug1(), online.getIp());
                fach.CrearCliente(1); //Unimos a la conexion en el segundo puerto
            }
            textNick2.Text = online.getJug2();
            textNick1.IsEnabled = false;
            textNick2.IsEnabled = false;
        }

        public vOnline getvOnline()
        {
            return online;
        }

        private void cargarImagen1(string ruta)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@ruta, UriKind.Relative);
            bi.EndInit();
            this.retrato1.Source = bi;
        }
        private void cargarImagen2(string ruta)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@ruta, UriKind.Relative);
            bi.EndInit();
            this.retrato2.Source = bi;
        }

        private void controlarError(string imagen, int fuerza, int defensa, int agilidad, string personaje)
        {
            if (jug1 == false && jug2 == false) //Si has elegido el jugador1 y no has introducido nombre
            {
                lblFallo1.Content = "X";
                lblEligeLuchador.Content = "Introduce el nombre...";
            }
            if (jug1 == true && textNick2.Text == "") //Si has elegido el jugador2 y no has introducido nombre
            {
                lblFallo2.Content = "X";
                lblEligeLuchador.Content = "Introduce el nombre...";
            }
            if (textNick1.Text != "" && jug1 == false) //Fijamos jugador 1
            {
                lblFallo1.Content = "";
                lblFallo2.Content = "";
                cargarImagen1(imagen);
                lblFuerza1.Content = fuerza;
                lblDefensa1.Content = defensa;
                lblAgilidad1.Content = agilidad;
                textNick1.IsEnabled = false;
                textNick2.IsEnabled = true;
                Personaje1 = personaje;
                jug1 = true;
            }
            if (jug1 == true && textNick2.Text != "") 
            {
                if (jug1 == true && textNick2.Text.Equals(textNick1.Text)) //Si son nombres iguales
                {
                    lblFallo2.Content = "X";
                    lblEligeLuchador.Content = "Introduce nombre diferente...";
                }
                else //Fijamos jugador 2
                {
                    lblFallo1.Content = "";
                    lblFallo2.Content = "";
                    cargarImagen2(imagen);
                    lblFuerza2.Content = fuerza;
                    lblDefensa2.Content = defensa;
                    lblAgilidad2.Content = agilidad;
                    textNick2.IsEnabled = false;
                    Personaje2 = personaje;
                    jug2 = true;
                }
            }
            
            if (jug1 == true && jug2 == true) //Deshabilitamos cuando ya han elegido los dos jugadores
            {
                btBlanka.IsEnabled = false;
                btCammy.IsEnabled = false;
                btKen.IsEnabled = false;
                btRyu.IsEnabled = false;
                btZangief.IsEnabled = false;
                btChunLi.IsEnabled = false;
                btDalshim.IsEnabled = false;
                btGuile.IsEnabled = false;
                btJugar.Visibility = Visibility.Visible;
            }
        }

        private void mirarCaracteristicas(string imagen, int fuerza, int defensa, int agilidad)
        {
            //Mostramos en pantalla las caracteristicas del jugador donde esta el cursor
            if (jug1 == false) 
            {
                lblFuerza1.Content = fuerza;
                lblDefensa1.Content = defensa;
                lblAgilidad1.Content = agilidad;
                cargarImagen1(imagen);
            }
            else
            {
                lblFuerza2.Content = fuerza;
                lblDefensa2.Content = defensa;
                lblAgilidad2.Content = agilidad;
                cargarImagen2(imagen);
            }
        }

        private void elegirPersonaje(string imagen, int fuerza, int defensa, int agilidad, string nomPers, int pers)
        {
            //Eleccion del personaje en modo Red
            Personaje1 = nomPers;
            conexionHabilitada(); //Esperamos a que el contrincante elija su jugador
            fach.EnviarTexto("#R " + pers); //Enviamos el jugador que he elegido
            cargarImagen1(imagen);
            lblFuerza2.Content = fuerza;
            lblDefensa2.Content = defensa;
            lblAgilidad2.Content = agilidad;
            btBlanka.IsEnabled = false;
            btCammy.IsEnabled = false;
            btKen.IsEnabled = false;
            btRyu.IsEnabled = false;
            btZangief.IsEnabled = false;
            btChunLi.IsEnabled = false;
            btDalshim.IsEnabled = false;
            btGuile.IsEnabled = false;
            while (fach.getPersonajeContrincante() == -1) { }
            Thread.Sleep(1000); 
            jug1 = true;
            jug2 = true;
            btJugar.Visibility = Visibility.Visible;
            lblEligeLuchador.Content = "Primero debe pulsar el servidor ... ";
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
            if (fach.getPersonajeContrincante() != -1) //Cuando nos envie el jugador el contrincante fijamos los datos
            {
                switch (fach.getPersonajeContrincante())
                {
                    case 0: Personaje2 = "Blanka"; cargarImagen2(blanka); lblFuerza2.Content = 8; lblDefensa2.Content = 6; lblAgilidad2.Content = 5; break;
                    case 1: Personaje2 = "Cammy"; cargarImagen2(cammy); lblFuerza2.Content = 4; lblDefensa2.Content = 7; lblAgilidad2.Content = 7; break;
                    case 2: Personaje2 = "Ken"; cargarImagen2(ken); lblFuerza2.Content = 7; lblDefensa2.Content = 5; lblAgilidad2.Content = 5; break;
                    case 3: Personaje2 = "Ryu"; cargarImagen2(ryu); lblFuerza2.Content = 6; lblDefensa2.Content = 7; lblAgilidad2.Content = 6; break;
                    case 4: Personaje2 = "Zangief"; cargarImagen2(zangief); lblFuerza2.Content = 9; lblDefensa2.Content = 3; lblAgilidad2.Content = 4; break;
                    case 5: Personaje2 = "Sagat"; cargarImagen2(sagat); lblFuerza2.Content = 5; lblDefensa2.Content = 9; lblAgilidad2.Content = 4; break;
                    case 6: Personaje2 = "Dhalsim"; cargarImagen2(dhalsim); lblFuerza2.Content = 6; lblDefensa2.Content = 8; lblAgilidad2.Content = 5; break;
                    case 7: Personaje2 = "Adon"; cargarImagen2(adonn); lblFuerza2.Content = 8; lblDefensa2.Content = 2; lblAgilidad2.Content = 5; break;
                }
            }
        }

        private void btBlanka_Click(object sender, RoutedEventArgs e)
        {
            if (online == null) controlarError(blanka, 8, 6, 5, "Blanka"); //Si la partida es local
            else elegirPersonaje(blanka, 8, 6, 5, "Blanka", 0); //Si la partida es online
        }

        private void btCammy_Click(object sender, RoutedEventArgs e)
        {
            if (online == null) controlarError(cammy, 4, 7, 7, "Cammy");
            else elegirPersonaje(cammy, 4, 7, 7, "Cammy", 1);
        }

        private void btKen_Click(object sender, RoutedEventArgs e)
        {
            if (online == null) controlarError(ken, 7, 5, 5, "Ken");
            else elegirPersonaje(ken, 7, 5, 5, "Ken", 2);
        }

        private void btRyu_Click(object sender, RoutedEventArgs e)
        {
            if (online == null) controlarError(ryu, 6, 7, 6, "Ryu");
            else elegirPersonaje(ryu, 6, 7, 6, "Ryu", 3);
        }

        private void btZangief_Click(object sender, RoutedEventArgs e)
        {
            if (online == null) controlarError(zangief, 9, 3, 4, "Zangief");
            else elegirPersonaje(zangief, 9, 3, 4, "Zangief", 4);
        }

        private void btChunLi_Click(object sender, RoutedEventArgs e)
        {
            if (online == null) controlarError(sagat, 5, 9, 4, "Sagat");
            else elegirPersonaje(sagat, 5, 9, 4, "Sagat", 5);
        }

        private void btDalshim_Click(object sender, RoutedEventArgs e)
        {
            if (online == null) controlarError(dhalsim, 6, 8, 5, "Dhalsim");
            else elegirPersonaje(dhalsim, 6, 8, 5, "Dhalsim", 6);
        }

        private void btGuile_Click(object sender, RoutedEventArgs e)
        {
            if (online == null) controlarError(adonn, 8, 2, 5, "Adon");
            else elegirPersonaje(adonn, 8, 2, 5, "Adon", 7);
        }

        private void btBlanka_MouseMove(object sender, MouseEventArgs e)
        {
            mirarCaracteristicas(blanka, 8, 6, 5);
        }

        private void btCammy_MouseMove(object sender, MouseEventArgs e)
        {
            mirarCaracteristicas(cammy, 4, 7, 7);
            
        }

        private void btKen_MouseMove(object sender, MouseEventArgs e)
        {
            mirarCaracteristicas(ken, 7, 5, 5);
        }

        private void btRyu_MouseMove(object sender, MouseEventArgs e)
        {
            mirarCaracteristicas(ryu, 6, 7, 6);
        }

        private void btZangief_MouseMove(object sender, MouseEventArgs e)
        {
            mirarCaracteristicas(zangief, 9, 3, 4);
        }

        private void btChunLi_MouseMove(object sender, MouseEventArgs e)
        {
            mirarCaracteristicas(sagat, 5, 9, 4);
        }

        private void btDalshim_MouseMove(object sender, MouseEventArgs e)
        {
            mirarCaracteristicas(dhalsim, 6, 8, 5); 
        }

        private void btGuile_MouseMove(object sender, MouseEventArgs e)
        {
            mirarCaracteristicas(adonn, 8, 2, 5);
        }

        private void btJugar_Click(object sender, RoutedEventArgs e)
        {
            if (jug1 == true && jug2 == true) //Si estan bien introducidos los jugadores
            {
                //Cogemos los datos fijados para crear los dos jugadores
                int fuerza1 = (int)lblFuerza1.Content.ToString().First() - 48;
                int defensa1 = (int)lblDefensa1.Content.ToString().First() - 48;
                int agilidad1 = (int)lblAgilidad1.Content.ToString().First() - 48;
                int fuerza2 = (int)lblFuerza2.Content.ToString().First() - 48;
                int defensa2 = (int)lblDefensa2.Content.ToString().First() - 48;
                int agilidad2 = (int)lblAgilidad2.Content.ToString().First() - 48;
                jugador1 = new Jugador(textNick1.Text, retrato1, 1, fuerza1, defensa1, agilidad1, Personaje1);
                jugador2 = new Jugador(textNick2.Text, retrato2, 2, fuerza2, defensa2, agilidad2, Personaje2);
 
                if (online != null) //Parte online
                {
                    //Cerramos la conexion para la tercera conexion en la siguiente ventana
                    System.Threading.Thread.Sleep(1000);
                    fach.CerrarConexion();
                    if (online.getServClient() != "Servidor") escenario = new Juego(this, jugador2, jugador1);
                    else escenario = new Juego(this, jugador1, jugador2);
                    escenario.Show();
                    t1.Stop();
                    this.Close();
                }
                else //Parte local
                {
                    escenario = new Juego(this.getPrincipal(), jugador1, jugador2);
                    this.Close();
                    escenario.Show();
                }
            }
            else
            {
                if (jug1 == false)
                {
                    lblFallo1.Content = "X";
                    lblEligeLuchador.Content = "Introduzca jugador 1...";
                }
                else
                {
                    lblFallo2.Content = "X";
                    lblEligeLuchador.Content = "Introduzca jugador 2...";
                }
            }
        }

        private void btAtras_Click(object sender, RoutedEventArgs e)
        {
            if (online != null)
            {
                fach.CerrarConexion();
                online.Close();
            }
            this.Close();
            vPrin.Show();
        }
    }
}
