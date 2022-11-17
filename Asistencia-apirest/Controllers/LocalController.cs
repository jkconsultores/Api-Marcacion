using Asistencia_apirest.services;
using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Asistencia_apirest.Controllers
{
    public class LocalController : Controller
    {
        public readonly SampleContext _context;
        private cifrado _cifrado;
        private util _util;
        public LocalController(SampleContext context,cifrado cifrado_,util util_)
        {
            _context = context;
            _cifrado = cifrado_;
            _util = util_;
        }
        // GET: LocalController

        [HttpGet("local")]
        public async Task<IActionResult> GetLocal(string token)
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
            using (var context = new SampleContext(empresa.cadenaconexion)) {
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
                var query = await (from l in context.Local where locales.Contains(l.id) select l).ToListAsync();
                return Ok(query);
            }
        

        }

  
    }
}
