using AspnetRun.Application.Dnu_Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspnetRun.Web.BackgroundServiceHelper
{
    public class ReceiveMessageBackgroundTask : BackgroundService
    {
        public ReceiveMessageBackgroundTask(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
           
            //service locator pattern ??
            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IUdpStarter>();

                 scopedProcessingService.StartListeningPorts(1,7000);
            }
        }
    }
}
