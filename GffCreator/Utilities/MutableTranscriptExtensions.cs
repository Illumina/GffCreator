using System.Collections.Generic;
using System.IO;
using GffCreator.GFF;
using GffCreator.Mutable;

namespace GffCreator.Utilities
{
    public static class MutableTranscriptExtensions
    {
        // ReSharper disable once ParameterTypeCanBeEnumerable.Global
        public static void WriteGff(this List<MutableTranscript> transcripts, StreamWriter streamWriter)
        {
            using var writer = new GffWriter(streamWriter);
            var       output = new OutputPipeline(writer);
            output.Create(transcripts);
        }
    }
}