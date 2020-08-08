using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows;
using System.Collections;


namespace SSF
{
    class FachadaSocket : Window
    {
        private TcpListener tcpListener = null; //Socket a la espera de punto remoto 
        private TcpListener tcpListenerAux = null; //Socket a la espera de punto remoto segunda conexion
        private TcpListener tcpListenerAux2 = null; //Socket a la esepera de punto remoto tercera conexion
        private TcpClient tcp = null; //Socket de comunicación 
        private byte[] dataToSend = null; //Datos a enviar 
        private ThreadStart ethernetThreadStart = null; //Punto de inicio servidor 
        private Thread pollDevicesEthernetThread = null; //Hilo para el servidor 
        private Thread receiveThread = null; //Hilo para el cliente

        private string jug;
        private string ip;
        private string puerto = "4177"; //Puerto de la primera conexion
        private string MensajeRecibido;
        private string NombreContrincante; 
        private int PersonajeContrincante;
        private int fase = -1; //Si me toca atacar o defender
        private int turnoCl = 0; //Quien le toca empezar a jugar
        private int vida = -1; //Vida que tengo que modificar
        private int movimiento = -1; //Movimiento que tengo que hacer en el proceso contrario

        public FachadaSocket(string jugador) {
            this.jug = jugador;
        }

        public FachadaSocket(string jugador, string I) {
            this.jug = jugador;
            this.ip = I;
        }

        public int getVida()
        {
            return vida;
        }

        public void setVida(int v)
        {
            vida = v;
        }

        public int getMovimiento()
        {
            return movimiento;
        }

        public void setMovimiento(int m)
        {
            movimiento = m;
        }

        public string getMensajeRecibido()
        {
            return this.MensajeRecibido;
        }

        public void setMensajeRecibido(string mensj)
        {
            this.MensajeRecibido = mensj;
        }

        public void setJugador(int p)
        {
            this.PersonajeContrincante = p;
        }

        public string getNombreContrincante()
        {
            return NombreContrincante;
        }

        public int getPersonajeContrincante()
        {
            return PersonajeContrincante;
        }

        public int getFasePartida()
        {
            return fase;
        }

        public void setFasePartida(int f)
        {
            fase = f;
        }

        public int getTurnoCliente()
        {
            return turnoCl;
        }
        
