using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Data;
using Classes;






var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Emotions",
        Description = "Emotions ",
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


builder.Services.AddDbContext<DataContext>(opt =>
{


   
    if (builder.Environment.IsDevelopment())
    {
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        opt.UseSqlServer(connectionString);
    }
    else
    {
        opt.UseInMemoryDatabase("DataContext");
    }
});

builder.Services.AddCors(options =>

//cors
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      policy  =>
                      {
                        policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                        
                      });
});
var app = builder.Build();





app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("MyAllowSpecificOrigins");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
//}

app.MapControllers();

app.Run();



