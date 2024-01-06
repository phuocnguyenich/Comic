namespace Comic.API.Code.Interfaces
{
    public interface IViewCountQueue
    {
        /// <summary>
        ///     Add a background work item to the queue.
        /// </summary>
        /// <param name="workItem"></param>
        /// <returns></returns>
        ValueTask EnqueueAsync(int workItem);

        /// <summary>
        ///     Remove a background work item from the queue.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> DequeueAsync(CancellationToken cancellationToken);
    }
}
