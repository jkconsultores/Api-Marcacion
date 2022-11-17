using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Asistencia_apirest.Controllers
{
    public class EmpresaController : Controller
    {
        public readonly SampleContext _context;

        public EmpresaController(SampleContext context) => _context = context;
        // GET: LocalController

        [HttpGet("empresa")]
        public async Task<IActionResult> GetEmpresa()
        {
            var query = await (from a in _context.Empresa select new {a.id,a.descripcion}).ToListAsync();
            return Ok(query);

        }


    }
}
