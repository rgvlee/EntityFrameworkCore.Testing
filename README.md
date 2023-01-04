# EntityFrameworkCore.Testing

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/6d641ce391264f45b99acee8694a88d6?branch=5.x)](https://www.codacy.com/manual/rgvlee/EntityFrameworkCore.Testing?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=rgvlee/EntityFrameworkCore.Testing&amp;utm_campaign=Badge_Grade) [![Codacy Badge](https://app.codacy.com/project/badge/Coverage/6d641ce391264f45b99acee8694a88d6?branch=5.x)](https://www.codacy.com/manual/rgvlee/EntityFrameworkCore.Testing?utm_source=github.com&utm_medium=referral&utm_content=rgvlee/EntityFrameworkCore.Testing&utm_campaign=Badge_Coverage)

## Overview

EntityFrameworkCore.Testing adds relational support to the Microsoft EntityFrameworkCore in-memory database provider by mocking relational operations. It's easy to use (usually just a single line of code) with implementations for both Moq and NSubstitute.

The aim of this library is to allow you use the in-memory database provider in unit tests where the SUT invokes a relational operation. It'll allow you to specify expected results for these relational operations. It does not test your relational operations.

[Microsoft does not recommend mocking a db context](https://docs.microsoft.com/en-us/ef/core/testing/#unit-testing) and EntityFrameworkCore.Testing follows this advice by sending operations supported by the in-memory database provider to the in-memory database provider.

### With thanks to

- JetBrains for providing an all products pack open source licence

### EntityFrameworkCore 7

- [Source](https://github.com/rgvlee/EntityFrameworkCore.Testing/tree/5.x)
- [EntityFrameworkCore.Testing.Moq - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/5.0.0)
- [EntityFrameworkCore.Testing.NSubstitute - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/5.0.0)

### EntityFrameworkCore 6

- [Source](https://github.com/rgvlee/EntityFrameworkCore.Testing/tree/4.x)
- [EntityFrameworkCore.Testing.Moq - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/4.0.2)
- [EntityFrameworkCore.Testing.NSubstitute - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/4.0.2)

### EntityFrameworkCore 5

- [Source](https://github.com/rgvlee/EntityFrameworkCore.Testing/tree/3.x)
- [EntityFrameworkCore.Testing.Moq - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/3.0.5)
- [EntityFrameworkCore.Testing.NSubstitute - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/3.0.5)

### EntityFrameworkCore 3

- [Source](https://github.com/rgvlee/EntityFrameworkCore.Testing/tree/2.x)
- [EntityFrameworkCore.Testing.Moq - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/2.4.5)
- [EntityFrameworkCore.Testing.NSubstitute - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/2.4.5)

### EntityFrameworkCore 2 (2.1.0+)

- [Source](https://github.com/rgvlee/EntityFrameworkCore.Testing/tree/1.x)
- [EntityFrameworkCore.Testing.Moq - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/1.3.5)
- [EntityFrameworkCore.Testing.NSubstitute - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/1.3.5)

## Prerequisites

### An accessible constructor

Your db context must have an accessible constructor.

### Virtual sets/queries

Your db context set/query properties must be overridable:

```c#
public virtual DbSet<TestEntity> TestEntities { get; set; }
```

## Creating a mocked DbContext

If your db context has an accessible constructor with a single `DbContextOptions` or `DbContextOptions<TDbContext>` parameter, creating a mocked db context is as easy as:

```c#
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
```

Any accessible constructor can be used provided it has a `DbContextOptions` or `DbContextOptions<TDbContext>` parameter:

```c#
var mockedLogger = Mock.Of<ILogger<TestDbContext>>();
var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>(mockedLogger, dbContextOptions);
```

Both of the above examples automatically create and use a Microsoft in-memory provider instance for the EntityFrameworkCore provider. If you want more control e.g., to specify the EntityFrameworkCore provider instance, use the builder:

```c#
var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
var dbContextToMock = new TestDbContext(options);
var mockedDbContext = new MockedDbContextBuilder<TestDbContext>().UseDbContext(dbContextToMock).UseConstructorWithParameters(options).MockedDbContext;
```

There is no requirement to use the Microsoft in-memory provider. The following example uses the SQLite in-memory provider for a db context with a parameterless constructor:

```c#
using (var connection = new SqliteConnection("Filename=:memory:"))
{
    connection.Open();
    var testEntity = _fixture.Create<TestEntity>();
    var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseSqlite(connection).Options);
    dbContextToMock.Database.EnsureCreated();
    var mockedDbContext = new MockedDbContextBuilder<TestDbContext>().UseDbContext(dbContextToMock).MockedDbContext;

    mockedDbContext.Set<TestEntity>().Add(testEntity);
    mockedDbContext.SaveChanges();

    Assert.Multiple(() =>
    {
        Assert.AreNotEqual(default(Guid), testEntity.Id);
        Assert.DoesNotThrow(() => mockedDbContext.Set<TestEntity>().Single());
        Assert.AreEqual(testEntity, mockedDbContext.Find<TestEntity>(testEntity.Id));
    });
}
```

## Usage

Start by creating a mocked db context and, if the SUT requires, populate it as if you were using the real thing:

```c#
var testEntity = _fixture.Create<TestEntity>();
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
mockedDbContext.Set<TestEntity>().Add(testEntity);
mockedDbContext.SaveChanges();

Assert.Multiple(() =>
{
    Assert.AreNotEqual(default(Guid), testEntity.Id);
    Assert.DoesNotThrow(() => mockedDbContext.Set<TestEntity>().Single());
    Assert.AreEqual(testEntity, mockedDbContext.Find<TestEntity>(testEntity.Id));
});
```

The Moq implementation of `Create.MockedDbContextFor<T>()` returns the mocked db context. If you need the mock itself (e.g., to verify an invocation) use `Mock.Get(mockedDbSet)`:

```c#
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
mockedDbContext.Set<TestEntity>().AddRange(_fixture.CreateMany<TestEntity>().ToList());
mockedDbContext.SaveChanges();

Assert.Multiple(() =>
{
    var dbContextMock = Mock.Get(mockedDbContext);
    dbContextMock.Verify(m => m.SaveChanges(), Times.Once);
});
```

### FromSql

Use `AddFromSqlResult` to add a from SQL result to the mock. The following will return `expectedResult` for any `FromSql<TestEntity>` invocation:

```c#
var expectedResult = _fixture.CreateMany<TestEntity>().ToList();
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
mockedDbContext.Set<TestEntity>().AddFromSqlRawResult(expectedResult);

var actualResult = mockedDbContext.Set<TestEntity>().FromSqlRaw("[dbo].[USP_StoredProcedureWithNoParameters]").ToList();

Assert.Multiple(() =>
{
    Assert.IsNotNull(actualResult);
    Assert.IsTrue(actualResult.Any());
    CollectionAssert.AreEquivalent(expectedResult, actualResult);
});
```

The following will return `expectedResult` if the `FromSql` SQL query text contains `usp_StoredProcedureWithParameters` and a `@Parameter2` SQL parameter with a value of `Value2` has been provided:

```c#
var expectedResult = _fixture.CreateMany<TestEntity>().ToList();
var sqlParameters = new List<SqlParameter> { new SqlParameter("@Parameter2", "Value2") };
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
mockedDbContext.Set<TestEntity>().AddFromSqlRawResult("usp_StoredProcedureWithParameters", sqlParameters, expectedResult);

var actualResult = mockedDbContext.Set<TestEntity>()
    .FromSqlRaw("[dbo].[USP_StoredProcedureWithParameters] @Parameter1 @Parameter2",
        new SqlParameter("@parameter1", "Value1"),
        new SqlParameter("@parameter2", "value2"))
    .ToList();

Assert.Multiple(() =>
{
    Assert.IsNotNull(actualResult);
    Assert.IsTrue(actualResult.Any());
    CollectionAssert.AreEquivalent(expectedResult, actualResult);
});
```

SQL query text matching supports partial, case insensitive matches. Individual parameter name and value matching is also case insentive. Case insensitive interpolated strings are also supported:

```c#
var expectedResult = _fixture.CreateMany<TestEntity>().ToList();
var parameter1 = _fixture.Create<DateTime>();
var parameter2 = _fixture.Create<string>();
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
mockedDbContext.Set<TestEntity>().AddFromSqlInterpolatedResult($"usp_StoredProcedureWithParameters {parameter1}, {parameter2.ToUpper()}", expectedResult);

var actualResult = mockedDbContext.Set<TestEntity>().FromSqlInterpolated($"USP_StoredProcedureWithParameters {parameter1}, {parameter2.ToLower()}").ToList();

Assert.Multiple(() =>
{
    Assert.IsNotNull(actualResult);
    Assert.IsTrue(actualResult.Any());
    CollectionAssert.AreEquivalent(expectedResult, actualResult);
});
```

### FromSqlRaw/FromSqlInterpolated (EntityFrameworkCore 3.\*)

Use `AddFromSqlRawResult` and `AddFromSqlInterpolatedResult` to add results for `FromSqlRaw` and `FromSqlInterpolated` invocations. Refer to the FromSql section above for usage.

### Queries

Use `AddToReadOnlySource`, `AddRangeToReadOnlySource` and `ClearReadOnlySource` to manage a query source.

```c#
var expectedResult = _fixture.CreateMany<TestReadOnlyEntity>().ToList();
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

mockedDbContext.Set<TestReadOnlyEntity>().AddRangeToReadOnlySource(expectedResult);

Assert.Multiple(() =>
{
    CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.Set<TestReadOnlyEntity>().ToList());
    CollectionAssert.AreEquivalent(mockedDbContext.Set<TestReadOnlyEntity>().ToList(), mockedDbContext.TestReadOnlyEntities.ToList());
});
```

Specifying a from SQL result for a query is exactly the same as for a set.

### Keyless Sets (EntityFrameworkCore 3.\*)

Refer to the Queries section above for usage.

### ExecuteSqlCommand

Adding an execute SQL command result is similar to adding a from SQL result with the main difference being the return type. `ExecuteSqlCommand` returns an `int` (the number of rows affected by executing the SQL command text).

```c#
var commandText = "usp_StoredProcedureWithNoParameters";
var expectedResult = 1;
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
mockedDbContext.AddExecuteSqlRawResult(commandText, new List<SqlParameter>(), expectedResult);

var result = mockedDbContext.Database.ExecuteSqlRaw("USP_StoredProcedureWithNoParameters");

Assert.AreEqual(expectedResult, result);
```

All of the overloads have an optional `Action<string, IEnumerable<object>>` parameter which allows you to perform operations post invocation. The following provides a basic example where invoking `ExecuteSqlCommand` deletes a specified number of rows from a set:

```c#
//Arrange
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

var itemsToCreate = 100;
mockedDbContext.Set<TestEntity>().AddRange(_fixture.CreateMany<TestEntity>(itemsToCreate).ToList());
mockedDbContext.SaveChanges();

var numberOfRowsToDelete = itemsToCreate / 2;
var rowsToDelete = mockedDbContext.Set<TestEntity>().Take(numberOfRowsToDelete).ToList();
var remainingRows = mockedDbContext.Set<TestEntity>().Skip(numberOfRowsToDelete).ToList();

mockedDbContext.AddExecuteSqlRawResult("usp_MyStoredProc",
    numberOfRowsToDelete,
    (providedSql, providedParameters) =>
    {
        mockedDbContext.Set<TestEntity>().RemoveRange(rowsToDelete);
        mockedDbContext.SaveChanges();
    });

//Act
var actualResult = mockedDbContext.Database.ExecuteSqlRaw($"usp_MyStoredProc {numberOfRowsToDelete}");

//Assert
Assert.Multiple(() =>
{
    Assert.That(actualResult, Is.EqualTo(numberOfRowsToDelete));
    Assert.That(mockedDbContext.Set<TestEntity>().Count(), Is.EqualTo(itemsToCreate - numberOfRowsToDelete));
    Assert.That(mockedDbContext.Set<TestEntity>().ToList(), Is.EquivalentTo(remainingRows));
});
```

### ExecuteSqlRaw/ExecuteSqlInterpolated (EntityFrameworkCore 3.\*)

Use `AddExecuteSqlRawResult` and `AddExecuteSqlInterpolatedResult` to add results for `ExecuteSqlRaw` and `ExecuteSqlInterpolated` invocations. Refer to the ExecuteSqlCommand section above for usage.

### Async and LINQ queryable operations

Whenever you add a from SQL or execute SQL command result, EntityFrameworkCore.Testing sets up both the sync and async methods. It also automatically provides support for all sync and async LINQ queryable operations that are not supported by the Microsoft in-memory provider.

### Asserting mock invocations

The db context and each set, query and their respective query providers are separate mocks. The following Moq example asserts that the `DbContext.SaveChanges` and `DbSet<TestEntity>.AddRange` methods were both invoked once.

```c#
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
mockedDbContext.Set<TestEntity>().AddRange(_fixture.CreateMany<TestEntity>().ToList());
mockedDbContext.SaveChanges();

var dbContextMock = Mock.Get(mockedDbContext);
dbContextMock.Verify(m => m.SaveChanges(), Times.Once);

var byTypeDbSetMock = Mock.Get(mockedDbContext.Set<TestEntity>());
byTypeDbSetMock.Verify(m => m.AddRange(It.IsAny<IEnumerable<TestEntity>>()), Times.Once);

var byPropertyDbSetMock = Mock.Get(mockedDbContext.TestEntities);
byPropertyDbSetMock.Verify(m => m.AddRange(It.IsAny<IEnumerable<TestEntity>>()), Times.Once);
```
