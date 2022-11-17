using Asistencia_apirest.services;
using DemoAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<SampleContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddTransient<cifrado>();
builder.Services.AddTransient<util>();
//builder.Services.AddTransient<IEmpleadoRepository, EmpleadoRepository>();
//builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();

List<string> CorsOriginAllowed = builder.Configuration.GetSection("AllowedOrigins").Get<List<string>>();
string[] origins = CorsOriginAllowed != null ? CorsOriginAllowed.ToArray() : new string[] { "*" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .SetIsOriginAllowed((host) => true)
                  .AllowCredentials()
        );
});


var app = builder.Build();
app.UseCors("CorsPolicy");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();