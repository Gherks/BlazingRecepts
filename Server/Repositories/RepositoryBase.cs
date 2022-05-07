﻿using BlazingRecept.Server.Context;
using BlazingRecept.Server.Entities.Bases;
using BlazingRecept.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BlazingRecept.Server.Repositories;

public class RepositoryBase<Type> : IAsyncRepository<Type> where Type : BaseEntity
{
    private static readonly string _logProperty = "Domain";

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
            const string messageTemplate = "Repository failed to check existence of entity with id: {@Id}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, id);

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
            const string messageTemplate = "Repository failed to fetch entity with id: {@Id}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, id);

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
            const string errorMessage = "Repository failed to fetch many entities";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, errorMessage);

            return null;
        }
    }

    public virtual async Task<Type?> AddAsync(Type entity)
    {
        try
        {
            _context.Set<Type>().Add(entity);

            await _context.SaveChangesAsync();
            await _context.Entry(entity).ReloadAsync();
        }
        catch (Exception exception)
        {
            const string messageTemplate = "Repository failed to add entity: {@Entity}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, entity);

            return null;
        }

        return entity;
    }

    public async Task<IEnumerable<Type>?> AddManyAsync(IEnumerable<Type> entities)
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
            const string messageTemplate = "Repository failed to add many entities: {@Entities}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, entities);

            return null;
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
            const string messageTemplate = "Repository failed to update entity: {@Entity}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, entity);
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
            const string messageTemplate = "Repository failed to update many entities: {@Entities}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, entities);
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
            const string messageTemplate = "Repository failed to delete entity: {@Entity}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, entity);

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
            const string messageTemplate = "Repository failed to delete many entities: {@Entities}";
            Log.ForContext(_logProperty, GetType().Name).Error(exception, messageTemplate, entities);

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
