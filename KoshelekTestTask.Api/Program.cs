using KoshelekTestTask.Core.Interfaces;
using KoshelekTestTask.Infrastructure;
using KoshelekTestTask.Infrastructure.Data;
using KoshelekTestTask.Infrastructure.Handlers;
using KoshelekTestTask.Infrastructure.Hubs;
using KoshelekTestTask.Infrastructure.Loggers;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace KoshelekTestTask.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            var seqServerUrl = Environment.GetEnvironmentVariable("SEQ_SERVER_URL");
            var seqApiKey = Environment.GetEnvironmentVariable("SEQ_API_KEY");

            if (!string.IsNullOrWhiteSpace(seqServerUrl))
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Error)
                    .Enrich.With(new LogEnricher())
                    .WriteTo.Seq(seqServerUrl, apiKey: seqApiKey)
                    .CreateLogger();
            }
            else
            {
                var basePath = AppContext.BaseDirectory;
                var logsPath = Path.Combine(basePath, "logs");
                if (!Directory.Exists(logsPath)) Directory.CreateDirectory(logsPath);
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Error)
                    .Enrich.With(new LogEnricher())
                    .WriteTo.Console()
                    .WriteTo.File(@$"{logsPath}/log-{DateTime.Now:yyyy-MM-dd}.txt")
                    .CreateLogger();
            }

            builder.Host.UseSerilog();

            var configuration = builder.Configuration;

            // Configure services
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("http://localhost:82")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });
            builder.Services.AddSignalR();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "KoshelekTestTask API",
                        Version = "v1",
                        Description = "API for test task from Koshelek.ru"
                    }
                );
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "KoshelekTestTask.Api.xml");
                options.IncludeXmlComments(xmlPath);
            });
            builder.Services.AddTransient<IMessageDispatcher, MessageDispatcher>();
            builder.Services.AddTransient<IMessageHandler, MessageHandler>();
            builder.Services.AddTransient<IMessageService, MessageService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "KoshelekTestTask API"); });

            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<MessageHub>("/chat");

            try
            {
                Log.Information("Starting up Koshelek Api");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Koshelek Api terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
