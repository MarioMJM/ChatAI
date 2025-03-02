using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatAI.Converters
{
    /// <summary>
    /// Convertidor usado para determinar el color del contenedor de los mensajes. Si es un 
    /// mensaje del usuario, devuelve el primer color Color.FromRgb(75, 0, 130).
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new SolidColorBrush(Color.FromRgb(75, 0, 130)) : new SolidColorBrush(Color.FromRgb(30, 30, 30));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
