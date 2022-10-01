using System.Data.Common;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class FeatureTests : BaseForTests
    {
        [Test]
        public void Set_DbConnection()
        {
            var mockConnection = new Mock<DbConnection>();
            var mockedDbContext = new MockedDbContextBuilder<TestDbContext>().UseDbConnection(mockConnection.Object).MockedDbContext;
            var connection = mockedDbContext.Database.GetDbConnection();
            Assert.That(connection, Is.EqualTo(mockConnection.Object));
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