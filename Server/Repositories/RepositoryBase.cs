using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities.Base;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazingRecept.Server.Repositories;

public class RepositoryBase<Type> : IAsyncRepository<Type> where Type : BaseEntity
{
    protected internal BlazingReceptContext _context;

    public RepositoryBase(BlazingReceptContext context)
    {
        _context = context;
    }

    public virtual async Task<Type?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Type>().FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<Type>> ListAllAsync()
    {
        return await _context.Set<Type>().ToListAsync();
    }

    public async Task<Type> AddAsync(Type entity)
    {
        _context.Set<Type>().Add(entity);

        try
        {
            await _context.SaveChangesAsync();
            await _context.Entry(entity).ReloadAsync();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }

        return entity;
    }

    public async Task<IEnumerable<Type>> AddManyAsync(IEnumerable<Type> entities)
    {
        await _context.Set<Type>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        foreach (var entity in entities)
        {
            await _context.Entry(entity).ReloadAsync();
        }

        return entities;
    }

    public virtual async Task<Type> UpdateAsync(Type entity)
    {
        _context.Entry(entity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        await _context.Entry(entity).ReloadAsync();

        return entity;
    }

    public async Task UpdateManyAsync(IEnumerable<Type> entities)
    {
        var entityArray = entities.ToArray();

        if (!entityArray.Any())
        {
            return;
        }

        foreach (Type entity in entityArray)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Type entity)
    {
        _context.Set<Type>().Remove(entity);
        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<bool> DeleteManyAsync(IEnumerable<Type> entities)
    {
        _context.Set<Type>().RemoveRange(entities);
        return await _context.SaveChangesAsync() == entities.Count();
    }
}
