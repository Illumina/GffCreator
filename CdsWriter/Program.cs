using System;
using System.Collections.Generic;
using System.IO;
using Intervals;
using IO;
using VariantAnnotation.Caches;
using VariantAnnotation.Interface.AnnotatedPositions;
using VariantAnnotation.IO.Caches;
using VariantAnnotation.Providers;

namespace CdsWriter
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("USAGE: {0} <transcript cache path> <reference path> <output CDS TSV path>",
                    Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
            }
            
            string transcriptCachePath = args[0];
            string referencePath       = args[1];
            string outputPath          = args[2];

            List<ITranscript> transcripts = LoadCache(transcriptCachePath, referencePath);
            
            Console.Write("- writing TSV entries... ");
            WriteTsv(transcripts, outputPath);
            Console.WriteLine("finished.");
        }

        private static void WriteTsv(List<ITranscript> transcripts, string outputPath)
        {
            using var stream = new FileStream(outputPath, FileMode.Create);
            using var writer = new StreamWriter(stream) {NewLine = "\n"};

            writer.WriteLine(
                "#chromosome\tstart\tend\ttranscriptID\ttranscriptSource\tcodingRegionCdnaStart\tcodingRegionCdnaEnd\tmaxCdnaCoord");

            foreach (ITranscript transcript in transcripts)
            {
                if (transcript.Translation == null) continue;
                ICodingRegion codingRegion = transcript.Translation.CodingRegion;
                int           maxCdnaCoord = GetMaxCdnaCoord(transcript.TranscriptRegions);
                writer.WriteLine(
                    $"{transcript.Chromosome.UcscName}\t{transcript.Start}\t{transcript.End}\t{transcript.Id.WithVersion}\t{transcript.Source}\t{codingRegion.CdnaStart}\t{codingRegion.CdnaEnd}\t{maxCdnaCoord}");
            }
        }

        private static int GetMaxCdnaCoord(ITranscriptRegion[] regions)
        {
            int first = regions[0].CdnaEnd;
            int last  = regions[^1].CdnaEnd;
            return last > first ? last : first;
        }

        private static List<ITranscript> LoadCache(string cachePath, string referencePath)
        {
            Console.Write("- loading reference sequence... ");
            var sequenceProvider = new ReferenceSequenceProvider(FileUtilities.GetReadStream(referencePath));
            Console.WriteLine("finished.");

            Console.Write("- loading cache... ");
            IntervalArray<ITranscript>[] transcriptsByRef;

            using (var reader = new TranscriptCacheReader(FileUtilities.GetReadStream(cachePath)))
            {
                TranscriptCacheData cacheData = reader.Read(sequenceProvider.RefIndexToChromosome);
                transcriptsByRef = cacheData.TranscriptIntervalArrays;
            }
            Console.WriteLine("finished.");
            
            Console.Write("- extracting transcripts... ");
            List<ITranscript> transcripts = GetTranscripts(transcriptsByRef);
            Console.WriteLine($"{transcripts.Count:N0} found.");

            return transcripts;
        }

        private static List<ITranscript> GetTranscripts(IntervalArray<ITranscript>[] transcriptsByRef)
        {
            var transcripts  = new List<ITranscript>();

            foreach (IntervalArray<ITranscript> intervalArray in transcriptsByRef)
            {
                if (intervalArray == null) continue;
                foreach (Interval<ITranscript> interval in intervalArray.Array) transcripts.Add(interval.Value);
            }

            return transcripts;
        }
    }
}