
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
            var allCacheKeys = _memoryCache.GetCacheKeys().Where(x => x.StartsWith("chapterId"));

            foreach (var cacheKey in allCacheKeys)
            {
                if (_memoryCache.TryGetValue(cacheKey, out int viewCount) &&
                    TryExtractChapterId(cacheKey, out int chapterId))
                {
                    var today = DateTime.UtcNow.Date;
                    var existingRecord = dbContext.DailyComicViews
                        .FirstOrDefault(d => d.ChapterId == chapterId && d.ViewDate.Date == today);

                    if (existingRecord != null)
                    {
                        existingRecord.ViewCount += viewCount;
                        Console.WriteLine($"Updating {viewCount} views for Chapter {chapterId} in the database.");
                    }
                    else
                    {
                        var dailyComicView = new DailyComicView
                        {
                            ViewDate = today,
                            ViewCount = viewCount,
                            ChapterId = chapterId
                        };

                        dbContext.DailyComicViews.Add(dailyComicView);
                        Console.WriteLine($"Writing {viewCount} views for Chapter {chapterId} to the database.");
                    }

                    await dbContext.SaveChangesAsync();
                    _memoryCache.Remove(cacheKey);
                }
            }
        }
    }

    private bool TryExtractChapterId(string cacheKey, out int chapterId)
    {
        return int.TryParse(cacheKey.Replace("chapterId-", ""), out chapterId);
    }

}
