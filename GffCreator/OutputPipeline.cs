using System.Collections.Generic;
using GffCreator.GFF;
using GffCreator.Mutable;
using GffCreator.Utilities;
using Intervals;
using VariantAnnotation.Interface.AnnotatedPositions;

namespace GffCreator
{
    public sealed class OutputPipeline
    {
        private readonly GffWriter    _writer;
        private readonly HashSet<int> _observedGenes = new();

        public OutputPipeline(GffWriter writer) => _writer = writer;

        public void Create(IEnumerable<MutableTranscript> transcripts)
        {
            foreach (MutableTranscript transcript in transcripts) Write(transcript);
        }

        private void Write(MutableTranscript transcript)
        {
            (GffEntry geneEntry, GffAttribute geneAttribute) = GetGene(transcript.Gene, transcript.Source);
            GffEntry transcriptEntry = GetTranscript(transcript, geneAttribute);

            WriteGene(geneEntry, transcript.Gene.InternalGeneId);
            WriteEntry(transcriptEntry);

            IEnumerable<ITranscriptRegion> exons        = transcript.TranscriptRegions.GetExons();
            ICodingRegion                  codingRegion = transcript.Translation?.CodingRegion;

            foreach (ITranscriptRegion exon in exons) HandleExon(transcriptEntry, codingRegion, exon);
        }

        private void WriteGene(GffEntry geneEntry, int internalGeneId)
        {
            if (_observedGenes.Contains(internalGeneId)) return;
            _observedGenes.Add(internalGeneId);
            _writer.WriteEntry(geneEntry);
        }

        private void HandleExon(GffEntry transcriptEntry, IInterval codingRegion, ITranscriptRegion exon)
        {
            GffAttribute attribute = transcriptEntry.Attribute with {ExonNumber = exon.Id};
            WriteExon(exon, transcriptEntry, attribute);
            WriteCds(exon, codingRegion, transcriptEntry, attribute);
            WriteUtr(exon, codingRegion, transcriptEntry, attribute);
        }

        private void WriteUtr(IInterval exon, IInterval codingRegion, GffEntry transcript, GffAttribute attribute)
        {
            if (!GffUtilities.HasUtr(codingRegion, exon)) return;
            if (exon.Start < codingRegion.Start) Write5PrimeUtr(codingRegion, exon, transcript, attribute);
            if (exon.End   > codingRegion.End) Write3PrimeUtr(codingRegion, exon, transcript, attribute);
        }

        private void Write5PrimeUtr(IInterval codingRegion, IInterval exon, GffEntry transcript, GffAttribute attribute)
        {
            int utrEnd                    = codingRegion.Start - 1;
            if (utrEnd > exon.End) utrEnd = exon.End;

            GffEntry entry = transcript with
            {
                Feature = GffFeature.UTR,
                Start = exon.Start,
                End = utrEnd,
                Attribute = attribute
            };

            _writer.WriteEntry(entry);
        }

        private void Write3PrimeUtr(IInterval codingRegion, IInterval exon, GffEntry transcript, GffAttribute attribute)
        {
            int utrStart                        = codingRegion.End + 1;
            if (utrStart < exon.Start) utrStart = exon.Start;

            GffEntry entry = transcript with
            {
                Feature = GffFeature.UTR,
                Start = utrStart,
                End = exon.End,
                Attribute = attribute
            };

            _writer.WriteEntry(entry);
        }

        private void WriteCds(ITranscriptRegion exon, IInterval codingRegion, GffEntry transcript,
                              GffAttribute      attribute)
        {
            if (!GffUtilities.HasCds(codingRegion, exon)) return;
            IInterval cds = GffUtilities.GetCdsCoordinates(codingRegion, exon);

            GffEntry entry = transcript with
            {
                Feature = GffFeature.CDS,
                Start = cds.Start,
                End = cds.End,
                Attribute = attribute
            };

            WriteEntry(entry);
        }

        private void WriteExon(IInterval exon, GffEntry transcriptEntry, GffAttribute attribute)
        {
            GffEntry entry = transcriptEntry with
            {
                Feature = GffFeature.exon,
                Start = exon.Start,
                End = exon.End,
                Attribute = attribute
            };

            WriteEntry(entry);
        }

        private static GffEntry GetTranscript(MutableTranscript transcript, GffAttribute geneAttribute)
        {
            GffAttribute attribute = geneAttribute with
            {
                TranscriptId = transcript.Id.WithVersion,
                BioType = GetBioType(transcript.BioType),
                IsCanonical = transcript.IsCanonical,
                ProteinId = transcript.Translation?.ProteinId.WithVersion,
                EnsemblGeneId = null,
                EntrezGeneId = null
            };

            return new GffEntry(transcript.Chromosome, transcript.Source, GffFeature.transcript, transcript.Start,
                transcript.End, transcript.Gene.OnReverseStrand, attribute);
        }

        private static (GffEntry GeneEntry, GffAttribute geneAttribute) GetGene(MutableGene gene, Source source)
        {
            string geneId = source == Source.Ensembl
                ? gene.EnsemblId.WithVersion
                : gene.EntrezGeneId.WithVersion;

            var attribute = new GffAttribute(geneId, gene.EntrezGeneId.WithVersion, gene.EnsemblId.WithVersion,
                gene.Symbol, gene.InternalGeneId, null, null, null, false, null);

            var entry = new GffEntry(gene.Chromosome, source, GffFeature.gene, gene.Start, gene.End,
                gene.OnReverseStrand, attribute);

            return (entry, attribute);
        }

        private void WriteEntry(GffEntry entry) => _writer.WriteEntry(entry);

        private static string GetBioType(BioType bioType) => bioType == BioType.three_prime_overlapping_ncRNA
            ? "3prime_overlapping_ncRNA"
            : bioType.ToString();
    }
}