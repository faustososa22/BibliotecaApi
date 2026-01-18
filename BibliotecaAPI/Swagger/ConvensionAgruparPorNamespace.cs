using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BibliotecaAPI.Swagger
{
    public class ConvensionAgruparPorNamespace : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceDelControlador = controller.ControllerType.Namespace;
            var version =namespaceDelControlador?.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = version;
        }
    }
}
