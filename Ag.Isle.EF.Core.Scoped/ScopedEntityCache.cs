using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace Ag.Isle.EF.Core.Scoped
{

    /// <summary>
    /// Container for a cache of entities intended to be used 
    /// within a DbContextScope. The cached entities remain 
    /// detached but a copy will be associated with the 
    /// DbContext within the current scope before being returned.
    /// </summary>
    /// <remarks>
    /// Each cache is stamped with a Read date/time when the 
    /// cache is first created and when the cache was last updated.
    /// When the cache detects it is expired it will reload from the 
    /// DbContext scope.
    /// </remarks>
    public partial class ScopedEntityCache<TDbContext, TEntity> where TDbContext : DbContext where TEntity : class, ILookupEntity<TEntity>
    {
        private readonly CacheOptions _cacheOptions = new();
        public DateTime? ReadDateTime { get; private set; }

        private Dictionary<string, TEntity> _cache { get; set; } = [];

        private readonly Func<TEntity, string> _keySelector;

        public CacheOptions Options
        {
            get => _cacheOptions;
        }

        private bool CacheExpired
        {
            get => !ReadDateTime.HasValue
                || DateTime.Now.Subtract(ReadDateTime.Value).TotalSeconds > _cacheOptions.ExpirySeconds;
        }

        public TEntity? Get(string key, TDbContext? context = null)
        {
            TryGet(key, context, out TEntity? entity);
            return entity;
        }

        public bool TryGet(string key, out TEntity? value)
        {
            return TryGet(key, Context, out value);
        }

        public bool TryGet(string key, TDbContext? context, out TEntity? value)
        {
            value = null;

            if (context == null)
                context = Context;

            if (context != null)
            {
                var existingItem = context.Set<TEntity>().Local.FirstOrDefault(x => x.Key == key);
                if (existingItem != null)
                {
                    value = existingItem;
                    return true;
                }

                if (CacheExpired)
                    refreshCache(context);

                var result = _cache.TryGetValue(key, out TEntity? contentValue);
                if (!result)
                    return false;

                contentValue = contentValue!.Clone();
                context.Attach(contentValue);
                value = contentValue;
                return true;
            }
            else
            {
                var result = _cache.TryGetValue(key, out TEntity? contentValue);
                if (!result)
                    return false;
                contentValue = contentValue!.Clone();
                value = contentValue;
                return true;
            }
        }

        /// <summary>
        /// Update the cache values and reset the last read 
        /// date/time.
        /// </summary>
        private void refreshCache(TDbContext? context = null)
        {
            _cache = (context ?? Context)!.Set<TEntity>().ToDictionary(_keySelector);
            ReadDateTime = DateTime.Now;
        }

        public class CacheOptions
        {
            public const string SettingName = nameof(CacheOptions);
            public const int DefaultCacheExpirySeconds = 600;

            [Required]
            [Range(30, 3600, ErrorMessage = "Value for {0} must be between {1} and {2} seconds.")]
            public virtual int ExpirySeconds { get; set; } = DefaultCacheExpirySeconds;
        }
    }
}
