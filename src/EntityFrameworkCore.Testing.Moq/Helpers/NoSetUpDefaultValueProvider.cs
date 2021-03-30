using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Moq;
using rgvlee.Core.Common.Extensions;
using rgvlee.Core.Common.Helpers;
using ProjectExpressionHelper = EntityFrameworkCore.Testing.Common.Helpers.ExpressionHelper;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    internal class NoSetUpDefaultValueProvider<TDbContext> : DefaultValueProvider where TDbContext : DbContext
    {
        private static readonly ILogger<NoSetUpDefaultValueProvider<TDbContext>> Logger = LoggingHelper.CreateLogger<NoSetUpDefaultValueProvider<TDbContext>>();

        private readonly List<IEntityType> _allModelEntityTypes;

        private readonly TDbContext _dbContext;

        private readonly List<PropertyInfo> _dbContextModelProperties;

        public NoSetUpDefaultValueProvider(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _allModelEntityTypes = _dbContext.Model.GetEntityTypes().ToList();
            _dbContextModelProperties = _dbContext.GetType()
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToList();
        }

        protected override object GetDefaultValue(Type type, Mock mock)
        {
            var lastInvocation = mock.Invocations.Last();

            var modelType = GetModelType(lastInvocation);
            if (modelType == null)
            {
                return type.GetDefaultValue();
            }

            Logger.LogDebug("Setting up model '{type}'", modelType);

            var modelEntityType = _allModelEntityTypes.SingleOrDefault(x => x.ClrType.Equals(modelType));
            if (modelEntityType == null)
            {
                throw new InvalidOperationException(string.Format(ExceptionMessages.CannotCreateDbSetTypeNotIncludedInModel,
                    lastInvocation.Method.GetGenericArguments().Single().Name));
            }

            var setUpModelMethod = typeof(NoSetUpDefaultValueProvider<TDbContext>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(x => x.Name.Equals(modelEntityType.FindPrimaryKey() != null ? "SetUpModel" : "SetUpReadOnlyModel"));

            setUpModelMethod.MakeGenericMethod(modelType).Invoke(this, new[] { mock });

            return lastInvocation.Method.Invoke(mock.Object, lastInvocation.Arguments?.ToArray());
        }

        private Type GetModelType(IInvocation invocation)
        {
            var dbContextModelProperty = _dbContextModelProperties.SingleOrDefault(x => x.GetMethod.Name.Equals(invocation.Method.Name));
            if (dbContextModelProperty != null)
            {
                return dbContextModelProperty.PropertyType.GetGenericArguments().Single();
            }

            if (!invocation.Method.IsGenericMethod)
            {
                return null;
            }

            var dbContextMethod = typeof(DbContext).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(x => x.IsGenericMethod && x.GetGenericMethodDefinition().Equals(invocation.Method.GetGenericMethodDefinition()));

            if (dbContextMethod != null)
            {
                return invocation.Method.GetGenericArguments().Single();
            }

            return null;
        }

        private void SetUpModel<TEntity>(Mock<TDbContext> dbContextMock) where TEntity : class
        {
            var mockedDbSet = _dbContext.Set<TEntity>().CreateMockedDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));

            if (property != null)
            {
                var propertyExpression = ProjectExpressionHelper.CreatePropertyExpression<TDbContext, DbSet<TEntity>>(property);
                dbContextMock.Setup(propertyExpression).Returns(() => mockedDbSet);
            }
            else
            {
                Logger.LogDebug("Could not find a DbContext property for type '{type}'", typeof(TEntity));
            }

            dbContextMock.Setup(m => m.Set<TEntity>()).Returns(() => mockedDbSet);

            dbContextMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                .Returns((TEntity providedEntity, CancellationToken providedCancellationToken) => _dbContext.AddAsync(providedEntity, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Attach(providedEntity));

            dbContextMock.Setup(m => m.Entry(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Entry(providedEntity));

            dbContextMock.Setup(m => m.Find<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => _dbContext.Find<TEntity>(providedKeyValues));

            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => _dbContext.FindAsync<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] providedKeyValues, CancellationToken providedCancellationToken) => _dbContext.FindAsync<TEntity>(providedKeyValues, providedCancellationToken));

            dbContextMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Remove(providedEntity));

            dbContextMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => _dbContext.Update(providedEntity));
        }

        private void SetUpReadOnlyModel<TEntity>(Mock<TDbContext> dbContextMock) where TEntity : class
        {
            var mockedReadOnlyDbSet = _dbContext.Set<TEntity>().CreateMockedReadOnlyDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));
            if (property != null)
            {
                var propertyExpression = ProjectExpressionHelper.CreatePropertyExpression<TDbContext, DbSet<TEntity>>(property);
                dbContextMock.Setup(propertyExpression).Returns(() => mockedReadOnlyDbSet);
            }
            else
            {
                Logger.LogDebug("Could not find a DbContext property for type '{type}'", typeof(TEntity));
            }

            dbContextMock.Setup(m => m.Set<TEntity>()).Returns(() => mockedReadOnlyDbSet);
        }
    }
}