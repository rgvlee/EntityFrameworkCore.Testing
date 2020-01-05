

# EntityFrameworkCore.Testing
__*Moq and NSubstitute mocking libraries for EntityFrameworkCore*__

EntityFrameworkCore.Testing is an EntityFrameworkCore mocking library for Moq and NSubstitute. It's designed to be used in conjunction with the [Microsoft in-memory provider](https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory), extending it's functionality by providing support for:
- FromSql *(EntityFrameworkCore 2.1.0-2.2.6)*
- FromSqlRaw *(EntityFrameworkCore 3.0.0)*
- FromSqlInterpolated *(EntityFrameworkCore 3.0.0)*
- ExecuteSqlCommand
- ExecuteSqlRaw *(EntityFrameworkCore 3.0.0)*
- ExecuteSqlInterpolated *(EntityFrameworkCore 3.0.0)*
- DbQuery\<TQuery\>
- Keyless DbSet\<TEntity\> *(EntityFrameworkCore 3.0.0)*
- ElementAt
- ElementAtOrDefault
- Indexed Select (Queryable.Select(Func\<T, int, TResult\>))
- SkipWhile
- TakeWhile
- Indexed TakeWhile (Queryable.Select(Func\<T, int, bool\>))

There may be more functionality that isn't supported by the in-memory provider; this is what I've discovered/added support for so far. If you come across unsupported functionality let me know so I can add support for it.

In addition to the above you also get all of the benefits of using a mocking framework (e.g., the ability to verify method invocation).

## NuGet Packages
#### EntityFrameworkCore 3.0.0+
- [EntityFrameworkCore.Testing.Moq](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/2.0.3)
- [EntityFrameworkCore.Testing.NSubstitute](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/2.0.3)
#### EntityFrameworkCore 2.1.0-2.2.6
- [EntityFrameworkCore.Testing.Moq](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/1.0.6)
- [EntityFrameworkCore.Testing.NSubstitute](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/1.0.6)

# Moq

## Creating a mocked DbContext
There are two ways you can create the mocked `DbContext`:

### Creating by type
This method requires that you have a `DbContext` with a constructor that has a single `DbContextOptions<TDbContext>` or `DbContextOptions` parameter - with the most specific constructor being used.

```
public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
}

[Test]
public void Method_WithSpecifiedInput_ReturnsAResult()
{
    var mockedDbContext = Create.MockedDbContextFor<MyDbContext>();
     
    //...
}
```

The library supports creation using a parameterless constructor if that's all you have, however the assumption is that using such a constructor will result in a usable `DbContext`. To be clear, this library is intended to extend the in-memory provider and as such I don't recommend falling back on this unless you know what you're doing.

Finally, if you don't have a constructor that meets these requirements and you haven't specified any constructor parameters, you'll get a `MissingMethodException` and you'll need to add one or specify constructor parameters as per the next section.

### Creating by type using a specific constructor on your DbContext
If you're like me your `DbContext` constructor will have other parameters; loggers, current user and so on. Simply provide the constructor parameters for the constructor you want the library to use.

```
public class MyDbContextWithConstructorParameters : DbContext
{
    public MyDbContextWithConstructorParameters(
        ILogger<MyDbContextWithConstructorParameters> logger, 
        DbContextOptions<MyDbContextWithConstructorParameters> options) : base(options) { }
}

[Test]
public void AnotherMethod_WithSpecifiedInput_ReturnsAResult()
{
    var mockedDbContext = Create.MockedDbContextFor<MyDbContextWithConstructorParameters>(
        Mock.Of<ILogger<MyDbContextWithConstructorParameters>>(),
        new DbContextOptionsBuilder<MyDbContextWithConstructorParameters>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options
        );

    //...
}
```

## Example Usage

### MVP
- Create mocked `DbContext`
- Consume

```
[Test]
public void SetAddAndPersist_Item_AddAndPersistsItem()
{
    var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

    var testEntity = Fixture.Create<TestEntity>();

    mockedDbContext.Set<TestEntity>().Add(testEntity);
    mockedDbContext.SaveChanges();

    Assert.Multiple(() =>
    {
        Assert.AreNotEqual(default(Guid), testEntity.Guid);
        Assert.DoesNotThrow(() => mockedDbContext.Set<TestEntity>().Single());
        Assert.AreEqual(testEntity, mockedDbContext.Find<TestEntity>(testEntity.Guid));
    });
}
```

### FromSql
- Specify a `FromSql` result
- Consume

