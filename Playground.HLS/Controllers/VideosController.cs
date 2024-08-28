using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Playground.Multimedia;
using System.Diagnostics;
using Xabe.FFmpeg;

namespace Playground.HLS.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class VideosController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IMediaConverter _mediaConverter;

        public VideosController(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, IMediaConverter mediaConverter)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _mediaConverter = mediaConverter;
        }

        [HttpPost(Name = "ConvertToHls")]
        public async Task<ActionResult> ConvertToHls([FromForm] UploadVideo request)
        {
            byte[] filesAsBytes;
            using (var memoryStream = new MemoryStream())
            {
                await request.File.CopyToAsync(memoryStream);
                filesAsBytes = memoryStream.ToArray();
            }
            await _mediaConverter.ConvertToHLS(filesAsBytes, Path.Combine("HLS", request.File.FileName));
            return Ok();

        }
        [HttpPost(Name = "SaveVideoWithWaterMark")]
        public async Task<IActionResult> SaveVideoWithWaterMark([FromForm] UploadVideoWithWatermark request)
        {

            byte[] filesAsBytes;
            byte[] filesAsBytesWaterMark;
            using (var memoryStream = new MemoryStream())
            {
                await request.File.CopyToAsync(memoryStream);
                filesAsBytes = memoryStream.ToArray();
            }
            using (var memoryStream = new MemoryStream())
            {
                await request.WaterMark.CopyToAsync(memoryStream);
                filesAsBytesWaterMark = memoryStream.ToArray();
            }
            await _mediaConverter.AddWatermarkToVideo(filesAsBytes, Path.Combine("Watermarks", request.File.FileName),
                filesAsBytesWaterMark, request.File.FileName);
            return Ok();

        }
        [HttpPost(Name = "GetVideoDuration")]
        public async Task<IActionResult> GetVideoDuration([FromForm] GetVideoMetaData request)
        {

            byte[] filesAsBytes;
            using (var memoryStream = new MemoryStream())
            {
                await request.File.CopyToAsync(memoryStream);
                filesAsBytes = memoryStream.ToArray();
            }
            var result = await _mediaConverter.GetMediaInfo(filesAsBytes, request.File.FileName);
            return Ok(result);

        }

    }
}
