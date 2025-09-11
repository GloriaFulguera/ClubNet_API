using ClubNet.Services.Handlers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();

PostgresHandler.ConnectionString = builder.Configuration.GetConnectionString("defaultConnection");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock.Backend");
    c.RoutePrefix = string.Empty;
});

app.Run();
