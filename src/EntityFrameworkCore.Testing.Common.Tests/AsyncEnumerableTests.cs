using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public class AsyncEnumerableTests : BaseForQueryableTests<TestEntity>
    {
        private IQueryable<TestEntity> _source;

        protected override IQueryable<TestEntity> Queryable => _source;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _source = new AsyncEnumerable<TestEntity>(new List<TestEntity>());
        }

        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.Build<TestEntity>().With(p => p.CreatedAt, DateTime.Today).With(p => p.LastModifiedAt, DateTime.Today).CreateMany().ToList();
            _source = new AsyncEnumerable<TestEntity>(itemsToAdd);
            ItemsAddedToQueryableSource = itemsToAdd;
        }
    }
}