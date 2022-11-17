using System.ComponentModel.DataAnnotations;

namespace Asistencia_apirest.Entidades
{
    public class Empresa
    {
        [Key]
        public int id { get; set; }
        [MaxLength(50)]
        public string? descripcion { get; set; }
        public int ruc { get; set; }
        [MaxLength(100)]
        public string? direccion { get; set; }
        public int telefono { get; set; }
        [MaxLength(100)]
        public string? cadenaconexion { get; set; }
        public int grupo { get; set; }
        public string? app { get; set; }
    }
}
