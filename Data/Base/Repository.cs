using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Data.Base;

public class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class
{
    private readonly TravelCompanyDbContext _context;

    public Repository(TravelCompanyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IEnumerable<TEntity> GetAll()
    {
        return _context.Set<TEntity>().ToList();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public TEntity? GetById(TId id)
    {
        return _context.Set<TEntity>().Find(id);
    }

    public IQueryable<TEntity> GetQuaryable()
    {
        return _context.Set<TEntity>();
    }

    public void Insert(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // Реализация метода GetContext
    public DbContext GetContext()
    {
        return _context;
    }
}