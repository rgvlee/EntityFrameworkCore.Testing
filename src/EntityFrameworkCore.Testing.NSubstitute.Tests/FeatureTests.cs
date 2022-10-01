using System.Data.Common;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class FeatureTests : BaseForTests
    {
        [Test]
        public void Set_DbConnection()
        {
            var mockConnection = Substitute.For<DbConnection>();
            var mockedDbContext = new MockedDbContextBuilder<TestDbContext>().UseDbConnection(mockConnection).MockedDbContext;
            var connection = mockedDbContext.Database.GetDbConnection();
            Assert.That(connection, Is.EqualTo(mockConnection));
        }

        [Test]
        public void Default_DbConnection()
        {
            var mockedDbContext = new MockedDbContextBuilder<TestDbContext>().MockedDbContext;
            var connection = mockedDbContext.Database.GetDbConnection();
            Assert.IsNull(connection);
        }
    }
}