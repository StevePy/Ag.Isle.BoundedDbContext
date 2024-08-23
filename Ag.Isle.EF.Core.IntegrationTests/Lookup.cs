using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ag.Isle.EF.Core
{
    [Table("Lookups")]
    public class Lookup : LookupEntity<Lookup>
    {
        public int LookupId { get; protected set; }
        public string Name { get; protected set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; }
        public DateTime LastModAt { get; protected set; }

        [Timestamp]
        protected byte[] RowVersion { get; }

        protected override string Key => LookupId.ToString();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Lookup() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
