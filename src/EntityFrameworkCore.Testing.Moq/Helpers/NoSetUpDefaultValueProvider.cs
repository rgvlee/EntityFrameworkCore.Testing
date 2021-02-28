using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Moq;
using rgvlee.Core.Common.Extensions;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    internal class NoSetUpDefaultValueProvider<TDbContext> : DefaultValueProvider where TDbContext : DbContext
    {
        private static readonly ILogger<NoSetUpDefaultValueProvider<TDbContext>> Logger = LoggingHelper.CreateLogger<NoSetUpDefaultValueProvider<TDbContext>>();

        private readonly List<IEntityType> _allModelEntityTypes;

        private readonly TDbContext _dbContext;

        private readonly List<PropertyInfo> _dbContextModelEntityProperties;

        public NoSetUpDefaultValueProvider(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _allModelEntityTypes = _dbContext.Model.GetEntityTypes().ToList();
            _dbContextModelEntityProperties = _dbContext.GetType()
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType &&
                            (x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) || x.PropertyType.GetGenericTypeDefinition() == typeof(DbQuery<>)))
                .ToList();
        }

        protected override object GetDefaultValue(Type type, Mock mock)
        {
            var lastInvocation = mock.Invocations.Last();
            var lastInvocationIsByType = lastInvocation.Method.Name.Equals("Set") || lastInvocation.Method.Name.Equals("Query");
            var dbContextModelEntityProperty = _dbContextModelEntityProperties.SingleOrDefault(x => x.GetMethod.Name.Equals(lastInvocation.Method.Name));
            var lastInvocationIsByProperty = dbContextModelEntityProperty != null;

            if (lastInvocationIsByType || lastInvocationIsByProperty)
            {
                var setType = lastInvocationIsByType
                    ? lastInvocation.Method.GetGenericArguments().Single()
                    : dbContextModelEntityProperty.PropertyType.GetGenericArguments().Single();

                Logger.LogDebug("Setting up setType: '{setType}'", setType);

                var entityType = _allModelEntityTypes.SingleOrDefault(x => x.ClrType.Equals(setType));
                if (entityType == null)
                {
                    throw new InvalidOperationException(string.Format(ExceptionMessages.CannotCreateDbSetTypeNotIncludedInModel,
                        lastInvocation.Method.GetGenericArguments().Single().Name));
                }

                var setUpModelEntityMethod = typeof(NoSetUpDefaultValueProvider<TDbContext>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Single(x => x.Name.Equals(entityType.FindPrimaryKey() != null ? "SetUpModelEntity" : "SetUpReadOnlyModelEntity"));

                setUpModelEntityMethod.MakeGenericMethod(setType).Invoke(this, new[] { mock });

                return lastInvocation.Method.Invoke(mock.Object, null);
            }

            return type.GetDefaultValue();
        }

        private void SetUpModelEntity<TEntity>(Mock<TDbContext> dbContextMock) where TEntity : class
        {
            var mockedDbSet = _dbContext.Set<TEntity>().CreateMockedDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));

            if (property != null)
            {
                var propertyExpression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbSet<TEntity>>(property);
                dbContextMock.Setup(propertyExpression).Returns(mockedDbSet);
            }
            else
            {
                Logger.LogDebug("Could not find a DbContext property for type '{type}'", typeof(TEntity));
            }

            dbContextMock.Setup(m => m.Set<TEntity>()).Returns(mockedDbSet);

            dbContextMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                .Returns((TEntity providedEntity, CancellationToken providedCancellationToken) => _dbContext.AddAsync(providedEntity, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Attach(providedEntity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => _dbContext.AttachRange(providedEntities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => _dbContext.AttachRange(providedEntities));

            dbContextMock.Setup(m => m.Entry(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Entry(providedEntity));

            dbContextMock.Setup(m => m.Find<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => _dbContext.Find<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.Find(typeof(TEntity), It.IsAny<object[]>()))
                .Returns((Type providedType, object[] providedKeyValues) => _dbContext.Find(providedType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => _dbContext.FindAsync<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] providedKeyValues, CancellationToken providedCancellationToken) => _dbContext.FindAsync<TEntity>(providedKeyValues, providedCancellationToken));

            dbContextMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Remove(providedEntity));

            dbContextMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Update(providedEntity));
        }

        private void SetUpReadOnlyModelEntity<TEntity>(Mock<TDbContext> dbContextMock) where TEntity : class
        {
            var mockedReadOnlyDbSet = _dbContext.Set<TEntity>().CreateMockedReadOnlyDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>) || p.PropertyType == typeof(DbQuery<TEntity>));
            if (property != null)
            {
                if (property.PropertyType == typeof(DbSet<TEntity>))
                {
                    var propertyExpression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbSet<TEntity>>(property);
                    dbContextMock.Setup(propertyExpression).Returns(mockedReadOnlyDbSet);
                }
                else
                {
                    var propertyExpression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbQuery<TEntity>>(property);
                    dbContextMock.Setup(propertyExpression).Returns(mockedReadOnlyDbSet);
                }
            }
            else
            {
                Logger.LogDebug("Could not find a DbContext property for type '{type}'", typeof(TEntity));
            }

            dbContextMock.Setup(m => m.Set<TEntity>()).Returns(mockedReadOnlyDbSet);
            dbContextMock.Setup(m => m.Query<TEntity>()).Returns(mockedReadOnlyDbSet);
        }
    }
}