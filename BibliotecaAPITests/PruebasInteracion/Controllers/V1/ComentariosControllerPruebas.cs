using BibliotecaAPI.Entidades;
using BibliotecaAPITests.Utilidades;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;

namespace BibliotecaAPITests.PruebasInteracion.Controllers.V1
{
    [TestClass]
    public class ComentariosControllerPruebas: BasePruebas
    {
        private readonly string url = "/api/v1/libros/1/comentarios";
        private readonly string nombreBD = Guid.NewGuid().ToString();

        private async Task CrearDataDePrueba() 
        { 
            var context = ConstruirContext(nombreBD);
            var autor = new Autor { Nombres = "Fausto", Apellidos = "Sosa" };
            context.Add(autor);
            await context.SaveChangesAsync();

            var libro = new Libro { Titulo = "Libro de prueba"};
            libro.Autores.Add( new AutoresLibros { Autor = autor } );
            context.Add(libro);
            await context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task Delete_Devuelve204_CuandoUsuarioBorraSuPropioComentario()
        {
            //preparacion
            await CrearDataDePrueba();

            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            var token = await CrearUsuario(nombreBD, factory);
            var context = ConstruirContext(nombreBD);
            var usuario = await context.Users.FirstAsync();

            var comentario = new Comentario
            {
                Cuerpo = "Comentario de prueba",
                LibroId = 1,
                UsuarioId = usuario.Id
            };

            context.Add(comentario);
            await context.SaveChangesAsync();

            var cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            //prueba
            var respuesta =  await cliente.DeleteAsync($"{url}/{comentario.Id}");
            //verificacion
            Assert.AreEqual(HttpStatusCode.NoContent, respuesta.StatusCode);
        }

        [TestMethod]
        public async Task Delete_Devuelve403_CuandoUsuarioIntentaBorrarElComentarioDeOtro()
        {
            //preparacion
            await CrearDataDePrueba();

            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            var emailCreadorComentario = "creador-comentario@gmail.com";
            
            await CrearUsuario(nombreBD, factory, [], emailCreadorComentario);
            var context = ConstruirContext(nombreBD);
            var usuarioCreadorComentario = await context.Users.FirstAsync();

            var comentario = new Comentario
            {
                Cuerpo = "Comentario de prueba",
                LibroId = 1,
                UsuarioId = usuarioCreadorComentario.Id
            };

            context.Add(comentario);
            await context.SaveChangesAsync();

            var tokenUsuarioDistinto = await CrearUsuario(nombreBD, factory, [], "usuario-distinto@gmail.com");

            var cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenUsuarioDistinto);

            //prueba
            var respuesta = await cliente.DeleteAsync($"{url}/{comentario.Id}");
            //verificacion
            Assert.AreEqual(HttpStatusCode.Forbidden, respuesta.StatusCode);
        }

    }
}
