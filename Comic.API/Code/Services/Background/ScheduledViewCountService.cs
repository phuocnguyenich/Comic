
using Comic.API.Code.Extensions;
using Comic.API.Data;
using Comic.API.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace Comic.API.Code.Services;

public class ScheduledViewCountService : BackgroundService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ScheduledViewCountService(
        IMemoryCache memoryCache,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _memoryCache = memoryCache;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessCacheAndWriteToDb();

            // Sleep for a specific interval before checking again
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessCacheAndWriteToDb()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ComicDbContext>();
            var allCacheKeys = _memoryCache.GetCacheKeys();

            foreach (var cacheChapterId in allCacheKeys)
            {

                if (_memoryCache.TryGetValue(cacheChapterId, out int viewCount))
                {
                    // Check if there is an existing record for the current day and chapter
                    var today = DateTime.UtcNow.Date;
                    var existingRecord = dbContext.DailyComicViews
                        .FirstOrDefault(d => d.ChapterId == cacheChapterId && d.ViewDate.Date == today);

                    if (existingRecord != null)
                    {
                        // Update the existing record
                        existingRecord.ViewCount += viewCount;

                        Console.WriteLine($"Updating {viewCount} views for Chapter {cacheChapterId} in the database.");
                    }
                    else
                    {
                        // Add a new record for the current day
                        var dailyComicView = new DailyComicView
                        {
                            ViewDate = today,
                            ViewCount = viewCount,
                            ChapterId = cacheChapterId
                        };

                        dbContext.DailyComicViews.Add(dailyComicView);

                        Console.WriteLine($"Writing {viewCount} views for Chapter {cacheChapterId} to the database.");
                    }

                    await dbContext.SaveChangesAsync();

                    // Clear the cache after writing to the database
                    _memoryCache.Remove(cacheChapterId);
                }
            }
        }
    }
}
