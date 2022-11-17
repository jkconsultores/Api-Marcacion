
using Asistencia_apirest.Entidades;
using Asistencia_apirest.services;
using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace DemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciaController : ControllerBase
    {
        private SampleContext _context;
        private cifrado _cifrado;
        private util _util;

        public AsistenciaController(SampleContext context, cifrado cifrado_, util util_)
        {
            _context = context;
            _cifrado = cifrado_;
            _util = util_;
        }
        //listar asis
        [HttpGet]
        public async Task<IActionResult> GetAsistenciasAsync(string token)
        { var vtoken = _cifrado.validarToken(token);
            if (vtoken == null)
            {
                return Problem("El token no es valido!");
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(x => x.descripcion == vtoken[0]&&x.app.Equals("MARCACION"));
            if (empresa == null) {
                return Problem("La empresa ingresada no es válida.");
            } if (empresa.cadenaconexion == null) {
                return Problem("La empresa ingresada no es válida.");
            }
            using (var context = new SampleContext(empresa.cadenaconexion))
            { var usuario = await context.Usuario.FirstOrDefaultAsync(res => res.nombreusuario.Equals(vtoken[1]) && res.contrasena.Equals(vtoken[2]));
                if (usuario == null)
                {
                    return Problem("El usuario ingresado no es valido");
                }
                var usuario_locales = await context.Usuario_local.Where(res => res.usuarioid.Equals(usuario.usuarioid)).ToListAsync();
                if (usuario_locales == null)
                {
                    return Problem("No hay locales asignados");
                }
                int[] locales = _util.convertirArray(usuario_locales);
                DateTime fecha = DateTime.Now;
                int mes = fecha.Month;
                int year = fecha.Year;
                int day = fecha.Day;
                var listado = await (from a in context.Asistencia
                                     join sa in context.Empleado on a.cod_empleado equals sa.codigo
                                     join local in context.Local on sa.local equals local.id
                                     where a.fecha.Month.Equals(mes) && locales.Contains(local.id) && a.fecha.Year.Equals(year) && a.fecha.Day.Equals(day)
                                     select new
                                     {
                                         a.id,
                                         a.cod_empleado,
                                         a.fecha,
                                         a.imagen,
                                         a.ip_public,
                                         a.identificador,
                                         a.tipo,
                                         sa.nombre,
                                         sa.local,
                                         sa.num_doc
                                     }).ToListAsync();
                return Ok(listado);
            }
        }

        [HttpGet("rangoAsistencia")]
        public async Task<IActionResult> GetAsistenciasRangoAsync(string token, string desde, string hasta)
        {
            var vtoken = _cifrado.validarToken(token);
            if (vtoken == null)
            {
                return Problem("El token no es valido!");
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(x => x.descripcion == vtoken[0] && x.app.Equals("MARCACION"));
            if (empresa == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            if (empresa.cadenaconexion == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            using (var context = new SampleContext(empresa.cadenaconexion))
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(res => res.nombreusuario.Equals(vtoken[1]) && res.contrasena.Equals(vtoken[2]));
                if (usuario == null)
                {
                    return Problem("El usuario ingresado no es valido");
                }
                var usuario_locales = await context.Usuario_local.Where(res => res.usuarioid.Equals(usuario.usuarioid)).ToListAsync();
                if (usuario_locales == null)
                {
                    return Problem("No hay locales asignados");
                }
                int[] locales = _util.convertirArray(usuario_locales);
                var listado = await (from a in context.Asistencia
                                     join sa in context.Empleado on a.cod_empleado equals sa.codigo
                                     join local in context.Local on sa.local equals local.id
                                     where a.fecha >= DateTime.Parse(desde) && a.fecha <= DateTime.Parse(hasta).AddDays(1) && locales.Contains(local.id)
                                     select new
                                     {
                                         a.id,
                                         a.cod_empleado,
                                         a.fecha,
                                         a.identificador,
                                         a.tipo,
                                         a.ip_public,
                                         sa.nombre,
                                         sa.local,
                                         sa.num_doc
                                     }).ToListAsync();
                return Ok(listado);
            }
        }

        [HttpGet("mes")]
        public async Task<IActionResult> GetAsistenciasPorMesAsync(string token,int mes,int year)
        {
            var vtoken = _cifrado.validarToken(token);
            if (vtoken == null)
            {
                return Problem("El token no es valido!");
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(x => x.descripcion == vtoken[0]&&x.app.Equals("MARCACION"));
            if (empresa == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            if (empresa.cadenaconexion == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            using (var context = new SampleContext(empresa.cadenaconexion))
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(res => res.nombreusuario.Equals(vtoken[1]) && res.contrasena.Equals(vtoken[2]));
                if (usuario == null)
                {
                    return Problem("El usuario ingresado no es valido");
                }
                var usuario_locales = await context.Usuario_local.Where(res => res.usuarioid.Equals(usuario.usuarioid)).ToListAsync();
                if (usuario_locales == null)
                {
                    return Problem("No hay locales asignados");
                }
                int[] locales = _util.convertirArray(usuario_locales);
                var Desde = new DateTime(year,mes,1);
                var Hasta = Desde.AddMonths(1).AddDays(1);
                var listado = await (from a in context.Asistencia
                                     join sa in context.Empleado on a.cod_empleado equals sa.codigo
                                     join local in context.Local on sa.local equals local.id
                                     where a.fecha<=Hasta && a.fecha >= Desde && locales.Contains(local.id)
                                     select new
                                     {
                                         a.id,
                                         a.cod_empleado,
                                         sa.num_doc,
                                         sa.nombre,
                                         a.fecha,
                                         a.tipo,
                                         a.identificador,
                                         a.ip_public
                                     }).ToListAsync();
                return Ok(listado);
            }
        }
        [HttpGet("empleado")]
        public async Task<IActionResult> GetAsistenciasPorEmleadoAsync(string token, string desde,string hasta,int empleado)
        {
            var vtoken = _cifrado.validarToken(token);
            if (vtoken == null)
            {
                return Problem("El token no es valido!");
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(x => x.descripcion == vtoken[0] && x.app.Equals("MARCACION"));
            if (empresa == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            if (empresa.cadenaconexion == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            using (var context = new SampleContext(empresa.cadenaconexion))
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(res => res.nombreusuario.Equals(vtoken[1]) && res.contrasena.Equals(vtoken[2]));
                if (usuario == null)
                {
                    return Problem("El usuario ingresado no es valido");
                }
                var usuario_locales = await context.Usuario_local.Where(res => res.usuarioid.Equals(usuario.usuarioid)).ToListAsync();
                if (usuario_locales == null)
                {
                    return Problem("No hay locales asignados");
                }
                int[] locales = _util.convertirArray(usuario_locales);
                DateTime fecha = DateTime.Now;
                var listado = await (from a in context.Asistencia
                                     join sa in context.Empleado on a.cod_empleado equals sa.codigo
                                     join local in context.Local on sa.local equals local.id
                                     where sa.id.Equals(empleado) &&a.fecha>= DateTime.Parse(desde) && a.fecha <=  DateTime.Parse(hasta).AddDays(1) && locales.Contains(local.id)
                                     select new
                                     {
                                         a.id,
                                         a.cod_empleado,
                                         a.fecha,
                                         a.identificador,
                                         a.tipo,
                                         a.ip_public,
                                         a.imagen,
                                         sa.nombre,
                                         sa.num_doc
                                     }).ToListAsync();
                return Ok(listado);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Asistencia>> GetAsistenciaById(string token,int id)
        {
            var vtoken = _cifrado.validarToken(token);
            if (vtoken == null)
            {
                return Problem("El token no es valido!");
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(x => x.descripcion == vtoken[0]&&x.app.Equals("MARCACION"));
            if (empresa == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            if (empresa.cadenaconexion == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            using (var context = new SampleContext(empresa.cadenaconexion))
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(res => res.nombreusuario.Equals(vtoken[1]) && res.contrasena.Equals(vtoken[2]));
                if (usuario == null)
                {
                    return Problem("El usuario ingresado no es valido");
                }
                var AsistenciaByID = await context.Asistencia.FirstOrDefaultAsync(res => res.id == id);
                if (AsistenciaByID == null)
                {
                    return Problem();
                }
                return AsistenciaByID;
            }
           
        }

        [HttpPost]
        public async Task<ActionResult<Asistencia>> MarcarAsistenciaAsync(string token,Asistencia asistencia)
        {
            var vtoken = _cifrado.validarToken(token);
            if (vtoken == null)
            {
                return Problem("El token no es valido!");
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(x => x.descripcion == vtoken[0]&&x.app.Equals("MARCACION"));
            if (empresa == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            if (empresa.cadenaconexion == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            using (var context = new SampleContext(empresa.cadenaconexion))
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(res => res.nombreusuario.Equals(vtoken[1]) && res.contrasena.Equals(vtoken[2]));
                if (usuario == null)
                {
                    return Problem("El usuario ingresado no es valido");
                }
                asistencia.fecha = DateTime.Now;
                await context.Set<Asistencia>().AddAsync(asistencia);
                await context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAsistenciaById), new { id = asistencia.id }, asistencia);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAsistencia(string token,Asistencia asistencia)
        {
            var vtoken = _cifrado.validarToken(token);
            if (vtoken == null)
            {
                return Problem("El token no es valido!");
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(x => x.descripcion == vtoken[0]&&x.app.Equals("MARCACION"));
            if (empresa == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            if (empresa.cadenaconexion == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            using (var context = new SampleContext(empresa.cadenaconexion))
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(res => res.nombreusuario.Equals(vtoken[1]) && res.contrasena.Equals(vtoken[2]));
                if (usuario == null)
                {
                    return Problem("El usuario ingresado no es valido");
                }
                var result = await context.Asistencia.FirstAsync(b => b.id == asistencia.id);
                if (result.id != null)
                {
                    result.tipo = asistencia.tipo;
                    result.identificador = asistencia.identificador;
                    context.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
              
        }

        [HttpPost("insert")]
        public async Task<ActionResult<Asistencia>> CreateAsistenciaAsync(string token, Asistencia asistencia)
        {
            var vtoken = _cifrado.validarToken(token);
            if (vtoken == null)
            {
                return Problem("El token no es valido!");
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(x => x.descripcion == vtoken[0]&&x.app.Equals("MARCACION"));
            if (empresa == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            if (empresa.cadenaconexion == null)
            {
                return Problem("La empresa ingresada no es válida.");
            }
            using (var context = new SampleContext(empresa.cadenaconexion))
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(res => res.nombreusuario.Equals(vtoken[1]) && res.contrasena.Equals(vtoken[2]));
                if (usuario == null)
                {
                    return Problem("El usuario ingresado no es valido");
                }
                await context.Set<Asistencia>().AddAsync(asistencia);
                await context.SaveChangesAsync();
                return Ok();
            }
        }
    }
}