# EntityFrameworkCore.Testing

## Overview

The intent of this library is to provide the ability to create and manage EntityFrameworkCore `DbContext` system mocks. It extends the Microsoft in-memory provider by providing configurable support for the following relational operations:

- FromSql *(EntityFrameworkCore 2.1.0-2.2.6)*
- FromSqlRaw *(EntityFrameworkCore 3.\*)*
- FromSqlInterpolated *(EntityFrameworkCore 3.\*)*
- ExecuteSqlCommand
- ExecuteSqlRaw *(EntityFrameworkCore 3.\*)*
- ExecuteSqlInterpolated *(EntityFrameworkCore 3.\*)*
- Queries
- Keyless Sets *(EntityFrameworkCore 3.\*)*

It also provides support for the following LINQ queryable operations:

- ElementAt
- ElementAtOrDefault
- Indexed Select (`Queryable.Select(Func<T, int, TResult>`))
- SkipWhile
- TakeWhile
- Indexed TakeWhile (`Queryable.Select(Func<T, int, bool>`))

It's easy to use with implementations for both Moq and NSubstitute.

## Resources

-   [Source repository](https://github.com/rgvlee/EntityFrameworkCore.Testing/)

### EntityFrameworkCore 3.\*

-   [EntityFrameworkCore.Testing.Moq](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/2.2.2)
-   [EntityFrameworkCore.Testing.NSubstitute](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/2.2.2)

### EntityFrameworkCore 2.1.0-2.2.6

-   [EntityFrameworkCore.Testing.Moq](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/1.1.2)
-   [EntityFrameworkCore.Testing.NSubstitute](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/1.1.2)

## Prerequisites

### Constructor parameters

Your `DbContext` must contain a constructor that accepts a `DbContextOptions` or `DbContextOptions<TDbContext>` parameter.

### Virtual sets/queries

Your `DbContext` set/query properties must be overridable:

```c#
public virtual DbSet<TestEntity> TestEntities { get; set; }
```

## Creating a mocked `DbContext`

### Creating by type

```c#
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
```

This requires you to have a `DbContext` with a constructor that has a single `DbContextOptions` or `DbContextOptions<TDbContext>` parameter - with the most specific constructor being used. Refer to the [documentation provided by Microsoft](https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory#add-a-constructor-for-testing) for further information.

```c#
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
}
```

### Using a specific constructor

Provided it has a `DbContextOptions` or `DbContextOptions<TDbContext>` parameter, you can use any accessible `DbContext` constructor to create the mocked `DbContext`.

```c#
var mockedCurrentUser = Mock.Of<ICurrentUser>();
var mockedLogger = Mock.Of<ILogger<TestDbContext>>();
var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>(mockedLogger, mockedCurrentUser, dbContextOptions);
```

### Advanced creation

If you need access to the Microsoft in-memory provider instance used by the mocked `DbContext` (e.g., to customize the mock), use the builder:

```c#
var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
var dbContextToMock = new TestDbContext(options);
var mockedDbContext = Build.MockedDbContextFor<TestDbContext>()
    .UsingDbContext(dbContextToMock)
    .UsingConstructorWithParameters(options)
    .Create();
```

Then extend the mock as required:

```c#
var dbContextMock = Mock.Get(mockedDbContext);
dbContextMock.Setup(x => x.SaveChanges())
    .Returns(() => dbContextToMock.SaveChanges())
    .Callback(() => Console.WriteLine("I am a meat popsicle"));
```

## Usage

Start by creating a mocked `DbContext` and, if the SUT requires, populate it as if you were using the real thing:

```c#
var testEntity = Fixture.Create<TestEntity>();
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

mockedDbContext.Set<TestEntity>().Add(testEntity);
mockedDbContext.SaveChanges();

Assert.Multiple(() =>
{
    Assert.AreNotEqual(default(Guid), testEntity.Guid);
    Assert.DoesNotThrow(() => mockedDbContext.Set<TestEntity>().Single());
    Assert.AreEqual(testEntity, mockedDbContext.Find<TestEntity>(testEntity.Guid));
});
```

The Moq implementation of `Create.MockedDbContextFor<T>()` returns the mocked `DbContext`. If you need the mock itself (e.g., to verify an invocation) use `Mock.Get(mockedDbSet)`:

```c#
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>().ToList());
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
var expectedResult = Fixture.CreateMany<TestEntity>().ToList();
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
mockedDbContext.Set<TestEntity>().AddFromSqlResult(expectedResult);

var actualResult = mockedDbContext.Set<TestEntity>().FromSql("[dbo].[USP_StoredProcedureWithNoParameters]").ToList();

Assert.Multiple(() =>
{
    Assert.IsNotNull(actualResult);
    Assert.IsTrue(actualResult.Any());
    CollectionAssert.AreEquivalent(expectedResult, actualResult);
});
```

We can be more specific; The following will return `expectedResult` if the `FromSql` SQL query text contains `usp_StoredProcedureWithParameters` and a `@Parameter2` SQL parameter with a value of `Value2` has been provided:

```c#
var expectedResult = Fixture.CreateMany<TestEntity>().ToList();
var sqlParameters = new List<SqlParameter> { new SqlParameter("@Parameter2", "Value2") };
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
mockedDbContext.Set<TestEntity>().AddFromSqlResult("usp_StoredProcedureWithParameters", sqlParameters, expectedResult);

var actualResult = mockedDbContext.Set<TestEntity>()
    .FromSql("[dbo].[USP_StoredProcedureWithParameters] @Parameter1 @Parameter2", new SqlParameter("@parameter1", "Value1"), new SqlParameter("@parameter2", "value2"))
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
var expectedResult = Fixture.CreateMany<TestEntity>().ToList();
var parameter1 = Fixture.Create<DateTime>();
var parameter2 = Fixture.Create<string>();
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

mockedDbContext.Set<TestEntity>().AddFromSqlResult($"usp_StoredProcedureWithParameters {parameter1}, {parameter2.ToUpper()}", expectedResult);

var actualResult = mockedDbContext.Set<TestEntity>().FromSql($"USP_StoredProcedureWithParameters {parameter1}, {parameter2.ToLower()}").ToList();

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
var expectedResult = Fixture.CreateMany<TestQuery>().ToList();
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

mockedDbContext.Query<TestQuery>().AddRangeToReadOnlySource(expectedResult);

Assert.Multiple(() =>
{
    CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.Query<TestQuery>().ToList());
    CollectionAssert.AreEquivalent(mockedDbContext.Query<TestQuery>().ToList(), mockedDbContext.TestView.ToList());
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
mockedDbContext.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

var result = mockedDbContext.Database.ExecuteSqlCommand("USP_StoredProcedureWithNoParameters");

Assert.AreEqual(expectedResult, result);
```

All of the overloads have an optional `Action<string, IEnumerable<object>>` parameter which allows you to perform operations post invocation. The following provides a basic example where invoking `ExecuteSqlCommand` deletes a specified number of rows from a set:

```c#
//Arrange
var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

var itemsToCreate = 100;
mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>(itemsToCreate).ToList());
mockedDbContext.SaveChanges();

var numberOfRowsToDelete = itemsToCreate / 2;
var rowsToDelete = mockedDbContext.Set<TestEntity>().Take(numberOfRowsToDelete).ToList();
var remainingRows = mockedDbContext.Set<TestEntity>().Skip(numberOfRowsToDelete).ToList();

mockedDbContext.AddExecuteSqlCommandResult("usp_MyStoredProc",
    numberOfRowsToDelete,
    (providedSql, providedParameters) =>
    {
        mockedDbContext.Set<TestEntity>().RemoveRange(rowsToDelete);
        mockedDbContext.SaveChanges();
    });

//Act
var actualResult = mockedDbContext.Database.ExecuteSqlCommand($"usp_MyStoredProc {numberOfRowsToDelete}");

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

Whenever you add a from SQL or execute SQL command result, the library sets up both the sync and async methods. It also intercepts and automatically provides support for all sync and async LINQ queryable operations that are not supported by the Microsoft in-memory provider.