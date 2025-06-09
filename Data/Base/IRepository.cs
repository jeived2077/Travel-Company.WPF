using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travel_Company.WPF.Data.Base;

public interface IRepository<T, TKey> where T : class
{
    IQueryable<T> GetQuaryable();
    IEnumerable<T> GetAll();
    Task<IEnumerable<T>> GetAllAsync(); // Добавьте этот метод
    void Insert(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SaveChanges();
    Task SaveChangesAsync(); // Добавьте этот метод
    DbContext GetContext();
}