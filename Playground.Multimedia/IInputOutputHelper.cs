using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Multimedia
{
    public interface IInputOutputHelper
    {
          Task<string> CreateTempFile(byte[] file, string fileName);
          void RemoveTempFile(string fileName);
    }
}
