using FoodRescue.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodRescue.API.Data;

public class FoodRescueDbContext : DbContext
{
    public FoodRescueDbContext(DbContextOptions<FoodRescueDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<Donation> Donations => Set<Donation>();
}