using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Moq.Protected;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public static class TestExtensions
    {
        public static void AddSqlQueryAsyncResult<T>(this DbContext mockedDbContext, string sql, IEnumerable<object> parameters, IEnumerable<T> expectedResult = null)
        {
            if (!typeof(T).GetProperties().Any())
            {
                mockedDbContext.AddExecuteSqlRawResult(sql, parameters, 0);
                return;
            }

            var dependencies = (IRelationalDatabaseFacadeDependencies) ((IDatabaseFacadeDependenciesAccessor) mockedDbContext.Database).Dependencies;
            var dependenciesMock = Mock.Get(dependencies);

            var relationalConnectionMock = new Mock<IRelationalConnection>();

            relationalConnectionMock.Setup(m => m.CommandTimeout).Returns(() => 0);

            var dbCommandMock = new Mock<DbCommand>();
            dbCommandMock.Protected().Setup<DbParameter>("CreateDbParameter").Returns(() => Mock.Of<DbParameter>());
            dbCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(() => Mock.Of<DbParameterCollection>());
            dbCommandMock.Protected()
                .Setup<Task<DbDataReader>>("ExecuteDbDataReaderAsync", ItExpr.IsAny<CommandBehavior>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => (DbDataReader) new DbDataReader<T>(expectedResult ?? new List<T>()));
            var dbCommand = dbCommandMock.Object;

            var dbConnectionMock = new Mock<DbConnection>();
            dbConnectionMock.Protected().Setup<DbCommand>("CreateDbCommand").Returns(() => dbCommand);
            var dbConnection = dbConnectionMock.Object;

            relationalConnectionMock.Setup(m => m.DbConnection).Returns(() => dbConnection);
            var relationalConnection = relationalConnectionMock.Object;

            dependenciesMock.Setup(m => m.RelationalConnection).Returns(() => relationalConnection);
        }

        private class DbDataReader<T> : DbDataReader
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly IEnumerable<T> _source;
            private readonly List<PropertyInfo> _typeProperties;

            public DbDataReader(IEnumerable<T> source)
            {
                _source = source;
                _enumerator = _source.GetEnumerator();
                _typeProperties = typeof(T).GetProperties().ToList();
            }

            public override int FieldCount => _typeProperties.Count;

            public override object this[int ordinal] => GetValue(ordinal);

            public override object this[string name] => GetValue(GetOrdinal(name));

            public override int RecordsAffected { get; }

            public override bool HasRows => _source.Any();

            public override bool IsClosed { get; }

            public override int Depth => throw new NotImplementedException();

            public override bool GetBoolean(int ordinal)
            {
                return (bool) GetValue(ordinal);
            }

            public override byte GetByte(int ordinal)
            {
                return (byte) GetValue(ordinal);
            }

            public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
            {
                throw new NotImplementedException();
            }

            public override char GetChar(int ordinal)
            {
                return (char) GetValue(ordinal);
            }

            public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
            {
                throw new NotImplementedException();
            }

            public override string GetDataTypeName(int ordinal)
            {
                return _typeProperties[ordinal].PropertyType.Name;
            }

            public override DateTime GetDateTime(int ordinal)
            {
                return (DateTime) GetValue(ordinal);
            }

            public override decimal GetDecimal(int ordinal)
            {
                return (decimal) GetValue(ordinal);
            }

            public override double GetDouble(int ordinal)
            {
                return (double) GetValue(ordinal);
            }

            public override Type GetFieldType(int ordinal)
            {
                return _typeProperties[ordinal].PropertyType;
            }

            public override float GetFloat(int ordinal)
            {
                return (float) GetValue(ordinal);
            }

            public override Guid GetGuid(int ordinal)
            {
                return (Guid) GetValue(ordinal);
            }

            public override short GetInt16(int ordinal)
            {
                return (short) GetValue(ordinal);
            }

            public override int GetInt32(int ordinal)
            {
                return (int) GetValue(ordinal);
            }

            public override long GetInt64(int ordinal)
            {
                return (long) GetValue(ordinal);
            }

            public override string GetName(int ordinal)
            {
                return _typeProperties[ordinal].Name;
            }

            public override int GetOrdinal(string name)
            {
                return _typeProperties.FindIndex(x => x.Name.Equals(name));
            }

            public override string GetString(int ordinal)
            {
                return (string) GetValue(ordinal);
            }

            public override object GetValue(int ordinal)
            {
                return _typeProperties[ordinal].GetValue(_enumerator.Current);
            }

            public override int GetValues(object[] values)
            {
                throw new NotImplementedException();
            }

            public override bool IsDBNull(int ordinal)
            {
                return GetValue(ordinal) == null;
            }

            public override bool NextResult()
            {
                return _enumerator.MoveNext();
            }

            public override bool Read()
            {
                return NextResult();
            }

            public override IEnumerator GetEnumerator()
            {
                return _source.GetEnumerator();
            }
        }
    }
}