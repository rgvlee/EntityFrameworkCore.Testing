using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    public class TestDbContext : DbContext {

        public virtual DbSet<TestEntity1> TestEntities { get; set; }
        public virtual DbQuery<TestEntity2> TestView { get; set; }

        public TestDbContext() {

        }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Query<TestEntity2>().ToView("SomeView");
        }
    }
}