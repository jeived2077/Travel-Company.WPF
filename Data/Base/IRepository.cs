using System.Collections.Generic;
using System.Linq;

namespace Travel_Company.WPF.Data.Base;

public interface IRepository<TEntity, TId>
{
    List<TEntity> GetAll();

    TEntity? GetById(TId id);

    IQueryable<TEntity> GetQuaryable();

    void Insert(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    void SaveChanges();
}