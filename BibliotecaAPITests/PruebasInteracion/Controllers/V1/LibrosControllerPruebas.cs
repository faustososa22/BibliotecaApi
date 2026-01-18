using BibliotecaAPI.DTOs;
using BibliotecaAPITests.Utilidades;
using System.Net;

namespace BibliotecaAPITests.PruebasInteracion.Controllers.V1
{
    [TestClass]
    public class LibrosControllerPruebas: BasePruebas
    {
        private readonly string url = "/api/v1/libros";
        private string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task Post_Devuelve400_CuandoAutoresIdsEsVacio()
        {
            //preparacion
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();
            var libroCreacionDTO = new LibroCreacionDTO()
            {
                Titulo = "Libro de prueba"
            };
            //prueba
            var respuesta =  await cliente.PostAsJsonAsync(url, libroCreacionDTO);
            //verificacion  
            Assert.AreEqual(HttpStatusCode.BadRequest, respuesta.StatusCode);
        }
    }
}
