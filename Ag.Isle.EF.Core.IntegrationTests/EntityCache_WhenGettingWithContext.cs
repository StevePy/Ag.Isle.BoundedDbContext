using NUnit.Framework;

namespace Ag.Isle.EF.Core
{
    [TestFixture]
    public class EntityCache_WhenGettingWithContext
    {
        [Test]
        public void ThenReturnedItemIsAssociatedWithDbContext()
        {
            EntityCache<TestDbContext, Lookup> cache;
            using (var context = new TestDbContext())
            {
                cache = new EntityCache<TestDbContext, Lookup>(context, x => x.LookupId.ToString());
            }

            Lookup? firstItemReference;
            Lookup? secondItemReference;
            using (var context = new TestDbContext())
            {
                Assert.That(context.Lookups.Local.FirstOrDefault(x => x.LookupId == 1), Is.Null);
                firstItemReference = cache.Get("1", context);
                Assert.That(firstItemReference, Is.Not.Null);
                // check that we are now tracking that lookup row.
                var localItem = context.Lookups.Local.FirstOrDefault(x => x.LookupId == 1);
                Assert.That(localItem, Is.Not.Null);
                // the lookup reference returned should be our tracked reference.
                Assert.That(firstItemReference, Is.SameAs(localItem));
                // Request the same item again, this should return the exact same reference.
                secondItemReference = cache.Get("1", context);
                Assert.That(secondItemReference, Is.Not.Null);
                Assert.That(firstItemReference, Is.SameAs(secondItemReference));
            }

            secondItemReference = null;

            using (var context = new TestDbContext())
            {
                Assert.That(context.Lookups.Local.FirstOrDefault(x => x.LookupId == 1), Is.Null);
                // Fetch the same item again with a new DbContext instance. We should get a different reference than what was given to the first DbContext.
                secondItemReference = cache.Get("1", context);
                Assert.That(secondItemReference, Is.Not.Null);
                var localItem = context.Lookups.Local.FirstOrDefault(x => x.LookupId == 1);
                Assert.That(localItem, Is.Not.Null);
                Assert.That(secondItemReference, Is.SameAs(localItem));
            }

            // References should not be the same.
            Assert.That(firstItemReference, Is.Not.SameAs(secondItemReference));
        }

        [Test]
        public void ThenArgumentNullThrownIfNoKeySelectorProvided()
        {
            using var context = new TestDbContext();
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var cache = new EntityCache<TestDbContext, Lookup>(context, null!);
            });
            Assert.That(exception!.ParamName, Is.EqualTo("keySelector"));
        }

        [Test]
        public void ThenLookupsGetLoaded()
        {
            EntityCache<TestDbContext, Lookup> cache;
            using (var context = new TestDbContext())
            {
                cache = new EntityCache<TestDbContext, Lookup>(context, x => x.LookupId.ToString());
            }

            Assert.That(cache.Get("1"), Is.Not.Null);
            Assert.That(cache.Get("2"), Is.Not.Null);
            Assert.That(cache.Get("3"), Is.Not.Null);
            Assert.That(cache.Get("4"), Is.Not.Null);
            Assert.That(cache.Get("5"), Is.Not.Null);
            Assert.That(cache.Get("6"), Is.Not.Null);
        }



    }
}
