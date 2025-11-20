using ClubNet.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubNet.Services.Workers
{
    public class WebhookWorker:BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WebhookWorker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                bool proceso = false;
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var cobranzaService = scope.ServiceProvider.GetRequiredService<ICobranzaRepository>();

                        var pendiente = await cobranzaService.ObtenerSiguientePendiente();

                        if(pendiente!= null)
                        {
                            await cobranzaService.ProcesarPagoPendienteAsync(pendiente);
                            proceso = true;
                        }
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error en WebhookWorker: {ex.Message}");
                    proceso = false;
                }
                if (proceso)
                    await Task.Delay(1000, stoppingToken);
                else                
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
