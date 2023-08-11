using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controller
{
	[ApiController]
	//[Authorize]
	[ApiVersion("1.0")]
	[ApiVersion("2.0")]
	[Route("api/v{version:apiVersion}/cities")]
	public class CitiesController : ControllerBase
	{
		//private readonly CitiesDataStore citiesDatatore;
		private readonly ICityInfoRepository cityInfoRepository;
		private readonly IMapper mapper;
		const int maxCitiesPageSize = 20;

		// Rewriting code to fetch data from local repository rather than from in memory datastore
		public CitiesController(/*CitiesDataStore citiesDataStore*/ ICityInfoRepository cityInfoRepository, IMapper mapper)
		{
			//this.citiesDatatore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
			this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
			this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}
		[HttpGet]
		public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
		{
			pageSize = pageSize > maxCitiesPageSize ? maxCitiesPageSize : pageSize; //pageSize should be in [1, 20]

			var (cityEntities, paginationMetadata) = await this.cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

			//Add pagination metadata to header of the response
			Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

			return Ok(this.mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities)); // mapping entities object -> Dto object

			// manually mapping citie entities to city dto
			/*
			var result = new List<CityWithoutPointsOfInterestDto>();

			foreach(var cityEntity in cityEntities) 
			{
				result.Add(new CityWithoutPointsOfInterestDto
				{
					Id = cityEntity.Id,
					Name = cityEntity.Name,
					Description = cityEntity.Description
				});
			}
			return Ok(result);
			*/
			//return Ok(this.citiesDatatore.Cities);
			//return new JsonResult(CitiesDataStore.Current.Cities);
		}

		[HttpGet("{id}")]
		public async /*Task<ActionResult<CityDto>>*/ Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
		{
			//var cityToReturn = this.citiesDatatore.Cities.FirstOrDefault(c => c.Id == id);
			//return cityToReturn is null ? NotFound() : Ok(cityToReturn);

			//return new JsonResult(CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));

			var city = await this.cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
			if(city == null)
			{
				return NotFound();
			}

			if (includePointsOfInterest)
			{
				return Ok(this.mapper.Map<CityDto>(city));
			}

			return Ok(this.mapper.Map<CityWithoutPointsOfInterestDto>(city));
		}
	}
}
