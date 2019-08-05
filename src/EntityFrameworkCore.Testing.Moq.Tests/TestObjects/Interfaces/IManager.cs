using System;

namespace EntityFrameworkCore.Testing.Moq.Tests {
    public interface IManager<TEntity> where TEntity : class {
        TEntity GetById(Guid id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}