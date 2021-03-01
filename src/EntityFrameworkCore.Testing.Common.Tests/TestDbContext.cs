using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public class TestDbContext : DbContext
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger<TestDbContext>();

        public TestDbContext() { }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public TestDbContext(ILogger<TestDbContext> logger, DbContextOptions<TestDbContext> options) : base(options) { }

        public virtual DbSet<TestEntity> TestEntities { get; set; }
        public virtual DbSet<TestReadOnlyEntity> TestReadOnlyEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(c => c.Id);

            modelBuilder.Entity<TestReadOnlyEntity>().HasNoKey().ToView("TestReadOnlyEntities");
        }

        public override int SaveChanges()
        {
            Logger.LogDebug("SaveChanges invoked");
            return base.SaveChanges();
        }
    }
}