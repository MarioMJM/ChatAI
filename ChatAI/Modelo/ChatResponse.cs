using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAI.Modelo
{
    /// <summary>
    /// Clase envoltorio que contiene una matriz de elecciones de la clase <see cref="Choice"/>
    /// </summary>
    public class ChatResponse
    {
        public Choice[] choices { get; set; }
    }

    /// <summary>
    /// Clase que contiene un mensaje de la clase <see cref="Message"/>.
    /// </summary>
    public class Choice
    {
        public Message message { get; set; }
    }

    /// <summary>
    /// Clase envoltorio para mensajes.
    /// </summary>
    public class Message
    {
        public string content { get; set; }
    }
}
