using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CourseWorkDataBase.Helpers;

namespace CourseWorkDataBase.Data;

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
        _logger.LogInformation("Служба генерации слотов запущена.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerateSlotsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации слотов.");
            }

            await Task.Delay(_delay, stoppingToken);
        }

        _logger.LogInformation("Служба генерации слотов остановлена.");
    }

    public async Task GenerateSlotsAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<SlotInitializer>();
            await initializer.InitializeSlotAsync();
        }

        _logger.LogInformation("Слоты успешно сгенерированы.");
    }
}