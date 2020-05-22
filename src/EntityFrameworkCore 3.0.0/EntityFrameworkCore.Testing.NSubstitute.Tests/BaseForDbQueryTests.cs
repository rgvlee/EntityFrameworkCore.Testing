﻿using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public abstract class BaseForDbQueryTests<T> : BaseForReadOnlyDbSetTests<T> where T : BaseTestEntity
    {
        [SetUp]
        public override void SetUp()
        {
            MockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            base.SetUp();
        }

        protected TestDbContext MockedDbContext;

        protected override void AddFromSqlRawResult(DbSet<T> mockedDbSet, IEnumerable<T> expectedResult)
        {
            mockedDbSet.AddFromSqlRawResult(expectedResult);
        }

        protected override void AddFromSqlRawResult(DbSet<T> mockedDbSet, string sql, IEnumerable<T> expectedResult)
        {
            mockedDbSet.AddFromSqlRawResult(sql, expectedResult);
        }

        protected override void AddFromSqlRawResult(DbSet<T> mockedDbSet, string sql, IEnumerable<object> parameters, IEnumerable<T> expectedResult)
        {
            mockedDbSet.AddFromSqlRawResult(sql, parameters, expectedResult);
        }

        protected override void AddFromSqlInterpolatedResult(DbSet<T> mockedDbSet, IEnumerable<T> expectedResult)
        {
            mockedDbSet.AddFromSqlInterpolatedResult(expectedResult);
        }

        protected override void AddFromSqlInterpolatedResult(DbSet<T> mockedDbSet, FormattableString sql, IEnumerable<T> expectedResult)
        {
            mockedDbSet.AddFromSqlInterpolatedResult(sql, expectedResult);
        }

        protected override void AddFromSqlInterpolatedResult(DbSet<T> mockedDbSet, string sql, IEnumerable<object> parameters, IEnumerable<T> expectedResult)
        {
            mockedDbSet.AddFromSqlInterpolatedResult(sql, parameters, expectedResult);
        }

        protected override void AddToReadOnlySource(DbSet<T> mockedDbQuery, T item)
        {
            mockedDbQuery.AddToReadOnlySource(item);
        }

        protected override void AddRangeToReadOnlySource(DbSet<T> mockedDbQuery, IEnumerable<T> items)
        {
            mockedDbQuery.AddRangeToReadOnlySource(items);
        }

        protected override void ClearReadOnlySource(DbSet<T> mockedDbQuery)
        {
            mockedDbQuery.ClearReadOnlySource();
        }
    }
}