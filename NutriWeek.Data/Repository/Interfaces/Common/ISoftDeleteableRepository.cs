namespace CinemaApp.Data.Repository.Contracts.Common
{
    using Data.Common.Models;
    using NutriWeek.Data.Repository.Interfaces.Common;

    public interface ISoftDeleteableRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, ISoftDeleteableEntity
    {
        IQueryable<TEntity> AllWithDeleted();

        IQueryable<TEntity> AllWithDeletedAsNoTracking();

        void SoftDelete(TEntity entity);

        void Recover(TEntity entity);
    }
}
