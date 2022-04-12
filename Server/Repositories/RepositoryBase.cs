using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities.Bases;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BlazingRecept.Server.Repositories;

public class RepositoryBase<Type> : IAsyncRepository<Type> where Type : BaseEntity
{
    protected internal BlazingReceptContext _context;

    public RepositoryBase(BlazingReceptContext context)
    {
        _context = context;
    }

    public virtual async Task<bool> AnyAsync(Guid id)
    {
        try
        {
            return await _context.Set<Type>().AnyAsync(entity => entity.Id == id);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to check existence of entity with id: {@Id}", id);
            return false;
        }
    }

    public virtual async Task<Type?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Set<Type>().FindAsync(id);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to fetch entity with id: {@Id}", id);
            return null;
        }
    }

    public virtual async Task<IReadOnlyList<Type>?> ListAllAsync()
    {
        try
        {
            return await _context.Set<Type>().ToListAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to fetch many entities");
            return null;
        }
    }

    public virtual async Task<Type> AddAsync(Type entity)
    {
        try
        {
            _context.Set<Type>().Add(entity);

            await _context.SaveChangesAsync();
            await _context.Entry(entity).ReloadAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to add entity: {@Entity}", entity);
        }

        return entity;
    }

    public async Task<IEnumerable<Type>> AddManyAsync(IEnumerable<Type> entities)
    {
        try
        {
            await _context.Set<Type>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            foreach (var entity in entities)
            {
                await _context.Entry(entity).ReloadAsync();
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to add many entities: {@Entities}", entities);
        }

        return entities;
    }

    public virtual async Task<Type> UpdateAsync(Type entity)
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            await _context.Entry(entity).ReloadAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to update entity: {@Entity}", entity);
        }

        return entity;
    }

    public virtual async Task UpdateManyAsync(IEnumerable<Type> entities)
    {
        try
        {
            if (!entities.Any())
            {
                return;
            }

            foreach (Type entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to update many entities: {@Entities}", entities);
        }
    }

    public virtual async Task<bool> DeleteAsync(Type entity)
    {
        try
        {
            _context.Set<Type>().Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to delete entity: {@Entity}", entity);
            return false;
        }
    }

    public virtual async Task<bool> DeleteManyAsync(IEnumerable<Type> entities)
    {
        try
        {
            _context.Set<Type>().RemoveRange(entities);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Repository failed to delete many entities: {@Entities}", entities);
            return false;
        }
    }

    protected internal void ApplyAddedOrModifiedState(BaseEntity baseEntity)
    {
        if (baseEntity.Id != Guid.Empty)
        {
            _context.Entry(baseEntity).State = EntityState.Modified;
        }
        else
        {
            _context.Entry(baseEntity).State = EntityState.Added;
        }
    }
}
