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
    /// Convertidor usado para alternar la visibilidad de los elementos de un mensaje de la IA.
    /// Cuando el usuario escribe un mensaje se espera una respuesta, y mientras esta carga se muestra
    /// un GIF para simular que la IA piensa. Se manejan también los botones de control de cada mensaje.
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverse = parameter as string == "True";
            bool isEmpty = string.IsNullOrEmpty(value as string);
            return isEmpty ? (inverse ? Visibility.Visible : Visibility.Collapsed)
                           : (inverse ? Visibility.Collapsed : Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
