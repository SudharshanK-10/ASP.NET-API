using CityInfo.API.DBContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
	public class CityInfoRepository : ICityInfoRepository
	{
		private readonly CityInfoContext context;

		public CityInfoRepository(CityInfoContext context) {
			this.context = context?? throw new ArgumentNullException(nameof(context));
		}
		public async Task<IEnumerable<City>> GetCitiesAsync()
		{
			return await this.context.Cities.OrderBy(c => c.Name).ToListAsync();
		}
		// Overloading
		public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
		{
			//Paging has to implemented no matter how many cities we have, hence we can return all cities at once

			//if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(searchQuery))
			//{
			//	return await GetCitiesAsync();
			//}

			//Collection to start from, works on the Database level (before being fetched)
			var collection = this.context.Cities as IQueryable<City>;

			//filtering
			if(!string.IsNullOrEmpty(name)) 
			{
				name = name.Trim();
				collection = collection.Where(c => c.Name == name);
			}

			//searching
			if(!string.IsNullOrEmpty(searchQuery))
			{
				searchQuery = searchQuery.Trim();
				collection = collection.Where(c => c.Name.Contains(searchQuery) || (c.Description != null && c.Description.Contains(searchQuery)));
			}

			var totalItemCount = await collection.CountAsync();
			var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

			// paging
			//return await collection.OrderBy(c => c.Name)
			//	.Skip(pageSize * (pageNumber - 1))	//skip pageSize no of elements, pageNumber - 1 times
			//	.Take(pageSize)
			//	.ToListAsync();

			var collectionToReturn = await collection.OrderBy(c => c.Name)
				.Skip(pageSize * (pageNumber - 1))  //skip pagesize no of elements, pagenumber - 1 times
				.Take(pageSize)
				.ToListAsync();

			return (collectionToReturn, paginationMetadata);

			//name = name.Trim();

			//Since we accept name from user as part of a query there is a chance of encountering SQL Injection
			//EntityFrameWorkCore is an ORM (Object-Relation Mapping) framework, which handles SQL Injection vulnerabilities

			//return await this.context.Cities
			//	.Where(c => c.Name == name)
			//	.OrderBy(c => c.Name)
			//	.ToListAsync();
		}

		public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
		{
			// syntax different from IEnumerable -> this.context.Cities.FirstOrDefault(c => c.Id == cityId);
			if(includePointsOfInterest)
			{
				return await this.context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
			}

			return await this.context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
		}

		public async Task<bool> CityExistsAsync(int cityId)
		{
			return await this.context.Cities.AnyAsync(c => c.Id == cityId);
		}

		public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
		{
			return await this.context.PointsOfInterest.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
		{
			return await this.context.PointsOfInterest.Where(p => p.CityId == cityId).ToListAsync();
		}

		public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
		{
			var city = await GetCityAsync(cityId, false);
			if(city != null) 
			{
				city.PointsOfInterest.Add(pointOfInterest);
			}
		}

		public async Task<bool> SaveChangesAsync()
		{
			return (await this.context.SaveChangesAsync() >= 0);	
		}

		public void DeletePointOfInterestForCityAsync(PointOfInterest pointOfInterest)
		{
			this.context.PointsOfInterest.Remove(pointOfInterest);
		}

		public async Task<bool> CityNameMatchesCityId(string? name, int cityId)
		{
			return await this.context.Cities.AnyAsync(c => c.Id == cityId && c.Name == name);
		}
	}
}
