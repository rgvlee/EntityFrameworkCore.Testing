using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    public class TestContext : DbContext {

        public virtual DbSet<TestEntity1> TestEntities { get; set; }
        public virtual DbQuery<TestEntity2> TestView { get; set; }

        public TestContext() {

        }

        public TestContext(DbContextOptions<TestContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Query<TestEntity2>().ToView("SomeView");
        }
    }
}