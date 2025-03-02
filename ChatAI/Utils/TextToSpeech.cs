using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace ChatAI.Utils
{
    /// <summary>
    /// Clase que implementa funcionalidad para leer texto en voz alta.
    /// </summary>
    public class TextToSpeech
    {
        private SpeechSynthesizer Synthesizer;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TextToSpeech"/>.
        /// El metodo inicializa el sintetizador de voz.
        /// </summary>
        public TextToSpeech()
        {
            InitializeSpeechSynthesiser();
        }

        /// <summary>
        /// Inicializa el sintetizador de voz con volumen de voz y la velocidad de lectura.
        /// </summary>
        private void InitializeSpeechSynthesiser()
        {
            Synthesizer = new SpeechSynthesizer
            {
                Volume = 100,
                Rate = 0
            };
        }

        /// <summary>
        /// Lee el texto pasado por parámetro.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ReadText(string text)
        {
            if (Synthesizer == null)
            {
                InitializeSpeechSynthesiser();
            }

            Synthesizer.SpeakAsync(text);
        }
    }
}
