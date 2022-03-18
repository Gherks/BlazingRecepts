using BlazingRecept.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazingRecept.Server.Context;

public class BlazingReceptContext : DbContext
{
    public BlazingReceptContext()
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public BlazingReceptContext(DbContextOptions options) : base(options)
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<Recipe> Recipe => Set<Recipe>();
    public DbSet<Ingredient> Ingredient => Set<Ingredient>();
    public DbSet<IngredientCategory> IngredientCategory => Set<IngredientCategory>();
}
