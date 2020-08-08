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
using System.Threading;
using System.Windows.Threading;

namespace SSF
{
    public partial class Juego : Window
    {
        private Jugador jugador1;
        private Jugador jugador2;
        private int ronda_actual; //Ronda en la que estamos
        private int rondas_jug1; //Rondas ganadas por el jugador 1
        private int rondas_jug2; //Rondas ganadas por el jugador 2
        private int turno; //Quien es el que empieza al principio de la partida(no cambia solo vale al principio)
        private int boton = 0;
        Random aleatorio; //Para sortear el turno
        MainWindow vPrin; 
        vOnline online = null;
        ElegirJugador elegirJugador;
        FachadaSocket fachada;
        String combinacion; //Combinacion de caracteres 
        Timer tiempo_seg1; //Tiempo seg a seg para introducir la combinacion del jugador 1
        Timer tiempo_seg2; //Tiempo seg a seg para introducir la combinacion del jugador 2
        DispatcherTimer tiempo_turno1; //Comprobacion de la combinacion del jugador 1
        DispatcherTimer tiempo_turno2; //Comprobacion de la combinacion del jugador 2
        Timer tiempo_ronda; //Tiempo que dura cada ronda
        Timer entre_turno; //Tiempo entre cada turno para mostrar mensajes de indicacion
        private int tiempoprueba = 99; //Tiempo de prueba que dura cada ronda
        private int tiempoRonda; //Tiempo que dura cada ronda
        private int nivel1; //Numero de caracteres de la combinacion del jugador 1
        private int nivel2; //Numero de caracteres de la combinacion del jugador 2
        private int tiempo_jugador1; //Tiempo (agilidad) de ataque o defensa del jugador 1
        private int tiempo_jugador2; //Tiempo (agilidad) de ataque o defensa del jugador 2
        private int ataca; //Para saber si me toca atacar o defender
        private int rachaJug1; //Numero de ataques seguidos acertados del jugador 1
        private int rachaJug2; //Numero de ataques seguidos acertados del jugador 2
        private int tiempo_entre_turno; //Tiempo para mostrar los mensajes de indicacion entre cada turno
        private int fase; //Para saber que mensajes tengo que mostrar entre cada turno
        Timer MovNormalJ1 = new Timer(); //Timer para hacer que el jugador 1 se mueva normal
        private int intervaloJ1 = 120;
        Timer MovNormalJ2 = new Timer(); //Timer para hacer que el jugador 2 se mueva normal
        private int intervaloJ2 = 120;
        Timer Ataque1 = new Timer(); //Timer para hacer que el jugador 1 ataque
        private int intervaloAtaqueJ1 = 80;
        Timer Ataque2 = new Timer(); //Timer para hacer que el jugador 2 ataque
        int intervaloAtaqueJ2 = 80;
        private string Personaje1; //Nombre del personaje 1
        private string Personaje2; //Nombre del personaje 2
        private int i, j, k, l, m; //Variables para ayudar a mover a los personajes
        Timer SonidoRonda = new Timer(); //Timer para hacer sonar el sonido de fondo
        private int intervaloSonidoRonda = 600;
        Timer repeticionFondo = new Timer(); //Timer para volver a empezar el sonido de fondo
        //Timer para la parte online
        DispatcherTimer f1; //Timer para saber quien empieza a atacar
        DispatcherTimer f2; //Timer para esperar el turno del jugador 1
        DispatcherTimer f3; //Timer para esperar el turno del jugador 2
        DispatcherTimer v1; //Timer para esperar la vida que se ha modificado
        DispatcherTimer m1; //Timer para saber que movimiento hace el contrario

        public Juego(MainWindow vp, Jugador j1, Jugador j2)
        {
            //Parte local
            InitializeComponent();
            jugador1 = j1;
            jugador2 = j2;
            vPrin = vp;

            inicializarEscenario();
        }

        public Juego(ElegirJugador ej, Jugador j1, Jugador j2)
        {
            //Parte online
            InitializeComponent();
            jugador1 = j1;
            jugador2 = j2;
            elegirJugador = ej;
            vPrin = ej.getPrincipal();
            online = ej.getvOnline();
            if (online.getServClient() == "Servidor")
            {
                //Realizamos la tercera conexion
                fachada = new FachadaSocket(online.getJug1());
                fachada.CrearServidor(2);
                //Deshabilitamos las herramientas del contrario
                racha2.Visibility = Visibility.Hidden;
                puntuacion2.Visibility = Visibility.Hidden;
                textBox2.Visibility = Visibility.Hidden;
            }
            else
            {
                //Realizamos la tercera conexion
                System.Threading.Thread.Sleep(1000);
                fachada = new FachadaSocket(online.getJug1(), online.getIp());
                fachada.CrearCliente(2);
                //Deshabilitamos las herramientas del contrario
                puntuacion1.Visibility = Visibility.Hidden;
                racha1.Visibility = Visibility.Hidden;
                textBox1.Visibility = Visibility.Hidden;
            }
            inicializarEscenario();
        }

