using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAI.Modelo
{
    /// <summary>
    /// Clase envoltorio para los mensajes del usuario y la IA.
    /// </summary>
    public class Mensaje
    {
        public string Contenido { get; set; }
        public bool EsUsuario { get; set; }
    }
}
