using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatAI.Commands
{
    /// <summary>
    /// Implementación de ICommand que define un comando sin parámetros.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        /// <summary>
        /// Se dispara cuando cambia la disponibilidad del comando.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Inicializa una nueva instancia de RelayCommand.
        /// </summary>
        /// <param name="execute">Acción que se ejecuta.</param>
        /// <param name="canExecute">Función opcional que determina si el comando puede ejecutarse.</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determina si el comando se puede ejecutar.
        /// </summary>
        /// <param name="parameter">Parámetro opcional.</param>
        /// <returns>True si el comando puede ejecutarse, False en caso contrario.</returns>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        /// <summary>
        /// Ejecuta la acción asociada al comando.
        /// </summary>
        /// <param name="parameter">Parámetro opcional.</param>
        public void Execute(object parameter) => _execute();

        /// <summary>
        /// Notifica a los suscriptores que el estado de CanExecute ha cambiado.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }


    /// <summary>
    /// Implementación de ICommand que define un comando con parámetros.
    /// </summary>
    /// <typeparam name="T">Tipo del parámetro que recibe el comando.</typeparam>
    public class RelayCommandAdvanced<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<bool> _canExecute;
        /// <summary>
        /// Se dispara cuando cambia la disponibilidad del comando.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Inicializa una nueva instancia de RelayCommandAdvanced.
        /// </summary>
        /// <param name="execute">Acción que ejecuta con un parámetro de tipo T.</param>
        /// <param name="canExecute">Función opcional que determina si el comando se puede ejecutar.</param>
        public RelayCommandAdvanced(Action<T> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determina si el comando se puede ejecutar.
        /// </summary>
        /// <param name="parameter">Parámetro opcional del tipo T.</param>
        /// <returns>True si el comando puede ejecutarse, False en caso contrario.</returns>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        /// <summary>
        /// Ejecuta la acción asociada al comando con el parámetro especificado.
        /// </summary>
        /// <param name="parameter">Parámetro de tipo T.</param>
        public void Execute(object parameter) => _execute((T)parameter);

        /// <summary>
        /// Notifica a los suscriptores que el estado de CanExecute ha cambiado.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
