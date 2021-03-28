using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class Issue47Tests : Issue47Tests<TestDbContext, TestEntity>
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            MockedDbContext = Create.MockedDbContextFor<TestDbContext>();
        }

        protected override void AddExecuteSqlRawResult(string sql, IEnumerable<object> parameters, int expectedResult)
        {
            MockedDbContext.AddExecuteSqlRawResult(sql, parameters, expectedResult);
        }

        protected override void AddFromSqlRawResult(string sql, IEnumerable<object> parameters, IEnumerable<TestEntity> expectedResult)
        {
            MockedDbContext.Set<TestEntity>().AddFromSqlRawResult(sql, parameters, expectedResult);
        }
    }
}