using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BibliotecaAPI.Servicios.V1
{
    public class GeneradorEnlaces : IGeneradorEnlaces
    {
        private readonly LinkGenerator linkGenerator;
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;


        public GeneradorEnlaces(LinkGenerator linkGenerator, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            this.linkGenerator = linkGenerator;
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ColeccionDeRecursosDTO<AutorDTO>> GenerarEnlaces(List<AutorDTO> autores)
        {
            var resultados = new ColeccionDeRecursosDTO<AutorDTO>
            {
                Valores = autores
            };

            var usuario = httpContextAccessor.HttpContext!.User;
            var esAdmin = await authorizationService.AuthorizeAsync(usuario, "esadmin");

            foreach (var autorDTO in autores)
            {
                GenerarEnlaces(autorDTO, esAdmin.Succeeded);
            }

            resultados.Enlaces.Add(new DatosHATEOASDTO(linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, "ObtenerAutoresV1", new { })!, "self", "GET"));

            if (esAdmin.Succeeded)
            {
                resultados.Enlaces.Add(new DatosHATEOASDTO(linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, "CrearAutorV1", new { })!, "autor-crear", "POST"));
                resultados.Enlaces.Add(new DatosHATEOASDTO(linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, "CrearAutorConFotoV1", new { })!, "autor-crear-con-foto", "POST"));
            }

            return resultados;

        }

        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var usuario = httpContextAccessor.HttpContext!.User;
            var esAdmin = await authorizationService.AuthorizeAsync(usuario, "esadmin");
            GenerarEnlaces(autorDTO, esAdmin.Succeeded);
        }

        private void GenerarEnlaces(AutorDTO autorDTO, bool esAdmin)
        {

            autorDTO.Enlaces.Add(new DatosHATEOASDTO(linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, "ObtenerAutorV1", new { id = autorDTO.Id })!, "self", "GET"));

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatosHATEOASDTO(linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, "ActualizarAutorV1", new { id = autorDTO.Id })!, "autor-actualizar", "PUT"));

                autorDTO.Enlaces.Add(new DatosHATEOASDTO(linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, "PatchAutorV1", new { id = autorDTO.Id })!, "autor-patch", "PATCH"));

                autorDTO.Enlaces.Add(new DatosHATEOASDTO(linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, "BorrarAutorV1", new { id = autorDTO.Id })!, "autor-borrar", "DELETE"));
            }




        }
    }
}