        private void inicializarEscenario()
        {
            //Inicializamos todas las variables
            lblNombre1.Content = jugador1.Nombre;
            lblNombre2.Content = jugador2.Nombre;
            imgJug1.Source = jugador1.Retrato.Source;
            imgJug2.Source = jugador2.Retrato.Source;
            cargarImagen("Images/circulorojo.png", 0); //Carga la imagen dependiendo la posicion que le pases
            cargarImagen("Images/circulorojo.png", 1); //Carga imagen circulo rojo
            cargarImagen("Images/circulorojo.png", 2);
            cargarImagen("Images/circulorojo.png", 3);
            cargarImagen("Images/1.png", 4); //Carga la imagen del jugador 1
            cargarImagen("Images/1_2.png", 5); //Carga la imagen del jugador 2
            rachaJug1 = 0;
            rachaJug2 = 0;
            i = 0; j = 1; k = 0; l = 1; m = 1;
            puntuacion1.Content = "Puntuacion: " + jugador1.Puntuacion;
            puntuacion2.Content = "Puntuacion: " + jugador2.Puntuacion;
            racha1.Content = "Racha: " + rachaJug1;
            racha2.Content = "Racha: " + rachaJug2;
            ronda_actual = 1;
            rondas_jug1 = 0;
            rondas_jug2 = 0;
            aleatorio = new Random();
            if (online != null)
            {
                //Solo el servidor sortea el turno para enviarselo despues al cliente
                if (online.getServClient() == "Servidor") turno = aleatorio.Next(2);
            }
            else turno = aleatorio.Next(2);
            Vida1.Value = jugador1.Vida;
            Vida2.Value = jugador2.Vida;
            tiempo_jugador1 = jugador1.Agilidad; //Tiempo segun la agilidad del jugador 1
            tiempo_jugador2 = jugador2.Agilidad; //Tiempo segun la agilidad del jugador 2
            tiempoRonda = tiempoprueba;
            tiempo_entre_turno = 3; //Segundos para mostrar mensajes de indicacion
            fase = 0;
            nivel1 = 6; //El numero de letras de la combinacion
            nivel2 = 6;
            ataca = 0; //Para saber cuando esta atacando o defendiendo
            tiempo_seg1 = new Timer(); //Muestra los segundos que quedan al atacar o defender(Jugador1)
            tiempo_seg1.Tick += new EventHandler(TickReloj1);
            tiempo_seg1.Interval = TimeSpan.FromSeconds(1);
            tiempo_seg2 = new Timer(); //Muestra los segundos que quedan al atacar o defender(Jugador2)
            tiempo_seg2.Tick += new EventHandler(TickReloj2);
            tiempo_seg2.Interval = TimeSpan.FromSeconds(1);
            tiempo_turno1 = new DispatcherTimer(); //Comprueba si la combinacion la ha hecho bien(Jugador 1)
            tiempo_turno1.Tick += new EventHandler(jugar1);
            tiempo_turno1.Interval = TimeSpan.FromSeconds(tiempo_jugador1+1);
            tiempo_turno2 = new DispatcherTimer();//Comprueba si la combinacion la ha hecho bien(Jugador 2)
            tiempo_turno2.Tick += new EventHandler(jugar2);
            tiempo_turno2.Interval = TimeSpan.FromSeconds(tiempo_jugador2+1);
            tiempo_ronda = new Timer(); //Muestra los segundos para acabar la ronda y comprueba quien ha ganado la ronda
            tiempo_ronda.Tick += new EventHandler(TickRelojRonda);
            tiempo_ronda.Interval = TimeSpan.FromMilliseconds(1500);
            entre_turno = new Timer(); //Muestra los segundos para acabar la ronda y comprueba quien ha ganado la ronda
            entre_turno.Tick += new EventHandler(TickRelojTurno);
            entre_turno.Interval = TimeSpan.FromMilliseconds(1000);
            if (online != null)
            {
                //Parte online
                f2 = new DispatcherTimer();
                f2.Tick += new EventHandler(fachTiempo2);
                f2.Interval = TimeSpan.FromMilliseconds(500);
                f3 = new DispatcherTimer();
                f3.Tick += new EventHandler(fachTiempo3);
                f3.Interval = TimeSpan.FromMilliseconds(500);
                v1 = new DispatcherTimer();
                v1.Tick += new EventHandler(modificarVida);
                v1.Interval = TimeSpan.FromMilliseconds(500);
                v1.Start();
                m1 = new DispatcherTimer();
                m1.Tick += new EventHandler(realizarMovimiento);
                m1.Interval = TimeSpan.FromMilliseconds(500);
                m1.Start();
                if (online.getServClient() == "Servidor")
                {
                    if (turno == 0) //Le toca al servidor
                    {
                        entre_turno.Start();
                        fachada.EnviarTexto("#J " + "1"); //Lo comunicamos al cliente
                    }
                    else //Le toca al cliente
                    {
                        entre_turno.Start();
                        fachada.EnviarTexto("#J " + "2"); //Lo comunicamos al cliente
                        f2.Start(); //Esperamos a que el cliente termine
                    }
                }
                else 
                {
                    f1 = new DispatcherTimer();
                    f1.Tick += new EventHandler(fachTiempo1);
                    f1.Interval = TimeSpan.FromMilliseconds(500);
                    f1.Start();
                }
            }
            else entre_turno.Start();
            MovNormalJ1.Tick += new EventHandler(MovimientoNormalJ1);
            MovNormalJ1.Interval = MovNormalJ1.Interval = TimeSpan.FromMilliseconds(intervaloJ1);
            MovNormalJ1.Start();
            Ataque1.Tick += new EventHandler(movAtaqueJ1);
            Ataque1.Interval = Ataque1.Interval = TimeSpan.FromMilliseconds(intervaloAtaqueJ1);

            MovNormalJ2.Tick += new EventHandler(MovimientoNormalJ2);
            MovNormalJ2.Interval = MovNormalJ1.Interval = TimeSpan.FromMilliseconds(intervaloJ2);
            MovNormalJ2.Start();
            Ataque2.Tick += new EventHandler(movAtaqueJ2);
            Ataque2.Interval = Ataque1.Interval = TimeSpan.FromMilliseconds(intervaloAtaqueJ2);

            SonidoRonda.Tick += new EventHandler(sonidoRonda);
            SonidoRonda.Interval = SonidoRonda.Interval = TimeSpan.FromMilliseconds(intervaloSonidoRonda);
            repeticionFondo.Tick += new EventHandler(repFondo);
            repeticionFondo.Interval = SonidoRonda.Interval = TimeSpan.FromSeconds(206);
            repeticionFondo.Start();

            Personaje1 = jugador1.Personaje;
            Personaje2 = jugador2.Personaje;
            reproducirAudio("..\\..\\Audio\\Round1.wav", 3);
            reproducirAudio("..\\..\\Audio\\MusicaFondo1.mp3", 4);
        }

