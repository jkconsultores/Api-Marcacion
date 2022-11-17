using Asistencia_apirest.Entidades;
using Empleado_apirest.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DemoAPI.Models
{
    public class SampleContext : DbContext
    {
        public SampleContext(DbContextOptions options) : base(options)
        {
        }
        
        private readonly string? _connectionString;
        public SampleContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString != null)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
  
        public DbSet<Asistencia> Asistencia { get; set; }
        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Local> Local { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Usuario_local> Usuario_local { get; set; }
    }
}