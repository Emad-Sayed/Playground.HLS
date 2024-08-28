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

        public async Task ConvertToHLS(byte[] file, string fileName, int chunkDuration)
        {

            var tempFilePath = await _ioHelper.CreateTempFile(file, Path.GetFileNameWithoutExtension(fileName), "mp4");
            try
            {
                var outputPath = Path.Combine(_options.Outputpath, fileName);
                Directory.CreateDirectory(outputPath);
                var output = Path.Combine(outputPath, "index.m3u8");
                var conversion = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{tempFilePath}\"")
                    .AddParameter("-codec: copy")
                    .AddParameter($"-f hls -hls_time {chunkDuration} -hls_playlist_type vod \"{output}\"");

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
        public async Task AddWatermarkToVideo(byte[] file, string fileName, byte[] waterMark, string waterMarkFileName, string overlay = "10:10")
        {

            var tempFilePath = await _ioHelper.CreateTempFile(file, Path.GetFileNameWithoutExtension(fileName), "mp4");
            var tempFilePathWaterMarkPath = await _ioHelper.CreateTempFile(waterMark, Path.GetFileNameWithoutExtension(waterMarkFileName), "jpg");
            try
            {
                var outputPath = Path.Combine(_options.Outputpath, fileName);
                Directory.CreateDirectory(outputPath);
                string outputFilePath = Path.Combine(outputPath, Path.GetFileName(fileName));

                var conversion = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{tempFilePath}\"")
                    .AddParameter($"-i \"{tempFilePathWaterMarkPath}\"")
                    .AddParameter($"-filter_complex \"overlay={overlay}\"")
                    .AddParameter($"\"{outputFilePath}\"");

                // Start the conversion
                await conversion.Start();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _ioHelper.RemoveTempFile(tempFilePath);
                _ioHelper.RemoveTempFile(tempFilePathWaterMarkPath);
            }

        }

        public async Task<MediaInfoResult> GetMediaInfo(byte[] file, string fileName)
        {
            var tempFilePath = await _ioHelper.CreateTempFile(file, Path.GetFileNameWithoutExtension(fileName), "mp4");
            MediaInfoResult mediaInfo = new MediaInfoResult();
            try
            {

                mediaInfo.MediaInfo = await FFmpeg.GetMediaInfo(tempFilePath);


            }
            catch (Exception ex)
            {

            }
            finally
            {
                _ioHelper.RemoveTempFile(tempFilePath);
            }
            return mediaInfo;

        }
    }
}
