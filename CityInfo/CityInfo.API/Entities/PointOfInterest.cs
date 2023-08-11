using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities
{
	public class PointOfInterest
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // New key will be generated every time a city is added
		public int Id { get; set; }

		[Required]
		[MaxLength(60)]
		public string Name {get; set; }

		[MaxLength(200)]
		public string Description { get; set; }

		[ForeignKey("CityId")]
		public City? City { get; set; }
		public int CityId { get; set; }

		public PointOfInterest(string name)
		{
			this.Name = name;
		}
	}
}
