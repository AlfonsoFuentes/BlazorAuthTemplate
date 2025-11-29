using LazyCache;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Server.Services
{
    public interface ICache
    {
        string TenantId { get; }
        Task<T?> GetOrAddCacheAsync<T>(Func<Task<T?>> addItemFactory, string key, bool IsTenanted = false) where T : class; // T sigue siendo class, pero el resultado puede ser null
        void InvalidateCache(params string[] types);
    }
    public class Cache : ICache
    {
        private readonly IAppCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConcurrentDictionary<string, byte> _cacheKeys = new();
        public string TenantId =>
           _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value
           ?? "default";

        public Cache(IAppCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<T?> GetOrAddCacheAsync<T>(Func<Task<T?>> addItemFactory, string key, bool isTenanted = false) where T : class
        {
            
            var tenantPart = isTenanted ? $"-{TenantId}" : "";

            var finalKey = $"{key}{tenantPart}";

            _cacheKeys.TryAdd(finalKey, 0);
            return await _cache.GetOrAddAsync(finalKey, addItemFactory); // ✅ Ahora acepta T?
        }
        public void InvalidateCache(params string[] keysToRemove) 
        {
           

            foreach (var key in keysToRemove)
            {
                _cacheKeys.TryRemove(key, out _);
                _cache.Remove(key);
            }
        }
    }
    //public interface ICache2
    //{
    //    string TenantId { get; }

    //    Task<T?> GetOrAddCacheAsync<T>(
    //     Func<Task<T?>> addItemFactory,
    //     string key)
    //     where T : class; // T sigue siendo class, pero el resultado puede ser null


    //    // ✅ Una sola firma: params Type[]
    //    void InvalidateCache<T>(params Type[] types) where T : class; // T sigue siendo class, pero el resultado puede ser null;

    //}
    //public class Cache2 : ICache2
    //{
    //    private readonly IAppCache _cache;
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private readonly ConcurrentDictionary<string, byte> _cacheKeys = new();

    //    public string TenantId =>
    //        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value
    //        ?? "default";

    //    public Cache(IAppCache cache, IHttpContextAccessor httpContextAccessor)
    //    {
    //        _cache = cache;
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    public async Task<T?> GetOrAddCacheAsync<T>(
    // Func<Task<T?>> addItemFactory,
    // string key)
    // where T : class
    //    {
    //        var entityType = GetEntityTypeFromCacheType<T>();
    //        var className = entityType.Name;
    //        var isTenanted = typeof(ITennant).IsAssignableFrom(entityType);
    //        var tenantPart = isTenanted ? $"-{TenantId}" : "";

    //        var finalKey = $"{key}-{className}{tenantPart}";

    //        _cacheKeys.TryAdd(finalKey, 0);
    //        return await _cache.GetOrAddAsync(finalKey, addItemFactory); // ✅ Ahora acepta T?
    //    }

    //    // ✅ Solo este método — recibe params Type[]
    //    public void InvalidateCache<T>(params Type[] types) where T : class
    //    {
    //        if (types == null || types.Length == 0)
    //            return;

    //        var classNames = types.Select(t => t.Name).ToArray();
    //        var keysToRemove = _cacheKeys.Keys
    //            .Where(k => classNames.Any(name => k.Contains(name)))
    //            .ToList();

    //        foreach (var key in keysToRemove)
    //        {
    //            _cacheKeys.TryRemove(key, out _);
    //            _cache.Remove(key);
    //        }
    //    }

    //    private static Type GetEntityTypeFromCacheType<T>()
    //    {
    //        var type = typeof(T);
    //        if (type.IsGenericType)
    //        {
    //            var def = type.GetGenericTypeDefinition();
    //            if (def == typeof(List<>) || def == typeof(IList<>) ||
    //                def == typeof(IEnumerable<>) || def == typeof(IReadOnlyList<>))
    //                return type.GetGenericArguments()[0];
    //        }
    //        return type;
    //    }
    //}
}
