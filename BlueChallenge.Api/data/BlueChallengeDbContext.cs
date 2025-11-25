namespace BlueChallenge.Api.Data;

using BlueChallenge.Api.Model.Schedule;
using BlueChallenge.Api.Model.User;
using Microsoft.EntityFrameworkCore;

public class BlueChallengeDbContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<ScheduleModel> Schedules { get; set; }

    public BlueChallengeDbContext(DbContextOptions<BlueChallengeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlueChallengeDbContext).Assembly);
    }
}