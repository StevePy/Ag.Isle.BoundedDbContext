namespace Ag.Isle.EF.Core
{
    public interface ILookupEntity<TEntity> where TEntity : class
    {
        string Key { get; }

        /// <summary>
        /// Make a detached copy of the entitiy.
        /// </summary>
        TEntity Clone();
    }

    /// <summary>
    /// This base class should only be used on lookup entities that are
    /// safe for detached use, including cloning. Lookup entities have 
    /// no navigation properties.
    /// </summary>
    public abstract class LookupEntity<TEntity> : ILookupEntity<TEntity> where TEntity : class
    {
        protected abstract string Key { get; }

        string ILookupEntity<TEntity>.Key => Key;

        /// <summary>
        /// Make a detached, shallow copy of the entitiy.
        /// </summary>
        public TEntity Clone()
        {
            var clone = (TEntity)MemberwiseClone();
            return clone;
        }
    }
}
