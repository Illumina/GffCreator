using System.Collections.Generic;
using GffCreator.Comparers;
using GffCreator.Mutable;
using Intervals;
using VariantAnnotation.Interface.AnnotatedPositions;

namespace GffCreator.Utilities
{
    public static class CacheUtilities
    {
        // ReSharper disable once ParameterTypeCanBeEnumerable.Global
        public static List<MutableTranscript> FilterBySource(IntervalArray<ITranscript>[] transcriptsByRef,
                                                             Source                       desiredSource,
                                                             Dictionary<IGene, int>       geneToInternalId)
        {
            var transcripts  = new List<MutableTranscript>();
            var geneComparer = new GeneComparer();
            var genes        = new HashSet<MutableGene>(geneComparer);

            foreach (IntervalArray<ITranscript> intervalArray in transcriptsByRef)
            {
                if (intervalArray == null) continue;

                foreach (Interval<ITranscript> interval in intervalArray.Array)
                {
                    ITranscript transcript = interval.Value;
                    if (transcript.Source != desiredSource) continue;

                    int         internalGeneId = geneToInternalId[transcript.Gene];
                    MutableGene tempGene       = GetGene(transcript.Gene, internalGeneId);
                    

                    if (!genes.TryGetValue(tempGene, out MutableGene mutableGene))
                    {
                        mutableGene = tempGene;
                        genes.Add(tempGene);
                    }

                    MutableTranscript mutableTranscript = GetTranscript(transcript, mutableGene);
                    transcripts.Add(mutableTranscript);
                }
            }

            return transcripts;
        }

        private static MutableTranscript GetTranscript(ITranscript t, MutableGene mutableGene) => new(t.Chromosome,
            t.Start, t.End, t.Id, t.Translation, t.BioType, mutableGene,
            t.IsCanonical, t.TranscriptRegions, t.Source);

        private static MutableGene GetGene(IGene g, int internalGeneId) => new(g.Chromosome, g.Start, g.End, g.OnReverseStrand, g.Symbol,
            g.EntrezGeneId, g.EnsemblId, internalGeneId);

        // ReSharper disable once ParameterTypeCanBeEnumerable.Global
        public static (int NumTranscriptsUpdated, int NumGenesUpdated) FixTranscripts(
            List<MutableTranscript> transcripts)
        {
            var numTranscriptsUpdated = 0;
            var updatedGenes          = new HashSet<int>();

            foreach (MutableTranscript transcript in transcripts)
            {
                int min = transcript.TranscriptRegions[0].Start;
                int max = transcript.TranscriptRegions[^1].End;

                var updatedTranscript = false;
                var updatedGene       = false;
                
                if (min < transcript.Start)
                {
                    transcript.Start = min;
                    if (min < transcript.Gene.Start)
                    {
                        transcript.Gene.Start = min;
                        updatedGene           = true;
                    }
                    updatedTranscript = true;
                }

                if (max > transcript.End)
                {
                    transcript.End = max;
                    if (max > transcript.Gene.End)
                    {
                        transcript.Gene.End = max;
                        updatedGene         = true;
                    }
                    updatedTranscript = true;
                }

                if (updatedTranscript) numTranscriptsUpdated++;
                if (updatedGene) updatedGenes.Add(transcript.Gene.InternalGeneId);
            }

            return (numTranscriptsUpdated, updatedGenes.Count);
        }
    }
}