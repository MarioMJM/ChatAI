using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace ChatAI.Utils
{
    public class TextToSpeech
    {
        private SpeechSynthesizer Synthesizer;

        public TextToSpeech()
        {
            InitializeSpeechSynthesiser();
        }

        private void InitializeSpeechSynthesiser()
        {
            Synthesizer = new SpeechSynthesizer
            {
                Volume = 100,
                Rate = 0
            };
        }

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
