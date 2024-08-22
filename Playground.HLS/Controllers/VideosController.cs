using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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

        public VideosController(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpPost(Name = "ConvertToHls")]
        public async Task ConvertToHls([FromForm] UploadVideo request)
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
            string outputFilePath = Path.Combine(baseDirectory, Path.GetFileName(uploadedFile.FileName), "index.m3u8");
            //  string outputFilePath = Path.Combine(baseDirectory, "DoneVideos", "index.m3u8");

            try
            {
                // Save the uploaded file to the temporary file
                using (var stream = new FileStream(tempInputFilePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                // Create output folder if it doesn't exist
                Directory.CreateDirectory(outputFolderPath);

                // Set up the FFmpeg conversion
                var conversion = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{tempInputFilePath}\"")
                    .AddParameter("-codec: copy")
                    .AddParameter($"-f hls -hls_time 10 -hls_playlist_type vod \"{outputFilePath}\"");

                // Start the conversion
                await conversion.Start();
            }
            catch (Exception ex)
            {
                // Handle the exception
            }
            finally
            {
                // Clean up the temporary file
                if (System.IO.File.Exists(tempInputFilePath))
                {
                    System.IO.File.Delete(tempInputFilePath);
                }
            }
        }
    }
}
