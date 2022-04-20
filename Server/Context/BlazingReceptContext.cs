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

    public DbSet<Category> Category => Set<Category>();
    public DbSet<DailyIntakeEntry> DailyIntakeEntry => Set<DailyIntakeEntry>();
    public DbSet<Ingredient> Ingredient => Set<Ingredient>();
    public DbSet<Recipe> Recipe => Set<Recipe>();
}
