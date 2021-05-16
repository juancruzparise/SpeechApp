using System;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Data.OleDb;


namespace SpeechApp
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine reconocedor = new SpeechRecognitionEngine();

        int RecTimeOut = 0;
        bool escuchando = false;
        string userName = Environment.UserName;

        public Form1()
        {
            InitializeComponent();
        }

        //Cuando empieza a escuchar setea el temporizador en 0
        private void reconocedor_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            RecTimeOut = 0;
        }

        private void Default_SpeechRecognizer(object sender, SpeechRecognizedEventArgs e)
        {
            lblTexto.Text = "Usted quiso decir esto?: " + e.Result.Text;
            string speech = e.Result.Text.ToLower();

            if (speech.Contains("buscar"))
            {
                string palabraClave = speech.Replace("buscar", "");
                buscar(palabraClave);
            }

            if (speech.Contains("abrir"))
            {
                string palabraClaveEspaciada = speech.Replace("abrir", "");
                string palabraClave = palabraClaveEspaciada.Replace(" ", "");
                abrir(palabraClave);              
            }
        }

        private void abrir(string palabraClave)
        {
            //Utilizamos la API de OleDB que se indexa al sitema operativo y nos permite realizar consultas SQL sobre la tabla SystemIndex
            var connection = new OleDbConnection(@"Provider=Search.CollatorDSO;Extended Properties=""Application=Windows""");

            //Creo la consulta limitando el tipo de item que busco y donde se ubica
            var query = @"SELECT TOP 1 System.ItemUrl FROM SystemIndex " +
             @"WHERE scope = 'file:C:/Users/" + userName + "' AND System.ItemType = 'Directory' AND System.Itemname LIKE '%" + palabraClave + "%' ";

            connection.Open();

            var command = new OleDbCommand(query, connection);

            using (var r = command.ExecuteReader())
            {
                while (r.Read())
                {
                    System.Diagnostics.Process.Start(@"" + r[0]);
                }
            }

            connection.Close();
        }

        private void buscar(string palabraClave)
        {
            Console.WriteLine(palabraClave);
            string url = "https://www.google.com/search?q=" + palabraClave;
            System.Diagnostics.Process.Start(url);
        }

        //Limita el tiempo de escucha a 10 segundos
        private void TmrSpeaking_Tick(object sender, EventArgs e)
        {
            if (RecTimeOut == 10)
            {
                reconocedor.RecognizeAsyncCancel();
            }
            else if (RecTimeOut == 10)
            {
                TmrSpeaking.Stop();
                RecTimeOut = 0;
            }
        }

        //Activa y desactiva la escucha manualmente
        private void btnSwitch_Click(object sender, EventArgs e)
        {
            if (!escuchando)
            {
                reconocedor.RecognizeAsync(RecognizeMode.Multiple);
                this.btnSwitch.Text = "Desactivar";
                escuchando = true;

            }
            else
            {
                reconocedor.RecognizeAsyncCancel();
                this.btnSwitch.Text = "Activar";
                this.lblTexto.Text = "";
                escuchando = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Define el microfono default como dispositivo de escucha
            reconocedor.SetInputToDefaultAudioDevice();
            //Le digo que palabras debe reconocer
            reconocedor.LoadGrammarAsync(new DictationGrammar());
            //Funcion por default mientras escucha
            reconocedor.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognizer);
            //Funcion cuando empieza a escuchar
            reconocedor.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(reconocedor_SpeechRecognized);
        }
    }
}
