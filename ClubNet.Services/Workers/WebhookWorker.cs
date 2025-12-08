using ClubNet.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClubNet.Services.Workers
{
    public class WebhookWorker : BackgroundService
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
                    using var scope = _serviceScopeFactory.CreateScope();
                    var cobranzaService = scope.ServiceProvider.GetRequiredService<ICobranzaRepository>();

                    var pendiente = await cobranzaService.ObtenerSiguientePendiente();

                    if (pendiente != null)
                    {
                        await cobranzaService.ProcesarPagoPendienteAsync(pendiente);
                        proceso = true;
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch
                {
                    proceso = false;
                }

                var delay = proceso ? TimeSpan.FromSeconds(1) : TimeSpan.FromSeconds(30);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch
                {
                    break; // Cancela el worker de manera limpia
                }
            }
        }
    }
}
