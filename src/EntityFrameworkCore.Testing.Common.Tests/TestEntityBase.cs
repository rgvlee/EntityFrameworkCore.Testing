using System;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class TestEntityBase
    {
        public virtual Guid Guid { get; set; }

        public string String { get; set; }

        public int Int { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime FixedDateTime => DateTime.Parse("2019-01-01");

        public override string ToString()
        {
            return $"Guid: {Guid}; String: {String}; Int: {Int}; DateTime: {DateTime:F}; FixedDateTime: {FixedDateTime:F}";
        }
    }
}