using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;

namespace SpeechApp
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine reconocedor = new SpeechRecognitionEngine();

        int RecTimeOut = 0;
        bool escuchando = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void reconocedor_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            RecTimeOut = 0;
        }

        private void Default_SpeechRecognizer(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;
            if (speech == "Browser" || speech == "browser")
            {
                System.Diagnostics.Process.Start("chrome.exe");
            }
            if (speech == "Carpeta de fotos" || speech == "carpeta de fotos")
            {
               System.Diagnostics.Process.Start(@"C:\Users\JuanC\Desktop\Carpeta_test");
            }
            this.lblTexto.Text = speech;
        }

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
            reconocedor.SetInputToDefaultAudioDevice();
            reconocedor.LoadGrammarAsync(new DictationGrammar());
            reconocedor.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognizer);
            reconocedor.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(reconocedor_SpeechRecognized);
            //reconocedor.RecognizeAsync(RecognizeMode.Multiple);
        }
    }
}
