using System;
using System.IO;

namespace Fumiki
{
    public interface IFileStream : IDisposable
    {
        // long Position { get;set; }
        void Seek(long offset, SeekOrigin seekOrigin);
        long Read(byte[] buffer, int offset, int count);
    }
}