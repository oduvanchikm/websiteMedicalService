using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
// using CourseWorkDataBase.Helpers;

namespace CourseWorkDataBase.Services;

public class SlotGenerationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SlotGenerationService> _logger;
    private readonly TimeSpan _delay = TimeSpan.FromHours(24); 

    public SlotGenerationService(IServiceProvider serviceProvider, ILogger<SlotGenerationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("The slot generation service is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerateSlotsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when generating slots.");
            }

            await Task.Delay(_delay, stoppingToken);
        }

        _logger.LogInformation("The slot generation service has been stopped.");
    }

    private async Task GenerateSlotsAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<SlotInitializer>();
            await initializer.InitializeSlotAsync();
        }

        _logger.LogInformation("Slots have been successfully generated.");
    }
}