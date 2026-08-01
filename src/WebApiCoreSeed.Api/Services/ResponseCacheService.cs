using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using WebApiCoreSeed.Api.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApiCoreSeed.Api.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public ResponseCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeTimeLive, CancellationToken cancellationToken = default)
        {
            if (response == null)
            {
                return;
            }

            var serializedResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeTimeLive
            }, cancellationToken);
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey, CancellationToken cancellationToken = default)
        {
            var cachedResponse = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
        }
    }
}
