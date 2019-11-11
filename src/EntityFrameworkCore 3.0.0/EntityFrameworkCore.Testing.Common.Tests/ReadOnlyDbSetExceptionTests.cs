using System;
using System.ComponentModel;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class ReadOnlyDbSetExceptionTests<TEntity> : TestBase
        where TEntity : TestEntityBase
    {
        protected abstract DbSet<TEntity> DbSet { get; }

        [Test]
        public void Add_Item_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { DbSet.Add(Fixture.Create<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }

        [Test]
        public void AddAsync_Item_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => { await DbSet.AddAsync(Fixture.Create<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }

        [Test]
        public void AddRange_Items_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { DbSet.AddRange(Fixture.CreateMany<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }

        [Test]
        public void AddRangeAsync_Items_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => { await DbSet.AddRangeAsync(Fixture.CreateMany<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }

        [Test]
        public void Attach_Item_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { DbSet.Attach(Fixture.Create<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }

        [Test]
        public void AttachRange_Items_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { DbSet.AttachRange(Fixture.CreateMany<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }

        [Test]
        public void ContainsListCollection_ReturnsFalse()
        {
            var containsListCollection = ((IListSource) DbSet).ContainsListCollection;
            Assert.That(containsListCollection, Is.False);
        }

        [Test]
        public void Find_Item_ThrowsException()
        {
            var itemToFind = Fixture.Create<TEntity>();
            var ex = Assert.Throws<NullReferenceException>(() => { DbSet.Find(itemToFind.Guid); });
            Assert.That(ex.Message, Is.EqualTo("Object reference not set to an instance of an object."));
        }

        [Test]
        public void Find_Items_ThrowsException()
        {
            var itemsToFind = Fixture.CreateMany<TEntity>();
            var ex = Assert.ThrowsAsync<NullReferenceException>(async () => { await DbSet.FindAsync(itemsToFind); });
            Assert.That(ex.Message, Is.EqualTo("Object reference not set to an instance of an object."));
        }

        [Test]
        public void Local_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var localView = DbSet.Local;
            });
            Assert.That(ex.Message, Is.EqualTo($"The invoked method is cannot be used for the entity type '{typeof(TEntity).Name}' because it does not have a primary key."));
        }

        [Test]
        public void Remove_Item_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { DbSet.Remove(Fixture.Create<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }

        [Test]
        public void RemoveRange_Items_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { DbSet.RemoveRange(Fixture.CreateMany<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }

        [Test]
        public void Update_Item_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { DbSet.Update(Fixture.Create<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }

        [Test]
        public void UpdateRange_Items_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { DbSet.UpdateRange(Fixture.CreateMany<TEntity>()); });
            Assert.That(ex.Message, Is.EqualTo($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked."));
        }
    }
}