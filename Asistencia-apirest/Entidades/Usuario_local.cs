using System.ComponentModel.DataAnnotations;

namespace Asistencia_apirest.Entidades
{
    public class Usuario_local
    {   [Key]
        public int usuarioid { get; set; }
        
        public int localid { get; set; }
    }
}
