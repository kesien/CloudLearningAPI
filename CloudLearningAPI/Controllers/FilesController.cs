using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CloudLearningAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly string _wordSavePath;
        private readonly string _importPath;
        public FilesController(IConfiguration config)
        {
            _wordSavePath = config["WordPath"];
            _importPath = config["ImportFilePath"];
        }
        [HttpGet("import/{filename}")]
        public ActionResult DownloadImportFile(string filename) 
        {
            var filePath = Path.Combine(_importPath, filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var file = System.IO.File.ReadAllBytes(filePath);
            var result = new FileContentResult(file, "application/octet-stream")
            {
                FileDownloadName = filename
            };

            return result;
        }
        
        [HttpGet("word/{filename}")]
        public ActionResult DownloadWordFile(string filename)
        {
            var filePath = Path.Combine(_wordSavePath, filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var file = System.IO.File.ReadAllBytes(filePath);
            var result = new FileContentResult(file, "application/octet-stream")
            {
                FileDownloadName = filename
            };

            return result;
        }
    }
}