        private void fachTiempo1(object o, EventArgs e)
        {
            if (fachada.getTurnoCliente() != 0) //Recibimos el turno
            {
                turno = fachada.getTurnoCliente() - 1;
                if (turno == 0) f3.Start(); //No me toca, espero a que el contrincante me de el turno
                entre_turno.Start();
                f1.Stop(); // Paro el timer, no se volvera a utilizar, el turno se gestiona con f2 y f3
            }
        }

        private void fachTiempo2(object o, EventArgs e)
        {
            if (fachada.getFasePartida() != -1) //Le toca al servido
            {
                fase = 1;
                ataca = fachada.getFasePartida();
                if (ataca == 0) label7.Content = jugador2.Nombre + " ataca mal, ataca " + jugador1.Nombre;
                entre_turno.Start();
                fachada.setFasePartida(-1);
                f2.Stop();
            }
        }

        private void fachTiempo3(object o, EventArgs e)
        {
            if (fachada.getFasePartida() != -1) //Le toca al cliente
            {
                fase = 2;
                ataca = fachada.getFasePartida();
                if (ataca == 0) label7.Content = jugador1.Nombre + " ataca mal, ataca " + jugador2.Nombre;
                entre_turno.Start();
                fachada.setFasePartida(-1);
                f3.Stop();
            }
        }

        private void modificarVida(object o, EventArgs e)
        {
            if (fachada.getVida() != -1) //Hay que modificar la vida
            {
                if (online.getServClient() == "Servidor")
                {
                    if(fachada.getVida() == 0) Vida2.Value -= obtenerAtaque(0); //Ha defendido mal le quitamos mas vida
                    else if(fachada.getVida() == 1) Vida2.Value -= (obtenerAtaque(0) - obtenerDefensa(1)); //Ha defendido bien
                }
                else
                {
                    if (fachada.getVida() == 0) Vida1.Value -= obtenerAtaque(1);
                    else if (fachada.getVida() == 1) Vida1.Value -= (obtenerAtaque(1) - obtenerDefensa(0));
                }
                fachada.setVida(-1); //Reiniciamos para seguir a la espera
            }
        }

        private void realizarMovimiento(object o, EventArgs e)
        {
            if (fachada.getMovimiento() != -1)
            {
                if (online.getServClient() == "Servidor")
                {
                    if (fachada.getMovimiento() == 0) //He atacado bien, movimiento de ataque
                    {
                        label7.Content = jugador2.Nombre + " ataca bien, defiende " + jugador1.Nombre;
                        MovNormalJ2.Stop();
                        Ataque2.Start();
                        reproducirAudio("", 2);
                    }
                    if (fachada.getMovimiento() == 1) //He defendido bien, movimiento de defensa
                    {
                        MovNormalJ2.Stop();
                        cargarImagenCombateJ2("Images/" + Personaje2 + "Defensa.png");
                    }
                    if (fachada.getMovimiento() == 2) //He defendido mal, movimiento de herido
                    {
                        MovNormalJ2.Stop();
                        cargarImagenCombateJ2("Images/" + Personaje2 + "Golpeado.png");
                    }
                }
                else
                {
                    if (fachada.getMovimiento() == 0)
                    {
                        label7.Content = jugador1.Nombre + " ataca bien, defiende " + jugador2.Nombre;
                        MovNormalJ1.Stop();
                        Ataque1.Start();
                        reproducirAudio("", 1);
                    }
                    if (fachada.getMovimiento() == 1)
                    {
                        MovNormalJ1.Stop();
                        cargarImagenCombateJ1("Images/" + Personaje1 + "Defensa.png");
                    }
                    if (fachada.getMovimiento() == 2)
                    {
                        MovNormalJ1.Stop();
                        cargarImagenCombateJ1("Images/" + Personaje1 + "Golpeado.png");
                    }
                }
                fachada.setMovimiento(-1);
            }
        }

