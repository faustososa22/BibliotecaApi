using BibliotecaAPI.Controllers.V1;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPITests.Utilidades;
using BibliotecaAPITests.Utilidades.Dobles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.CodeCoverage.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaAPITests.PruebasUnitarias.Controllers.V1
{
    [TestClass]
    public class AutoresControllerPruebas : BasePruebas
    {
        IAlmacenadorArchivos almacenadorArchivos = null!;
        ILogger<AutoresController> logger = null!;
        IOutputCacheStore outputCacheStore = null!;
        private string nombreBD = Guid.NewGuid().ToString();
        private AutoresController controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            almacenadorArchivos = Substitute.For<IAlmacenadorArchivos>();
            logger = Substitute.For<ILogger<AutoresController>>();
            outputCacheStore = Substitute.For<IOutputCacheStore>();

            controller = new AutoresController(context, mapper, almacenadorArchivos, logger, outputCacheStore);
        }


        [TestMethod]
        public async Task Get_Retorna404_CuandoAutorConIdNoExiste()
        {
            // Prueba
            var respuesta = await controller.Get(1);

            // Verificación
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(expected: 404, actual: resultado!.StatusCode);
        }



        [TestMethod]
        public async Task Get_RetornaAutor_CuandoAutorConIdExiste()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);

            context.Autores.Add(new Autor { Nombres = "Fausto", Apellidos = "Sosa" });
            context.Autores.Add(new Autor { Nombres = "Ines", Apellidos = "Molinari" });

            await context.SaveChangesAsync();
            // Prueba
            var respuesta = await controller.Get(1);

            // Verificación
            var resultado = respuesta.Value;
            Assert.AreEqual(expected: 1, actual: resultado!.Id);
        }


        [TestMethod]
        public async Task Get_RetornaAutorConLibros_CuandoAutorTieneLibros()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);
            var libro1 = new Libro{ Titulo = "Libro 1" };
            var libro2 = new Libro{ Titulo = "Libro 2" };

            var autor = new Autor
            { 
              Nombres = "Fausto", 
              Apellidos = "Sosa", 
              Libros = new List<AutoresLibros> 
              { 
                  new AutoresLibros{ Libro = libro1 },
                  new AutoresLibros{ Libro = libro2 }
              } 
            };

            context.Add(autor);

            await context.SaveChangesAsync();
            // Prueba
            var respuesta = await controller.Get(1);

            // Verificación
            var resultado = respuesta.Value;
            Assert.AreEqual(expected: 1, actual: resultado!.Id);
            Assert.AreEqual(expected: 2, actual: resultado.Libros.Count);
        }



        [TestMethod]
        public async Task Post_DebeCrearAutor_CuandoEnviamosAutor()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);


            var nuevoAutor = new AutorCreacionDTO()
            {
                Nombres = "Pepito",
                Apellidos = "Sosa"
            };
            //Prueba

            var respuesta = await controller.Post(nuevoAutor);

            //Verificación
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);
            var context2 = ConstruirContext(nombreBD);
            var cantidad = await context2.Autores.CountAsync();
            Assert.AreEqual(expected: 1, actual: cantidad);
        }

        [TestMethod]
        public async Task Put_Retorna404_CuandoAutorNoExiste()
        {
            //Prueba
            var respuestas = await controller.Put(1, autorCreacionDTO: null!);
            //Verificación
            var resultado = respuestas as StatusCodeResult;
            Assert.AreEqual(expected: 404, actual: resultado!.StatusCode);
        }


        private const string contenedor = "autores";
        private const string cache = "autores-obtener";
        [TestMethod]
        public async Task Put_ActualizaAutor_CuandoEnviamosAutorSinFoto()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);
            context.Autores.Add(new Autor { Nombres = "Fausto", Apellidos = "Sosa", Identificacion = "id" });
            await context.SaveChangesAsync();

            var autorCreacionDTO = new AutorCreacionDTOConFoto()
            {
                Nombres = "Fausto2",
                Apellidos = "Sosa2",
                Identificacion = "Id2"
            };

            //Prueba
            var respuestas = await controller.Put(1, autorCreacionDTO);
            //Verificación
            var resultado = respuestas as StatusCodeResult;
            Assert.AreEqual(expected: 204, actual: resultado!.StatusCode);

            var context2 = ConstruirContext(nombreBD);
            var autorActualizado = await context2.Autores.SingleAsync();
            Assert.AreEqual(expected: "Fausto2", actual: autorActualizado.Nombres);
            Assert.AreEqual(expected: "Sosa2", actual: autorActualizado.Apellidos);
            Assert.AreEqual(expected: "Id2", actual: autorActualizado.Identificacion);

            await outputCacheStore.Received(1).EvictByTagAsync(cache, default);
            await almacenadorArchivos.DidNotReceiveWithAnyArgs().Editar(default, default!, default!);
        }

        [TestMethod]
        public async Task Put_ActualizaAutor_CuandoEnviamosAutorConFoto()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);

            var urlAnterior = "url-1";
            var urlnueva = "url-2";
            almacenadorArchivos.Editar(default, default!, default!).ReturnsForAnyArgs(urlnueva);

            context.Autores.Add(new Autor { Nombres = "Fausto", Apellidos = "Sosa", Identificacion = "id", Foto = urlAnterior });
            await context.SaveChangesAsync();

            var formFile = Substitute.For<IFormFile>();

            var autorCreacionDTO = new AutorCreacionDTOConFoto()
            {
                Nombres = "Fausto2",
                Apellidos = "Sosa2",
                Identificacion = "Id2",
                Foto = formFile
            };

            //Prueba
            var respuestas = await controller.Put(1, autorCreacionDTO);
            //Verificación
            var resultado = respuestas as StatusCodeResult;
            Assert.AreEqual(expected: 204, actual: resultado!.StatusCode);

            var context2 = ConstruirContext(nombreBD);
            var autorActualizado = await context2.Autores.SingleAsync();
            Assert.AreEqual(expected: "Fausto2", actual: autorActualizado.Nombres);
            Assert.AreEqual(expected: "Sosa2", actual: autorActualizado.Apellidos);
            Assert.AreEqual(expected: "Id2", actual: autorActualizado.Identificacion);
            Assert.AreEqual(expected: urlnueva, actual: autorActualizado.Foto);
            await outputCacheStore.Received(1).EvictByTagAsync(cache, default);
            await almacenadorArchivos.Received(1).Editar(urlAnterior, contenedor, formFile);
        }

        [TestMethod]
        public async Task Patch_Retorna400_CuandoPatchEsNulo()
        {
            //Prueba
            var respuesta = await controller.Patch(1, patchDoc: null!);
            //Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(expected: 400, actual: resultado!.StatusCode);
        }

        [TestMethod]
        public async Task Patch_Retorna404_CuandoActorNoExiste()
        {
            //Preparacion
            var patchDoc = new JsonPatchDocument<AutorPatchDTO>();
            //Prueba
            var respuesta = await controller.Patch(1, patchDoc);
            //Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(expected: 404, actual: resultado!.StatusCode);
        }

        [TestMethod]
        public async Task Patch_RetornaValidationProblem_CuandoHayErrorDeValidacion()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);
            context.Autores.Add(new Autor { Nombres = "Fausto", Apellidos = "Sosa", Identificacion = "123" });
            await context.SaveChangesAsync();

            var objectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator = objectValidator;

            var mensajeDeError = "mensaje de error";
            controller.ModelState.AddModelError("", mensajeDeError);

            var patchDoc = new JsonPatchDocument<AutorPatchDTO>();
            //Prueba
            var respuesta = await controller.Patch(1, patchDoc);
            //Verificación
            var resultado = respuesta as ObjectResult;
            var problemDetails = resultado!.Value as ValidationProblemDetails;
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(expected: 1, actual: problemDetails.Errors.Keys.Count);
            Assert.AreEqual(expected: mensajeDeError, actual: problemDetails.Errors.Values.First().First());


        }

        [TestMethod]
        public async Task Patch_ActualizaUnCampo_CuandoSeLeEnviaUnaOperacion()
        {
            //Preparacion
            var context = ConstruirContext(nombreBD);
            context.Autores.Add(new Autor { Nombres = "Fausto", Apellidos = "Sosa", Identificacion = "123", Foto = "URL-1" });
            await context.SaveChangesAsync();

            var objectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator = objectValidator;

            var patchDoc = new JsonPatchDocument<AutorPatchDTO>();
            patchDoc.Operations.Add(new Operation<AutorPatchDTO>("replace", "/nombres", null, "Fausto2"));
            //Prueba
            var respuesta = await controller.Patch(1, patchDoc);

            //Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(expected: 204, actual: resultado!.StatusCode);

            var context2 = ConstruirContext(nombreBD);
            var autorBD = await context2.Autores.SingleAsync();
            Assert.AreEqual(expected: "Fausto2", actual: autorBD.Nombres);
            Assert.AreEqual(expected: "Sosa", actual: autorBD.Apellidos);
            Assert.AreEqual(expected: "123", actual: autorBD.Identificacion);
            Assert.AreEqual(expected: "URL-1", actual: autorBD.Foto);


        }

        [TestMethod]
        public async Task Delete_Retorna404_CuandoAutorNoExiste() 
        {
            //Prueba
            var respuesta = await controller.Delete(1);


            //Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(expected: 404, actual: resultado!.StatusCode);
        }

        [TestMethod]
        public async Task Delete_BorraAutor_CuandoAutorExiste()
        {
            //Preparacion
            var urlFoto = "url-1";
            var context = ConstruirContext(nombreBD);
            context.Autores.Add(new Autor { Nombres = "Fausto", Apellidos = "Sosa", Foto = urlFoto });
            context.Autores.Add(new Autor { Nombres = "Fausto2", Apellidos = "Sosa2" });
            await context.SaveChangesAsync();

            //Prueba
            var respuesta = await controller.Delete(1);


            //Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(expected: 204, actual: resultado!.StatusCode);

            var context2 = ConstruirContext(nombreBD);
            var cantidad = await context2.Autores.CountAsync();
            Assert.AreEqual(expected: 1, actual: cantidad);
            var autor2Existe = await context2.Autores.AnyAsync(a => a.Nombres == "Fausto2");
            Assert.IsTrue(autor2Existe);
            await outputCacheStore.Received(1).EvictByTagAsync(cache, default);
            await almacenadorArchivos.Received(1).Borrar(urlFoto, contenedor);
        }
    }
}
