using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace ChatAI.Converters
{
    /// <summary>
    /// Convertidor usado para alternar la visibilidad del mensaje de ayuda "Habla conmigo" de 
    /// la caja de texto principal.
    /// Si hay texto, lo hace invisible.
    /// Si no hay texto, lo hace visible.
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class TextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
