using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatAI.VistaModelo
{
    /// <summary>
    /// Clase personalizada que implementa <see cref="INotifyPropertyChanged"/>.
    /// Se usa como base para el modelo MVVM usado en ChatViewModel.
    /// Implementa un método <see cref="OnPropertyChanged(string)"/> para notificar actualizaciones.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Método llamado cuando hay cambios. Actualiza la interfaz del usuario.
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad cambiada.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
