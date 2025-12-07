using ClubNet.Services;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;
using ClubNet.Services.Workers;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Env.Load();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    // Obtiene el nombre del archivo XML de documentación generado (ej: ClubNet.Api.xml)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    // Incluye el archivo XML de documentación para que Swagger muestre los comentarios
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddEndpointsApiExplorer();//revisar
builder.Services.AddCors(options => options.AddDefaultPolicy(builder => {
    builder.AllowAnyOrigin();
    builder.AllowAnyMethod();
    builder.AllowAnyHeader();
}));//revisar


PostgresHandler.ConnectionString = builder.Configuration.GetConnectionString("defaultConnection");
builder.Services.AddSingleton<ILoginRepository, LoginService>();
builder.Services.AddSingleton<IUsuarioRepository, UsuarioService>();
builder.Services.AddSingleton<IActividadRepository, ActividadService>();
builder.Services.AddSingleton<IClasesRepository, ClaseService>();
builder.Services.AddSingleton<ICobranzaRepository, CobranzaService>();

// JWT setup
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddHttpClient();
builder.Services.AddHostedService<WebhookWorker>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock.Backend");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.UseCors();
//app.UseHttpsRedirection();
app.UseAuthentication(); // 1. Autenticar (leer y validar el JWT)
app.UseAuthorization();  // 2. Autorizar (verificar si el usuario tiene permiso)
app.Run();
