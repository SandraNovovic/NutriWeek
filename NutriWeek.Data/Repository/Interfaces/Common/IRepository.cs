using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.Data.Repository.Interfaces.Common
{
    public interface IRepository<TEntity>
        where TEntity : class
    {

        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllAsNoTracking();
        IQueryable<TEntity> GetAllAsNoTrackingWithDeleted();
        IQueryable<TEntity> GetAllWithDeleted();



        Task<TEntity> GetById(Guid id);
        Task<TEntity> GetByIdAsNoTracking(Guid id);
        Task<TEntity> GetByIdWithDeleted(Guid id);
        Task<TEntity> GetByIdAsNoTrackingWithDeleted(Guid id);


        Task<TEntity> Add(TEntity entity);
        Task AddAsync(TEntity entity);

        void UpdateAsync(TEntity entity);
        void Update(TEntity entity);

        void Remove(TEntity entity);
        void RemoveAsync(TEntity entity);

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
