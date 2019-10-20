
# EntityFrameworkCore.Testing
__*Moq mocking library for EntityFrameworkCore*__

EntityFrameworkCore.Testing.Moq is a EntityFrameworkCore mocking library for Moq. It's designed to be used in conjunction with the [Microsoft in-memory provider](https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory) to cover the EntityFrameworkCore functionality that it does not provide. It creates DbContext and DbSet proxy mocks and provides support for:
- FromSql
- ExecuteSqlCommand
- DbQuery\<TQuery\>
- ElementAt
- ElementAtOrDefault
- Indexed Select (Queryable.Select(Func\<T, int, TResult\>))
- SkipWhile
- TakeWhile
- Indexed TakeWhile (Queryable.Select(Func\<T, int, bool\>))
- _and probably more..._

In addition to the above you also get all of the benefits of using a mocking framework (e.g., the ability to verify method invocation).

## Resources
- [Source repository](https://github.com/rgvlee/EntityFrameworkCore.Testing)
- [EntityFrameworkCore.Testing.Moq - NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Testing.Moq/)

# Moq
## Example Usage
- Create db context mock
- Consume

```
[Test]
public void SetAddAndPersist_Item_Persists()
{
	var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
	var mockedContext = dbContextToMock.CreateMock();

	var testEntity1 = Fixture.Create<TestEntity1>();

	mockedContext.Set<TestEntity1>().Add(testEntity1);
	mockedContext.SaveChanges();

	Assert.Multiple(() =>
	{
		Assert.AreNotEqual(default(Guid), testEntity1.Id);
		Assert.DoesNotThrow(() => mockedContext.Set<TestEntity1>().Single());
		Assert.AreEqual(testEntity1, mockedContext.Find<TestEntity1>(testEntity1.Id));
		Mock.Get(mockedContext).Verify(m => m.SaveChanges(), Times.Once);
	});
}
```

### FromSql
- Specify a FromSql result
- Consume

```
[Test]
public void SetUpFromSqlResult_AnyStoredProcedureWithNoParameters_ReturnsExpectedResult()
{
	var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
	var mockedContext = dbContextToMock.CreateMock();

	var expectedResult = Fixture.CreateMany<TestEntity1>().ToList();

	mockedContext.Set<TestEntity1>().AddFromSqlResult(expectedResult);

	var actualResult = mockedContext.Set<TestEntity1>().FromSql("sp_NoParams").ToList();

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
public void SetUpFromSql_SpecifiedStoredProcedureAndParameters_ReturnsExpectedResult()
{
	var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
	var mockedContext = dbContextToMock.CreateMock();

	var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
	var expectedResult = Fixture.CreateMany<TestEntity1>().ToList();

	mockedContext.Set<TestEntity1>().AddFromSqlResult("sp_Specified", sqlParameters, expectedResult);

	var actualResult = mockedContext.Set<TestEntity1>().FromSql("[dbo].[sp_Specified] @SomeParameter1 @SomeParameter2", new SqlParameter("@someparameter2", "Value2")).ToList();

	Assert.Multiple(() =>
	{
		Assert.IsNotNull(actualResult);
		Assert.IsTrue(actualResult.Any());
		CollectionAssert.AreEquivalent(expectedResult, actualResult);
	});
}
```

### Queries
DbQueries are initialized as part of the DbContext mock initialization but you'll want to seed them to do anything useful with them. Use the `Add` , `AddRange` and `Clear` DbQuery extensions to set up the DbQuery source.

```
[Test]
public void QueryAddRange_Enumeration_AddsToQuerySource()
{
	var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
	var mockedContext = dbContextToMock.CreateMock();

	var expectedResult = Fixture.CreateMany<TestQuery1>().ToList();

	mockedContext.Query<TestQuery1>().AddRange(expectedResult);

	Assert.Multiple(() =>
	{
		CollectionAssert.AreEquivalent(expectedResult, mockedContext.Query<TestQuery1>().ToList());
		CollectionAssert.AreEquivalent(mockedContext.Query<TestQuery1>().ToList(), mockedContext.TestView.ToList());
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
	var mockedContext = dbContextToMock.CreateMock();

	var commandText = "sp_NoParams";
	var expectedResult = 1;

	mockedContext.AddExecuteSqlCommandResult(commandText, new List<SqlParameter>(), expectedResult);

	var result = mockedContext.Database.ExecuteSqlCommand("sp_NoParams");

	Assert.AreEqual(expectedResult, result);
}
```

And with parameters:

```
[Test]
public void ExecuteSqlCommand_SpecifiedStoredProcedureAndSqlParameters_ReturnsExpectedResult()
{
	var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
	var mockedContext = dbContextToMock.CreateMock();

	var commandText = "sp_WithParams";
	var sqlParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
	var expectedResult = 1;

	mockedContext.AddExecuteSqlCommandResult(commandText, sqlParameters, expectedResult);

	var result = mockedContext.Database.ExecuteSqlCommand("[dbo.[sp_WithParams] @SomeParameter2", sqlParameters);

	Assert.AreEqual(expectedResult, result);
}
```

### The rest
LINQ Queryable operations such as ElementAt, indexed Select, SkipWhile etc just work as you would expect so there is no need to provide an example.

# NSubstitute
Coming soon!