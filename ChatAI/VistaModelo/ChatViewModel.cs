using ChatAI.Commands;
using ChatAI.Modelo;
using ChatAI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly TextToSpeech textToSpeech = new();
        private TaskCompletionSource<string> _taskCompletitionSource;
        private StringBuilder _recognizedTextBuilder;
        private string _text;
        private bool _isSend;
        private bool _isRecording = false;
        private int _audioLevel = 0;
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
                        ? new BitmapImage(new Uri("pack://application:,,,/Resources/Images/mic_button_w.png"))
                        : new BitmapImage(new Uri("pack://application:,,,/Resources/Images/send_button_w.png"));
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

        public int AudioLevel
        {
            get => _audioLevel;
            set
            {
                _audioLevel = value;
                OnPropertyChanged();
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
        public ICommand EnterKeyCommand { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ChatViewModel"/>.
        /// Esto inicializa a su vez los comandos asociados a los controles de la ventana
        /// y el icono del botón principal.
        /// </summary>
        public ChatViewModel()
        {
            ButtonClickedCommand = new RelayCommand(() => HandleButtonToggle(), () => true);
            ReadMessageCommand = new RelayCommandAdvanced<string>(ReadMessage, () => true);
            CopyToClipboardCommand = new RelayCommandAdvanced<string>(CopyToClipboard, () => true);
            IconSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/mic_button_w.png"));
            EnterKeyCommand = new RelayCommandAdvanced<KeyEventArgs>(ExecuteEnterKey, () => true);
        }

        /// <summary>
        /// Simula una pulsación de la tecla Intro. El método sirve para enviar
        /// el texto que introduce el usuario al pulsar Intro.
        /// </summary>
        /// <param name="e">El parametro <see cref="KeyEventArgs"/> contiene la información del evento.</param>
        private void ExecuteEnterKey(KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(Text))
            {
                _ = SendMessage();
            }
        }

        /// <summary>
        /// Método que alterna la funcionalidad del botón principal entre enviar un mensaje
        /// o grabar el audio del usuario. Cuando la caja de texto está vacía, el método
        /// por defecto es <see cref="RecordSpeech"/>. Cuando el usuario introduce texto
        /// o graba audio, el cual se muestra en la caja de texto, se cambia a <see cref="SendMessage"/>.
        /// </summary>
        private async void HandleButtonToggle()
        {
            if (IsSend)
            {
                await SendMessage();
            }
            else
            {
                await RecordSpeech();
            }
        }

        /// <summary>
        /// Método que toma el contenido de la caja de texto, crea una solicitud HTTP, recibe
        /// una respuesta y finalmente muestra el mensaje del usuario y la respuesta en la lista
        /// de mensajes. El método también limpia la caja de texto y notifica el cambio.
        /// </summary>
        private async Task SendMessage()
        {
            var mensajeUsuario = new Mensaje { Contenido = Text, EsUsuario = true };
            MessageHistory.Add(mensajeUsuario);

            var mensajeBotLoading = new Mensaje { Contenido = "", EsUsuario = false };
            MessageHistory.Add(mensajeBotLoading);

            int loadingIndex = MessageHistory.IndexOf(mensajeBotLoading);

            Text = string.Empty;
            IsSend = false;
            ((RelayCommand)ButtonClickedCommand).RaiseCanExecuteChanged();

            HttpContent httpContent = CreateContent(mensajeUsuario);

            try
            {
                var response = await _httpClient.PostAsync(Settings.Default.RequestUri, httpContent);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                Trace.WriteLine("Respuesta recibida");
                var resultado = JsonSerializer.Deserialize<ChatResponse>(responseBody);

                var mensajeBot = new Mensaje { Contenido = resultado.choices[0].message.content, EsUsuario = false };

                if (loadingIndex >= 0)
                {
                    MessageHistory[loadingIndex] = mensajeBot;
                }
            }
            catch (Exception ex)
            {
                if (loadingIndex >= 0)
                {
                    MessageHistory[loadingIndex] = new Mensaje { Contenido = "Error en la respuesta: " + ex.Message, EsUsuario = false };
                }
            }
        }

        /// <summary>
        /// Crea el contenido de una solicitud Http dado el mensaje del usuario.
        /// Usa las variables contenidas en Settings.settings para describir los roles
        /// y las instrucciones de la solicitud./>
        /// </summary>
        /// <param name="message">El mensaje del usuario.</param>
        /// <returns>Devuelve el contenido serializado de la solicitud Http.</returns>
        private HttpContent CreateContent(Mensaje message)
        {
            var requestBody = new
            {
                messages = new[]
                {
                    new {
                        content = Settings.Default.Instructions,
                        role = "system"
                    },
                    new {
                        content = message.Contenido,
                        role = "user"
                    }
                },
                model = Settings.Default.Model,
                max_tokens = 2048
            };

            var json = JsonSerializer.Serialize(requestBody);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Inicia o detiene el reconocimiento de voz. 
        /// Si no está grabando, comienza la captura de audio y procesa el reconocimiento.
        /// Si ya está grabando, detiene el reconocimiento y devuelve el texto capturado.
        /// </summary>
        private async Task RecordSpeech()
        {
            if (!IsRecording)
            {
                IsRecording = true;
                _taskCompletitionSource = new TaskCompletionSource<string>();
                _recognizedTextBuilder = new StringBuilder();

                speechToText.SpeechRecognized -= OnSpeechRecognized;
                speechToText.SpeechRecognized += OnSpeechRecognized;

                speechToText.AudioLevelUpdated -= OnAudioLevelUpdated;
                speechToText.AudioLevelUpdated += OnAudioLevelUpdated;

                speechToText.StartRecognition();
            }
            else
            {
                speechToText.StopRecognition();

                _taskCompletitionSource.TrySetResult(_recognizedTextBuilder.ToString());
                string result = await _taskCompletitionSource.Task;
                Text = result;

                IsRecording = false;
            }
        }

        /// <summary>
        /// Acumula el texto reconocido. Se hace de esta manera para que todo el texto reconocido
        /// se muestre al apagar el micrófono.
        /// </summary>
        /// <param name="recognizedText">Texto reconocido por el sistema de voz.</param>
        private void OnSpeechRecognized(string recognizedText)
        {
            if (IsRecording)
            {
                _recognizedTextBuilder.Append(recognizedText).Append(" ");
            }
        }

        /// <summary>
        /// Maneja el evento de actualización del nivel de audio. Lo muestra en la interfaz
        /// a través de una barra de progreso con ayuda del convertidor AudioLevelToOpacityConverter.
        /// </summary>
        /// <param name="audioLevel">Nivel actual del audio capturado.</param>
        private void OnAudioLevelUpdated(int audioLevel)
        {
            AudioLevel = audioLevel;
        }

        /// <summary>
        /// Reproduce el mensaje utilizando el sistema de conversión de texto a voz (TTS).
        /// </summary>
        /// <param name="message">El mensaje que se leerá en voz alta.</param>
        private void ReadMessage(string message)
        {
            textToSpeech.ReadText(message);
        }

        /// <summary>
        /// Copia el contenido especificado al portapapeles del usuario.
        /// </summary>
        /// <param name="content">El contenido que se copia.</param>
        private void CopyToClipboard(string content)
        {
            Clipboard.SetText(content);
        }
    }
}
