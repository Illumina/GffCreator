using System;
using System.IO;

namespace GffCreator.GFF
{
    public sealed class GffWriter : IDisposable
    {
        private readonly StreamWriter _writer;

        public GffWriter(StreamWriter writer) => _writer = writer;

        public void Dispose() => _writer.Dispose();

        public void WriteEntry(GffEntry entry) => _writer.WriteLine(entry);
    }
}