using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mantel.Programming.Tasks.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mantel.Programming.Tasks.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogAnalyserController : ControllerBase
    {
        private readonly ILogFileAnalyserService _logFileAnalyserService;

        public LogAnalyserController(ILogFileAnalyserService logFileAnalyserService)
        {
            _logFileAnalyserService = logFileAnalyserService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadLogAsync()
        {
            // Assumption that only one file will be uploaded at a time
            if (!Request.Form.Files.Any() || Request.Form.Files[0].Length == 0)
                return BadRequest();

            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                await Request.Form.Files[0].CopyToAsync(stream);
            }
            var response =  await _logFileAnalyserService.ProcessFileAsync(filePath);
            return response.IsSuccessful
                ? Ok(response.Data)
                : BadRequest(response.ErrorMessages);
        }

        [HttpGet("Urls/Top/{count:int}")]
        public IActionResult FindTopUrls(int count)
        {
            var result = _logFileAnalyserService.FindMostVisitedUrls(count);
            return result.IsSuccessful
                ? Ok(result.Data)
                : BadRequest(result.ErrorMessages);
        }

        [HttpGet]
        [HttpGet("IpAddresses/Top/{count:int}")]
        public IActionResult FindTopIpAddresses(int count)
        {
            var result = _logFileAnalyserService.FindMostActiveAddresses(count);
            return result.IsSuccessful
                ? Ok(result.Data)
                : BadRequest(result.ErrorMessages);
        }

        [HttpGet]
        [HttpGet("IpAddresses/Unique/count")]
        public IActionResult CountUniqueIpAddresses()
        {
            var result = _logFileAnalyserService.CountUniqueIpAddresses();
            return Ok(result);
        }

    }
}