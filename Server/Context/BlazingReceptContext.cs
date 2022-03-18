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

    public DbSet<Recipe> Recipe { get; set; }
    public DbSet<Ingredient> Ingredient { get; set; }
    public DbSet<IngredientCategory> IngredientCategory { get; set; }
}
