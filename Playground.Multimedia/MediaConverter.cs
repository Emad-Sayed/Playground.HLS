using Microsoft.Extensions.Options;
using Xabe.FFmpeg;

namespace Playground.Multimedia
{
    public class MediaConverter : IMediaConverter
    {
        public MultiMediaOptions _options { get; set; }
        public IInputOutputHelper _ioHelper { get; set; }

        public MediaConverter(MultiMediaOptions options, IInputOutputHelper ioHelper)
        {
            _options = options;
            _ioHelper = ioHelper;
        }

        public async Task ConvertToHLS(byte[] file, string fileName)
        {

            var tempFilePath = await _ioHelper.CreateTempFile(file, Path.GetFileNameWithoutExtension(fileName));
            try
            {
                var outputPath = Path.Combine(_options.Outputpath, fileName);
                Directory.CreateDirectory(outputPath);
                var output = Path.Combine(outputPath, "index.m3u8");
                var conversion = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{tempFilePath}\"")
                    .AddParameter("-codec: copy")
                    .AddParameter($"-f hls -hls_time 10 -hls_playlist_type vod \"{output}\"");

                await conversion.Start();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _ioHelper.RemoveTempFile(tempFilePath);
            }

        }
    }
}
