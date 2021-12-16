using System.IO;
using System.Threading.Tasks;
using CloudLearningAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CloudLearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IDropboxService _dropboxService;
        public FilesController(IDropboxService dropboxService)
        {
            _dropboxService = dropboxService;
        }

        [HttpGet("{dirname}/{filename}")]
        public async Task<IActionResult> DownloadImportFile(string dirname, string filename) 
        {
            var file = await _dropboxService.GetFile($"/{dirname}/{filename}");
            if (file is null)
            {
                return NotFound($"Couldn't find file: {filename}");
            }
            var result = new FileContentResult(file, "application/octet-stream")
            {
                FileDownloadName = filename
            };

            return result;
        }
        
    }
}