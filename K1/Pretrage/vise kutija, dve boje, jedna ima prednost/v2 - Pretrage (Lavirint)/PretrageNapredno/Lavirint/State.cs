using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

/*
 *  Imamo kutije narandzaste i plave boje 
 */
namespace Lavirint
{
    public class State
    {
        // moguce je kretanje po jedno polje u bilo kom pravcu, pa i po dijagonali
        private static int[,] koraci = { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };

        public static int[,] lavirint;
        State parent;

        public int markI, markJ; //vrsta i kolona
        public double cost;
        private Dictionary<Point, bool> kutijeN = new Dictionary<Point, bool>();
        private Dictionary<Point, bool> kutijeP = new Dictionary<Point, bool>();
        int blue;
        int orange;


        public State()
        {
            foreach (Point point in Main.obaveznaStanjaNar)
            {
                // treba postaviti sve kutije na false jer jos nijedna nije pokupljena
                kutijeN.Add(point, false);
            }

            foreach (Point point in Main.obaveznaStanjaPlav)
            {
                // treba postaviti sve kutije na false jer jos nijedna nije pokupljena
                kutijeP.Add(point, false);
            }
            this.blue = 0;
            this.orange = 0;
        }

        public State sledeceStanje(int markI, int markJ)
        {
            State rez = new State();
            rez.markI = markI;
            rez.markJ = markJ;
            rez.parent = this;
            rez.cost = this.cost + 1;
            rez.orange = this.orange;
            rez.blue = this.blue;

            bool nar = true;

            foreach (KeyValuePair<Point, bool> entry in this.kutijeN)
            {
                rez.kutijeN[entry.Key] = entry.Value;
            }

            foreach (KeyValuePair<Point, bool> entry in this.kutijeP)
            {
                rez.kutijeP[entry.Key] = entry.Value;
            }

            if (lavirint[markI, markJ] == 4)
            {
                bool vec_obradjen = false;
                foreach (KeyValuePair<Point, bool> entry in this.kutijeN)
                {

                    if (entry.Key.X == markI && entry.Key.Y == markJ && entry.Value == true)
                    {
                        vec_obradjen = true;
                    }

                }

                if (!vec_obradjen)
                {

                    foreach (KeyValuePair<Point, bool> entry in this.kutijeN)
                    {

                        if (entry.Key.X == markI && entry.Key.Y == markJ)
                        {
                            rez.kutijeN[entry.Key] = true;
                            rez.orange++;
                        }

                    }
                }

            }

            // provera da li smo prvo pokupili sve narandzaste kutije
            foreach (KeyValuePair<Point, bool> narKutija in this.kutijeN)
            {
                if (this.orange == 2)
                {
                    nar = true;
                    break;
                }

                if (this.kutijeN[narKutija.Key] == false)
                {
                    // ako se nadje bar jedna narandzasta koja nije pokupljena, moramo sacekati sa kupljenjem plavih
                    nar = false;
                    break;
                }
            }

            if (nar)
            {
                if (lavirint[markI, markJ] == 5)
                {
                    bool vec_obradjen = false;
                    foreach (KeyValuePair<Point, bool> entry in this.kutijeP)
                    {

                        if (entry.Key.X == markI && entry.Key.Y == markJ && entry.Value == true)
                        {
                            vec_obradjen = true;
                        }

                    }
                    // ako se naidje na polje gde je plava kutija, treba je pokupiti
                    if (!vec_obradjen)
                    {
                        foreach (KeyValuePair<Point, bool> entry in this.kutijeP)
                        {
                            if (entry.Key.X == markI && entry.Key.Y == markJ)
                            {
                                rez.kutijeP[entry.Key] = true;
                                rez.blue++;
                            }

                        }
                    }
                }
            }

            return rez;
        }

        public List<State> mogucaSledecaStanja()
        {
            //TODO1: Implementirati metodu tako da odredjuje dozvoljeno kretanje u lavirintu
            //TODO2: Prosiriti metodu tako da se ne moze prolaziti kroz sive kutije
            List<State> rez = new List<State>();

            for (int i = 0; i < koraci.GetLength(0); ++i)
            {
                int novoI = this.markI + koraci[i, 0];
                int novoJ = this.markJ + koraci[i, 1];

                if (novoI < 0 || novoI >= Main.brojVrsta || novoJ < 0 || novoJ >= Main.brojKolona)
                {
                    // ovaj potez nije validan
                    continue;
                }

                if (lavirint[novoI, novoJ] == 1)
                {
                    // potez nije validan jer je naredno polje zapravo zid
                    continue;
                }

                State dozvoljenoStanje = sledeceStanje(novoI, novoJ);
                rez.Add(dozvoljenoStanje);
            }

            return rez;
        }

        public override int GetHashCode()
        {
            int i = 128; // 1 000 000
            int key = 10 * this.markI + this.markJ;

            foreach (KeyValuePair<Point, bool> entry in this.kutijeP)
            {
                if (entry.Value == true)
                {
                    key = key | i;
                }
                i <<= 1;
            }

            i = 8192; // 10 000 000 000 000
            foreach (KeyValuePair<Point, bool> entry in this.kutijeN)
            {
                if (entry.Value == true)
                {
                    key = key | i;
                }
                i <<= 1;
            }

            return key;
        }

        public bool isKrajnjeStanje()
        {
            return Main.krajnjeStanje.markI == markI && Main.krajnjeStanje.markJ == markJ && this.blue == 1 && this.orange == 2;
        }

        public List<State> path()
        {
            List<State> putanja = new List<State>();
            State tt = this;
            while (tt != null)
            {
                putanja.Insert(0, tt);
                tt = tt.parent;
            }
            return putanja;
        }
    }
}