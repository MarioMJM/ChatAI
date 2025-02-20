using ChatAI.Commands;
using ChatAI.Modelo;
using ChatAI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChatAI.VistaModelo
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly HttpClient _httpClient = new();
        private readonly SpeechToText speechToText = new SpeechToText();
        private readonly TextToSpeech textToSpeech = new TextToSpeech();
        private string _text;
        private bool _isSend;
        private bool _isRecording = false;
        private BitmapImage _iconSource;

        public ObservableCollection<Mensaje> MessageHistory { get; } = new ObservableCollection<Mensaje>();

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                    IsSend = !string.IsNullOrEmpty(_text);
                    IconSource = string.IsNullOrWhiteSpace(_text)
                        ? new BitmapImage(new Uri("pack://application:,,,/Resources/Images/mic_button.png"))
                        : new BitmapImage(new Uri("pack://application:,,,/Resources/Images/send_button.png"));
                    ((RelayCommand)ButtonClickedCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsSend
        {
            get => _isSend;
            set
            {
                if (_isSend != value)
                {
                    _isSend = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRecording
        {
            get => _isRecording;
            set
            {
                if (_isRecording != value)
                {
                    _isRecording = value;
                    OnPropertyChanged();
                }
            }
        }

        public BitmapImage IconSource
        {
            get => _iconSource;
            set
            {
                if (_iconSource != value)
                {
                    _iconSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ButtonClickedCommand { get; }
        public ICommand ReadMessageCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        public ChatViewModel()
        {
            ButtonClickedCommand = new RelayCommand(() => HandleButtonToggle(), () => true);
            ReadMessageCommand = new RelayCommandAdvanced<string>(ReadMessage, () => true);
            CopyToClipboardCommand = new RelayCommandAdvanced<string>(CopyToClipboard, () => true);
            IconSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/mic_button.png"));
        }

        private async void HandleButtonToggle()
        {
            if (IsSend)
            {
                await EnviarMensaje();
            }
            else
            {
                await RecordSpeech();
            }
        }

        private async Task EnviarMensaje()
        {
            var mensajeUsuario = new Mensaje { Contenido = Text, EsUsuario = true };
            MessageHistory.Add(mensajeUsuario);
            Text = string.Empty;
            IsSend = false;
            ((RelayCommand)ButtonClickedCommand).RaiseCanExecuteChanged();

            var requestBody = new
            {
                messages = new[]
                {
        new { content = "Me llamo Goyo y soy un asistente que habla en español.", role = "system" },
        new { content = mensajeUsuario.Contenido, role = "user" }
    },
                model = "llama3.2-1b-instruct",
                max_tokens = 2048
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:1337/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<ChatResponse>(responseBody);

                var mensajeBot = new Mensaje { Contenido = resultado.choices[0].message.content, EsUsuario = false };
                MessageHistory.Add(mensajeBot);
            }
            catch (Exception ex)
            {
                MessageHistory.Add(new Mensaje { Contenido = "Error en la respuesta: " + ex.Message, EsUsuario = false });
            }
        }

        private HttpContent CreateContent(Mensaje message)
        {
            var requestBody = new
            {
                messages = new[]
                {
                    new {
                        content = "Me llamo Goyo y soy un asistente que habla en español.",
                        role = "system"
                    },
                    new {
                        content = message.Contenido,
                        role = "user"
                    }
                },
                model = "llama3.2-1b-instruct",
                max_tokens = 2048
            };

            var json = JsonSerializer.Serialize(requestBody);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private TaskCompletionSource<string> _taskCompletitionSource;
        private StringBuilder _recognizedTextBuilder;

        private async Task RecordSpeech()
        {
            if (!IsRecording)
            {
                IsRecording = true;
                _taskCompletitionSource = new TaskCompletionSource<string>();
                _recognizedTextBuilder = new StringBuilder();

                speechToText.SpeechRecognized -= OnSpeechRecognized;
                speechToText.SpeechRecognized += OnSpeechRecognized;

                speechToText.StartRecognition();
            }
            else
            {
                speechToText.StopRecognition();

                _taskCompletitionSource.TrySetResult(_recognizedTextBuilder.ToString());
                string result = await _taskCompletitionSource.Task;
                Console.WriteLine($"Reconocido: {result}");
                Text = result;

                IsRecording = false;
            }
        }

        private void OnSpeechRecognized(string recognizedText)
        {
            if (IsRecording)
            {
                _recognizedTextBuilder.Append(recognizedText).Append(" ");
                Console.WriteLine($"Escuchado: {recognizedText}");
            }
        }

        private void ReadMessage(string message)
        {
            textToSpeech.ReadText(message);
        }

        private void CopyToClipboard(string content)
        {
            Clipboard.SetText(content);
        }
    }
}
