using System.Collections.Generic;
using GffCreator.Mutable;
using GffCreator.Utilities;
using Intervals;
using UnitTests.Resources;
using VariantAnnotation.AnnotatedPositions.Transcript;
using VariantAnnotation.Caches.DataStructures;
using VariantAnnotation.Interface.AnnotatedPositions;
using Xunit;

namespace UnitTests.Utilities
{
    public class CacheUtilitiesTests
    {
        private static readonly IntervalArray<ITranscript>[] TranscriptsByRef = GetTranscriptsByRef();
        private static readonly Dictionary<IGene, int>       GeneToInternalId = GetGeneToInternalId();

        private static IntervalArray<ITranscript>[] GetTranscriptsByRef()
        {
            var transcriptsByRef = new IntervalArray<ITranscript>[Chromosomes.NumRefSeqs];

            // chr1
            var chr1Intervals = new Interval<ITranscript>[]
            {
                new(Transcripts.NM_002524.Start, Transcripts.NM_002524.End, Transcripts.NM_002524)
            };

            transcriptsByRef[0] = new IntervalArray<ITranscript>(chr1Intervals);

            // chr12
            var chr12Intervals = new Interval<ITranscript>[]
            {
                new(Transcripts.NM_033360.Start, Transcripts.NM_033360.End, Transcripts.NM_033360)
            };

            transcriptsByRef[11] = new IntervalArray<ITranscript>(chr12Intervals);

            // chr17
            var chr17Intervals = new Interval<ITranscript>[]
            {
                new(Transcripts.NM_000546.Start, Transcripts.NM_000546.End, Transcripts.NM_000546),
                new(Transcripts.ENST00000610292.Start, Transcripts.ENST00000610292.End, Transcripts.ENST00000610292)
            };

            transcriptsByRef[16] = new IntervalArray<ITranscript>(chr17Intervals);

            return transcriptsByRef;
        }

        private static Dictionary<IGene, int> GetGeneToInternalId() =>
            new() {[Genes.NRAS] = 0, [Genes.KRAS] = 1, [Genes.TP53] = 2};

        [Fact]
        public void FilterBySource_RefSeq_ExpectedResults()
        {
            const Source expectedSource = Source.RefSeq;
            List<MutableTranscript> actualTranscripts =
                CacheUtilities.FilterBySource(TranscriptsByRef, expectedSource, GeneToInternalId);

            Assert.Equal(3, actualTranscripts.Count);
            foreach (MutableTranscript transcript in actualTranscripts) Assert.Equal(expectedSource, transcript.Source);
        }

        [Fact]
        public void FilterBySource_Ensembl_ExpectedResults()
        {
            const Source expectedSource = Source.Ensembl;
            List<MutableTranscript> actualTranscripts =
                CacheUtilities.FilterBySource(TranscriptsByRef, expectedSource, GeneToInternalId);

            Assert.Single(actualTranscripts);
            foreach (MutableTranscript transcript in actualTranscripts) Assert.Equal(expectedSource, transcript.Source);
        }

        [Fact]
        public void FixTranscripts_ExpectedResults()
        {
            List<MutableTranscript> transcripts = GetMutableTranscripts();
            (int numTranscriptsUpdated, int numGenesUpdated) = CacheUtilities.FixTranscripts(transcripts);

            Assert.Equal(2, numTranscriptsUpdated);
            Assert.Equal(1, numGenesUpdated);

            MutableTranscript transcriptB  = transcripts[1];
            Assert.Equal(295, transcriptB.Start);
            Assert.Equal(500, transcriptB.End);
            Assert.Equal(295, transcriptB.Gene.Start);
            Assert.Equal(505, transcriptB.Gene.End);
            
            MutableTranscript transcriptB2 = transcripts[2];
            Assert.Equal(300, transcriptB2.Start);
            Assert.Equal(505, transcriptB2.End);
            Assert.Equal(295, transcriptB2.Gene.Start);
            Assert.Equal(505, transcriptB2.Gene.End);
        }

        private static List<MutableTranscript> GetMutableTranscripts()
        {
            MutableGene geneA = new(Chromosomes.Chr1, 100, 200, false, "A", CompactId.Empty, CompactId.Empty, 0);
            MutableGene geneB = new(Chromosomes.Chr1, 300, 500, false, "B", CompactId.Empty, CompactId.Empty, 1);
            MutableGene geneC = new(Chromosomes.Chr1, 700, 900, false, "C", CompactId.Empty, CompactId.Empty, 2);

            var transcriptRegionsA = new ITranscriptRegion[]
                {new TranscriptRegion(TranscriptRegionType.Exon, 0, geneA.Start, geneA.End, 1, 299)};
            
            var transcriptRegionsB = new ITranscriptRegion[]
                {new TranscriptRegion(TranscriptRegionType.Exon, 0, 295, geneB.End, 1, 299)};
            
            var transcriptRegionsB2 = new ITranscriptRegion[]
                {new TranscriptRegion(TranscriptRegionType.Exon, 0, geneB.Start, 505, 1, 299)};
            
            var transcriptRegionsC = new ITranscriptRegion[]
                {new TranscriptRegion(TranscriptRegionType.Exon, 0, geneC.Start, geneC.End, 1, 299)};

            MutableTranscript transcriptA = new(Chromosomes.Chr1, geneA.Start, geneA.End, CompactId.Empty, null,
                BioType.protein_coding, geneA, false, transcriptRegionsA, Source.RefSeq);

            MutableTranscript transcriptB = new(Chromosomes.Chr1, geneB.Start, geneB.End, CompactId.Empty, null,
                BioType.protein_coding, geneB, false, transcriptRegionsB, Source.RefSeq);

            MutableTranscript transcriptB2 = new(Chromosomes.Chr1, geneB.Start, geneB.End, CompactId.Empty, null,
                BioType.protein_coding, geneB, false, transcriptRegionsB2, Source.RefSeq);

            MutableTranscript transcriptC = new(Chromosomes.Chr1, geneC.Start, geneC.End, CompactId.Empty, null,
                BioType.protein_coding, geneC, false, transcriptRegionsC, Source.RefSeq);

            return new List<MutableTranscript> {transcriptA, transcriptB, transcriptB2, transcriptC};
        }
    }
}