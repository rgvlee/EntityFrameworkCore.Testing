using System;
using System.Collections.Generic;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    public interface IRepository<TEntity> where TEntity : class {
        IEnumerable<TEntity> GetAll();
        TEntity GetById(Guid id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}