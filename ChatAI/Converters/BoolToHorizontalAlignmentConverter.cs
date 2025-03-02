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
    /// Convertidor usado para determinar la posición de los mensajes según se trate del 
    /// usuario o de la IA. Si es un mensaje del usuario, devuelve HorizontalAlignment.Right;
    /// de lo contrario, HorizontalAlignment.Left.
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class BoolToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool esUsuario && esUsuario)
            {
                return HorizontalAlignment.Right;
            }
            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
