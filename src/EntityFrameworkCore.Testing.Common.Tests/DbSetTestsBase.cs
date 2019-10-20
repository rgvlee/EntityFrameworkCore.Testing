using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class DbSetTestsBase<TDbContext, TEntity> : MockQueryableTestsBase<TEntity>
        where TDbContext : DbContext
        where TEntity : TestEntityBase
    {
        [SetUp]
        public override void SetUp()
        {
            MockedDbContext = CreateMockedDbContext();
            base.SetUp();
        }

        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.CreateMany<TEntity>().ToList();
            DbSet.AddRange(itemsToAdd);
            MockedDbContext.SaveChanges();
            ItemsAddedToQueryableSource = itemsToAdd;
        }

        protected TDbContext MockedDbContext;
        protected DbSet<TEntity> DbSet => (DbSet<TEntity>) Queryable;

        protected abstract TDbContext CreateMockedDbContext();

        [Test]
        public virtual void AddAndPersist_Entity_Persists()
        {
            var expectedResult = new Fixture().Create<TEntity>();

            DbSet.Add(expectedResult);
            MockedDbContext.SaveChanges();

            Assert.Multiple(() =>
            {
                Assert.That(DbSet.Single(), Is.EqualTo(expectedResult));
                Assert.That(DbSet.Single(), Is.EqualTo(expectedResult));
            });
        }

        [Test]
        public virtual void AddAndPersist_Enumeration_Persists()
        {
            var expectedResult = new Fixture().CreateMany<TEntity>().ToList();

            DbSet.AddRange(expectedResult);
            MockedDbContext.SaveChanges();

            var actualResult = DbSet.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.EquivalentTo(expectedResult));
                Assert.That(DbSet.ToList(), Is.EquivalentTo(actualResult));
            });
        }

        [Test]
        public virtual async Task AddAndPersistAsync_Entity_Persists()
        {
            var expectedResult = new Fixture().Create<TEntity>();

            await DbSet.AddAsync(expectedResult);
            await MockedDbContext.SaveChangesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(DbSet.Single(), Is.EqualTo(expectedResult));
                Assert.That(DbSet.Single(), Is.EqualTo(expectedResult));
            });
        }

        [Test]
        public virtual async Task AddAndPersistAsync_Enumeration_Persists()
        {
            var expectedResult = new Fixture().CreateMany<TEntity>().ToList();

            await DbSet.AddRangeAsync(expectedResult);
            await MockedDbContext.SaveChangesAsync();

            var actualResult = DbSet.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.EquivalentTo(expectedResult));
                Assert.That(DbSet.ToList(), Is.EquivalentTo(actualResult));
            });
        }

        [Test]
        public virtual void AnyThenAddAndPersistThenAny_ReturnsFalseThenTrue()
        {
            var actualResult1 = DbSet.Any();
            DbSet.Add(Fixture.Create<TEntity>());
            MockedDbContext.SaveChanges();
            var actualResult2 = DbSet.Any();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.False);
                Assert.That(actualResult2, Is.True);
            });
        }
    }
}