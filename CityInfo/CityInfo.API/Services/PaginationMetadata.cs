namespace CityInfo.API.Services
{
	public class PaginationMetadata
	{
		public int TotalItemCount { get; set; }
		public int TotalPageCount { get; set; }
		public int PageSize { get; set; }
		public int CurrentPage { get; set; }

		public PaginationMetadata(int totalItemCount, int pageSize, int currentPage) 
		{
			this.TotalItemCount = totalItemCount;
			this.PageSize = pageSize;
			this.CurrentPage = currentPage;
			this.TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
		}
	}
}
