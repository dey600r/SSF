using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SSF
{
    public class Jugador
    {
        private string nombre;
        private int puntuacion;
        private int vida;
        private Image retrato;
        private int id;
        private int fuerza;
        private int defensa;
        private int agilidad;
        private string personaje;

        public Jugador(string n, Image retrato1, int ident, int fuerza1, int defensa1, int agilidad1, string personaje1)
        {
            this.nombre = n;
            this.retrato = retrato1;
            this.id = ident;
            this.fuerza = fuerza1;
            this.defensa = defensa1;
            this.agilidad = agilidad1;
            this.vida = 100;
            this.puntuacion = 0;
            this.personaje = personaje1;
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public Image Retrato
        {
            get { return retrato; }
            set { retrato = value; }
        }

        public int Puntuacion
        {
            get { return puntuacion; }
            set { puntuacion = value; }
        }

        public int Vida
        {
            get { return vida; }
            set { vida = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Fuerza
        {
            get { return fuerza; }
            set { fuerza = value; }
        }

        public int Defensa
        {
            get { return defensa; }
            set { defensa = value; }
        }

        public int Agilidad
        {
            get { return agilidad; }
            set { agilidad = value; }
        }

        public string Personaje
        {
            get { return personaje; }
            set { personaje = value; }
        }

    }
}
