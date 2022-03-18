using Microsoft.EntityFrameworkCore;
using Receptacle.Server.Entities;

namespace Receptacle.Server.Context;

public class ReceptacleContext : DbContext
{
    public ReceptacleContext()
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public ReceptacleContext(DbContextOptions options) : base(options)
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<Recipe> Recipe { get; set; }
    public DbSet<Ingredient> Ingredient { get; set; }
    public DbSet<IngredientCategory> IngredientCategory { get; set; }
}
