
using Asistencia_apirest.Entidades;
using Asistencia_apirest.services;
using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private SampleContext _context;
        private cifrado _cifrado;
        public UsuarioController(SampleContext context_,cifrado cifrado_)
        {
            _context = context_;
            _cifrado = cifrado_;
        }

        [HttpPost("login")]
        public async Task<IActionResult> GetUsuariosAsync(Usuario usuario)
        {
            var query = await _context.Empresa.FirstOrDefaultAsync(res=>res.descripcion.Equals(usuario.empresa)&&res.app.Equals("MARCACION"));
            if (query == null) {
                return Problem("No se encontro la empresa");
            }
            if (query.cadenaconexion == null) { 
                return Problem("No se encontro la empresa"); 
            }
            using (var context = new SampleContext(query.cadenaconexion)) {
                var result = await context.Usuario.FirstOrDefaultAsync(res => res.nombreusuario.Equals(usuario.nombreusuario) && res.contrasena.Equals(usuario.contrasena));
                if (result==null)
                {
                    return Problem("No se encontro ningun usuario");
                }
                var cifrado= _cifrado.EncryptStringAES(usuario.empresa+" "+usuario.nombreusuario+" "+usuario.contrasena);
                return Ok("{\"token\":\"" + cifrado + "\"}");
            }
        }
        [HttpPost("vallogin")]
        public async Task<IActionResult> validarlogin(string token)
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
                return Ok(usuario);
            }

            }



    }
}