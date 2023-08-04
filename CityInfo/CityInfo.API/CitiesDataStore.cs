using CityInfo.API.Models;

namespace CityInfo.API
{
	public class CitiesDataStore
	{
		// in memory Datastore
		public List<CityDto> Cities { get; set; }  

		public static CitiesDataStore Current { get; } = new CitiesDataStore();

		public CitiesDataStore() { 
			// TODO - fetch from a database
			this.Cities = new List<CityDto>(){
				new CityDto()
				{
					Id = 1,
					Name = "Chennai",
					Description	= "Namma Chennai!",
					PointsOfInterest = new List<PointOfInterestDto>()
					{
						new PointOfInterestDto()
						{
							Id = 1,
							Name = "Marina Beach",
							Description = "2nd Longest Beach in the World!"
						}
					}
				},
				new CityDto()
				{
					Id = 2,
					Name = "Mumbai",
					Description = "Stock Exchange!",
					PointsOfInterest = new List<PointOfInterestDto>()
					{
						new PointOfInterestDto()
						{
							Id = 2,
							Name = "Bombay Stock Exchange",
							Description = "Turning point in the India's Economy!"
						}
					}
				},
				new CityDto()
				{
					Id = 3,
					Name = "Hyderabad",
					Description = "Nawabs!",
					PointsOfInterest = new List<PointOfInterestDto>()
					{
						new PointOfInterestDto()
						{
							Id = 3,
							Name = "Salar Jung Musuem",
							Description = "Learn about India's History!"
						}
					}
				}
			};
		}
	}
}
