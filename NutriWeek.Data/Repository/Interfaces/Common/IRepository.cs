namespace NutriWeek.Data.Repository.Interfaces.Common
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllAsNoTracking();
        IQueryable<TEntity> GetAllAsNoTrackingWithDeleted();
        IQueryable<TEntity> GetAllWithDeleted();

        Task<TEntity?> GetByIdAsync(int id);


        void Add(TEntity entity);

        Task AddAsync(TEntity entity);


        void Update(TEntity entity);

        void Remove(TEntity entity);


        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}