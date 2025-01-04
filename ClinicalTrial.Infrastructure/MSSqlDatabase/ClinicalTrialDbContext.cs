using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClinicalTrial.Infrastructure.MSSqlDatabase;

public class ClinicalTrialDbContext : DbContext
{
    public DbSet<Domain.Entities.ClinicalTrial> ClinicalTrials { get; set; }
    private readonly IConfiguration _configuration;

    public ClinicalTrialDbContext()
    {
            
    }
    public ClinicalTrialDbContext(DbContextOptions<ClinicalTrialDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Default"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.ClinicalTrial>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Domain.Entities.ClinicalTrial>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();
    }
}