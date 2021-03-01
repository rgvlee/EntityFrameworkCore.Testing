using System;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class BaseTestEntity
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public decimal Weight { get; set; }

        public decimal Height { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModifiedAt { get; set; }
    }
}