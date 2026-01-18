using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BibliotecaAPITests.PruebasUnitarias.Servicios
{
    [TestClass]
    public class ServiciosUsuariosPruebas
    {
        private UserManager<Usuario> userManager = null!;
        private IHttpContextAccessor contextAccessor = null!;
        private ServiciosUsuarios servicioUsuarios = null!;

        [TestInitialize]
        public void Setup()
        {
            userManager = Substitute.For<UserManager<Usuario>>(Substitute.For<IUserStore<Usuario>>(),
                null, null, null, null, null, null, null, null);
            contextAccessor = Substitute.For<IHttpContextAccessor>();
            servicioUsuarios = new ServiciosUsuarios(userManager, contextAccessor);
        }

        [TestMethod]
        public async Task ObtenerUsuario_RetornaNulo_CuandoNoHayEmail()
        {
            //Preparacion
            var httpContext = new DefaultHttpContext();
            contextAccessor.HttpContext.Returns(httpContext);
            //Prueba
            var usuario = await servicioUsuarios.ObtenerUsuarioActual();
            //Verificacion
            Assert.IsNull(usuario);
        }

        [TestMethod]
        public async Task ObtenerUsuario_RetornaUsuario_CuandoHayEmail()
        {
            //Preparacion
            var email = "prueba@gmail.com";
            var usuarioEsperado = new Usuario() { Email = email };

            userManager.FindByEmailAsync(email)!.Returns(Task.FromResult(usuarioEsperado));
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("email", email)
            }));

            var httpContext = new DefaultHttpContext() { User = claims};
            contextAccessor.HttpContext.Returns(httpContext);
            //Prueba
            var usuario = await servicioUsuarios.ObtenerUsuarioActual();
            //Verificacion
            Assert.IsNotNull(usuario);
            Assert.AreEqual(email, usuario!.Email);
        }

        [TestMethod]
        public async Task ObtenerUsuario_RetornaNulo_CuandoUsuarioNoExiste()
        {
            //Preparacion
            var email = "prueba@gmail.com";
            var usuarioEsperado = new Usuario() { Email = email };

            userManager.FindByEmailAsync(email)!.Returns(Task.FromResult<Usuario>(null!));
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("email", email)
            }));

            var httpContext = new DefaultHttpContext() { User = claims };
            contextAccessor.HttpContext.Returns(httpContext);
            //Prueba
            var usuario = await servicioUsuarios.ObtenerUsuarioActual();
            //Verificacion
            Assert.IsNull(usuario);

        }

    }
}
