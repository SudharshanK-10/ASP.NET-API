using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
	public class City
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // New key will be generated every time a city is added
		public int Id { get; set; }

		[Required]
		[MaxLength(60)]
		public string Name { get; set; }

		[MaxLength(200)]
		public string? Description { get; set; }

		public ICollection<PointOfInterest> PointsOfInterest { get; set; } = new List<PointOfInterest>();

		public City(string name) 
		{
			this.Name = name;
		}
	}
}
