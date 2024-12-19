using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API;

public class CityInfoContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<PointOfInterest> PointOfInterests { get; set; }
    public CityInfoContext(DbContextOptions<CityInfoContext> options)
        : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new City("City1")
            {
                Id = 1,
                Description = "D1"
            },
            new City("City2")
            {
                Id = 2,
                Description = "D2"
            },
            new City("City3")
            {
                Id = 3,
                Description = "D3"
            }
            );



        modelBuilder.Entity<PointOfInterest>().HasData(
            new PointOfInterest("PointOfInterest1")
            {
                Id = 1,
                Description = "P1",
                CityId = 1,
            },
            new PointOfInterest("PointOfInterest2")
            {
                Id = 2,
                Description = "P2",
                CityId = 1,

            },
            new PointOfInterest("PointOfInterest3")
            {
                Id = 3,
                Description = "P3",
                CityId = 2
            }
        );

        base.OnModelCreating(modelBuilder);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlite("");
    //}
}
