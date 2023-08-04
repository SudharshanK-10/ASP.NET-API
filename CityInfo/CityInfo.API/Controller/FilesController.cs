using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controller
{
	[Route("api/files")]
	[ApiController]
	public class FilesController : ControllerBase
	{

		private readonly FileExtensionContentTypeProvider fileExtensionContentTypeProvider;
		
		// when invalid file format is returned
		public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
		{
			this.fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? 
				throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));
		}

		[HttpGet("{fileId}")]
		 public ActionResult GetFile(int fileId)
		{
			var pathToFile = "sample.pdf";
			
			if(!System.IO.File.Exists(pathToFile))
			{
				return NotFound(); 
			}

			if(!this.fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
			{
				contentType = "application/octet-stream";
			}

			var bytes = System.IO.File.ReadAllBytes(pathToFile);
			return File(bytes, contentType, Path.GetFileName(pathToFile));
		}
	}
}
