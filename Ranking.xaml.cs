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
using System.Xml;

namespace SSF
{

    public partial class Ranking : Window
    {
        MainWindow vPrin;
        string[] nombres = SSFRanking.getNombres();
        int[] puntuaciones = SSFRanking.getPuntuaciones();

        public Ranking(MainWindow vp)
        {
            InitializeComponent();
            vPrin = vp;
            cargarRanking();
        }

        void cargarRanking()
        {
            lbl1.Content = "1st";
            lbl2.Content = nombres[0];
			lbl3.Content = puntuaciones[0];
            
			lbl4.Content = "2nd";
			lbl5.Content = nombres[1];
            lbl6.Content = puntuaciones[1];

            lbl7.Content = "3th";
            lbl8.Content = nombres[2];
			lbl9.Content = puntuaciones[2];

            lbl10.Content = "4th";
            lbl11.Content = nombres[3];
			lbl12.Content = puntuaciones[3];

            lbl13.Content = "5th";
            lbl14.Content = nombres[4];
            lbl15.Content = puntuaciones[4];
			
            lbl16.Content = "6th";
            lbl17.Content = nombres[5];
            lbl18.Content = puntuaciones[5];

            lbl19.Content = "7th";
            lbl20.Content = nombres[6];
            lbl21.Content = puntuaciones[6];
			
            lbl22.Content = "8th";
            lbl23.Content = nombres[7];
            lbl24.Content = puntuaciones[7];

            lbl25.Content = "9th";
            lbl26.Content = nombres[8];
            lbl27.Content = puntuaciones[8];

            lbl28.Content = "10th";
            lbl29.Content = nombres[9];
            lbl30.Content = puntuaciones[9];
        }

        private void btAtras_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SimonSFRanking_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(vPrin.IsActive == false) vPrin.Show();
        }
    }
}
