using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DBContexts
{
	public class CityInfoContext : DbContext
	{
		public DbSet<City> Cities { get; set; } = null!;
		public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

		public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options) 
		{ 

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Creating sample records in the database
			modelBuilder.Entity<City>()
				.HasData(
				new City("Chennai")
				{
					Id = 1,
					Description = "Namma Chennai!"
				},
				new City("Mumbai")
				{
					Id = 2,
					Description = "Stock Exchange!"
				},
				new City("Hyderabad")
				{
					Id = 3,
					Description = "Nawabs!"
				});

			modelBuilder.Entity<PointOfInterest>()
				.HasData(
				new PointOfInterest("Marina Beach")
				{
					Id = 1,
					CityId = 1,
					Description = "2nd Longest Beach in the World!"
				},
				new PointOfInterest("Bombay Stock Exchange")
				{
					Id = 2,
					CityId = 2,
					Description = "Turing point in the Indian Economy!"
				},
				new PointOfInterest("Salar Jung Museum")
				{
					Id = 3,
					CityId = 3,
					Description = "Learn about India's History!"
				});

			base.OnModelCreating(modelBuilder);
		}

		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	optionsBuilder.UseSqlite("connectionstring");
		//	base.OnConfiguring(optionsBuilder);
		//}
	}
}
