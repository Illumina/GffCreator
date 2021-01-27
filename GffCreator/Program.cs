using System;
using System.Collections.Generic;
using System.IO;
using Compression.Utilities;
using GffCreator.GFF;
using Intervals;
using IO;
using VariantAnnotation.Caches;
using VariantAnnotation.Interface.AnnotatedPositions;
using VariantAnnotation.IO.Caches;
using VariantAnnotation.Providers;

namespace GffCreator
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("USAGE: {0} <cache prefix> <reference path> <output GFF path>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            string cachePrefix   = args[0];
            string referencePath = args[1];
            string outputPath    = args[2];
            
            string cachePath = CacheConstants.TranscriptPath(cachePrefix);

            (IGene[] genes, IntervalArray<ITranscript>[] transcriptsByRef) = LoadCache(cachePath, referencePath);
            IDictionary<IGene, int> geneToInternalId = InternalGenes.CreateDictionary(genes);

            transcriptsByRef.WriteGff(outputPath, geneToInternalId);
        }

        private static (IGene[] Genes, IntervalArray<ITranscript>[] TranscriptsByRef) LoadCache(
            string cachePath, string referencePath)
        {
            IntervalArray<ITranscript>[] transcriptsByRef;
            IGene[]                      genes;

            var sequenceProvider = new ReferenceSequenceProvider(FileUtilities.GetReadStream(referencePath));

            using (var reader = new TranscriptCacheReader(FileUtilities.GetReadStream(cachePath)))
            {
                TranscriptCacheData cacheData = reader.Read(sequenceProvider, sequenceProvider.RefIndexToChromosome);
                transcriptsByRef = cacheData.TranscriptIntervalArrays;
                genes            = cacheData.Genes;
            }

            return (genes, transcriptsByRef);
        }
        
        private static void WriteGff(this IntervalArray<ITranscript>[] transcriptsByRef, string outputPath, IDictionary<IGene, int> geneToInternalId)
        {
            using (var writer = new GffWriter(GZipUtilities.GetStreamWriter(outputPath)))
            {
                var output = new OutputPipeline(writer, geneToInternalId);
                output.Create(transcriptsByRef);
            }
        }
    }
}