using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    [DebuggerDisplay("{nameof(Id)}: {Id}")]
    public class TestEntity3 {
        [Key]
        public Guid Id { get; set; }

        public TestEntity3() {

        }

        public TestEntity3(Guid id) {
            Id = id;
        }
    }
}