        private void TickRelojTurno(object o, EventArgs e)
        {
            //Mostramos mensajes de indicacion
            //Fase=-1 --- Tiempo para indicar quien ha ganado la ronda
            //Fase=0 ---- Tiempo entre cada turno
            //Fase=1 ---- Le damos el turno al jugador 1
            //Fase=2 ---- Le damos el turno al jugador 2
            if(fase!=-1) label8.Content = tiempo_entre_turno.ToString(); 
            tiempo_entre_turno--;
            if (fase == 0)
            {
                if (turno == 0) label7.Content = "Comienza " + jugador1.Nombre;
                else label7.Content = "Comienza " + jugador2.Nombre;
                textBox1.Text = "";
                textBox2.Text = "";
            }
            if (tiempo_entre_turno == -1) 
            {
                if (fase == -1) SonidoRonda.Start();
                if (fase != -1) label8.Content = "¡YA!";
                entre_turno.Stop();
                tiempo_entre_turno = 3;
                if (fase == 0)
                {
                    tiempo_ronda.Start();
                    if (online != null)
                    {
                        //Parte online
                        //Si eres el servidor no hace falta el turno 2
                        //Si eres el cliente no hace falta el turno 1
                        if (online.getServClient() == "Servidor")
                        {
                            if (turno == 0) turnoJugador1();
                        }
                        else if (turno == 1) turnoJugador2();
                    }
                    else
                    {
                        //Parte local
                        if (turno == 0) turnoJugador1();
                        else turnoJugador2();
                    }
                }
                if (online != null)
                {
                    //Parte online
                    //Si eres el servidor no hace falta el turno 2
                    //Si eres el cliente no hace falta el turno 1
                    if (online.getServClient() == "Servidor")
                    {
                        if (fase == 1) turnoJugador1();
                    }
                    else if (fase == 2) turnoJugador2();
                }
                else
                {
                    //Parte local
                    if (fase == 1) turnoJugador1();
                    if (fase == 2) turnoJugador2();
                }
                if (fase == -1)
                {
                    fase = 0;
                    entre_turno.Start();
                }
                textBox1.Text = "";
                textBox2.Text = "";
                MovNormalJ1.Start();
                MovNormalJ2.Start();
            }
        }

        private void jugar1(object o, EventArgs e) // estamos en el jugador1
        {
            if (ataca == 0) // si toca atacar
            {
                if (textBox1.Text.Equals(combinacion)) // si ha hecho bien la combinacion
                {
                    MovNormalJ1.Stop();
                    Ataque1.Start();
                    reproducirAudio("", 1);
                    label2.Content = "¡BIEN!";
                    if (online != null)
                    {
                        fachada.EnviarTexto("#M " + "0");//Enviamos el movimiento
                        fachada.EnviarTexto("#F " + "1");//Enviamos la fase
                        f2.Start();
                    }
                    ataca = 1;
                    tiempo_turno1.Stop();
                    textBox1.Text = "";
                    label7.Content = jugador1.Nombre + " ataca bien, defiende " + jugador2.Nombre;
                    nivel1++;
                    rachaJug1++;
                    jugador1.Puntuacion += (1024*rachaJug1+jugador1.Fuerza*rachaJug1);
                    puntuacion1.Content = "Puntuacion: " + jugador1.Puntuacion;
                    racha1.Content = "Racha: " + rachaJug1;
                    fase = 2;
                    if (online == null) entre_turno.Start();
                }
                else
                {
                    label2.Content = "¡MAL!";
                    if (online != null)
                    {
                        fachada.EnviarTexto("#F " + "0");
                        f2.Start();
                    }
                    tiempo_turno1.Stop();
                    textBox1.Text = "";
                    label7.Content = jugador1.Nombre+ " ataca mal, ataca " +jugador2.Nombre;
                    rachaJug1 = 0;
                    racha1.Content = "Racha: " +rachaJug1;
                    fase = 2;
                    if (online == null) entre_turno.Start();
                }
            }
            else //Si nos toca defender
            {
                if (textBox1.Text.Equals(combinacion)) //Si ha hecho bien la combinacion
                {
                    MovNormalJ1.Stop();
                    cargarImagenCombateJ1("Images/" + Personaje1 + "Defensa.png");
                    label2.Content = "¡BIEN!";
                    tiempo_turno1.Stop();
                    textBox1.Text = "";
                    Vida1.Value -= (obtenerAtaque(1)-obtenerDefensa(0));
                    if (online != null)
                    {
                        fachada.EnviarTexto("#M " + "1");
                        fachada.EnviarTexto("#V " + "1");
                    }
                    ataca = 0;
                    label7.Content = jugador1.Nombre + " defiende bien, ahora le toca atacar";
                    if (rachaJug1 == 0) jugador1.Puntuacion += 512+jugador1.Defensa*2;
                    else jugador1.Puntuacion += (512*rachaJug1+jugador1.Defensa*2);
                    puntuacion1.Content = "Puntuacion: " + jugador1.Puntuacion;
                    fase = 1;
                    entre_turno.Start();
                }
                else
                {
                    MovNormalJ1.Stop();
                    cargarImagenCombateJ1("Images/" + Personaje1 + "Golpeado.png");
                    label2.Content = "¡MAL!";
                    tiempo_turno1.Stop();
                    textBox1.Text = "";
                    Vida1.Value -= obtenerAtaque(1);
                    if (online != null)
                    {
                        fachada.EnviarTexto("#M " + "2");
                        fachada.EnviarTexto("#V " + "0");
                    }
                    ataca = 0;
                    label7.Content = jugador1.Nombre + " defiende mal, ahora le toca atacar";
                    fase = 1;
                    entre_turno.Start();
                }
            }
            //Reiniciamos label
            label1.Content = "";
            label4.Content = "";
            label3.Content = "";
            label6.Content = "";
        }

