using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiCoreSeed.Api.Services.Interfaces
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive, CancellationToken cancellationToken = default);

        Task<string> GetCachedResponseAsync(string cacheKey, CancellationToken cancellationToken = default);
    }
}
