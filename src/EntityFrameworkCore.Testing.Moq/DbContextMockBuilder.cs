using EntityFrameworkCore.Testing.Moq.Extensions;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace EntityFrameworkCore.Testing.Moq {
    /// <summary>
    /// A builder that creates a DbContext mock.
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext to mock type.</typeparam>
    public class DbContextMockBuilder<TDbContext>
        where TDbContext : DbContext {

        private readonly TDbContext _dbContextToMock;

        private readonly Mock<TDbContext> _dbContextMock;
        
        private readonly Dictionary<Type, Mock> _mockCache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="addSetUpForAllDbSets">If set to true all of the DbContext sets will be set up automatically.</param>
        /// <remarks>Automatically creates a new in-memory database that will be used to back the DbContext mock.
        /// Requires the <see>
        ///     <cref>TDbContext</cref>
        /// </see>
        /// type to have a DbContextOptions constructor.</remarks>
        public DbContextMockBuilder(bool addSetUpForAllDbSets = true) :
            this((TDbContext)Activator.CreateInstance(typeof(TDbContext), new DbContextOptionsBuilder<TDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options), 
                addSetUpForAllDbSets) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dbContextToMock">The DbContext to mock.</param>
        /// <param name="addSetUpForAllDbSets">If set to true all of the DbContext sets will be set up automatically.</param>
        public DbContextMockBuilder(TDbContext dbContextToMock, bool addSetUpForAllDbSets = true) {
            _mockCache = new Dictionary<Type, Mock>();

            _dbContextToMock = dbContextToMock;
            
            _dbContextMock = _dbContextToMock.CreateDbContextMock();

            if (addSetUpForAllDbSets) AddSetUpForAllDbSets();
        }

        private Mock GetMockFromCache(Type key) {
            var mock = _mockCache[key];
            if (mock == null) throw new Exception($"No set up found for '<{key.Name}>'.");
            return mock;
        }

        /// <summary>
        /// Creates mocks for all of the DbContext sets.
        /// </summary>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddSetUpForAllDbSets() {

            var properties = _dbContextToMock.GetType().GetProperties().Where(p =>
                p.PropertyType.IsGenericType && //must be a generic type for the next part of the predicate
                (typeof(DbSet<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition())));

            foreach (var property in properties) {
                var entityType = property.PropertyType.GenericTypeArguments.Single();
                // ReSharper disable PossibleNullReferenceException
                // ReSharper disable UnusedVariable
                // ReSharper disable ArrangeThisQualifier
                var expression = typeof(ExpressionHelper).GetMethods().Single(m => m.Name.Equals(nameof(ExpressionHelper.CreatePropertyExpression)) && m.GetParameters().ToList().Count == 1).MakeGenericMethod(typeof(TDbContext), property.PropertyType).Invoke(this, new []{ property });
                var builder = this.GetType().GetMethods().Single(m => m.Name.Equals(nameof(AddSetUpFor)) && m.GetParameters().ToList().Count == 1).MakeGenericMethod(entityType).Invoke(this, new []{ expression });
                // ReSharper restore ArrangeThisQualifier
                // ReSharper restore UnusedVariable
                // ReSharper restore PossibleNullReferenceException
            }

            return this;
        }

        /// <summary>
        /// Adds the mock set up for an entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="dbSetMock">The mock DbSet.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddSetUpFor<TEntity>(
            Expression<Func<TDbContext, DbSet<TEntity>>> expression, Mock<DbSet<TEntity>> dbSetMock) where TEntity : class {

            _dbContextMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity entity) => _dbContextToMock.Add(entity));
            _dbContextMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>())).Returns((TEntity entity, CancellationToken cancellationToken) => _dbContextToMock.AddAsync(entity, cancellationToken));
            _dbContextMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity entity) => _dbContextToMock.Attach(entity));
            _dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] entities) => _dbContextToMock.AttachRange(entities));
            _dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> entities) => _dbContextToMock.AttachRange(entities));
            _dbContextMock.Setup(m => m.Entry(It.IsAny<TEntity>())).Returns((TEntity entity) => _dbContextToMock.Entry(entity));
            _dbContextMock.Setup(m => m.Find<TEntity>(It.IsAny<object[]>())).Returns((object[] keyValues) => _dbContextToMock.Find<TEntity>(keyValues));
            _dbContextMock.Setup(m => m.Find(typeof(TEntity), It.IsAny<object[]>())).Returns((Type type, object[] keyValues) => _dbContextToMock.Find(type, keyValues));
            _dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>())).Returns((object[] keyValues) => _dbContextToMock.FindAsync<TEntity>(keyValues));
            _dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((object[] keyValues, CancellationToken cancellationToken) => _dbContextToMock.FindAsync<TEntity>(keyValues, cancellationToken));
            _dbContextMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity entity) => _dbContextToMock.Remove(entity));
            _dbContextMock.Setup(expression).Returns(() => dbSetMock.Object);
            _dbContextMock.Setup(m => m.Set<TEntity>()).Returns(() => dbSetMock.Object);
            _dbContextMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity entity) => _dbContextToMock.Update(entity));

            return this;
        }

        /// <summary>
        /// Adds the mock set up for an entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddSetUpFor<TEntity>(Expression<Func<TDbContext, DbSet<TEntity>>> expression) where TEntity : class {
            var key = expression.ReturnType.GetGenericArguments().Single();

            if (!_mockCache.ContainsKey(key)) {
                _mockCache.Add(key, _dbContextToMock.Set<TEntity>().CreateDbSetMock());
            }
            var dbSetMock = (Mock<DbSet<TEntity>>)_mockCache[key];

            AddSetUpFor(expression, dbSetMock);

            return this;
        }

        /// <summary>
        /// Adds the mock set up for a query.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="dbQueryMock">The mock DbQuery.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddSetUpFor<TQuery>(Expression<Func<TDbContext, DbQuery<TQuery>>> expression, Mock<DbQuery<TQuery>> dbQueryMock) where TQuery : class
        {
            var key = expression.ReturnType.GetGenericArguments().Single();

            if (!_mockCache.ContainsKey(key)) {
                _mockCache.Add(key, dbQueryMock);
            }
            else
            {
                _mockCache[key] = dbQueryMock;
            }

            _dbContextMock.SetUpDbQueryFor(expression, dbQueryMock);

            return this;
        }

        /// <summary>
        /// Adds the mock set up for a query.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="sequence">The sequence to use for operations on the query.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddSetUpFor<TQuery>(Expression<Func<TDbContext, DbQuery<TQuery>>> expression, IEnumerable<TQuery> sequence) where TQuery : class {
            return AddSetUpFor(expression, DbQueryHelper.CreateDbQueryMock(sequence));
        }
        
        /// <summary>
        /// Adds the specified query provider mock to the mock set up for the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="queryProviderMock">The query provider mock to add.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddQueryProviderMockFor<TEntity>(Expression<Func<TDbContext, IQueryable<TEntity>>> expression, Mock<IQueryProvider> queryProviderMock)
            where TEntity : class {
            var mock = GetMockFromCache(expression.ReturnType.GetGenericArguments().Single());
            mock.As<IQueryable<TEntity>>().SetUpProvider(queryProviderMock);
            return this;
        }
        
        /// <summary>
        /// Mocks the FromSql result for the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="expectedFromSqlResult">The expected FromSql result.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddFromSqlResultFor<TEntity>(Expression<Func<TDbContext, IQueryable<TEntity>>> expression, IEnumerable<TEntity> expectedFromSqlResult)
            where TEntity : class {
            var mock = GetMockFromCache(expression.ReturnType.GetGenericArguments().Single());
            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.SetUpFromSql(expectedFromSqlResult);
            mock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryProviderMock.Object);
            return this;
        }

        /// <summary>
        /// Mocks the FromSql result for invocations containing the specified sql string for the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="sql">The FromSql sql string. Mock set up supports case insensitive partial matches.</param>
        /// <param name="expectedFromSqlResult">The expected FromSql result.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddFromSqlResultFor<TEntity>(Expression<Func<TDbContext, IQueryable<TEntity>>> expression, string sql, IEnumerable<TEntity> expectedFromSqlResult)
            where TEntity : class {
            var mock = GetMockFromCache(expression.ReturnType.GetGenericArguments().Single());
            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.SetUpFromSql(sql, expectedFromSqlResult);
            mock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryProviderMock.Object);
            return this;
        }

        /// <summary>
        /// Mocks the FromSql result for invocations containing the specified sql string and sql parameters for the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="sql">The FromSql sql string. Mock set up supports case insensitive partial matches.</param>
        /// <param name="sqlParameters">The FromSql sql parameters. Mock set up supports case insensitive partial sql parameter sequence matching.</param>
        /// <param name="expectedFromSqlResult">The expected FromSql result.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddFromSqlResultFor<TEntity>(Expression<Func<TDbContext, IQueryable<TEntity>>> expression, string sql, IEnumerable<SqlParameter> sqlParameters, IEnumerable<TEntity> expectedFromSqlResult)
            where TEntity : class {
            var mock = GetMockFromCache(expression.ReturnType.GetGenericArguments().Single());
            var queryProviderMock = new Mock<IQueryProvider>();
            queryProviderMock.SetUpFromSql(sql, sqlParameters, expectedFromSqlResult);
            mock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryProviderMock.Object);
            return this;
        }

        /// <summary>
        /// Sets up ExecuteSqlCommand invocations containing a specified sql string and sql parameters to return a specified result. 
        /// </summary>
        /// <param name="executeSqlCommandCommandText">The ExecuteSqlCommand sql string. Mock set up supports case insensitive partial matches.</param>
        /// <param name="sqlParameters">The ExecuteSqlCommand sql parameters. Mock set up supports case insensitive partial sql parameter sequence matching.</param>
        /// <param name="expectedResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddExecuteSqlCommandResult(string executeSqlCommandCommandText, IEnumerable<SqlParameter> sqlParameters, int expectedResult)
        {
            _dbContextMock.AddExecuteSqlCommandResult(executeSqlCommandCommandText, sqlParameters, expectedResult);
            return this;
        }

        /// <summary>
        /// Sets up ExecuteSqlCommand invocations containing a specified sql string to return a specified result. 
        /// </summary>
        /// <param name="executeSqlCommandCommandText">The ExecuteSqlCommand sql string. Mock set up supports case insensitive partial matches.</param>
        /// <param name="expectedResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddExecuteSqlCommandResult(string executeSqlCommandCommandText, int expectedResult) {
            return AddExecuteSqlCommandResult(executeSqlCommandCommandText, new List<SqlParameter>(), expectedResult);
        }

        /// <summary>
        /// Sets up ExecuteSqlCommand invocations to return a specified result. 
        /// </summary>
        /// <param name="expectedResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The DbContext mock builder.</returns>
        public DbContextMockBuilder<TDbContext> AddExecuteSqlCommandResult(int expectedResult) {
            return AddExecuteSqlCommandResult(string.Empty, new List<SqlParameter>(), expectedResult);
        }

        /// <summary>
        /// Gets the set up DbContext mock.
        /// </summary>
        /// <returns>The DbContext mock.</returns>
        public Mock<TDbContext> GetDbContextMock() {
            return _dbContextMock;
        }

        /// <summary>
        /// Gets the set up mocked DbContext.
        /// </summary>
        /// <returns>The mocked DbContext.</returns>
        public TDbContext GetMockedDbContext() {
            return _dbContextMock.Object;
        }

        /// <summary>
        /// Gets the set up DbSet mock for the specified DbContext property.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <returns>The DbSet mock.</returns>
        public Mock<DbSet<TEntity>> GetDbSetMockFor<TEntity>() where TEntity : class {
            var mock = ((Mock<DbSet<TEntity>>)GetMockFromCache(typeof(TEntity)));
            return mock;
        }

        /// <summary>
        /// Gets the set up DbSet mock for the specified DbContext property.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The DbContext property.</param>
        /// <returns>The DbSet mock.</returns>
        public Mock<DbSet<TEntity>> GetDbSetMockFor<TEntity>(Expression<Func<TDbContext, DbSet<TEntity>>> expression) where TEntity : class {
            var mock = ((Mock<DbSet<TEntity>>)GetMockFromCache(expression.ReturnType.GetGenericArguments().Single()));
            return mock;
        }

        /// <summary>
        /// Gets the set up mocked DbSet for the specified DbContext property.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <returns>The mocked DbSet.</returns>
        public DbSet<TEntity> GetMockedDbSetFor<TEntity>() where TEntity : class {
            return GetDbSetMockFor<TEntity>().Object;
        }

        /// <summary>
        /// Gets the set up mocked DbSet for the specified DbContext property.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The DbContext property.</param>
        /// <returns>The mocked DbSet.</returns>
        public DbSet<TEntity> GetMockedDbSetFor<TEntity>(Expression<Func<TDbContext, DbSet<TEntity>>> expression) where TEntity : class {
            return GetDbSetMockFor(expression).Object;
        }
        
        /// <summary>
        /// Gets the set up DbQuery mock for the specified DbContext property.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <returns>The DbQuery mock.</returns>
        public Mock<DbQuery<TQuery>> GetDbQueryMockFor<TQuery>() where TQuery : class {
            var mock = ((Mock<DbQuery<TQuery>>)GetMockFromCache(typeof(TQuery)));
            return mock;
        }

        /// <summary>
        /// Gets the set up DbQuery mock for the specified DbContext property.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="expression">The DbContext property.</param>
        /// <returns>The DbQuery mock.</returns>
        public Mock<DbQuery<TQuery>> GetDbQueryMockFor<TQuery>(Expression<Func<TDbContext, DbQuery<TQuery>>> expression) where TQuery : class {
            var mock = ((Mock<DbQuery<TQuery>>)GetMockFromCache(expression.ReturnType.GetGenericArguments().Single()));
            return mock;
        }

        /// <summary>
        /// Gets the set up mocked DbQuery for the specified DbContext property.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <returns>The mocked DbQuery.</returns>
        public DbQuery<TQuery> GetMockedDbQueryFor<TQuery>() where TQuery : class {
            return GetDbQueryMockFor<TQuery>().Object;
        }

        /// <summary>
        /// Gets the set up mocked DbQuery for the specified DbContext property.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="expression">The DbContext property.</param>
        /// <returns>The mocked DbQuery.</returns>
        public DbQuery<TQuery> GetMockedDbQueryFor<TQuery>(Expression<Func<TDbContext, DbQuery<TQuery>>> expression) where TQuery : class {
            return GetDbQueryMockFor(expression).Object;
        }
    }
}