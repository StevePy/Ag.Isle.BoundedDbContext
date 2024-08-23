using NUnit.Framework;

namespace Ag.Isle.EF.Core
{
    [TestFixture]
    public class EntityCache_WhenConstructing
    {
        [Test]
        public void ThenArgumentNullThrownIfNoDbContextProvided()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var cache = new EntityCache<TestDbContext, Lookup>(null!, x => x.LookupId.ToString());
            });

            Assert.That(exception!.ParamName, Is.EqualTo("context"));
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
