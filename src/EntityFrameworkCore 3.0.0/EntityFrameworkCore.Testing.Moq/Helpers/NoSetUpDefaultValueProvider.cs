using System;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using Moq;
using rgvlee.Core.Common.Extensions;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    internal class NoSetUpDefaultValueProvider : DefaultValueProvider
    {
        protected override object GetDefaultValue(Type type, Mock mock)
        {
            var lastInvocation = mock.Invocations.Last();
            if (lastInvocation.Method.Name.Equals("Query") || lastInvocation.Method.Name.Equals("Set"))
            {
                throw new InvalidOperationException(string.Format(ExceptionMessages.CannotCreateDbSetTypeNotIncludedInModel,
                    lastInvocation.Method.GetGenericArguments().Single().Name));
            }

            return type.GetDefaultValue();
        }
    }
}