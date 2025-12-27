using BibliotecaAPI;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

//Area de servicios

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
                .WithExposedHeaders("mi-cabecera");
    });
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddNewtonsoftJson();


builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

//Configuracion de Identity

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServiciosUsuarios, ServiciosUsuarios>();

builder.Services.AddHttpContextAccessor(); //Nos permite usar el contexto http desde cualquier clase

//Configuracion de Authentication y Authorization

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

builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});

builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Biblioteca API",
        Description = "Este es un web api para trabajar con datos de autores y libros",
        Contact = new Microsoft.OpenApi.OpenApiContact
        {
            Email = "faustomartinsosa@gmail.com",
            Name = "Fausto Sosa"
        }
    });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    opciones.OperationFilter<FiltroAutorizacion>();

});





var app = builder.Build();

//Area de middlewares

//swagger
app.UseSwagger();
app.UseSwaggerUI();


//Habilitar CORS
app.UseCors();

app.MapControllers();

app.Run();
