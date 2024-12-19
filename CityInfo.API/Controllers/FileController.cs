using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/files")]
    [ApiController]
   
     public class FileController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FileController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
           _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
        }
        
        [HttpGet("{fileId}")]
        [ApiVersion(0.1 , Deprecated =true)]
        public ActionResult GetFile(string fileId)
        {
            string path = "2025-ICPC Palestinian CPC-Falastin Bawaqna-PLACE.pdf";

            if (!System.IO.File.Exists(path)) {
                return NotFound();
            }
            if (!_fileExtensionContentTypeProvider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";

            }
            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes,contentType, Path.GetFileName(path));
        }

        [HttpPost]
        public  async Task<ActionResult> UploadFile(IFormFile file)
        {
            if(file.Length == 0 || file.Length>12345231 || file.ContentType != "application/pdf")
            {
                return BadRequest("invalid inputted file!");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded{Guid.NewGuid()}.pdf");
            using(var stream = new FileStream(path,FileMode.Create))
            {
                 await file.CopyToAsync(stream);
            }
            return Ok("Your file uploaded successfully!");
        }
    }
}
