using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public class TestDbContext : DbContext
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(TestDbContext));

        //public TestDbContext() { }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public virtual DbSet<TestEntity> TestEntities { get; set; }
        public virtual DbSet<TestReadOnlyEntity> TestReadOnlyEntities { get; set; }
        public virtual DbQuery<TestQuery> TestView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(c => c.Guid);
            modelBuilder.Entity<TestReadOnlyEntity>().HasNoKey().ToView("TestReadOnlyEntity");
            modelBuilder.Entity<TestQuery>().HasNoKey().ToView("TestQuery");
        }

        public override int SaveChanges()
        {
            Logger.LogDebug("SaveChanges invoked");
            return base.SaveChanges();
        }
    }
}