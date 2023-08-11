using AutoMapper;

namespace CityInfo.API.Profiles
{
	public class CityProfile : Profile
	{
		public CityProfile()
		{ 
			// Map entity object to dto
			CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
			CreateMap<Entities.City, Models.CityDto>();
		}
	}
}
