using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;

namespace Comic.API.Code.Extensions
{
    public static class CacheExtensions
    {
        public static IEnumerable<string> GetCacheKeys(this IMemoryCache memoryCache)
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(memoryCache) as ICollection;
            var items = new List<string>();
            foreach (var item in collection)
            {
                var methodInfo = item.GetType().GetProperty("Key");
                var val = methodInfo.GetValue(item);
                items.Add(val.ToString());
            }

            return items;
        }
    }
}
