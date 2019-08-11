using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace EntityFrameworkCore.Testing.Moq.PackageVerification.Tests {
    [DebuggerDisplay("{nameof(Id)}: {Id}")]
    public class TestEntity1 {
        [Key]
        public Guid Id { get; set; }

        public TestEntity1() {

        }

        public TestEntity1(Guid id) {
            Id = id;
        }
    }
}