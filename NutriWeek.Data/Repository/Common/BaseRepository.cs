using Microsoft.EntityFrameworkCore;
using NutriWeek.Data.Repository.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.Data.Repository.Common
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {

        protected BaseRepository(NutriWeekDbContext dbContext)
        {

           this.DbContext = dbContext;
           this.DbSet = dbContext.Set<TEntity>();
        }

        protected NutriWeekDbContext DbContext { get; }
        protected DbSet<TEntity> DbSet { get; }

        public void Add(TEntity entity)
        {
             DbSet.Add(entity);
        }

        public void AddAsync(TEntity entity)
        {
                DbSet.AddAsync(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public IQueryable<TEntity> GetAllAsNoTracking()
        {
            return DbSet.AsNoTracking().AsQueryable();
        }

        public IQueryable<TEntity> GetAllAsNoTrackingWithDeleted()
        {
            return DbSet.AsNoTracking().IgnoreQueryFilters().AsQueryable();
        }

        public IQueryable<TEntity> GetAllWithDeleted()
        {
            return DbSet.IgnoreQueryFilters().AsQueryable();
        }

        public void GetById(Guid id)
        {
             DbSet.FindAsync(id);
        }


        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }



        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return DbContext.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }


    }
}
