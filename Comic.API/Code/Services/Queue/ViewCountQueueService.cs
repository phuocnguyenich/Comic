using Comic.API.Code.Interfaces;
using System.Threading.Channels;

namespace Comic.API.Code.Services;

public class ViewCounterQueueService : IViewCountQueue
{
    private readonly Channel<int> _queue;

    public ViewCounterQueueService()
    {
        // Note: capacity can also be set using IOptions using DI
        var options = new BoundedChannelOptions(int.MaxValue)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<int>(options);
    }

    public async ValueTask EnqueueAsync(int workItem)
    {
        if (workItem == 0)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<int> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}
