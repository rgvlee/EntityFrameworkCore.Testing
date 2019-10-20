using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [DebuggerDisplay("{nameof(Id)}: {Id}")]
    public abstract class TestEntityBase
    {
        [Key] public Guid Id { get; set; }

        public string Name { get; set; }
    }
}