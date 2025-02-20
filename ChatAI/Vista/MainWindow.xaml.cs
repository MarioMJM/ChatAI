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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ChatViewModel();

            //SwitchMode.IsOn = WindowHelper.GetUseModernWindowStyle(this);
            SwitchMode.IsOn = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
        }

        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            PopupConfig.IsOpen = !PopupConfig.IsOpen;
        }

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