        private void jugar2(object o, EventArgs e) // estamos en el jugador2 (idéntico al metodo jugar1)
        {
            if (ataca == 0)
            {
                if (textBox2.Text.Equals(combinacion))
                {
                    MovNormalJ2.Stop();
                    Ataque2.Start();
                    reproducirAudio("", 2);
                    label5.Content = "¡BIEN!";
                    if (online != null)
                    {
                        fachada.EnviarTexto("#M " + "0");
                        fachada.EnviarTexto("#F " + "1");
                        f3.Start();
                    }
                    ataca = 1;
                    tiempo_turno2.Stop();
                    textBox2.Text = "";
                    label7.Content = jugador2.Nombre + " ataca bien, defiende " + jugador1.Nombre;
                    nivel2++;
                    rachaJug2++;
                    jugador2.Puntuacion += (1024*rachaJug2+jugador2.Fuerza*rachaJug2);
                    puntuacion2.Content = "Puntuacion: " +jugador2.Puntuacion;
                    racha2.Content = "Racha: " +rachaJug2;
                    fase = 1;
                    if (online == null) entre_turno.Start();
                }
                else
                {
                    label5.Content = "¡MAL!";
                    if (online != null) 
                    {
                        fachada.EnviarTexto("#F " + "0");
                        f3.Start();
                    }
                    tiempo_turno2.Stop();
                    textBox2.Text = "";
                    label7.Content = jugador2.Nombre+ " ataca mal, ataca " +jugador1.Nombre;
                    rachaJug2 = 0;
                    racha2.Content = "Racha: " +rachaJug2;
                    fase = 1;
                    if (online == null) entre_turno.Start();
                }
            }
            else
            {
                if (textBox2.Text.Equals(combinacion))
                {
                    MovNormalJ2.Stop();
                    cargarImagenCombateJ2("Images/" + Personaje2 + "Defensa.png");
                    label5.Content = "¡BIEN!";
                    tiempo_turno2.Stop();
                    textBox2.Text = "";
                    Vida2.Value -= (obtenerAtaque(0)-obtenerDefensa(1));
                    if (online != null)
                    {
                        fachada.EnviarTexto("#M " + "1");
                        fachada.EnviarTexto("#V " + "1");
                    }
                    ataca = 0;
                    label7.Content = jugador2.Nombre + " defiende bien, ahora le toca atacar";
                    if (rachaJug2 == 0) jugador2.Puntuacion+=512+jugador2.Defensa*2;
                    else jugador2.Puntuacion += (512*rachaJug2+jugador2.Defensa*2);
                    puntuacion2.Content = "Puntuacion: " + jugador2.Puntuacion;
                    fase = 2;
                    entre_turno.Start();
                }
                else
                {
                    MovNormalJ2.Stop();
                    cargarImagenCombateJ2("Images/" + Personaje2 + "Golpeado.png");
                    label5.Content = "¡MAL!";
                    tiempo_turno2.Stop();
                    textBox2.Text = "";
                    Vida2.Value -= obtenerAtaque(0);
                    if (online != null)
                    {
                        fachada.EnviarTexto("#M " + "2");
                        fachada.EnviarTexto("#V " + "0");
                    }
                    ataca = 0;
                    label7.Content = jugador2.Nombre + " defiende mal, ahora le toca atacar";
                    fase = 2;
                    entre_turno.Start();
                }
            }
            label1.Content = "";
            label4.Content = "";
            label3.Content = "";
            label6.Content = "";
        }

        private void TickReloj1(object o, EventArgs e) // cada segundo se ejecuta y te muestra el tiempo
        {
            label3.Content = tiempo_jugador1.ToString();
            tiempo_jugador1--; // se va decrementando 
            if (tiempo_jugador1 == -1) // cuando llega a 0 se para y vuelve a empezar cuando le des al boton
            {
                tiempo_seg1.Stop();
                tiempo_jugador1 = jugador1.Agilidad;
            }
        }

        private void TickReloj2(object o, EventArgs e) // igual que TickReloj1 pero para jugador2
        {
            label6.Content = tiempo_jugador2.ToString();
            tiempo_jugador2--;
            if (tiempo_jugador2 == -1)
            {
                tiempo_seg2.Stop();
                tiempo_jugador2 = jugador2.Agilidad;
            }
        }

