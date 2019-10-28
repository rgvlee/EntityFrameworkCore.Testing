using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public class TestDbContext : DbContext
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(TestDbContext));

        public TestDbContext() { }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public virtual DbSet<TestEntity1> TestEntities { get; set; }
        public virtual DbQuery<TestQuery1> TestView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<TestQuery1>().ToView("SomeView");
        }

        public override int SaveChanges()
        {
            Logger.LogDebug("SaveChanges invoked");
            return base.SaveChanges();
        }
    }
}