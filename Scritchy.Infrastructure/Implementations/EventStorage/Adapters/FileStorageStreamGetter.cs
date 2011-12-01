using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scritchy.Infrastructure.Implementations.EventStorage.Adapters
{
    public static class FileStorageStreamGetter
    {
        public static Stream GetStreamForFileName(string filename, StreamAdapter.StreamAccess access)
        {
            return File.Open(filename, FileMode.OpenOrCreate,access == StreamAdapter.StreamAccess.ForReading ? FileAccess.Read : FileAccess.Write);
        }
    }
}
