using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     The mocked db context factory options.
    /// </summary>
    /// <typeparam name="TDbContext">The db context type.</typeparam>
    public class MockedDbContextFactoryOptions<TDbContext> where TDbContext : DbContext
    {
        /// <summary>
        ///     The db context instance that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        public TDbContext DbContext { get; set; }

        /// <summary>
        ///     The parameters that will be used to create the mocked db context and, if one is not provided,
        ///     the in-memory context that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        public IEnumerable<object> ConstructorParameters { get; set; }
    }
}