        private void TickRelojRonda(object o, EventArgs e) // igual que los TickReloj anteriores pero comprueba si pierden o ganan
        {
            lblTiempoRonda.Content = tiempoRonda.ToString();
            tiempoRonda--;
            if (tiempoRonda == -1) //Si el contador llega a 0 mira quien gana e inicia todo de nuevo
            {
                reproducirAudio("..\\..\\Audio\\TimeOver.wav", 3);
                tiempo_ronda.Stop();
                tiempoRonda = tiempoprueba;
                if (Vida1.Value > Vida2.Value)
                {
                    if (rondas_jug1 == 0) cargarImagen("Images/circuloverde.png", 0);
                    if (rondas_jug1 == 1) cargarImagen("Images/circuloverde.png", 1);
                    rondas_jug1++; // Numero de rondas ganadas por el jugador 1
                    label7.Content = jugador1.Nombre + " gana la ronda";
                }
                else
                {
                    if (Vida2.Value > Vida1.Value)
                    {
                        if (rondas_jug2 == 0) cargarImagen("Images/circuloverde.png", 3);
                        if (rondas_jug2 == 1) cargarImagen("Images/circuloverde.png", 2);
                        rondas_jug2++; // Numero de rondas ganadas por el jugador 2
                        label7.Content = jugador2.Nombre + " gana la ronda";
                    }
                    else
                    {
                        label7.Content = "Empate, ronda nula";
                        ronda_actual--;
                    }
                }
                Vida1.Value = 100;
                Vida2.Value = 100;
                tiempo_turno1.Stop();
                tiempo_seg1.Stop();
                tiempo_jugador1 = jugador1.Agilidad;
                tiempo_turno2.Stop();
                tiempo_seg2.Stop();
                tiempo_jugador2 = jugador2.Agilidad;
                ronda_actual++;  //Incrementamos ronda
                lblRonda.Content = "Ronda " + ronda_actual;
                ataca = 0;
                nivel1 = 6;
                nivel2 = 6;
                rachaJug1 = 0;
                rachaJug2 = 0;
                label3.Content = "";
                label6.Content = "";
                label1.Content = "";
                label4.Content = "";
                fase = -1;
                if (online != null)
                {
                    //Parte online
                    if (online.getServClient() == "Servidor")
                    {
                        //Doy el turno a quien le a tocado al inicial la partida
                        if (turno == 0) entre_turno.Start();
                        else
                        {
                            entre_turno.Start();
                            f2.Start();
                        }
                    }
                    else
                    {
                        if (turno == 1) entre_turno.Start();
                        else
                        {
                            entre_turno.Start();
                            f3.Start();
                        }
                    }
                }
                else entre_turno.Start();
            }
            if (Vida1.Value <= 0) //Si jugador 1 le matan
            {
                reproducirAudio("..\\..\\Audio\\J2Win.wav", 3);
                if (rondas_jug2 == 0) cargarImagen("Images/circuloverde.png", 3);
                if (rondas_jug2 == 1) cargarImagen("Images/circuloverde.png", 2);
                tiempo_ronda.Stop();
                tiempoRonda = tiempoprueba;
                rondas_jug2++;
                label7.Content = jugador2.Nombre +" gana la ronda";
                label8.Content = "";
                Vida1.Value = 100;
                Vida2.Value = 100;
                tiempo_turno1.Stop();
                tiempo_seg1.Stop();
                tiempo_jugador1 = jugador1.Agilidad;
                tiempo_turno2.Stop();
                tiempo_seg2.Stop();
                tiempo_jugador2 = jugador2.Agilidad;
                ronda_actual++;
                lblRonda.Content = "Ronda " + ronda_actual;
                ataca = 0;
                nivel1 = 6;
                nivel2 = 6;
                rachaJug1 = 0;
                rachaJug2 = 0;
                fase = -1;
                if (online != null)
                {
                    if (online.getServClient() == "Servidor")
                    {
                        if (turno == 0) entre_turno.Start();
                        else
                        {
                            entre_turno.Start();
                            f2.Start();
                        }
                    }
                    else
                    {
                        if (turno == 1) entre_turno.Start();
                        else
                        {
                            entre_turno.Start();
                            f3.Start();
                        }
                    }
                }
                else entre_turno.Start();
            }
            if (Vida2.Value <= 0) // Si jugador 2 le matan
            {
                reproducirAudio("..\\..\\Audio\\J1Win.wav", 3);
                if (rondas_jug1 == 0) cargarImagen("Images/circuloverde.png", 0);
                if (rondas_jug1 == 1) cargarImagen("Images/circuloverde.png", 1);
                tiempo_ronda.Stop();
                tiempoRonda = tiempoprueba;
                rondas_jug1++;
                label7.Content = jugador1.Nombre + " gana la ronda";
                label8.Content = "";
                Vida1.Value = 100;
                Vida2.Value = 100;
                tiempo_turno1.Stop();
                tiempo_seg1.Stop();
                tiempo_jugador1 = jugador1.Agilidad;
                tiempo_turno2.Stop();
                tiempo_seg2.Stop();
                tiempo_jugador2 = jugador2.Agilidad;
                ronda_actual++;
                lblRonda.Content = "Ronda " + ronda_actual;
                ataca = 0;
                nivel1 = 6;
                nivel2 = 6;
                rachaJug1 = 0;
                rachaJug2 = 0;
                fase = -1;
                if (online != null)
                {
                    if (online.getServClient() == "Servidor")
                    {
                        if (turno == 0) entre_turno.Start();
                        else
                        {
                            entre_turno.Start();
                            f2.Start();
                        }
                    }
                    else
                    {
                        if (turno == 1) entre_turno.Start();
                        else
                        {
                            entre_turno.Start();
                            f3.Start();
                        }
                    }
                }
                else entre_turno.Start();
            }
            if (rondas_jug1 == 2) //Si jugador 1 ya ha ganado 2 rondas
            {
                label7.Content = jugador1.Nombre.ToUpper() + " ES EL GANADOR";
                lblRonda.Content = "GAME OVER";
                MovNormalJ1.Start();
                MovNormalJ2.Stop();
                image6.Source = null;
                cargarImagenMuertoJ2("Images/" + Personaje2 + "Muerto.png");
                label8.Content = "";
                tiempo_turno1.Stop();
                tiempo_seg1.Stop();
                tiempo_turno2.Stop();
                tiempo_seg2.Stop();
                tiempo_ronda.Stop();
                entre_turno.Stop();
                mediaFondo.Close();
                mediaJ1.Close();
                mediaJ2.Close();
                mediaAmbiente.Close();
                SonidoRonda.Stop();
                repeticionFondo.Stop();
                btSalir.Visibility = Visibility.Visible;
                btRanking.Visibility = Visibility.Visible;
                if (online != null)
                {
                    //Parte online
                    f2.Stop();
                    f3.Stop();
                    v1.Stop();
                    m1.Stop();
                    fachada.CerrarConexion();
                    online.Close();
                    elegirJugador.Close();
                }
                //Solo actualizamos quien haya ganado la partida
                this.actualizarRanking(jugador1.Nombre, jugador1.Puntuacion);
            }
            if (rondas_jug2 == 2) //Si jugador 2 ya ha ganado 2 rondas
            {                
                label7.Content = jugador2.Nombre.ToUpper() + " ES EL GANADOR";
                lblRonda.Content = "GAME OVER";
                MovNormalJ2.Start();
                MovNormalJ1.Stop();
                image5.Source = null;
                cargarImagenMuertoJ1("Images/" + Personaje1 + "Muerto.png");
                label8.Content = "";
                tiempo_turno1.Stop();
                tiempo_seg1.Stop();
                tiempo_turno2.Stop();
                tiempo_seg2.Stop();
                tiempo_ronda.Stop();
                entre_turno.Stop();
                mediaFondo.Close();
                mediaJ1.Close();
                mediaJ2.Close();
                mediaAmbiente.Close();
                SonidoRonda.Stop();
                repeticionFondo.Stop();
                btSalir.Visibility = Visibility.Visible;
                btRanking.Visibility = Visibility.Visible;
                if (online != null)
                {
                    f2.Stop();
                    f3.Stop();
                    v1.Stop();
                    m1.Stop();
                    fachada.CerrarConexion();
                    online.Close();
                    elegirJugador.Close();
                }
                this.actualizarRanking(jugador2.Nombre, jugador2.Puntuacion);
            }
        }

