using System;
using System.Collections;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Forms;
namespace ProgettoPizza
{
    public partial class Form1 : Form
    {
        private ArrayList codaPizze = new ArrayList();
        private bool pizzaiolo1Occupato = false;
        private bool pizzaiolo2Occupato = false;

        Thread threadPizzaiolo1;
        Thread threadPizzaiolo2;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AggiornaStatoPizzaioli();
        }

        private void bottonePizza_Click(object sender, EventArgs e)
        {
            string nomePizza = (sender as Button).Text;

            if (!pizzaiolo1Occupato)
            {
                AvviaPizzaiolo(1, nomePizza);
            }
            else if (!pizzaiolo2Occupato)
            {
                AvviaPizzaiolo(2, nomePizza);
            }
            else
            {
                codaPizze.Add(nomePizza); 
                AggiornaCoda();
            }
        }

        private void AvviaPizzaiolo(int numeroPizzaiolo, string nomePizza)
        {
            if (numeroPizzaiolo == 1)
            {
                pizzaiolo1Occupato = true;
                threadPizzaiolo1 = new Thread(() => PreparaPizza(1, nomePizza));
                threadPizzaiolo1.Start();
            }
            else
            {
                pizzaiolo2Occupato = true;
                threadPizzaiolo2 = new Thread(() => PreparaPizza(2, nomePizza));
                threadPizzaiolo2.Start();
            }
            AggiornaStatoPizzaioli();
        }

        private void PreparaPizza(int numeroPizzaiolo, string nomePizza)
        {
            int tempoPreparazione = 0;
            if (nomePizza == "MARGHERITA")
                tempoPreparazione = 150;
            else
                tempoPreparazione = 180;

            for (int i = tempoPreparazione; i >= 0; i--)
            {
                if (numeroPizzaiolo == 1)
                {
                    Invoke(new Action(() => label7.Text = $"{i / 60:D2}:{i % 60:D2}"));
                }
                else
                {
                    Invoke(new Action(() => label8.Text = $"{i / 60:D2}:{i % 60:D2}"));
                }
                Thread.Sleep(1000);
            }

            Invoke(new Action(() =>
            {
                MessageBox.Show($"Il pizzaiolo {numeroPizzaiolo} ha finito di preparare la pizza {nomePizza}");
                if (numeroPizzaiolo == 1) pizzaiolo1Occupato = false;
                else pizzaiolo2Occupato = false;
                AggiornaStatoPizzaioli();
                Coda();
            }));
        }

        private void Coda()
        {
            if (codaPizze.Count > 0)
            {
                string prossimaPizza = (string)codaPizze[0]; 
                codaPizze.RemoveAt(0); 
                AggiornaCoda();

                if (!pizzaiolo1Occupato)
                {
                    AvviaPizzaiolo(1, prossimaPizza);
                }
                else if (!pizzaiolo2Occupato)
                {
                    AvviaPizzaiolo(2, prossimaPizza);
                }
            }
        }

        private void AggiornaStatoPizzaioli()
        {
            label5.Text = pizzaiolo1Occupato ? "Preparando una pizza" : "Libero";
            label6.Text = pizzaiolo2Occupato ? "Preparando una pizza" : "Libero";

            if (!pizzaiolo1Occupato) label7.Text = "00:00";
            if (!pizzaiolo2Occupato) label8.Text = "00:00";
        }

        private void AggiornaCoda()
        {
            listBox1.Items.Clear();
            foreach (string pizza in codaPizze)
            {
                listBox1.Items.Add(pizza); 
            }
        }

        private void bottoneSvuotaCoda_Click(object sender, EventArgs e)
        {
            codaPizze.Clear();
            AggiornaCoda();
        }

        private void bottoneRimuoviUltimo_Click(object sender, EventArgs e)
        {
            if (codaPizze.Count > 0)
            {
                codaPizze.RemoveAt(codaPizze.Count - 1); 
                AggiornaCoda();
            }
        }
    }

}


