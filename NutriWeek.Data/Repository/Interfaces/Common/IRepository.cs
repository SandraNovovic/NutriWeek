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



        void GetById(Guid id);


        void Add(TEntity entity);
        void AddAsync(TEntity entity);


        void Update(TEntity entity);

        void Remove(TEntity entity);


        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
