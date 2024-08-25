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
            await _mediaConverter.ConvertToHLS(filesAsBytes, request.File.FileName);
            return Ok();

        }
        [HttpPost(Name = "Downgrade")]
        public async Task DowngradeVideoQuality([FromForm] DowngradeVideo request)
        {
            var uploadedFile = request.File;
            var outputFolderPath = "videos";
            var exePath = _configuration.GetSection("FFmpeg")["Path"];
            FFmpeg.SetExecutablesPath(exePath);
            string baseDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "videos");

            // Create directories if they don't exist
            Directory.CreateDirectory(Path.Combine(baseDirectory, "TempVideos"));
            Directory.CreateDirectory(Path.Combine(baseDirectory, Path.GetFileName(uploadedFile.FileName)));

            string tempInputFilePath = Path.Combine(baseDirectory, "TempVideos", Path.GetFileName(uploadedFile.FileName));
            string outputFilePath = Path.Combine(baseDirectory, "Downgraded", Path.GetFileName(uploadedFile.FileName));

            Directory.CreateDirectory(outputFolderPath);


            using (var stream = new FileStream(tempInputFilePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }


            // Build the conversion command
            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{tempInputFilePath}\"")  // Input file
                .AddParameter($"-b:v {request.Bitrate}k")     // Video bitrate (e.g., 500k for 500 kbps)
                .AddParameter($"-vf scale={request.Width}:{request.Height}")  // Resize video
                .AddParameter($"\"{outputFilePath}\"");   // Output file

            try
            {
                await conversion.Start();
                Console.WriteLine("Video quality downgraded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during video processing: {ex.Message}");
            }
        }
        [HttpPost(Name = "SaveVideoWithWaterMark")]
        public async Task<IActionResult> SaveVideoWithWaterMark([FromForm] UploadVideoWithWatermark request)
        {
            var uploadedFile = request.File;
            var exePath = _configuration.GetSection("FFmpeg")["Path"];
            FFmpeg.SetExecutablesPath(exePath);
            string baseDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "videos");

            // Create directories if they don't exist
            Directory.CreateDirectory(Path.Combine(baseDirectory, "TempVideos"));
            Directory.CreateDirectory(Path.Combine(baseDirectory, Path.GetFileName(uploadedFile.FileName)));

            string tempInputFilePath = Path.Combine(baseDirectory, "TempVideos", Path.GetFileName(uploadedFile.FileName));
            string tempWatermarkFilePath = Path.Combine(baseDirectory, "Watermarks", "OIP.jfif");
            string outputFilePath = Path.Combine(baseDirectory, Path.GetFileName(uploadedFile.FileName), "watermarked_" + Path.GetFileName(uploadedFile.FileName));

            try
            {
                using (var stream = new FileStream(tempInputFilePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }


                // Set up the FFmpeg conversion with a watermark
                var conversion = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{tempInputFilePath}\"")
                    .AddParameter($"-i \"{tempWatermarkFilePath}\"")
                    .AddParameter("-filter_complex \"overlay=10:10\"")
                    .AddParameter($"\"{outputFilePath}\"");

                // Start the conversion
                await conversion.Start();

                // Return success response
                return Ok(new { Message = "Video saved with watermark successfully.", FilePath = outputFilePath });
            }
            catch (Exception ex)
            {
                // Handle the exception
                return StatusCode(500, new { Message = "An error occurred while processing the video.", Error = ex.Message });
            }
            finally
            {
                // Clean up the temporary files
                if (System.IO.File.Exists(tempInputFilePath))
                {
                    System.IO.File.Delete(tempInputFilePath);
                }
            }
        }

    }
}
