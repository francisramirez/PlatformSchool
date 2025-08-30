
using PlatformSchool.Application.Contracts;
using PlatformSchool.Domain.Repositories;
using PlatformSchool.Persistence.Repositories;
using PlatformSchool.Application.Contracts;
using PlatformSchool.Application.Services;

namespace PlatformSchool.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            // registrar el dbcontext //

            // Los repositorios de datos //
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            // servicios de aplicacion //
            builder.Services.AddTransient<IDepartmentService, DepartmentService>();



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
