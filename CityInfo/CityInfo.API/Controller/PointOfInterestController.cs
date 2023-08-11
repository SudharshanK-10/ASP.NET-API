using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controller
{
	[Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
	[ApiController]
	//[Authorize(Policy = "MustBeFromChennai")]
	[ApiVersion("2.0")]
	public class PointsOfInterestController : ControllerBase
	{
		private readonly ILogger<PointsOfInterestController> logger;
		private readonly IMailService mailService;
		private readonly ICityInfoRepository cityInfoRepository;
		private readonly IMapper mapper;

		//private readonly CitiesDataStore citiesDatastore;

		// constructor injection 
		public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, /*CitiesDataStore citiesDataStore*/
			ICityInfoRepository cityInfoRepository, IMapper mapper
			)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
			this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
			this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));


			//this.citiesDatastore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
		}

		[HttpGet]
		public async Task <ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
		{
			// Changing code to use entities and repository
			//try
			//{
			//	// throw new Exception("Sample Exception");
			//	var cityToReturn = this.citiesDatastore.Cities.FirstOrDefault(c => c.Id == cityId);
			//	if (cityToReturn == null)
			//	{
			//		this.logger.LogInformation($"City with ID {cityId} cannot be found");
			//		return NotFound();
			//	}

			//	return Ok(cityToReturn.PointsOfInterest);
			//}
			//catch(Exception ex) 
			//{
			//	this.logger.LogCritical($"Exception while getting points of interest for city {cityId}", ex);
			//	return StatusCode(500, "Problem occured while handling your request");
			//}

			// user can only view points of interest in their city
			//var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
			//if(!await this.cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
			//{
			//	return Forbid();	// 403 status code -> Authenticated, but no access
			//}

			bool cityExists = await this.cityInfoRepository.CityExistsAsync(cityId);

			if(!cityExists) 
			{
				this.logger.LogInformation($"City with {cityId} cannot be found!");
				return NotFound();
			}

			var pointsOfInterest = await this.cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
			return Ok(this.mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest));	
			
		}

		[HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
		public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
		{
			//var cityToReturn = this.citiesDatastore.Cities.FirstOrDefault(c => c.Id == cityId);
			//if (cityToReturn == null)
			//{
			//	return NotFound();
			//}

			//var pointOfInterestToReturn = cityToReturn.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			//if (pointOfInterestToReturn == null)
			//{
			//	return NotFound();
			//}

			//return Ok(pointOfInterestToReturn);

			bool cityExists = await this.cityInfoRepository.CityExistsAsync(cityId);
			if(!cityExists)
			{
				this.logger.LogInformation($"City with Id {cityId} cannot be found!");
				return NotFound();
			}

			var pointOfInterest = await this.cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
			if(pointOfInterest == null)
			{
				return NotFound();
			}
			return Ok(this.mapper.Map<PointOfInterestDto>(pointOfInterest));
		}

		[HttpPost]
		public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
		{
			//Model state - dictionary containing state of the model
			// checks whether the properties in the post request are valids

			//if (!ModelState.IsValid)
			//{
			//	return BadRequest();
			//}

			//var city = this.citiesDatastore.Cities.FirstOrDefault(c => c.Id == cityId);
			//if (city == null)
			//{
			//	return NotFound();
			//}

			bool cityExists = await this.cityInfoRepository.CityExistsAsync(cityId);
			if (!cityExists)
			{
				this.logger.LogInformation($"City with Id {cityId} cannot be found!");
				return NotFound();
			}

			// demo - select the point of interest with max value
			//var maxPointOfInterestId = this.citiesDatastore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

			//var finalPointOfInterest = new PointOfInterestDto()
			//{
			//	Id = ++maxPointOfInterestId,
			//	Name = pointOfInterest.Name,
			//	Description = pointOfInterest.Description
			//};

			var finalPointOfInterest = this.mapper.Map<Entities.PointOfInterest>(pointOfInterest);
			await this.cityInfoRepository.AddPointOfInterestForCityAsync(cityId , finalPointOfInterest);
			await this.cityInfoRepository.SaveChangesAsync();

			var createdPointOfInterestToReturn = this.mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

			//city.PointsOfInterest.Add(finalPointOfInterest);

			return CreatedAtRoute("GetPointOfInterest", 
				new
				{
					cityId = cityId,
					pointOfInterestId = /*finalPointOfInterest.Id*/ createdPointOfInterestToReturn.Id
				},
				createdPointOfInterestToReturn
				);
		}

		[HttpPut("{pointOfInterestId}")]
		public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
		{
			//var city = this.citiesDatastore.Cities.FirstOrDefault(c => c.Id == cityId);
			//if (city == null)
			//{
			//	return NotFound();
			//}

			if(!await this.cityInfoRepository.CityExistsAsync(cityId)) 
			{
				this.logger.LogInformation($"City with id {cityId} does not exists!");
				return NotFound();
			}

			//var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			//if (pointOfInterestFromStore == null)
			//{
			//	return NotFound();
			//}

			var pointOfInterestEntity = await this.cityInfoRepository.GetPointOfInterestForCityAsync (cityId, pointOfInterestId);
			if(pointOfInterestEntity == null) 
			{
				return NotFound();
			}

			this.mapper.Map(pointOfInterest, pointOfInterestEntity);
			await this.cityInfoRepository.SaveChangesAsync ();

			//pointOfInterestFromStore.Name = pointOfInterest.Name;
			//pointOfInterestFromStore.Description = pointOfInterest.Description;

			return NoContent();
		}

		[HttpPatch("{pointOfInterestId}")]
		public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
		{
			//var city = this.citiesDatastore.Cities.FirstOrDefault(c => c.Id == cityId);
			//if (city == null)
			//{
			//	return NotFound();
			//}

			if(!await this.cityInfoRepository.CityExistsAsync(cityId))
			{
				this.logger.LogInformation($"City with id {cityId} does not exists!");
				return NotFound();
			}

			//var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			//if (pointOfInterestFromStore == null)
			//{
			//	return NotFound();
			//}

			var pointOfInterestEntity = await this.cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
			if (pointOfInterestEntity == null)
			{
				return NotFound();
			}

			//var pointOfInterstToPatch = new PointOfInterestForUpdateDto()
			//{
			//	Name = pointOfInterestFromStore.Name,
			//	Description = pointOfInterestFromStore.Description
			//};

			var pointOfInterestToPatch = this.mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

			patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

			// checks whether the properties submitted in the body is valid
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (!TryValidateModel(pointOfInterestToPatch))
			{
				return BadRequest(ModelState);
			}

			this.mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
			await this.cityInfoRepository.SaveChangesAsync();

			//pointOfInterestFromStore.Name = pointOfInterstToPatch.Name;
			//pointOfInterestFromStore.Description = pointOfInterstToPatch.Description;

			return NoContent();
		}

		[HttpDelete("{pointOfInterestId}")]
		public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
		{
			//var city = this.citiesDatastore.Cities.FirstOrDefault(c => c.Id == cityId);
			//if (city == null)
			//{
			//	return NotFound();
			//}

			if (!await this.cityInfoRepository.CityExistsAsync(cityId))
			{
				this.logger.LogInformation($"City with id {cityId} does not exists!");
				return NotFound();
			}

			//var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			//if (pointOfInterestFromStore == null)
			//{
			//	return NotFound();
			//}

			var pointOfInterestEntity = await this.cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
			if (pointOfInterestEntity == null)
			{
				return NotFound();
			}

			this.cityInfoRepository.DeletePointOfInterestForCityAsync (pointOfInterestEntity);
			await this.cityInfoRepository.SaveChangesAsync();

			//city.PointsOfInterest.Remove(pointOfInterestFromStore);
			this.mailService.Send("Point of Interest Deleted", $"Point of interest {pointOfInterestEntity.Id} for City {pointOfInterestEntity.CityId} is deleted!");

			return NoContent();
		}
	}
}
