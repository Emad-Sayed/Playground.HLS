using Xabe.FFmpeg;

namespace Playground.Multimedia
{
    public interface IMediaConverter
    {
        Task ConvertToHLS(byte[] file, string fileName, int chunkDuration = 10);
        Task AddWatermarkToVideo(byte[] file, string fileName, byte[] waterMark, string waterMarkFileName, string overlay = "10:10");
        Task<MediaInfoResult> GetMediaInfo(byte[] file, string fileName);
    }
}
