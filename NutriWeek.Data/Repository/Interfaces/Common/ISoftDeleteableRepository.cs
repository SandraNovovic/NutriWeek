namespace NutriWeek.Data.Repository.Interfaces.Common
{
    using Data.Repository.Common;
    using NutriWeek.Data.Repository.Interfaces.Common;
    using NutriWeek.GCommon.Models;

    public interface ISoftDeleteableRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, ISoftDeleteableEntity
    {
        IQueryable<TEntity> AllWithDeleted();

        IQueryable<TEntity> AllWithDeletedAsNoTracking();

        void SoftDelete(TEntity entity);

        void Recover(TEntity entity);
    }
}
