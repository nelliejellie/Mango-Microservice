
using Mango.MessagePublisher.Services;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Extensions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("localConnection")));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddScoped<IRabbitPublisher, RabbitPublisher>();
            builder.Services.AddHttpClient();
            builder.Services.AddSwaggerGen();
            builder.AddAppAuthentication();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
