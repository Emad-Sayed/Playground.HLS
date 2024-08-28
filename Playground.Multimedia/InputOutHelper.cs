using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Playground.Multimedia
{
    public class InputOutHelper : IInputOutputHelper
    {
        public MultiMediaOptions _options { get; set; }
        public InputOutHelper(MultiMediaOptions options)
        {
            _options = options;
        }
        public async Task<string> CreateTempFile(byte[] file, string fileName , string extension)
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(_options.TempFilePath));

                string tempInputFilePath = Path.Combine(_options.TempFilePath, $"{fileName}.{extension}");

                await File.WriteAllBytesAsync(tempInputFilePath, file);

                return tempInputFilePath;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public void RemoveTempFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
        }
    }
}
