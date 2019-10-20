using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() { }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public virtual DbSet<TestEntity1> TestEntities { get; set; }
        public virtual DbQuery<TestQuery1> TestView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<TestQuery1>().ToView("SomeView");
        }
    }
}