using Contracts.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common;

public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly TContext _context;

    public UnitOfWork(TContext context)
    {
        _context = context;
    }
    
    public Task<int> CommitAsync() => _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}