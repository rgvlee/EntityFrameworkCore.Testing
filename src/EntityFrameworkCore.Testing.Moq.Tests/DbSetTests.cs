using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common.Tests;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    [TestFixture]
    public class DbSetTests : TestBase {
        [Test]
        public void AddAndPersist_Entity_Persists() {
            var testEntity1 = new TestEntity1();

            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var dbContext = new TestDbContext(options);
            var dbSet = dbContext.Set<TestEntity1>();

            dbSet.Add(testEntity1);
            dbContext.SaveChanges();

            Assert.Multiple(() => {
                Assert.IsTrue(dbContext.Set<TestEntity1>().Any());
                Assert.IsTrue(dbContext.TestEntities.Any());
            });
        }

        [Test]
        public async Task AddAndPersistAsync_Entity_Persists() {
            var testEntity1 = new TestEntity1();

            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var dbContext = new TestDbContext(options);
            var dbSet = dbContext.Set<TestEntity1>();

            await dbSet.AddAsync(testEntity1);
            await dbContext.SaveChangesAsync();

            Assert.Multiple(() => {
                Assert.IsTrue(dbContext.Set<TestEntity1>().Any());
                Assert.IsTrue(dbContext.TestEntities.Any());
            });
        }
    }
}