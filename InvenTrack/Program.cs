
using InvenTrack.Data;
using InvenTrack.Repositories.Implementation;
using InvenTrack.Repositories.Interfaces;
using InvenTrack.Services.Implementation;
using InvenTrack.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace InvenTrack
{
    public class Program
    {
        public async  static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();

                await DataSeeder.SeedAsync(context);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
