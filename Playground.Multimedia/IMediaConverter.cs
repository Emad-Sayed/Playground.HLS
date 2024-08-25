﻿namespace Playground.Multimedia
{
    public interface IMediaConverter
    {
        Task ConvertToHLS(byte[] file, string fileName, int chunkDuration = 10);
    }
}
