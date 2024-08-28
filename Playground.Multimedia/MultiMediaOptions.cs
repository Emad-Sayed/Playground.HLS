using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Playground.Multimedia
{
    public class MultiMediaOptions
    {
        public string ContentRootPath { get; private set; }
        public string TempFilePath { get; private set; } = Path.GetTempPath();
        public string Outputpath { get; private set; }
        public string FFmpegPath { get; private set; }
        public void SetPaths(string FFmpegPath, string contentRootPath, string outputPath, string tempFilePath = null)
        {
            FFmpegPath = FFmpegPath;
            ContentRootPath = contentRootPath;
            TempFilePath = tempFilePath ?? Path.GetTempPath();
            Outputpath = outputPath;
            FFmpeg.SetExecutablesPath(FFmpegPath);
        }
    }
}
