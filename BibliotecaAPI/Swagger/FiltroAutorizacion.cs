using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection.Metadata;

namespace BibliotecaAPI.Swagger
{
    public class FiltroAutorizacion : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var tieneAllowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .Any();

            if (tieneAllowAnonymous)
            {
                // 🔥 Esto quita el candado SOLO para públicos
                operation.Security?.Clear();
            }
          
        }
    }
}