using CourseWorkDataBase.Helpers;

namespace CourseWorkDataBase.Services;

public class SlotGenerationService(IServiceProvider serviceProvider, ILogger<SlotGenerationService> logger)
    : BackgroundService
{
    private readonly TimeSpan _delay = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("The slot generation service is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerateSlotsAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when generating slots.");
            }

            await Task.Delay(_delay, stoppingToken);
        }

        logger.LogInformation("The slot generation service has been stopped.");
    }

    private async Task GenerateSlotsAsync()
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<SlotInitializer>();
            await initializer.InitializeSlotAsync();
        }

        logger.LogInformation("Slots have been successfully generated.");
    }
}