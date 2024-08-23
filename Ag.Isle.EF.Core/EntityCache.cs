using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ag.Isle.EF.Core
{

    /// <summary>
    /// Container for a cache of entities intended to be used 
    /// stand-alone with a passed DbContext reference. 
    /// The cached entities remain detached but a copy will be 
    /// associated with the passed DbContext before being 
    /// returned.
    /// </summary>
    /// <remarks>
    /// Each cache is stamped with a Read date/time when the 
    /// cache is first created and when the cache was last updated.
    /// When the cache detects it is expired it will reload from the 
    /// passed DbContext.
    /// 
    /// Cached entities should be simple lookup entities with no 
    /// navigation properties under them. The cache does not store
    /// nor copy across details like navigation properties.
    /// </remarks>
    public partial class EntityCache<TDbContext, TEntity> where TDbContext : DbContext where TEntity : class, ILookupEntity<TEntity>
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

        public EntityCache(TDbContext context, Func<TEntity, string> keySelector, CacheOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(keySelector, nameof(keySelector));

            _keySelector = keySelector;
            if (options != null)
                _cacheOptions = options;

            refreshCache(context);
        }

        /// <summary>
        /// Fetch an entity by Id from the cache, attaching it 
        /// to the provided DbContext.
        /// </summary>
        public TEntity? Get(string key, TDbContext? context = null)
        {
            TryGet(key, context, out TEntity? entity);
            return entity;
        }

        /// <summary>
        /// Try to fetch an entity by Id from the cache, attaching it 
        /// to the provided DbContext.
        /// </summary>
        /// <remarks>
        /// If we do not pass a DbContext then this will simply return
        /// a cache copy if we have one. If a DbContext is provided
        /// we first check if the DbContext is tracking the requested
        /// item. The "key" value should be a string representation of 
        /// the PK for the lookup table.
        /// </remarks>
        public bool TryGet(string key, TDbContext? context, out TEntity? value)
        {
            value = null;

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
        private void refreshCache(TDbContext context)
        {
            _cache = context.Set<TEntity>().ToDictionary(_keySelector);
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


