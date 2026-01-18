using BibliotecaAPI.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibliotecaAPITests.PruebasUnitarias.Validaciones
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributePrueba
    {
        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(null)]
        [DataRow("Fausto")]
        public void IsValid_RetornaExitoso_SiValueNoTieneLaPrimeraLetraMinuscula(string value)
        {
            //Preparacion
            var primeraLetraMayusculaAttribute = new PrimeraLetraMayusculaAttribute();
            var validationContext = new ValidationContext(new Object());

            //Prueba
            var resultado = primeraLetraMayusculaAttribute.GetValidationResult(value, validationContext);

            //Verificacion
            Assert.AreEqual(expected: ValidationResult.Success, actual: resultado);
        }

        [TestMethod]
        [DataRow("fausto")]
        public void IsValid_RetornaError_SiValueTieneLaPrimeraLetraMinuscula(string value)
        {
            //Preparacion
            var primeraLetraMayusculaAttribute = new PrimeraLetraMayusculaAttribute();
            var validationContext = new ValidationContext(new Object());

            //Prueba
            var resultado = primeraLetraMayusculaAttribute.GetValidationResult(value, validationContext);

            //Verificacion
            Assert.AreEqual(expected: "La primera letra debe ser mayúscula", actual: resultado!.ErrorMessage);
        }
    }
}
