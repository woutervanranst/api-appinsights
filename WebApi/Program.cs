
using CorrelationId;
using CorrelationId.DependencyInjection;
using System.Diagnostics;

namespace WebApi.Bff;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationInsightsTelemetry();

        builder.Services.AddHttpClient("WebApi2", c =>
        {
            c.BaseAddress = new Uri("http://localhost:5233/");
        });

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDefaultCorrelationId(a =>
        {
            a.CorrelationIdGenerator = () =>
            {
                return Activity.Current?.TraceId.ToString();
            };
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        //app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseCorrelationId();

        app.MapControllers();

        app.Run();
    }
}