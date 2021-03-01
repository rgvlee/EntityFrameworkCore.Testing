using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EntityFrameworkCore.Testing.Common
{
    internal class FakeEntityType : EntityType
    {
        public FakeEntityType(Type type) : base(type, new Model(), default) { }
    }
}