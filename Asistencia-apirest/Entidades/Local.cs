using System.ComponentModel.DataAnnotations;

namespace Asistencia_apirest.Entidades
{
    public class Local
    {   [Key]
        public int id { get; set; }
        public string? descripcion { get; set; }
        public int ruc { get; set; }
    }
}
