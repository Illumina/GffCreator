using System;
using System.Collections.Generic;
using System.IO;
using Compression.Utilities;
using GffCreator.GFF;
using GffCreator.Mutable;
using GffCreator.Utilities;
using Intervals;
using IO;
using VariantAnnotation.Caches;
using VariantAnnotation.Interface.AnnotatedPositions;
using VariantAnnotation.IO.Caches;
using VariantAnnotation.Providers;

namespace GffCreator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("USAGE: {0} <transcript source> <transcript cache path> <reference path> <output GFF path>",
                    Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }

            Source source              = SourceUtilities.GetSource(args[0]);
            string transcriptCachePath = args[1];
            string referencePath       = args[2];
            string outputPath          = args[3];

            if (!outputPath.EndsWith(".gff.gz"))
            {
                Console.WriteLine("ERROR: Output GFF filename must end in .gff.gz");
                Environment.Exit(1);
            }

            List<MutableTranscript> transcripts = LoadCache(transcriptCachePath, referencePath, source);
            transcripts.WriteGff(outputPath);
        }

        private static List<MutableTranscript> LoadCache(string cachePath, string referencePath, Source source)
        {
            Console.Write("- loading reference sequence... ");
            var sequenceProvider = new ReferenceSequenceProvider(FileUtilities.GetReadStream(referencePath));
            Console.WriteLine("finished.");

            Console.Write("- loading cache... ");
            IntervalArray<ITranscript>[] transcriptsByRef;
            Dictionary<IGene, int> geneToInternalId;
            
            using (var reader = new TranscriptCacheReader(FileUtilities.GetReadStream(cachePath)))
            {
                TranscriptCacheData cacheData = reader.Read(sequenceProvider, sequenceProvider.RefIndexToChromosome);
                transcriptsByRef = cacheData.TranscriptIntervalArrays;
                geneToInternalId = GeneUtilities.CreateDictionary(cacheData.Genes);
            }
            Console.WriteLine("finished.");
            
            Console.Write("- filter by transcript source... ");
            List<MutableTranscript> transcripts = CacheUtilities.FilterBySource(transcriptsByRef, source, geneToInternalId);
            Console.WriteLine($"{transcripts.Count:N0} remaining.");

            Console.Write("- updating transcript & gene coordinates... ");
            (int numTranscriptsUpdated, int numGenesUpdated) = CacheUtilities.FixTranscripts(transcripts);
            Console.WriteLine($"{numTranscriptsUpdated} transcripts & {numGenesUpdated} genes updated.");

            return transcripts;
        }

        private static void WriteGff(this List<MutableTranscript> transcripts, string outputPath)
        {
            Console.Write("- writing GFF entries... ");
            using var writer = new GffWriter(GZipUtilities.GetStreamWriter(outputPath));
            var       output = new OutputPipeline(writer);
            output.Create(transcripts);
            Console.WriteLine("finished.");
        }
    }
}