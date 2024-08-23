using Zejji.Entity;

namespace Ag.Isle.EF.Core.Scoped
{
    /// <remarks>
    /// The dependency pattern supports a Lazy injection or regular
    /// constructor-injection. Lazy injection is a pattern I use for 
    /// dependencies which is helpful for testing when classes have 
    /// several dependencies and code under test might only 
    /// "touch" a few, if any of them. Tests construct instances under
    /// test with nothing passed in the constructor then use the 
    /// setters to set mocks for just what is expected to be used.
    /// </remarks>
    partial class ScopedEntityCache<TDbContext, TEntity>
    {
        private TDbContext? Context => DbContextLocator.Get<TDbContext>();

        private readonly Lazy<IAmbientDbContextLocator>? _lazyDbContextLocator = null;
        private IAmbientDbContextLocator? _dbContextLocator = null;
        public IAmbientDbContextLocator DbContextLocator
        {
            get { return _dbContextLocator ??= _lazyDbContextLocator?.Value ?? throw new ArgumentNullException($"{nameof(DbContextLocator)} was not provided."); }
            set { _dbContextLocator = value; }
        }

        public ScopedEntityCache(Func<TEntity, string> keySelector, CacheOptions? options = null,
            Lazy<IAmbientDbContextLocator>? dbContextLocator = null)
        {
            _keySelector = keySelector;
            if (options != null)
                _cacheOptions = options;

            _lazyDbContextLocator = dbContextLocator;

            refreshCache();
        }

        public ScopedEntityCache(IAmbientDbContextLocator dbContextLocator,
            Func<TEntity, string> keySelector, CacheOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(keySelector, nameof(keySelector));
            ArgumentNullException.ThrowIfNull(DbContextLocator, nameof(dbContextLocator));

            _keySelector = keySelector;
            if (options != null)
                _cacheOptions = options;

            _dbContextLocator = dbContextLocator;

            refreshCache();
        }
    }
}
