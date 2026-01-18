using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Conventions;
using BibliotecaAPI;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Servicios.V1;
using BibliotecaAPI.Swagger;
using BibliotecaAPI.Utilidades;
using BibliotecaAPI.Utilidades.V1;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

//Area de servicios

//Servicio de cache con tiempo de vida de 60 segundos
builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60);
});

//Configuracion de redis
//builder.Services.AddStackExchangeRedisOutputCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("redis");
//});

//servicio de encriptacion
builder.Services.AddDataProtection();

//Configurar CORS
var origenesPermitidos = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy( politica =>
    {
        politica.WithOrigins(origenesPermitidos)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("cantidad-total-registros");
    });
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers(opciones =>
{
}).AddNewtonsoftJson();


//Configuracion de versionamiento de API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
//Configuracion del explorador de versiones
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


//Configuracion de Entity Framework y SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

//Configuracion de Identity
builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServiciosUsuarios, ServiciosUsuarios>();
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();

//Filtro de accion personalizado
builder.Services.AddScoped<FiltroValidacionLibro>();

builder.Services.AddScoped<IGeneradorEnlaces, GeneradorEnlaces>();
builder.Services.AddScoped<HATEOASAutorAttribute>();
builder.Services.AddScoped<HATEOASAutoresAttribute>();

//Nos permite usar el contexto http desde cualquier clase
builder.Services.AddHttpContextAccessor();

//Configuracion de Authentication y Authorization con JWT
builder.Services.AddAuthentication().AddJwtBearer( opciones =>
{
    opciones.MapInboundClaims = false; //Para que asp.net core no cambie nombre de claim por otro automaticamente

    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        //Que vamos a tener en cuenta para que un token sea valido
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

//Politicas de autorizacion
builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});

//Area de configuracion de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    // 1. Definir el esquema Bearer (JWT)
    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    opciones.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            // Usamos OpenApiSecuritySchemeReference en lugar de Reference
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        }
    );
    opciones.OperationFilter<FiltroAutorizacion>();

});
//Configuracion de opciones de Swagger por version
builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();




var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
}

//Area de middlewares


//Manejo global de errores
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;

    var error = new Error
    {
        MensajeDeError = exception.Message,
        StackTrace = exception.StackTrace,
        Fecha = DateTime.UtcNow
    };

    var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
    dbContext.Add(error);   
    await dbContext.SaveChangesAsync();
    await Results.InternalServerError(new
    {
        tipo = "Error",
        mensaje = "Ocurrio un error inesperdo",
        status = 500
    }).ExecuteAsync(context);
}));

//swagger
app.UseSwagger();
app.UseSwaggerUI(opciones =>
{
    var descriptions = app.DescribeApiVersions();

    foreach (var description in descriptions)
    {
        opciones.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            $"Biblioteca API {description.GroupName.ToUpper()}"
        );
    }
});



//Habilitar CORS
app.UseCors();

//CACHE
app.UseOutputCache();

app.MapControllers();

app.Run();

public partial class Program { }