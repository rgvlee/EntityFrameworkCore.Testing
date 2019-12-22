# EntityFrameworkCore.Testing
__*Moq and NSubstitute mocking libraries for EntityFrameworkCore*__

EntityFrameworkCore.Testing is an EntityFrameworkCore mocking library for Moq and NSubstitute. It's designed to be used in conjunction with the [Microsoft in-memory provider](https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory) to cover the EntityFrameworkCore functionality that it does not provide. It creates DbContext and DbSet proxy mocks and provides support for:
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

There may be more EntityFrameworkCore functionality that isn't supported by the Microsoft in-memory provider; this is what I've discovered/added support for so far. If you come across unsupported functionality let me know so I can add support for it.

In addition to the above you also get all of the benefits of using a mocking framework (e.g., the ability to verify method invocation).

## NuGet Packages
#### EntityFrameworkCore 3.0.0+
- [EntityFrameworkCore.Testing.Moq](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/2.0.2)
- [EntityFrameworkCore.Testing.NSubstitute](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/2.0.3-RC01)
#### EntityFrameworkCore 2.1.0-2.2.6
- [EntityFrameworkCore.Testing.Moq](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/1.0.5)
- [EntityFrameworkCore.Testing.NSubstitute](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/1.0.5)

# Moq
## Example Usage
- Create mocked db context
- Consume

```
[Test]
public void SetAddAndPersist_Item_AddsAndPersistsItem()
{
    var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

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
- Specify a FromSql result
- Consume

```
[Test]
public void FromSql_AnyStoredProcedureWithNoParameters_ReturnsExpectedResult()
{
    var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

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
Expanding on the previous example, for this test I am going to specify the FromSql SQL and parameters.

The mock FromSql __sql__ matching is case insensitive and supports partial matches; in the example I've left out the schema name and it'll match on just the stored procedure name. The mock FromSql __parameters__ matching is case insensitive and supports partial sequence matching however it does not support partial matches on the parameter name/value; in this example we're only specifying one of the parameters. You only need to specify the bare distinct minimum.

```
[Test]
public void FromSql_SpecifiedStoredProcedureAndParameters_ReturnsExpectedResult()
{
    var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

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

### Queries
Queries are initialized as part of the DbContext mock initialization but you'll want to seed them to do anything useful with them. Use the `AddToReadOnlySource` , `AddRangeToReadOnlySource` and `ClearReadOnlySource` extensions to seed/manipulate the query source.

```
[Test]
public void QueryAddRangeToReadOnlySource_Enumeration_AddsToQuerySource()
{
    var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

    var expectedResult = Fixture.CreateMany<TestQuery>().ToList();

    mockedDbContext.Query<TestQuery>().AddRangeToReadOnlySource(expectedResult);

    Assert.Multiple(() =>
    {
        CollectionAssert.AreEquivalent(expectedResult, mockedDbContext.Query<TestQuery>().ToList());
        CollectionAssert.AreEquivalent(mockedDbContext.Query<TestQuery>().ToList(), mockedDbContext.TestView.ToList());
    });
}
```

Specifying a FromSql result for a DbQuery is exactly the same as a DbSet.

### ExecuteSqlCommand/ExecuteSqlCommandAsync
Specifying an ExecuteSqlCommand result is similar to FromSql with the main difference being the return type. ExecuteSqlCommand always returns an `int`.

```
[Test]
public void ExecuteSqlCommand_SpecifiedStoredProcedure_ReturnsExpectedResult()
{
    var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

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
    var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    var mockedDbContext = Create.MockedDbContextFor(dbContextToMock);

    var commandText = "sp_WithParams";
    var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
    var expectedResult = 1;

    mockedDbContext.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);

    var result = mockedDbContext.Database.ExecuteSqlCommand("[dbo].[sp_WithParams] @SomeParameter2", sqlParameters);

    Assert.AreEqual(expectedResult, result);
}
```

### The rest
LINQ queryable operations such as ElementAt, indexed Select, SkipWhile etc just work as you would expect so there is no need to provide an example.

### Performing Verify operations
The Create factory always returns the mocked object, not the Mock\<TDbContext\> itself, so you'll need to use `Mock.Get<T>(T mocked)` to perform Moq Mock\<T\> specific operations.

Keep in mind that the DbContext, each DbSet\<TEntity\>, each DbQuery\<TQuery\>, and the query provider for each DbSet\<TEntity\> and DbQuery\<TQuery\> are separate mocks so you will need to invoke the verify operation on the appropriate mock.

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
The Create factory always returns the mocked object. This is deliberate as once created there should be no further set up required (other than what the EntityFrameworkCore.Testing interface provides of course). That being said the Mock\<T\> is always accessible using `Mock.Get<T>(T mocked)`. If you find yourself resorting to using this to support an unsupported operation :cry: get in touch so I can make it a supported operation :smile:.

# NSubstitute
It works the same/has the same interface. The only difference is if you want to mocks themselves. For Moq you need to invoke Mock.Get(mocked) to get it. For NSubstitute you don't need to do this.

### Performing Received operations
Received operations can be performed directly on the substitute as you would expect. Keep in mind that the DbContext, each DbSet\<TEntity\>, each DbQuery\<TQuery\>, and the query provider for each DbSet\<TEntity\> and DbQuery\<TQuery\> are separate substitutes so you will need to invoke the Received operation on the appropriate substitute.

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