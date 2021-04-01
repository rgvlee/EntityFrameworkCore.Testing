using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common.Tests;
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
            MockedDbContext.AddSqlQueryAsyncResult<object>(sql, parameters);
        }

        protected override void AddFromSqlRawResult(string sql, IEnumerable<object> parameters, IEnumerable<TestEntity> expectedResult)
        {
            MockedDbContext.AddSqlQueryAsyncResult(sql, parameters, expectedResult);
        }
    }
}