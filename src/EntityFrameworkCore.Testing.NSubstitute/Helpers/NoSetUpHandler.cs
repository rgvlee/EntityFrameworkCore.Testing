using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Extensions;
using rgvlee.Core.Common.Extensions;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    internal class NoSetUpHandler<TDbContext> : ICallHandler where TDbContext : DbContext
    {
        private static readonly ILogger<NoSetUpHandler<TDbContext>> Logger = LoggingHelper.CreateLogger<NoSetUpHandler<TDbContext>>();

        private readonly List<IEntityType> _allModelEntityTypes;

        private readonly TDbContext _dbContext;

        private readonly List<PropertyInfo> _dbContextModelProperties;

        public NoSetUpHandler(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _allModelEntityTypes = _dbContext.Model.GetEntityTypes().ToList();
            _dbContextModelProperties = _dbContext.GetType()
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToList();
        }

        public RouteAction Handle(ICall call)
        {
            var mockedDbContext = call.Target();
            var invokedMethod = call.GetMethodInfo();
            var arguments = call.GetArguments();

            var modelType = GetModelType(invokedMethod);
            if (modelType == null)
            {
                return invokedMethod.ReturnType != typeof(void) ? RouteAction.Return(invokedMethod.ReturnType.GetDefaultValue()) : RouteAction.Continue();
            }

            Logger.LogDebug("Setting up model '{type}'", modelType);

            var modelEntityType = _allModelEntityTypes.SingleOrDefault(x => x.ClrType.Equals(modelType));
            if (modelEntityType == null)
            {
                throw new InvalidOperationException(string.Format(ExceptionMessages.CannotCreateDbSetTypeNotIncludedInModel,
                    invokedMethod.GetGenericArguments().Single().Name));
            }

            var setUpModelMethod = typeof(NoSetUpHandler<TDbContext>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(x => x.Name.Equals(modelEntityType.FindPrimaryKey() != null ? "SetUpModel" : "SetUpReadOnlyModel"));

            setUpModelMethod.MakeGenericMethod(modelType).Invoke(this, new[] { mockedDbContext });

            return RouteAction.Return(invokedMethod.Invoke(mockedDbContext, arguments?.ToArray()));
        }

        private Type GetModelType(MethodInfo invokedMethod)
        {
            var dbContextModelProperty = _dbContextModelProperties.SingleOrDefault(x => x.GetMethod.Name.Equals(invokedMethod.Name));
            if (dbContextModelProperty != null)
            {
                return dbContextModelProperty.PropertyType.GetGenericArguments().Single();
            }

            if (!invokedMethod.IsGenericMethod)
            {
                return null;
            }

            var dbContextMethod = typeof(DbContext).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(x => x.IsGenericMethod && x.GetGenericMethodDefinition().Equals(invokedMethod.GetGenericMethodDefinition()));

            if (dbContextMethod != null)
            {
                return invokedMethod.GetGenericArguments().Single();
            }

            return null;
        }

        private void SetUpModel<TEntity>(TDbContext mockedDbContext) where TEntity : class
        {
            var mockedDbSet = _dbContext.Set<TEntity>().CreateMockedDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));

            if (property != null)
            {
                property.GetValue(mockedDbContext.Configure()).Returns(callInfo => mockedDbSet);
            }
            else
            {
                Logger.LogDebug("Could not find a DbContext property for type '{type}'", typeof(TEntity));
            }

            mockedDbContext.Configure().Set<TEntity>().Returns(callInfo => mockedDbSet);

            mockedDbContext.Add(Arg.Any<TEntity>()).Returns(callInfo => _dbContext.Add(callInfo.Arg<TEntity>()));
            mockedDbContext.AddAsync(Arg.Any<TEntity>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => _dbContext.AddAsync(callInfo.Arg<TEntity>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Attach(Arg.Any<TEntity>()).Returns(callInfo => _dbContext.Attach(callInfo.Arg<TEntity>()));

            mockedDbContext.Entry(Arg.Any<TEntity>()).Returns(callInfo => _dbContext.Entry(callInfo.Arg<TEntity>()));

            mockedDbContext.Find<TEntity>(Arg.Any<object[]>()).Returns(callInfo => _dbContext.Find<TEntity>(callInfo.Arg<object[]>()));

            mockedDbContext.FindAsync<TEntity>(Arg.Any<object[]>()).Returns(callInfo => _dbContext.FindAsync<TEntity>(callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync<TEntity>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => _dbContext.FindAsync<TEntity>(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Remove(Arg.Any<TEntity>()).Returns(callInfo => _dbContext.Remove(callInfo.Arg<TEntity>()));

            mockedDbContext.Update(Arg.Any<TEntity>()).Returns(callInfo => _dbContext.Update(callInfo.Arg<TEntity>()));
        }

        private void SetUpReadOnlyModel<TEntity>(TDbContext mockedDbContext) where TEntity : class
        {
            var mockedReadOnlyDbSet = _dbContext.Set<TEntity>().CreateMockedReadOnlyDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));
            if (property != null)
            {
                property.GetValue(mockedDbContext.Configure()).Returns(callInfo => mockedReadOnlyDbSet);
            }
            else
            {
                Logger.LogDebug("Could not find a DbContext property for type '{type}'", typeof(TEntity));
            }

            mockedDbContext.Configure().Set<TEntity>().Returns(callInfo => mockedReadOnlyDbSet);
        }
    }
}