```
[Test]
public void FromSql_AnyStoredProcedureWithNoParameters_ReturnsExpectedResult()
{
    var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

    var expectedResult = Fixture.CreateMany<TestEntity>().ToList();

    mockedDbContext.Set<TestEntity>().AddFromSqlResult(expectedResult);

    var actualResult = mockedDbContext.Set<TestEntity>().FromSql("sp_NoParams").ToList();

    Assert.Multiple(() =>
    {
        Assert.IsNotNull(actualResult);
        Assert.IsTrue(actualResult.Any());
        CollectionAssert.AreEquivalent(expectedResult, actualResult);
    });
}
```

### FromSql with SQL/Parameters
The following example shows how to specify a result for a specific `FromSql` invocation.

The __sql__ matching is case insensitive and supports partial matches; in the example I've left out the schema name and it'll match on just the stored procedure name. The __parameters__ matching is case insensitive and supports partial sequence matching however it does not support partial matches on the parameter name/value; in this example we're only specifying one of the parameters. You only need to specify the bare distinct minimum.

```
[Test]
public void FromSql_SpecifiedStoredProcedureAndParameters_ReturnsExpectedResult()
{
    var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

    var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
    var expectedResult = Fixture.CreateMany<TestEntity>().ToList();

    mockedDbContext.Set<TestEntity>().AddFromSqlResult("sp_Specified", sqlParameters, expectedResult);

    var actualResult = mockedDbContext.Set<TestEntity>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

    Assert.Multiple(() =>
    {
        Assert.IsNotNull(actualResult);
        Assert.IsTrue(actualResult.Any());
        CollectionAssert.AreEquivalent(expectedResult, actualResult);
    });
}
```

### FromSqlRaw/FromSqlInterpolated (EntityFrameworkCore 3.0.0)
As above except the method names have *Raw and *Interpolated suffixes.

### Queries
Queries are initialized automatically but you'll want to seed them to do anything useful with them. Use the `AddToReadOnlySource` , `AddRangeToReadOnlySource` and `ClearReadOnlySource` extensions to seed/manipulate the query source.

```
[Test]
public void QueryAddRangeToReadOnlySource_Enumeration_AddsToQuerySource()
{
    var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

    var expectedResult = Fixture.CreateMany<TestQuery>().ToList();

    mockedDbContext.Query<TestQuery>().AddRangeToReadOnlySource(expectedResult);

    Assert.Multiple(() =>
    {
        CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.Query<TestQuery>().ToList());
        CollectionAssert.AreEquivalent(mockedDbContext.Query<TestQuery>().ToList(), mockedDbContext.TestView.ToList());
    });
}
```

Specifying a `FromSql` result for a query is exactly the same as a set.

### Keyless DbSets (EntityFrameworkCore 3.0.0)
As above. 

### ExecuteSqlCommand
Specifying an `ExecuteSqlCommand` result is similar to `FromSql` with the main difference being the return type. `ExecuteSqlCommand` always returns an `int`.

```
[Test]
public void ExecuteSqlCommand_SpecifiedStoredProcedure_ReturnsExpectedResult()
{
    var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

    var commandText = "sp_NoParams";
    var expectedResult = 1;

    mockedDbContext.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

    var result = mockedDbContext.Database.ExecuteSqlCommand("sp_NoParams");

    Assert.AreEqual(expectedResult, result);
}
```

And with parameters:

```
[Test]
public void ExecuteSqlCommand_SpecifiedStoredProcedureAndSqlParameters_ReturnsExpectedResult()
{
    var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

    var commandText = "sp_WithParams";
    var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
    var expectedResult = 1;

    mockedDbContext.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);

    var result = mockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter2", sqlParameters);

    Assert.AreEqual(expectedResult, result);
}
```

### ExecuteSqlCommand with a callback
All of the ExecuteSql* extensions accept an optional `Action<string, IEnumerable<object>>` parameter which allows you to do something post ExecuteSql* invocation e.g., you're invoking a stored procedure which deletes rows in a table, the result of which forms part of your assertion(s).

The following shows a basic example where invoking `ExecuteSqlCommand` deletes a specified number of rows from a set. You have access to the SQL and parameters that were provided to the ExecuteSql* invocation in the callback so you're covered for cases where you need to set the value of an output parameter.

