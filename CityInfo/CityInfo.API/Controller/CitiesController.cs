using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controller
{
	[ApiController]
	[Route("api/cities")]
	public class CitiesController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<CityDto>> GetCities()
		{
			return Ok(CitiesDataStore.Current.Cities);

			//return new JsonResult(CitiesDataStore.Current.Cities);
		}

		[HttpGet("{id}")]
		public ActionResult<CityDto> GetCity(int id)
		{
			var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
			return cityToReturn is null ? NotFound() : Ok(cityToReturn);

			//return new JsonResult(CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
		}
	}
}
