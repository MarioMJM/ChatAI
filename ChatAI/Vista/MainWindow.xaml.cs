using ChatAI.VistaModelo;
using ModernWpf;
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatAI.Vista
{
    /// <summary>
    /// Lógica de interacción de MainWindow.xaml.
    /// Si bien se usa el patrón MVVM para manejar la lógica del programa, algunas
    /// funcionalidades se manejan desde esta clase por comodidad.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MainWindow"/>.
        /// Crea una instancia de <see cref="ChatViewModel"/> y la usa como contexto
        /// para la aplicación.
        /// Activa o desactiva el Switch del modo oscuro según el modo en el que empiece
        /// la aplicación.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ChatViewModel();

            //SwitchMode.IsOn = WindowHelper.GetUseModernWindowStyle(this);
            SwitchMode.IsOn = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
        }

        /// <summary>
        /// Abre una ventanita de configuración.
        /// </summary>
        /// <param name="sender">Fuente del evento.</param>
        /// <param name="e">La instancia de <see cref="RoutedEventArgs"/> que contiene la información del evento.</param>
        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            PopupConfig.IsOpen = !PopupConfig.IsOpen;
        }

        /// <summary>
        /// Abre una ventana flotante que advierte al usuario si quiere iniciar una nueva conversación.
        /// El usuario puede confirmar o cancelar la acción.
        /// </summary>
        /// <param name="sender">El objeto que lo invoca.</param>
        /// <param name="e">La instancia de <see cref="RoutedEventArgs"/> que contiene la información del evento.</param>
        private async void HandleNewChat(object sender, RoutedEventArgs e)
        {
            ContentDialog newChatDialog = new()
            {
                Title = "Iniciar nueva conversación",
                Content = "La conversación actual se borrará y no se guardará",
                PrimaryButtonText = "Aceptar",
                CloseButtonText = "Cancelar"
            };

            ContentDialogResult result = await newChatDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                DataContext = new ChatViewModel();
            }
        }

        /// <summary>
        /// Maneja la lógica del Switch del modo oscuro.
        /// </summary>
        /// <param name="sender">El objeto que lo invoca.</param>
        /// <param name="e">La instancia de <see cref="RoutedEventArgs"/> que contiene la información del evento.</param>
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
            if (toggleSwitch.IsOn)
            {
                //WindowHelper.SetUseModernWindowStyle(this, true);
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            } else
            {
                //WindowHelper.SetUseModernWindowStyle(this, false);
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }
        }
    }
}