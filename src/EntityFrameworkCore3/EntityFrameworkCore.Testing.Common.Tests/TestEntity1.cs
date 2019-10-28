using System;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public class TestEntity1 : TestEntityBase
    {
        [Key] public override Guid Guid { get; set; }
    }
}