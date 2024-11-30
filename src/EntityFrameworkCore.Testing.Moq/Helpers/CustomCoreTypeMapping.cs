using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.Testing.Moq.Helpers;

public class CustomCoreTypeMapping : CoreTypeMapping
{
    public CustomCoreTypeMapping() : base(new CoreTypeMappingParameters())
    {
    }

    protected override CoreTypeMapping Clone(CoreTypeMappingParameters parameters)
    {
        throw new NotImplementedException();
    }

    public override CoreTypeMapping WithComposedConverter(ValueConverter converter, ValueComparer comparer = null, ValueComparer keyComparer = null,
        CoreTypeMapping elementMapping = null,
        JsonValueReaderWriter jsonValueReaderWriter = null)
    {
        throw new NotImplementedException();
    }
}