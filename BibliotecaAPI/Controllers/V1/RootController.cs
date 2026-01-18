using Asp.Versioning;
using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BibliotecaAPI.Controllers.V1
{

    //Controlador para devolver todos las rutas de nuestra api
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [Authorize]
    public class RootController: ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }


        [MapToApiVersion("1.0")]
        [HttpGet(Name = "ObtenerRootV1")]
        [AllowAnonymous]

        public async Task<IEnumerable<DatosHATEOASDTO>> Get() 
        {
            var datosHATEOAS = new List<DatosHATEOASDTO>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esadmin");

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerRootV1", new {})!, Descripcion: "self", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerAutoresV1", new {})!, Descripcion: "autores-obtener", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("RegistroUsuarioV1", new { })!, Descripcion: "usuarios-crear", Metodo: "POST"));
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("LoginUsuarioV1", new { })!, Descripcion: "usuarios-login", Metodo: "POST"));

            if (User.Identity!.IsAuthenticated)
            {
                //Solo si el usuario esta logeado 
                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("PutUsuarioV1", new { })!, Descripcion: "usuarios-Actualizar", Metodo: "PUT"));
                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("RenovarTokenV1", new { })!, Descripcion: "token-renovar", Metodo: "GET"));
            }

            if (esAdmin.Succeeded)
            {
                //Acciones que solo users admin pueden realizar

                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearAutorV1", new { })!, Descripcion: "autor-crear", Metodo: "POST"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearAutoresV1", new { })!, Descripcion: "autores-crear", Metodo: "POST"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearLibroV1", new { })!, Descripcion: "libro-crear", Metodo: "POST"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerUsuariosV1", new { })!, Descripcion: "usuarios-obtener", Metodo: "GET"));
            }







            return datosHATEOAS;
        }

    }
}