        private string LocalIPAddress() {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    //if (ip.ToString().CompareTo("192.") < 0)
                    //{
                        localIP = ip.ToString();
                    //}
                }
            }
            return localIP;
        }

        public void CrearServidor(int Inicial) {
            if (Inicial == 1)
            { //Segunda conexion
                puerto = "4179";
            }
            if (Inicial == 2)
            {  //Tercera conexion
                puerto = "4190";
            }
            // Creamos el EndPoint
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(LocalIPAddress()), int.Parse(puerto));
            //Indicamos la Ip a la que se deberá conectar el Cliente
            if (Inicial == 0)
            {
                System.Windows.MessageBox.Show("El otro Jugador debe conectarse a la Ip: "
                    + IPAddress.Parse(LocalIPAddress()).ToString());
                // Instanciamos el canal TCP a la escucha.
                tcpListener = new TcpListener(localEndPoint);
                tcpListener.Start();
                // Permanece a la espera de una conexión del cliente
                tcp = tcpListener.AcceptTcpClient();
            }
            if(Inicial == 1) 
            {
                tcpListenerAux = new TcpListener(localEndPoint);
                tcpListenerAux.Start();
                // Permanece a la espera de una conexión del cliente
                tcp = tcpListenerAux.AcceptTcpClient();
            }
            if (Inicial == 2)
            {
                tcpListenerAux2 = new TcpListener(localEndPoint);
                tcpListenerAux2.Start();
                // Permanece a la espera de una conexión del cliente
                tcp = tcpListenerAux2.AcceptTcpClient();
            }
            //Cuando se conecta el cliente se crea un hilo independiente 
            ethernetThreadStart = new ThreadStart(this.EscuchaInfinita); 
            pollDevicesEthernetThread = new Thread(ethernetThreadStart); 
            pollDevicesEthernetThread.Name = "Listener's Receive Thread"; 
            pollDevicesEthernetThread.SetApartmentState(ApartmentState.MTA); 
            pollDevicesEthernetThread.Start();
        }

        public void CrearCliente(int Inicial) {
            if (Inicial == 1)
            {  //Segunda conexion
                puerto = "4179";
            }
            if (Inicial == 2)
            {  //Tercera conexion
                puerto = "4190";
            }
            //Crear socket indicando ip local y puerto local (+1)
            tcp = new TcpClient(new IPEndPoint(IPAddress.Parse(LocalIPAddress()), int.Parse(puerto) + 1));
            LingerOption lingerOption = new LingerOption(false, 1);
            tcp.LingerState = lingerOption;
            // Concetarnos al puerto e ip remotos
            tcp.Connect(IPAddress.Parse(ip), int.Parse(puerto));
            //Crear hilo para que esté atento a lo que le envían 
            receiveThread = new Thread(new ThreadStart(this.EscuchaInfinita)); 
            receiveThread.Name = "Hilo del Cliente"; 
            receiveThread.SetApartmentState(ApartmentState.MTA); 
            receiveThread.Start();
        }

        private void EscuchaInfinita() { 
            for (; ; ) { 
                Thread.Sleep(100); 
                byte[] msg = new Byte[Constants.maxNoOfBytes]; 
                byte count1 = 0x01; 
                for (int i = 0; i < msg.Length; i++) { 
                    msg[i] = count1++;
                }
                try { 
                    //Leemos el buffer por si se ha recibido algo 
                    int readBytes = tcp.GetStream().Read(msg, 0, msg.Length);
                    if (comprobarDesconexion(readBytes, msg))
                    {
                        return;
                    }
                    else
                    {
                        if (recibirNombreJugador(readBytes, msg)) { }
                        else
                        {
                            if (recibirJugador(readBytes, msg)) { }
                            else
                            {
                                if (fasePartida(readBytes, msg)) { }
                                else
                                {
                                    if (turnoCliente(readBytes, msg)) { }
                                    else
                                    {
                                        if (vidaCliente(readBytes, msg)) { }
                                        else
                                        {
                                            if (movimientoCliente(readBytes, msg)) { }
                                            else
                                            {                                              
                                                    ////intentamos recoger el mensaje y ponerlo en pantalla 
                                                    //StringBuilder str = new StringBuilder(Constants.maxNoOfBytes);
                                                    //for (int count = 0; count < readBytes; count++)
                                                    //{
                                                    //    char ch = (char)msg[count];
                                                    //    str = str.Append(ch);
                                                    //}
                                                    ////Pintamos el mensaje a través de un delegado. 
                                                    //this.delegarRecibirMensaje(str.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                } 
                catch (IOException) { 
                    return; 
                } 
            } 
        }

        //Delegado para escribir en la ventana los mensajes recibidos 
        private delegate void RecibirMensajeDel(string msg);
        private void delegarRecibirMensaje(string msg)
        {
            RecibirMensajeDel aux = new RecibirMensajeDel(this.escribirMensaje);
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, aux, msg);
        }
        private void escribirMensaje(string msg)
        {
            MensajeRecibido = msg;
        }

        public void EnviarTexto(string mensj) {
            string Mensaje = mensj;
            if (Mensaje.Length != 0) {
                //Prepara mensaje para enviar como array de bytes
                char[] charArray = Mensaje.ToCharArray(0, Mensaje.Length); 
                dataToSend = new byte[Mensaje.Length]; 
                for (int charCount = 0; charCount < Mensaje.Length; charCount++) { 
                    dataToSend[charCount] = (byte)charArray[charCount]; 
                } 
                //Usamos el socket para enviar el mensaje
                tcp.GetStream().Write(dataToSend, 0, dataToSend.Length); 
            }
        }

        public void CerrarConexion() {
            if (tcp.Connected) {
                this.enviarDesconexion(); 
                if (tcp != null) 
                    tcp.GetStream().Close(); 
                if (tcpListener != null) 
                    tcpListener.Stop();
                if (tcpListenerAux != null)
                    tcpListenerAux.Stop();
                if (tcpListenerAux2 != null)
                    tcpListenerAux2.Stop();
                if (tcp != null) 
                    tcp.Close();
            }
        }
        
        private void enviarDesconexion() { 
            dataToSend = new byte[] { (byte)'s', (byte)'h', (byte)'u', (byte)'t', 
                (byte)'d', (byte)'o', (byte)'w', (byte)'n' }; 
            tcp.GetStream().Write(dataToSend, 0, dataToSend.Length); 
        }

        private bool comprobarDesconexion(int tam, byte [] msg) { 
            //Si me envian "shutdown", desconecto los sockets 
            bool salida = false; 
            if (tam == 8) { 
                StringBuilder shutMessage = new StringBuilder(8); 
                for (int count = 0; count < 8; count++) { 
                    char ch = (char)msg[count]; 
                    shutMessage = shutMessage.Append(ch); 
                } 
                string shut = "shutdown"; 
                string receivedMessage = shutMessage.ToString(); 
                if (receivedMessage.Equals(shut)) { 
                    this.delegarRecibirMensaje("--Desconectado--"); 
                    if (tcp != null) 
                        tcp.GetStream().Close(); 
                    if (tcpListener != null) 
                        tcpListener.Stop();
                    if (tcpListenerAux != null)
                        tcpListenerAux.Stop();
                    if (tcpListenerAux2 != null)
                        tcpListenerAux2.Stop();
                    if (tcp != null) 
                        tcp.Close(); 
                    salida = true; 
                } 
            } 
            return salida; 
        }

        private bool recibirNombreJugador(int tam, byte[] msg)
        {
            //Si me envian "#N ", estoy recibiendo el nombre del contrincante
            bool salida = false;
            StringBuilder NombMessage = new StringBuilder(3);
            for (int count = 0; count < 3; count++)
            {
                char ch = (char)msg[count];
                NombMessage = NombMessage.Append(ch);
            }
            string nomb = "#N ";
            string receivedMessage = NombMessage.ToString();
            if (receivedMessage.Equals(nomb))
            {
                //Nombre del Contrincante
                NombMessage = new StringBuilder(tam-4);
                for (int count = 3; count < tam; count++)
                {
                    char ch = (char)msg[count];
                    NombMessage = NombMessage.Append(ch);
                }
                receivedMessage = NombMessage.ToString();
                NombreContrincante = receivedMessage;
                salida = true;
            }
            return salida;
        }

        private bool recibirJugador(int tam, byte[] msg)
        {
            //Si me envian "#R " recivo el numero del personaje elegido
            bool salida = false;
            StringBuilder NombMessage = new StringBuilder(3);
            for (int count = 0; count < 3; count++)
            {
                char ch = (char)msg[count];
                NombMessage = NombMessage.Append(ch);
            }
            string nomb = "#R ";
            string receivedMessage = NombMessage.ToString();
            if (receivedMessage.Equals(nomb))
            {
                NombMessage = new StringBuilder(1);
                char pers = (char)msg[tam - 1];
                NombMessage = NombMessage.Append(pers);

                receivedMessage = NombMessage.ToString();
                PersonajeContrincante = int.Parse(receivedMessage.ToString());

                salida = true;
            }
            return salida;
        }

        private bool fasePartida(int tam, byte[] msg)
        {
            //Si me envian "#F " me toca atacar o defender
            bool salida = false;
            StringBuilder NombMessage = new StringBuilder(3);
            for (int count = 0; count < 3; count++)
            {
                char ch = (char)msg[count];
                NombMessage = NombMessage.Append(ch);
            }
            string nomb = "#F ";
            string receivedMessage = NombMessage.ToString();
            if (receivedMessage.Equals(nomb))
            {
                NombMessage = new StringBuilder(1);
                char pers = (char)msg[tam - 1];
                NombMessage = NombMessage.Append(pers);

                receivedMessage = NombMessage.ToString();
                fase = int.Parse(receivedMessage.ToString());

                salida = true;
            }
            return salida;
        }

        private bool turnoCliente(int tam, byte[] msg)
        {
            //Si me envian "#J " me toca empezar a atacar
            bool salida = false;
            StringBuilder NombMessage = new StringBuilder(3);
            for (int count = 0; count < 3; count++)
            {
                char ch = (char)msg[count];
                NombMessage = NombMessage.Append(ch);
            }
            string nomb = "#J ";
            string receivedMessage = NombMessage.ToString();
            if (receivedMessage.Equals(nomb))
            {
                NombMessage = new StringBuilder(1);
                char pers = (char)msg[tam - 1];
                NombMessage = NombMessage.Append(pers);

                receivedMessage = NombMessage.ToString();
                turnoCl = int.Parse(receivedMessage.ToString());

                salida = true;
            }
            return salida;
        }

        private bool vidaCliente(int tam, byte[] msg)
        {
            //Si me envian "#V " recivo la vida que le he quitado al contrincante
            bool salida = false;
            StringBuilder NombMessage = new StringBuilder(3);
            for (int count = 0; count < 3; count++)
            {
                char ch = (char)msg[count];
                NombMessage = NombMessage.Append(ch);
            }
            string nomb = "#V ";
            string receivedMessage = NombMessage.ToString();
            if (receivedMessage.Equals(nomb))
            {
                NombMessage = new StringBuilder(1);
                char pers = (char)msg[tam - 1];
                NombMessage = NombMessage.Append(pers);

                receivedMessage = NombMessage.ToString();
                vida = int.Parse(receivedMessage.ToString());

                salida = true;
            }
            return salida;
        }

        private bool movimientoCliente(int tam, byte[] msg)
        {
            //Si me envian "#M " recivo el movimiento que hace el contrincante
            bool salida = false;
            StringBuilder NombMessage = new StringBuilder(3);
            for (int count = 0; count < 3; count++)
            {
                char ch = (char)msg[count];
                NombMessage = NombMessage.Append(ch);
            }
            string nomb = "#M ";
            string receivedMessage = NombMessage.ToString();
            if (receivedMessage.Equals(nomb))
            {
                NombMessage = new StringBuilder(1);
                char pers = (char)msg[tam - 1];
                NombMessage = NombMessage.Append(pers);

                receivedMessage = NombMessage.ToString();
                movimiento = int.Parse(receivedMessage.ToString());

                salida = true;
            }
            return salida;
        }

        public class Constants { 
            public const int noOfBytesToReadForSuperPacket = 4; 
            public const int maxNoOfBytes = 784; 
        }
    }
}

