using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Extensions;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    /// Extensions for mocks.
    /// </summary>
    public static class MockExtensions
    {
        /// <summary>
        /// Sets up the provider for a DbQuery mock.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbQueryMock">The DbQuery mock.</param>
        /// <param name="queryProviderMock">The query provider mock.</param>
        /// <returns>The DbQuery mock.</returns>
        public static Mock<DbQuery<TEntity>> SetUpProvider<TEntity>(this Mock<DbQuery<TEntity>> dbQueryMock,
            Mock<IQueryProvider> queryProviderMock)
            where TEntity : class
        {
            dbQueryMock.As<IQueryable<TEntity>>().SetUpProvider(queryProviderMock);
            return dbQueryMock;
        }

        /// <summary>
        /// Sets up the provider for a DbSet mock.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSetMock">The DbSet mock.</param>
        /// <param name="queryProviderMock">The query provider mock.</param>
        /// <returns>The DbSet mock.</returns>
        public static Mock<DbSet<TEntity>> SetUpProvider<TEntity>(this Mock<DbSet<TEntity>> dbSetMock,
            Mock<IQueryProvider> queryProviderMock)
            where TEntity : class
        {
            dbSetMock.As<IQueryable<TEntity>>().SetUpProvider(queryProviderMock);
            return dbSetMock;
        }

        /// <summary>
        /// Sets up the provider for a queryable mock.
        /// </summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryableMock">The queryable mock.</param>
        /// <param name="queryProviderMock">The query provider mock.</param>
        /// <returns>The queryable mock.</returns>
        public static Mock<IQueryable<T>> SetUpProvider<T>(this Mock<IQueryable<T>> queryableMock,
            Mock<IQueryProvider> queryProviderMock)
            where T : class
        {
            queryableMock.Setup(m => m.Provider).Returns(queryProviderMock.Object);
            return queryableMock;
        }

        /// <summary>
        /// Sets up a query for a DbContext mock.
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext type.</typeparam>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbContextMock">The DbContext mock.</param>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="dbQueryMock">The mock DbQuery.</param>
        /// <returns>The DbContext mock.</returns>
        public static Mock<TDbContext> SetUpDbQueryFor<TDbContext, TQuery>(this Mock<TDbContext> dbContextMock,
            Expression<Func<TDbContext, DbQuery<TQuery>>> expression,
            Mock<DbQuery<TQuery>> dbQueryMock)
            where TDbContext : DbContext
            where TQuery : class
        {
           dbContextMock.Setup(expression)
                .Callback(() => ((IEnumerable<TQuery>) dbQueryMock.Object).GetEnumerator().Reset())
                .Returns(() => dbQueryMock.Object);

            dbContextMock.Setup(m => m.Query<TQuery>())
                .Callback(() => ((IEnumerable<TQuery>) dbQueryMock.Object).GetEnumerator().Reset())
                .Returns(() => dbQueryMock.Object);

            return dbContextMock;
        }

        /// <summary>
        /// Sets up a query for a DbContext mock.
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext type.</typeparam>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbContextMock">The DbContext mock.</param>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="sequence">The sequence to use for the DbQuery.</param>
        /// <returns>The DbContext mock.</returns>
        public static Mock<TDbContext> SetUpDbQueryFor<TDbContext, TQuery>(this Mock<TDbContext> dbContextMock,
            Expression<Func<TDbContext, DbQuery<TQuery>>> expression,
            IEnumerable<TQuery> sequence)
            where TDbContext : DbContext
            where TQuery : class
        {
            var dbQueryMock = DbQueryHelper.CreateDbQueryMock(sequence);
            dbContextMock.SetUpDbQueryFor(expression, dbQueryMock);
            return dbContextMock;
        }
        
        /// <summary>
        /// Sets up a query for a DbContext mock.
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext type.</typeparam>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbContextMock">The DbContext mock.</param>
        /// <param name="dbQueryMock">The mock DbQuery.</param>
        /// <returns>The DbContext mock.</returns>
        public static Mock<TDbContext> SetUpDbQueryFor<TDbContext, TQuery>(this Mock<TDbContext> dbContextMock,
            Mock<DbQuery<TQuery>> dbQueryMock)
            where TDbContext : DbContext
            where TQuery : class 
        {
            var properties = typeof(TDbContext).GetProperties().Where(p =>
                p.PropertyType.IsGenericType && //must be a generic type for the next part of the predicate
                typeof(DbQuery<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition()));

            var property = properties.Single(p => p.PropertyType.GenericTypeArguments.Single() == typeof(TQuery));

            var expression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbQuery<TQuery>>(property);

            return dbContextMock.SetUpDbQueryFor(expression, dbQueryMock);
        }

        /// <summary>
        /// Sets up a query for a DbContext mock.
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext type.</typeparam>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbContextMock">The DbContext mock.</param>
        /// <param name="sequence">The sequence to use for the DbQuery.</param>
        /// <returns>The DbContext mock.</returns>
        public static Mock<TDbContext> SetUpDbQueryFor<TDbContext, TQuery>(this Mock<TDbContext> dbContextMock,
            IEnumerable<TQuery> sequence)
            where TDbContext : DbContext
            where TQuery : class 
        {
            var dbQueryMock = DbQueryHelper.CreateDbQueryMock(sequence);
            dbContextMock.SetUpDbQueryFor(dbQueryMock);
            return dbContextMock;
        }


        /// <summary>
        /// Sets up ExecuteSqlCommand invocations containing a specified sql string and sql parameters to return a specified result. 
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext type.</typeparam>
        /// <param name="dbContextMock">The DbContext mock</param>
        /// <param name="executeSqlCommandCommandText">The ExecuteSqlCommand sql string. Mock set up supports case insensitive partial matches.</param>
        /// <param name="sqlParameters">The ExecuteSqlCommand sql parameters. Mock set up supports case insensitive partial sql parameter sequence matching.</param>
        /// <param name="expectedResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The DbContext mock.</returns>
        public static Mock<TDbContext> AddExecuteSqlCommandResult<TDbContext>(this Mock<TDbContext> dbContextMock,
            string executeSqlCommandCommandText, IEnumerable<SqlParameter> sqlParameters, int expectedResult)
            where TDbContext : DbContext
        {
            //ExecuteSqlCommand creates a RawSqlCommand then ExecuteNonQuery is executed on the relational command property.
            //We need to:
            //1) Mock the relational command ExecuteNonQuery method
            //2) Mock the RawSqlCommand (doesn't implement any interfaces so we have to use a the concrete class which requires a constructor to be specified)
            //3) Mock the IRawSqlCommandBuilder build method to return our RawSqlCommand
            //4) Mock multiple the database facade GetService methods to avoid the 'Relational-specific methods can only be used when the context is using a relational database provider.' exception.

            var relationalCommand = new Mock<IRelationalCommand>();
            relationalCommand.Setup(m => m.ExecuteNonQuery(It.IsAny<IRelationalConnection>(), It.IsAny<IReadOnlyDictionary<string, object>>())).Returns(() => expectedResult);
            relationalCommand.Setup(m => m.ExecuteNonQueryAsync(It.IsAny<IRelationalConnection>(), It.IsAny<IReadOnlyDictionary<string, object>>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(expectedResult));

            var rawSqlCommand = new Mock<RawSqlCommand>(MockBehavior.Strict, relationalCommand.Object, new Dictionary<string, object>());
            rawSqlCommand.Setup(m => m.RelationalCommand).Returns(() => relationalCommand.Object);
            rawSqlCommand.Setup(m => m.ParameterValues).Returns(new Dictionary<string, object>());

            var rawSqlCommandBuilder = new Mock<IRawSqlCommandBuilder>();
            rawSqlCommandBuilder.Setup(m => m.Build(It.Is<string>(s => s.Contains(executeSqlCommandCommandText, StringComparison.CurrentCultureIgnoreCase)), It.Is<IEnumerable<object>>(
                    parameters => !sqlParameters.Except(parameters.Select(p => (SqlParameter)p), new SqlParameterParameterNameAndValueEqualityComparer()).Any()
                    )))
                .Returns(rawSqlCommand.Object)
                .Callback((string sql, IEnumerable<object> parameters) => {
                    var sb = new StringBuilder();
                    sb.Append(sql.GetType().Name);
                    sb.Append(" sql: ");
                    sb.AppendLine(sql);

                    sb.AppendLine("Parameters:");
                    foreach (var sqlParameter in parameters.Select(p => (SqlParameter)p)) {
                        sb.Append(sqlParameter.ParameterName);
                        sb.Append(": ");
                        if (sqlParameter.Value == null)
                            sb.AppendLine("null");
                        else
                            sb.AppendLine(sqlParameter.Value.ToString());
                    }

                    Console.WriteLine(sb.ToString());
                });

            var databaseFacade = new Mock<DatabaseFacade>(MockBehavior.Strict, new Mock<TDbContext>().Object);
            databaseFacade.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance.GetService(It.Is<Type>(t => t == typeof(IConcurrencyDetector)))).Returns(new Mock<IConcurrencyDetector>().Object);
            databaseFacade.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance.GetService(It.Is<Type>(t => t == typeof(IRawSqlCommandBuilder)))).Returns(rawSqlCommandBuilder.Object);
            databaseFacade.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance.GetService(It.Is<Type>(t => t == typeof(IRelationalConnection)))).Returns(new Mock<IRelationalConnection>().Object);

            dbContextMock.Setup(m => m.Database).Returns(databaseFacade.Object);

            return dbContextMock;
        }
    }
}