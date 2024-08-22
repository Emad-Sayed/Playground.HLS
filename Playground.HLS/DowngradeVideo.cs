namespace Playground.HLS
{
    public class DowngradeVideo
    {
        public IFormFile File { get; set; }
        public int Bitrate { get; set; } = 1000;
        public int Width { get; set; } = 640;
        public int Height { get; set; } = 360;
    }
}
