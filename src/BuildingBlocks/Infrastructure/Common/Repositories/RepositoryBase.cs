using Contracts.Common.Interfaces;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Common;

public class RepositoryBaseAsync<T, K, TContext> : RepositoryQueryBase<T, K, TContext>,
    IRepositoryBaseAsync<T, K, TContext> where T : EntityBase<K>
where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly IUnitOfWork<TContext> _unitOfWork;

    public RepositoryBaseAsync(TContext dbContext, IUnitOfWork<TContext> unitOfWork) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public Task<IDbContextTransaction> BeginTransactionAsync() => _dbContext.Database.BeginTransactionAsync();

    public async Task EndTransactionAsync()
    {
        await SaveChangesAsync();
        await _dbContext.Database.CommitTransactionAsync();
    }

    public Task RollbackTransactionAsync() => _dbContext.Database.RollbackTransactionAsync();

    public void Create(T entity) 
        => _dbContext.Set<T>().Add(entity);

    public async Task<T> CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }
    public void CreateList(IEnumerable<T> entities) => _dbContext.Set<T>().AddRange(entities);
    public async Task<IList<K>> CreateListAsync(IEnumerable<T> entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
        return entities.Select(x => x.Id).ToList();
    }

    public void Update(T entity)
    {
        if(_dbContext.Entry(entity).State == EntityState.Unchanged) return;
        T? exist = _dbContext.Set<T>().Find(entity.Id);
        if (exist != null) _dbContext.Entry(exist).CurrentValues.SetValues(entity);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Unchanged) return entity;

        T exist = _dbContext.Set<T>().Find(entity.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(entity);
        await SaveChangesAsync();
        return entity;
    }
    
    public void UpdateLists(IEnumerable<T> entities)
        => _dbContext.Set<T>().UpdateRange(entities);
    
    public async Task UpdateListAsync(IEnumerable<T> entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
        await SaveChangesAsync();
    }

    public Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteListAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync() => _unitOfWork.CommitAsync();
    
    public void Delete(T entity)
        => _dbContext.Set<T>().Remove(entity);

    public void DeleteList(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }
}