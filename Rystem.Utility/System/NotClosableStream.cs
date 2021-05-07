using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.IO
{
    public sealed class NotClosableStream : MemoryStream
    {
        protected override void Dispose(bool disposing)
        {
            Flush();
            Seek(0, SeekOrigin.Begin);
        }
        public void ManualDispose()
        {
            base.Dispose(true);
        }

    }
}
