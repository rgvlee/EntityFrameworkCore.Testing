using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    public class TestManager<TDbContext, TRepository, TEntity> : IManager<TEntity> where TDbContext: DbContext
        where TRepository : IRepository<TEntity>
        where TEntity : class {

        private readonly TDbContext _context;
        private readonly TRepository _repository;

        public TestManager() {
            
        }

        public TestManager(TDbContext context, TRepository repository) {
            _context = context;
            _repository = repository;
        }

        public virtual TEntity GetById(Guid id) {
            return _repository.GetById(id);
        }

        public virtual void Add(TEntity entity) {
            _repository.Add(entity);
            _context.SaveChanges();
        }

        public virtual void Update(TEntity entity) {
            _repository.Update(entity);
            _context.SaveChanges();
        }

        public virtual void Remove(TEntity entity) {
            _repository.Update(entity);
            _context.SaveChanges();
        }
    }
}