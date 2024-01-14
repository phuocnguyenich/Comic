
using Comic.API.Code.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Comic.API.Code.Services;

public class ViewCountService : BackgroundService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IViewCountQueue _queueService;

    public ViewCountService(IMemoryCache memoryCache, IViewCountQueue queueService)
    {
        _memoryCache = memoryCache;
        _queueService = queueService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var chapterId = await _queueService.DequeueAsync(stoppingToken);
            // Check the queue for new messages
            if (chapterId != default)
            {
                if (!_memoryCache.TryGetValue($"chapterId-{chapterId}", out int viewCount))
                {
                    viewCount = 0;
                }

                _memoryCache.Set($"chapterId-{chapterId}", viewCount + 1);

                Console.WriteLine($"Received message for Chapter {chapterId}. New view count: {viewCount + 1}");
            }
        }
    }
}