        public int obtenerAtaque(int jug)
        {
            int ataque;
            if (jug == 0) ataque = jugador1.Fuerza*6; //Formula para saber cuanta vida quitamos
            else ataque = jugador2.Fuerza*6;
            return ataque;
        }

        public int obtenerDefensa(int jug)
        {
            int defensa;
            if (jug == 0) defensa = jugador1.Defensa*2; //Formula para saber la vida que restamos
            else defensa = jugador2.Defensa*2;
            return defensa;
        }

        public static string RandomString1(int size) // genera una cadena aleatoria
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))));
            }
            return builder.ToString();
        }

        public static string RandomString2(int size) // genera una cadena aleatoria
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))));
            }
            return builder.ToString();
        }

        private void turnoJugador1() // cambia el turno a jugador1
        {
            textBox1.IsEnabled = true;
            textBox2.IsEnabled = false;
            combinacion = RandomString1(nivel1);
            label1.Content = combinacion;
            label2.Content = ""; // ocultamos etiquetas jugador rival
            label4.Content = "";
            label5.Content = "";
            label6.Content = "";
            tiempo_seg1.Start();
            tiempo_turno1.Start();
        }

        public void turnoJugador2() // cambia el turno a jugador2
        {
            textBox1.IsEnabled = false;
            textBox2.IsEnabled = true;
            combinacion = RandomString2(nivel2);
            label4.Content = combinacion;
            label5.Content = ""; // ocultamos etiquetas jugador rival
            label3.Content = "";
            label2.Content = "";
            label1.Content = "";
            tiempo_seg2.Start();
            tiempo_turno2.Start();
        }

        // imágenes y movimientos
        private void cargarImagen(string ruta, int n)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@ruta, UriKind.Relative);
            bi.EndInit(); 
            switch(n) 
            {
                case 0: // imagen del circulo rojo/verde 1
                    this.image1.Source = bi;
                    break;
                case 1: // imagen del circulo rojo/verde 2
                    this.image2.Source = bi;
                    break;
                case 2: // imagen del circulo rojo/verde 3
                    this.image3.Source = bi;
                    break;
                case 3: // imagen del circulo rojo/verde 4
                    this.image4.Source = bi;
                    break;
            }
        }

        private void movAtaqueJ1(object sender, EventArgs e)
        {
            if (j < 6) //El contador actua de identificado para llamar a las imagenes
            {
                string rutaataque = "Images/" + Personaje1 + "Ataca" + j + ".png";
                cargarImagenCombateJ1(rutaataque);
            }
            else
            {
                j = 1;
                Ataque1.Stop();
                MovNormalJ1.Start();
            }
            j++;
        }

        private void movAtaqueJ2(object sender, EventArgs e) 
        {
            if (l < 6) //El contador actua de identificado para llamar a las imagenes
            {
                string rutaataque = "Images/" + Personaje2 + "Ataca" + l + ".png";
                cargarImagenCombateJ2(rutaataque);
            }
            else
            {
                l = 1;
                Ataque2.Stop();
                MovNormalJ2.Start();
            }
            l++;
        }

        private void MovimientoNormalJ1(object sender, EventArgs e)
        {
            i++; //El contador actua de identificado para llamar a las imagenes
            if (i > 1 && i <= 4)
            {
                string rutaimagen = "Images/" + Personaje1 + "Normal" + i + ".png";
                cargarImagenCombateJ1(rutaimagen);

            }
            else
            {
                i = 1;
                string rutaimagen = "Images/" + Personaje1 + "Normal" + i + ".png";
                cargarImagenCombateJ1(rutaimagen);
            }

        }

        private void MovimientoNormalJ2(object sender, EventArgs e)
        {
            k++; //El contador actua de identificado para llamar a las imagenes
            if (k > 1 && k <= 4)
            {
                string rutaimagen = "Images/" + Personaje2 + "Normal" + k + ".png";
                cargarImagenCombateJ2(rutaimagen);

            }
            else
            {
                k = 1;
                string rutaimagen = "Images/" + Personaje2 + "Normal" + k + ".png";
                cargarImagenCombateJ2(rutaimagen);
            }

        }

        private void cargarImagenCombateJ1(string ruta)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@ruta, UriKind.Relative);
            bi.EndInit();
            this.image5.Source = bi;
        }       

        private void cargarImagenCombateJ2(string ruta)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@ruta, UriKind.Relative);
            bi.EndInit();
            this.image6.Source = bi;
        }

        private void cargarImagenMuertoJ1(string ruta)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@ruta, UriKind.Relative);
            bi.EndInit();
            this.muerto1.Source = bi;
        }

        private void cargarImagenMuertoJ2(string ruta)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@ruta, UriKind.Relative);
            bi.EndInit();
            this.muerto2.Source = bi;
        }


        // audio
        private void reproducirAudio(string ruta, int tipo)
        {
            switch (tipo)
            {
                case 1:
                    if (ruta == "")
                    {
                        mediaJ1.Source = new Uri(@"..\..\Audio\" + Personaje1 + "Golpe.wav", UriKind.RelativeOrAbsolute);
                        mediaJ2.Source = new Uri(@"..\..\Audio\" + Personaje2 + "Damage.wav", UriKind.RelativeOrAbsolute);
                        mediaJ1.Play();
                        mediaJ2.Play();
                    }
                    else
                    {
                        mediaJ1.Source = new Uri(@ruta, UriKind.RelativeOrAbsolute);
                    }
                    break;
                case 2:
                    mediaJ2.Source = new Uri(@"..\..\Audio\" + Personaje2 + "Golpe.wav", UriKind.RelativeOrAbsolute);
                    mediaJ1.Source = new Uri(@"..\..\Audio\" + Personaje1 + "Damage.wav", UriKind.RelativeOrAbsolute);
                    mediaJ1.Play();
                    mediaJ2.Play();
                    break;
                case 3:
                    mediaAmbiente.Source = new Uri(@ruta, UriKind.RelativeOrAbsolute);
                    mediaAmbiente.Play();
                    break;
                case 4:
                    mediaFondo.Source = new Uri(@ruta, UriKind.RelativeOrAbsolute);
                    mediaFondo.Play();
                    break;
            }
        }

        private void repFondo(object sender, EventArgs e)
        {
            reproducirAudio("..\\..\\Audio\\MusicaFondo1.mp3", 4);
        }

        private void sonidoRonda(object sender, EventArgs e)
        {
            if (m == 0)
            {
                reproducirAudio("..\\..\\Audio\\Round" + ronda_actual + ".wav", 3);
                m = 1;
                SonidoRonda.Stop();

            }
            else m--;
        }

        // ranking e interfaz
        private void actualizarRanking(String jugador, int puntos)
        {
            if (SSFRanking.existeRanking())
            {
                if (SSFRanking.estaPuntuacionEnRanking(puntos)) {
                    label8.Content = "¡Enhorabuena, has logrado entrar en el ranking!";
                    SSFRanking.insertarPuntuacion(jugador,puntos);
                }
            }
        }

        private void btSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btRanking_Click(object sender, RoutedEventArgs e)
        {
            Ranking vRanking = new Ranking(vPrin);
            vRanking.Show();
            boton = 1; //Para saber si mostramos ya la pantalla inicial
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (online != null) fachada.CerrarConexion();
            if (boton == 0)
            {
                mediaFondo.Close();
                mediaJ1.Close();
                mediaJ2.Close();
                mediaAmbiente.Close();
                SonidoRonda.Stop();
                repeticionFondo.Stop();
                vPrin.Show();
            }
        }
    }
}