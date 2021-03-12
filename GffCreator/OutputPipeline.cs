using System.Collections.Generic;
using GffCreator.GFF;
using GffCreator.Mutable;
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
            GffFields         requiredFields = GetRequiredFields(transcript);
            GeneralAttributes attributes     = GetGeneralAttributes(transcript);

            WriteGene(transcript.Gene, requiredFields, transcript.Source);
            WriteTranscript(transcript, requiredFields, attributes);

            IEnumerable<ITranscriptRegion> exons        = transcript.TranscriptRegions.GetExons();
            ICodingRegion                  codingRegion = transcript.Translation?.CodingRegion;

            foreach (ITranscriptRegion exon in exons) WriteExon(exon, requiredFields, attributes, codingRegion);
        }

        private void WriteTranscript(IInterval interval, GffFields requiredFields, GeneralAttributes attributes) =>
            _writer.WriteTranscript(interval, requiredFields, attributes);

        private void WriteGene(MutableGene gene, GffFields requiredFields, Source source)
        {
            if (_observedGenes.Contains(gene.InternalGeneId)) return;
            _observedGenes.Add(gene.InternalGeneId);
            
            _writer.WriteGene(gene, requiredFields, source);
        }

        private void WriteExon(ITranscriptRegion exon, GffFields requiredFields, GeneralAttributes attributes,
                               IInterval         codingRegion)
        {
            _writer.WriteExonicRegion(exon, requiredFields, attributes, exon.Id, "exon");
            WriteCds(codingRegion, exon, requiredFields, attributes);
            WriteUtr(codingRegion, exon, requiredFields, attributes);
        }

        private void WriteUtr(IInterval         codingRegion, ITranscriptRegion exon, GffFields requiredFields,
                              GeneralAttributes attributes)
        {
            if (!GffUtilities.HasUtr(codingRegion, exon)) return;
            if (exon.Start < codingRegion.Start) Write5PrimeUtr(codingRegion, exon, requiredFields, attributes);
            if (exon.End   > codingRegion.End) Write3PrimeUtr(codingRegion, exon, requiredFields, attributes);
        }

        private void Write5PrimeUtr(IInterval         codingRegion, ITranscriptRegion exon, GffFields requiredFields,
                                    GeneralAttributes attributes)
        {
            int utrEnd                    = codingRegion.Start - 1;
            if (utrEnd > exon.End) utrEnd = exon.End;
            _writer.WriteExonicRegion(new Interval(exon.Start, utrEnd), requiredFields, attributes, exon.Id, "UTR");
        }

        private void Write3PrimeUtr(IInterval         codingRegion, ITranscriptRegion exon, GffFields requiredFields,
                                    GeneralAttributes attributes)
        {
            int utrStart                        = codingRegion.End + 1;
            if (utrStart < exon.Start) utrStart = exon.Start;
            _writer.WriteExonicRegion(new Interval(utrStart, exon.End), requiredFields, attributes, exon.Id, "UTR");
        }

        private void WriteCds(IInterval         codingRegion, ITranscriptRegion exon, GffFields requiredFields,
                              GeneralAttributes attributes)
        {
            if (!GffUtilities.HasCds(codingRegion, exon)) return;
            IInterval cds = GffUtilities.GetCdsCoordinates(codingRegion, exon);
            _writer.WriteExonicRegion(cds, requiredFields, attributes, exon.Id, "CDS");
        }

        private static GffFields GetRequiredFields(MutableTranscript transcript)
        {
            var source = transcript.Source.ToString();
            return new GffFields(transcript.Chromosome.UcscName, source, transcript.Start, transcript.End, transcript.Gene.OnReverseStrand);
        }

        private static GeneralAttributes GetGeneralAttributes(MutableTranscript transcript)
        {
            string bioType = GetBioType(transcript.BioType);

            string geneId = transcript.Source == Source.Ensembl
                ? transcript.Gene.EnsemblId.WithVersion
                : transcript.Gene.EntrezGeneId.WithVersion;

            return new GeneralAttributes(geneId, transcript.Gene.Symbol, transcript.Id.WithVersion,
                transcript.Translation?.ProteinId?.WithVersion, bioType, transcript.IsCanonical, transcript.Gene.InternalGeneId);
        }

        private static string GetBioType(BioType bioType) => bioType == BioType.three_prime_overlapping_ncRNA
            ? "3prime_overlapping_ncRNA"
            : bioType.ToString();
    }
}