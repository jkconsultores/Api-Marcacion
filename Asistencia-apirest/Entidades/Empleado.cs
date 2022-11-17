using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Empleado_apirest.Entidades
{
    public class Empleado
    {
        [Key]
        public int id { get; set; }
        public string? nombre { get; set; }
        public string? num_doc { get; set; }
        public string? tipo_doc { get; set; }
        public int? codigo { get; set; } 
        public int? local { get; set; }
        public Boolean? activo { get; set; }
    }
}
