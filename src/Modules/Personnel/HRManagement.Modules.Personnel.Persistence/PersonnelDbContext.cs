using HRManagement.Common.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HRManagement.Modules.Personnel.Persistence;

public class PersonnelDbContext : DbContext
{
    private readonly string _connectionString;

    public PersonnelDbContext(IOptions<AppSettings> settings)
    {
        _connectionString = settings.Value.ConnectionStrings.PersonnelManagement;
    }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(_connectionString)
            .UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonnelDbContext).Assembly);
    }
}