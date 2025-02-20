using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace ChatAI.Utils
{
    public class SpeechToText
    {
        private SpeechRecognitionEngine SpeechRecognition;
        public event Action<string> SpeechRecognized;
        public event Action<string> SpeechHypothesized;

        public SpeechToText()
        {
            InitializeRecognizerSynthesizer();
        }

        private void InitializeRecognizerSynthesizer()
        {
            RecognizerInfo selectedRecognizer = SpeechRecognitionEngine.InstalledRecognizers().FirstOrDefault(r => r.Culture.Name == "es-ES");
            //RecognizerInfo selectedRecognizer = (from o in SpeechRecognitionEngine.InstalledRecognizers() where o.Culture.Equals(Thread.CurrentThread.CurrentCulture) select o).FirstOrDefault();
            SpeechRecognition = new SpeechRecognitionEngine(selectedRecognizer);
            SpeechRecognition.AudioStateChanged += RecognizerAudioStateChanged;
            SpeechRecognition.SpeechHypothesized += RecognizerSpeechHypothesized;
            SpeechRecognition.SpeechRecognized += RecognizerSpeechRecognized;
            SpeechRecognition.LoadGrammar(new DictationGrammar());
            SpeechRecognition.SetInputToDefaultAudioDevice();

            /*
            foreach (var recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                Console.WriteLine($"Language: {recognizer.Culture}, Name: {recognizer.Description}");
            }
            */
        }

        public void StartRecognition()
        {
            if (SpeechRecognition != null)
            {
                SpeechRecognition.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        public void StopRecognition()
        {
            if (SpeechRecognition != null)
            {
                SpeechRecognition.RecognizeAsyncStop();
            }
        }

        private void RecognizerAudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            Console.WriteLine($"Estado: {e.AudioState}");
        }

        private void RecognizerSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            string hypothesizedText = e.Result.Text;
            SpeechHypothesized?.Invoke(hypothesizedText);
        }

        private void RecognizerSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;
            SpeechRecognized?.Invoke(recognizedText);
        }
    }
}
