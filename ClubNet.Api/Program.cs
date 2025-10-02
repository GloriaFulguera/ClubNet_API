using ClubNet.Services;
using ClubNet.Services.Handlers;
using ClubNet.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock.Backend");
    c.RoutePrefix = string.Empty;
});
app.MapControllers();
app.UseCors();//revisar desde
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();//revisar hasta
app.Run();
