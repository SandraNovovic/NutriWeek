using Microsoft.EntityFrameworkCore;

using NutriWeek.Data.Repository.Interfaces.Common;

namespace NutriWeek.Data.Repository.Common
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected BaseRepository( NutriWeekDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        protected NutriWeekDbContext DbContext { get; }
        protected DbSet<TEntity> DbSet { get; }

        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }


        public async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public IQueryable<TEntity> GetAllAsNoTracking()
        {
            return DbSet
                .AsNoTracking()
                .AsQueryable();
        }

        public IQueryable<TEntity> GetAllAsNoTrackingWithDeleted()
        {
            return DbSet
                .AsNoTracking()
                .IgnoreQueryFilters()
                .AsQueryable();
        }

        public IQueryable<TEntity> GetAllWithDeleted()
        {
            return DbSet
                .IgnoreQueryFilters()
                .AsQueryable();
        }


        public async Task<TEntity?> GetByIdAsync(
            int id)
        {
            return await DbSet.FindAsync(id);
        }
        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }
        public void Update(TEntity entity)
        {

            DbSet.Update(entity);

        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return DbContext.SaveChangesAsync();
        }
    }
}