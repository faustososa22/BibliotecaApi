using BibliotecaAPI.Controllers.V1;
using BibliotecaAPI.DTOs;
using BibliotecaAPITests.Utilidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaAPITests.PruebasUnitarias.Controllers.V1
{
    [TestClass]
    public class LibrosControllerPruebas: BasePruebas
    {
        [TestMethod]
        public async Task Get_RetornaCeroLibros_CuandoNoHayLibros()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            IOutputCacheStore outputCacheStore = null!;

            var controller = new LibrosController(context, mapper, outputCacheStore);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var paginacionDTO = new PaginacionDTO(1,1);
            //prueba

            var respuesta = await controller.Get(paginacionDTO);
            //verificacion

            Assert.AreEqual(0, respuesta.Count());
        }
    }
}
