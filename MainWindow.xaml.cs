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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SSF
{
    
    public partial class MainWindow : Window
    {
        Ranking vRanking;
        ElegirJugador vElegirJugador;
        vOnline online;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btLocal_Click(object sender, RoutedEventArgs e)
        {
            vElegirJugador = new ElegirJugador(this);
            vElegirJugador.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void btOnline_Click(object sender, RoutedEventArgs e)
        {
            online = new vOnline(this);
            online.Show();
            this.Visibility = Visibility.Hidden;
        }
       
        private void btRanking_Click(object sender, RoutedEventArgs e)
        {
            vRanking = new Ranking(this);
            vRanking.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void btSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
