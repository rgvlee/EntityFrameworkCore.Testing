using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [DebuggerDisplay("{nameof(Id)}: {Id}")]
    public abstract class TestEntityBase
    {
        [Key] public Guid Guid { get; set; }

        public string String { get; set; }

        public int Int { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime FixedDateTime => DateTime.Parse("2019-01-01");
    }
}