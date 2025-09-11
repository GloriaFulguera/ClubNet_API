// See https://aka.ms/new-console-template for more information
using ClubNet.Services.Handlers;

Console.WriteLine("Hello, World!");

string hash = BCrypt.Net.BCrypt.HashPassword("admin");
Console.WriteLine(hash);

string query = $"INSERT INTO Usuarios(email,clave) values ('a@a.com','{hash}')";

Console.WriteLine(PostgresHandler.Exec(query));