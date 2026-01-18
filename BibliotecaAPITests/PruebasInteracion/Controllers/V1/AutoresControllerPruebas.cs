using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPITests.Utilidades;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace BibliotecaAPITests.PruebasInteracion.Controllers.V1
{
    [TestClass]
    public class AutoresControllerPruebas: BasePruebas
    {
        private readonly string url = "/api/v1/autores";
        private string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task Get_Devuelve404_CuandoAutorNoExiste()
        {
            //Preparacion
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();
            //Prueba
            var respuesta =  await cliente.GetAsync($"{url}/1");

            //Verificacion  
            var statusCode = respuesta.StatusCode;
            Assert.AreEqual(expected: HttpStatusCode.NotFound, actual: respuesta.StatusCode);


        }

        [TestMethod]
        public async Task Get_DevuelveAutor_CuandoAutorExiste()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);
            context.Autores.Add(new Autor() { Nombres = "Fausto", Apellidos = "Sosa" });
            context.Autores.Add(new Autor() { Nombres = "Ines", Apellidos = "Molinari" });
            await context.SaveChangesAsync();

            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();
            //Prueba
            var respuesta = await cliente.GetAsync($"{url}/1");

            //Verificacion  
            respuesta.EnsureSuccessStatusCode();

            var autor = JsonSerializer.Deserialize<AutorConLibrosDTO>(await respuesta.Content.ReadAsStringAsync(), jsonSerializerOptions)!;

            Assert.AreEqual(expected: 1, actual: autor.Id);

        }

        [TestMethod]
        public async Task Post_Devuelve401_CuandoUsuarioNoEstaAutenticado()
        {
            //preparacion
            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            var cliente = factory.CreateClient();
            var autorCreacionDTO = new AutorCreacionDTO() 
            { 
                Nombres = "Fausto", 
                Apellidos = "Sosa" 
            };
            //prueba
            var respuesta = await cliente.PostAsJsonAsync(url, autorCreacionDTO);
            //verificacion
            Assert.AreEqual(expected: HttpStatusCode.Unauthorized, actual: respuesta.StatusCode);
        }


        [TestMethod]
        public async Task Post_Devuelve403_CuandoUsuarioNoEsAdmin()
        {
            //preparacion
            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            var token = CrearUsuario(nombreBD, factory);


            var cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await token);
            var autorCreacionDTO = new AutorCreacionDTO()
            {
                Nombres = "Fausto",
                Apellidos = "Sosa"
            };
            //prueba
            var respuesta = await cliente.PostAsJsonAsync(url, autorCreacionDTO);
            //verificacion
            Assert.AreEqual(expected: HttpStatusCode.Forbidden, actual: respuesta.StatusCode);
        }


        [TestMethod]
        public async Task Post_Devuelve201_CuandoUsuarioEsAdmin()
        {
            //preparacion
            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            var claims = new List<Claim> { adminClaim };
            var token = CrearUsuario(nombreBD, factory, claims);


            var cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await token);
            var autorCreacionDTO = new AutorCreacionDTO()
            {
                Nombres = "Fausto",
                Apellidos = "Sosa"
            };
            //prueba
            var respuesta = await cliente.PostAsJsonAsync(url, autorCreacionDTO);
            //verificacion
            respuesta.EnsureSuccessStatusCode();
            Assert.AreEqual(expected: HttpStatusCode.Created, actual: respuesta.StatusCode);
        }
    }
}
