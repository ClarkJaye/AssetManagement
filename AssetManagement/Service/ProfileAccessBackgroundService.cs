using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AssetManagement.Services
{
    public class ProfileAccessBackgroundService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public ProfileAccessBackgroundService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var profileAccessService = scope.ServiceProvider.GetRequiredService<ProfileAccessService>();
                    profileAccessService.CreateProfileAccesses();
                }

                // Wait for a specified interval before running the service again
                await Task.Delay(TimeSpan.FromMinutes(.2), stoppingToken);
            }
        }
    }
}