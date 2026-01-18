using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BibliotecaAPI.Swagger
{
    public class ConfigureSwaggerGenOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        // Configura Swagger para cada version de la API
        public void Configure(string? name, SwaggerGenOptions options)
        {

            foreach (var description in _provider.ApiVersionDescriptions)
            {
                // Configuracion de la documentacion de Swagger para cada version
                var openApiInfo = new OpenApiInfo
                {
                    Title = $"BibliotecaApi v{description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = "BibliotecaAPI con versionamiento"
                };

                // Marca la version como obsoleta si es necesario
                if (description.IsDeprecated)
                {
                    openApiInfo.Description += "Esta version de la api ha sido borrada.";
                }
                options.SwaggerDoc(description.GroupName, openApiInfo);
            }

            // Configura Swagger para incluir solo las acciones correspondientes a cada version
            options.DocInclusionPredicate((docName, apiDesc) => apiDesc.GroupName == docName);
        }

        public void Configure(SwaggerGenOptions options)
        {
            Configure(options);
        }
    }
}