```
[Test]
public void ExecuteSqlCommandWithCallback_InvokesCallback()
{
    //Arrange
    var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

    var itemsToCreate = 100;
    mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>(itemsToCreate).ToList());
    mockedDbContext.SaveChanges();

    var numberOfRowsToDelete = itemsToCreate / 2;
    var rowsToDelete = mockedDbContext.Set<TestEntity>().Take(numberOfRowsToDelete).ToList();
    var remainingRows = mockedDbContext.Set<TestEntity>().Skip(numberOfRowsToDelete).ToList();

    mockedDbContext.AddExecuteSqlCommandResult("usp_MyStoredProc", numberOfRowsToDelete, (providedSql, providedParameters) =>
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
}
```

### ExecuteSqlRaw/ExecuteSqlInterpolated (EntityFrameworkCore 3.0.0)
As above except the method names have *Raw and *Interpolated suffixes instead of *Command.

### Async operations
Whenever you add a FromSql* or ExecuteSql* result, the library sets up both the sync and async methods.

Additionally the library intercepts LINQ operations and returns an async emumerable/query provider to provide support for async LINQ operations that the in-memory provider doesn't provide.

### The rest
LINQ queryable operations such as `ElementAt`, indexed `Select`, `SkipWhile` etc just work as you would expect, there is nothing additional you need to do.

### Performing Verify operations
The `Create` factory always returns the mocked object, not the `Mock<TDbContext>` itself, so you'll need to get the mock using `Mock.Get<T>(T mocked)` if you want to perform specific operations on the mock itself.

Keep in mind that the `DbContext`, each `DbSet<TEntity>`, each `DbQuery<TQuery>`, and the query provider for each `DbSet<TEntity>` and `DbQuery<TQuery>` are all separate mocks; you need to invoke the `Verify` operation on the appropriate mock.

```
[Test]
public void AddRangeThenSaveChanges_CanAssertInvocationCount()
{
    var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

    mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>().ToList());
    mockedDbContext.SaveChanges();

    Assert.Multiple(() =>
    {
        var dbContextMock = Mock.Get(mockedDbContext);
        dbContextMock.Verify(m => m.SaveChanges(), Times.Once);

        //The db set is a mock, so we need to ensure we invoke the verify on the db set mock
        var byTypeDbSetMock = Mock.Get(mockedDbContext.Set<TestEntity>());
        byTypeDbSetMock.Verify(m => m.AddRange(It.IsAny<IEnumerable<TestEntity>>()), Times.Once);

        //This is the same mock instance as above, just accessed a different way
        var byPropertyDbSetMock = Mock.Get(mockedDbContext.TestEntities);

        Assert.That(byPropertyDbSetMock, Is.SameAs(byTypeDbSetMock));

        byPropertyDbSetMock.Verify(m => m.AddRange(It.IsAny<IEnumerable<TestEntity>>()), Times.Once);
    });
}
```

### Getting the Mock itself
The `Create` factory always returns the mocked object. This is deliberate as once created there should be no further set up required (other than what the EntityFrameworkCore.Testing interface provides of course). That being said the `Mock<T>` is always accessible using `Mock.Get<T>(T mocked)`. If you find yourself resorting to using this to support an unsupported operation :cry: get in touch so I can make it a supported operation :smile:.

# NSubstitute
As above! The only difference is if you want to mocks themselves. For Moq you need to invoke `Mock.Get<T>(T mocked)` to get it. For NSubstitute you don't need to do this.

### Performing Received operations
Received operations can be performed directly on the substitute as you would expect. Keep in mind that the `DbContext`, each `DbSet<TEntity>`, each `DbQuery<TQuery>`, and the query provider for each `DbSet<TEntity>` and `DbQuery<TQuery>` are all separate substitutes; you need to invoke the `Received` operation on the appropriate substitute.

```
[Test]
public void AddRangeThenSaveChanges_CanAssertInvocationCount()
{
    var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

    mockedDbContext.Set<TestEntity>().AddRange(Fixture.CreateMany<TestEntity>().ToList());
    mockedDbContext.SaveChanges();

    Assert.Multiple(() =>
    {
        mockedDbContext.Received(1).SaveChanges();

        //The db set is a mock, so we need to ensure we invoke the verify on the db set mock
        mockedDbContext.Set<TestEntity>().Received(1).AddRange(Arg.Any<IEnumerable<TestEntity>>());

        //This is the same mock instance as above, just accessed a different way
        mockedDbContext.TestEntities.Received(1).AddRange(Arg.Any<IEnumerable<TestEntity>>());

        Assert.That(mockedDbContext.TestEntities, Is.SameAs(mockedDbContext.Set<TestEntity>()));
    });
}
```