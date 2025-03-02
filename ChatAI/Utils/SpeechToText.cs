using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using Vosk;
using System.IO;

namespace ChatAI.Utils
{
    /// <summary>
    /// Clase que contiene la implementación de las funcionalidades
    /// para pasar de voz a texto. Contiene, además, la funcionalidad
    /// para calcular el nivel del audio.
    /// </summary>
    class SpeechToText
    {
        private WaveInEvent? Microphone;
        private VoskRecognizer? Recognizer;
        private Model? VoskModel;

        public event Action<string> SpeechRecognized;
        public event Action<string> SpeechHypothesized;
        public event Action<int> AudioLevelUpdated;

        private readonly string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Vosk", "vosk-model-small-es-0.42");

        /// <summary>
        /// Inicializa una instancia de la clase <see cref="SpeechToText"/>.
        /// </summary>
        public SpeechToText()
        {
            LoadVoskModel();
        }

        /// <summary>
        /// Inicia el reconocimiento de voz.
        /// </summary>
        public void StartRecognition()
        {
            if (Microphone == null)
            {
                Microphone = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(16000, 1)
                };

                Microphone.DataAvailable += OnDataAvailable;
                Microphone.StartRecording();
            }
        }

        /// <summary>
        /// Detiene el reconocimiento de voz.
        /// </summary>
        public void StopRecognition()
        {
            if (Microphone != null)
            {
                Microphone.DataAvailable -= OnDataAvailable;
                Microphone.StopRecording();
                Microphone.Dispose();
                Microphone = null;
            }
        }

        /// <summary>
        /// Maneja el evento de disponibilidad de datos de audio del micrófono.
        /// Procesa el audio entrante y lo envía al reconocedor de voz de Vosk.
        /// </summary>
        /// <param name="sender">El origen del evento.</param>
        /// <param name="e">Datos del evento que contienen el búfer de audio y la cantidad de bytes grabados.</param>
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (Recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                var result = JObject.Parse(Recognizer.Result())["text"]?.ToString();
                if (!string.IsNullOrEmpty(result))
                {
                    SpeechRecognized?.Invoke(result);
                }
            }
            else
            {
                var partialResult = JObject.Parse(Recognizer.PartialResult())["partial"]?.ToString();
                if (!string.IsNullOrEmpty(partialResult))
                {
                    SpeechHypothesized?.Invoke(partialResult);
                }
            }

            int audioLevel = CalculateAudioLevel(e.Buffer, e.BytesRecorded);
            AudioLevelUpdated?.Invoke(audioLevel);
        }

        /// <summary>
        /// Calcula el nivel de audio basado en los valores de amplitud de la señal de audio capturada.
        /// Se utiliza para dar al usuario un indicador visual cuando se detecta su voz.
        /// </summary>
        /// <param name="buffer">El búfer de audio con los datos capturados.</param>
        /// <param name="bytesRecorded">La cantidad de bytes grabados en el búfer.</param>
        /// <returns>Un valor entero representando el nivel de audio.</returns>
        private int CalculateAudioLevel(byte[] buffer, int bytesRecorded)
        {
            int sum = 0;
            for (int i = 0; i < bytesRecorded; i += 2)
            {
                short sample = (short)(buffer[i] | (buffer[i + 1] << 8));
                sum += Math.Abs(sample);
            }
            return (sum / (bytesRecorded / 2)) / 100;
        }

        /// <summary>
        /// Carga el modelo de reconocimiento de voz de Vosk y configura el reconocedor.
        /// </summary>
        private void LoadVoskModel()
        {
            try
            {
                VoskModel = new Model(modelPath);
                Recognizer = new VoskRecognizer(VoskModel, 16000.0f);
                Recognizer.SetMaxAlternatives(0);
                Recognizer.SetWords(true);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el modelo de Vosk: " + ex.Message);
            }
        }
    }
}
