# EntityFrameworkCore.Testing
__*Moq and NSubstitute mocking libraries for EntityFrameworkCore*__

EntityFrameworkCore.Testing allows you to create a mock DbContext and have it backed by an actual DbContext.

But why? There's a couple of reasons and it's from my own experience using the [Microsoft in-memory provider](https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory) in unit tests.

The in-memory provider is __great__ for most cases however it doesn't do everything. If you're invoking FromSql, ExecuteSqlCommand or using views you're out of luck. It just doesn't work and nor could it, while EntityFrameworkCore can work with them they are not an EntityFrameworkCore concern. They are a data source concern.

So the issue is simple. I want to use the in-memory provider for most things and then mock the couple of bits it can't do. Unfortunately the only way to do this (without modifying the test subject/s) is to mock the DbContext.

And that's just what this library does. The mocks will funnel the majority of the operations to the actual DbContext. For everything else, use a mock. Mocking views and the FromSql and ExecuteSqlCommand methods is easy using the provided builder. As a bonus you get all the benefits of using a mocking framework (e.g., the ability to verify method invocation). __You can have your cake and eat it too!__

## Resources
- [Source repository](https://github.com/rgvlee/EntityFrameworkCore.Testing)
- [EntityFrameworkCore.Testing.Moq - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/)
- [EntityFrameworkCore.Testing.NSubstitute - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.NSubstitute/)

## The disclaimer
The library sets up a lot of the DbContext functionality but I am not going to claim it does everything. I have built this based on my current needs. If you find this library useful and something is missing, not working as you'd expect or you need additional behaviour mocked flick me a message and I'll see what I can do.

## Fluent interface
The builder provides a fluent interface for building the mocks so it should be intuitive and discoverable. The examples below touch on a bit of the available functionality.

#Moq
## Example Usage
- Create the builder
- Get the db context mock
- Consume

In this example the builder automatically creates an in-memory context and sets up the mock set ups for all of the DbContext DbSets. Operations on the mock DbContext are funneled through to the in memory DbContext. You can add/update/remove on either and both will yield the same result.

__Note: automatic DbContext creation requires a DbContext constructor with a single DbContextOptions parameter__.

```
[Test]
public void Add_NewEntity_Persists() {
    var builder = new DbContextMockBuilder<TestContext>();
    var mockContext = builder.GetDbContextMock();
    var mockedContext = builder.GetMockedDbContext();
    var testEntity1 = new TestEntity1();

    mockedContext.Set<TestEntity1>().Add(testEntity1);
    mockedContext.SaveChanges();

    Assert.Multiple(() => {
        Assert.AreNotEqual(default(Guid), testEntity1.Id);
        Assert.DoesNotThrow(() => mockedContext.Set<TestEntity1>().Single());
        Assert.AreEqual(testEntity1, mockedContext.Find<TestEntity1>(testEntity1.Id));
        mockContext.Verify(m => m.SaveChanges(), Times.Once);
    });
}
```
Or if you want to provide your own DbContext and only set up a specified DbSet:
- Create the context to mock
- Create the builder providing the constructor parameters:
	- The context to mock you've just created
	- addSetUpForAllDbSets = false
- Set up the DbSet you want to mock
- Consume

The mock set up covers both the Set<TEntity> and DbContext DbSet<TEntity> property:

```
[Test]
public void AddWithSpecifiedDbContextAndDbSetSetUp_NewEntity_PersistsToBothDbSetAndDbContextDbSetProperty() {
    var contextToMock = new TestContext(new DbContextOptionsBuilder<TestContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    var builder = new DbContextMockBuilder<TestContext>(contextToMock, false);
    builder.AddSetUpFor(x => x.TestEntities);
    var mockedContext = builder.GetMockedDbContext();
            
    mockedContext.Set<TestEntity1>().Add(new TestEntity1());
    mockedContext.SaveChanges();

    Assert.Multiple(() => {
        Assert.DoesNotThrow(() => mockedContext.Set<TestEntity1>().Single());
        CollectionAssert.AreEquivalent(mockedContext.Set<TestEntity1>().ToList(), mockedContext.TestEntities.ToList());
    });
}
```

### Testing FromSql
The main difference here is that we need the seed data to set up FromSql to return the expected result.
- Generate seed data
- Create the builder
- Set up the FromSql result
- Get the db context mock
- Consume

In this case we didn't need to persist the seed data, whether you do or not will depend what you're doing with the FromSql result.
```
[Test]
public void SetUpFromSql_AnyStoredProcedureWithNoParameters_ReturnsExpectedResult() {
    var expectedResult = new List<TestEntity1> { new TestEntity1() };

    var builder = new DbContextMockBuilder<TestContext>();
    builder.AddFromSqlResultFor(x => x.TestEntities, expectedResult);
    var mockedContext = builder.GetMockedDbContext();

    var actualResult = mockedContext.Set<TestEntity1>().FromSql("sp_NoParams").ToList();

    Assert.Multiple(() => {
        Assert.IsNotNull(actualResult);
        Assert.IsTrue(actualResult.Any());
        CollectionAssert.AreEquivalent(expectedResult, actualResult);
    });
}
```

### Testing FromSql with SqlParameters
Expanding on the previous example, for this test we specify:
- The FromSql sql that we want to match;
- A sequence of FromSql SqlParameters

The mock FromSql __sql__ set up matching is case insensitive and supports partial matches; in the example we're able to match on just the stored procedure name. The mock FromSql __SqlParameters__ matching is case insensitive and supports partial sequence matching however it does not support partial matches on the parameter name/value. You only need to specify the bare minimum for a mock set up match.
```
[Test]
public void SetUpFromSql_SpecifiedStoredProcedureAndParameters_ReturnsExpectedResult() {
    var sqlParameters = new List<SqlParameter>() {new SqlParameter("@SomeParameter2", "Value2")};
    var expectedResult = new List<TestEntity1> {new TestEntity1()};
        
    var builder = new DbContextMockBuilder<TestContext>();
    builder.AddFromSqlResultFor(x => x.TestEntities, "sp_Specified", sqlParameters, expectedResult);
    var mockedContext = builder.GetMockedDbContext();

    var actualResult = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

    Assert.Multiple(() => {
        Assert.IsNotNull(actualResult);
        Assert.IsTrue(actualResult.Any());
        CollectionAssert.AreEquivalent(expectedResult, actualResult);
    });
}
```

### Roll your own query provider mock
You can always create your own mock query provider; below is functionally the same as the test from the previous section. 
```
[Test]
public void SetUpFromSql_MockQueryProviderWithSpecifiedStoredProcedureAndParameters_ReturnsExpectedResult() {
    var expectedResult = new List<TestEntity1> { new TestEntity1() };
            
    var mockQueryProvider = new Mock<IQueryProvider>();
    var sqlParameter = new SqlParameter("@SomeParameter2", "Value2");
    mockQueryProvider.SetUpFromSql("sp_Specified", new List<SqlParameter> { sqlParameter }, expectedResult);

    var builder = new DbContextMockBuilder<TestContext>();
    builder.AddQueryProviderMockFor(x => x.TestEntities, mockQueryProvider);
    var mockedContext = builder.GetMockedDbContext();
            
    var actualResult = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

    Assert.Multiple(() => {
        Assert.IsNotNull(actualResult);
        Assert.IsTrue(actualResult.Any());
        CollectionAssert.AreEquivalent(expectedResult, actualResult);
    });
}
```

### What about Queries?
Haven't forgotten about them. Queries can't really be set up automatically as to be useful you need to seed them. Not a big hassle though, set up is easy.
```
[Test]
public void SetUpQuery_ReturnsEnumeration() {
    var list1 = new List<TestEntity2>() { new TestEntity2(), new TestEntity2() };

    var builder = new DbContextMockBuilder<TestContext>();
    builder.AddSetUpFor(x => x.TestView, list1);
    var mockedContext = builder.GetMockedDbContext();
    
    Assert.Multiple(() => {
        CollectionAssert.AreEquivalent(list1, mockedContext.Query<TestEntity2>().ToList());
        CollectionAssert.AreEquivalent(mockedContext.Query<TestEntity2>().ToList(), mockedContext.TestView.ToList());
    });
}
```
The FromSql mocking works the same as with the sets.
```
[Test]
public void FromSql_SpecifiedStoredProcedureWithParameters_ReturnsExpectedResult() {
    var list1 = new List<TestEntity2> { new TestEntity2() };

    var builder = new DbContextMockBuilder<TestContext>();

    var mockQueryProvider = new Mock<IQueryProvider>();
    var sqlParameter = new SqlParameter("@SomeParameter2", "Value2");
    mockQueryProvider.SetUpFromSql("sp_Specified", new List<SqlParameter> { sqlParameter }, list1);
    builder.AddSetUpFor(x => x.TestView, list1).AddQueryProviderMockFor(x => x.TestView, mockQueryProvider);
    
    var context = builder.GetMockedDbContext();

    var result = context.Query<TestEntity2>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

    Assert.Multiple(() => {
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        CollectionAssert.AreEquivalent(list1, result);
    });
}
```

### Let's not forget ExecuteSqlCommand
Very similar to FromSql with the main difference being the return type.

Specified command text returning an expected result:
```
[Test]
public void Execute_SetUpSpecifiedQuery_ReturnsExpectedResult() {
    var builder = new DbContextMockBuilder<TestContext>();

    var commandText = "sp_NoParams";
    var expectedResult = 1;

    builder.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

    var mockedContext = builder.GetMockedDbContext();

    var result = mockedContext.Database.ExecuteSqlCommand("sp_NoParams");

    Assert.AreEqual(expectedResult, result);
}
```
Specified command text and sql parameters returning an expected result:
```
[Test]
public void Execute_SetUpSpecifiedQueryWithSqlParameters_ReturnsExpectedResult() {
    var builder = new DbContextMockBuilder<TestContext>();
    
    var commandText = "sp_WithParams";
    var sqlParameters = new List<SqlParameter>() {new SqlParameter("@SomeParameter2", "Value2")};
    var expectedResult = 1;

    builder.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);

    var mockedContext = builder.GetMockedDbContext();

    var result = mockedContext.Database.ExecuteSqlCommand("[dbo.[sp_WithParams] @SomeParameter2", sqlParameters);

    Assert.AreEqual(expectedResult, result);
}
```
Both ExecuteSqlCommand and ExecuteSqlCommandAsync are set up when AddExecuteSqlCommandResult is invoked. You're welcome!

## Advanced usage
If you want to get your hands dirty I've provided a few methods on the builder for you to use your own DbSet<>, DbQuery and IQueryProvider mocks. The extensions that I use to create these mocks are also available to get you going.