using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
	public class PointOfInterestForUpdateDto
	{
		[Required(ErrorMessage = "Name of the place must be provided")]
		[MaxLength(60)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(200)]
		public string? Description { get; set; }
	}
}
