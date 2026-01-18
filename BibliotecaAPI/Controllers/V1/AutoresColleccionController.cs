using Asp.Versioning;
using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers.V1


{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/autores-coleccion")]
    [Authorize(Policy = "esadmin")]
    public class AutoresColleccionController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresColleccionController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{ids}", Name = "ObtenerAutoresPorIdsV1")] // api/autores-coleccion/1,2,3
        public async Task<ActionResult<List<AutorConLibrosDTO>>> Get(string ids)
        {
            var idsColeccion = new List<int>();

            foreach (var id in ids.Split(","))
            {
                if (int.TryParse(id, out var idInt))
                {
                    idsColeccion.Add(idInt);
                }
            }
            if (!idsColeccion.Any())
            {
                ModelState.AddModelError(nameof(ids), "No se ha proporcionado ningún id válido.");
                return ValidationProblem();
            }

            var autores = await context.Autores
                .Include(x => x.Libros)
                    .ThenInclude(x => x.Libro)
                .Where(x => idsColeccion.Contains(x.Id))
                .ToListAsync();

            if (idsColeccion.Count != autores.Count)
            {
                return NotFound();
            }

            var autoresDTO = mapper.Map<List<AutorConLibrosDTO>>(autores);
            return autoresDTO;

        }

        [MapToApiVersion("1.0")]
        [HttpPost(Name = "CrearAutoresV1")]
        public async Task<ActionResult> Post(IEnumerable<AutorCreacionDTO> autorCreacionDTOs)
        {
            var autores = mapper.Map<IEnumerable<Autor>>(autorCreacionDTOs);
            context.AddRange(autores);
            await context.SaveChangesAsync();

            var autoresDTO = mapper.Map<IEnumerable<AutorDTO>>(autores);
            var ids = autores.Select(x => x.Id);
            var idsString = string.Join(",", ids);
            return CreatedAtRoute("ObtenerAutoresPorIdsV1", new { ids = idsString}, autoresDTO);
        }

    }
}
