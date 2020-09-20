using System;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using NSubstitute.Core;
using rgvlee.Core.Common.Extensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    internal class NoSetUpHandler : ICallHandler
    {
        public RouteAction Handle(ICall call)
        {
            var methodInfo = call.GetMethodInfo();

            if (methodInfo.Name.Equals("Query") || methodInfo.Name.Equals("Set"))
            {
                throw new InvalidOperationException(string.Format(ExceptionMessages.CannotCreateDbSetTypeNotIncludedInModel, methodInfo.GetGenericArguments().Single().Name));
            }

            if (methodInfo.ReturnType == typeof(void))
            {
                return RouteAction.Return(null);
            }

            return RouteAction.Return(methodInfo.ReturnType.GetDefaultValue());
        }
    }
}