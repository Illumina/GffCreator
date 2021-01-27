using System;
using System.Collections.Generic;
using GffCreator.GFF;
using Intervals;
using VariantAnnotation.Interface.AnnotatedPositions;

namespace GffCreator
{
    public sealed class OutputPipeline
    {
        private readonly IDictionary<IGene, int> _geneToInternalId;
        private readonly GffWriter               _writer;
        private readonly HashSet<int>            _observedGenes;

        public OutputPipeline(GffWriter writer, IDictionary<IGene, int> geneToInternalId)
        {
            _writer           = writer;
            _geneToInternalId = geneToInternalId;
            _observedGenes    = new HashSet<int>();
        }

        public void Create(IEnumerable<IntervalArray<ITranscript>> transcriptIntervalArrays)
        {
            Console.Write("- writing GFF entries... ");
            foreach (var transcriptArray in transcriptIntervalArrays)
            {
                if (transcriptArray == null) continue;
                foreach (var interval in transcriptArray.Array) Write(interval.Value);
            }

            Console.WriteLine("finished.");
        }

        private void Write(ITranscript transcript)
        {
            var requiredFields = GetRequiredFields(transcript);
            var attributes     = GetGeneralAttributes(transcript);

            WriteGene(transcript.Gene, requiredFields, attributes.GeneId, attributes.InternalGeneId);
            WriteTranscript(transcript, requiredFields, attributes);

            var exons        = transcript.TranscriptRegions.GetExons();
            var codingRegion = transcript.Translation?.CodingRegion;

            foreach (var exon in exons) WriteExon(exon, requiredFields, attributes, codingRegion);
        }

        private void WriteTranscript(IInterval          interval, IRequiredFields requiredFields,
                                     IGeneralAttributes attributes) =>
            _writer.WriteTranscript(interval, requiredFields, attributes);

        private void WriteGene(IGene gene, IRequiredFields requiredFields, string geneId, int internalGeneId)
        {
            if (_observedGenes.Contains(internalGeneId)) return;

            _observedGenes.Add(internalGeneId);
            var gffGene = GetGene(gene, geneId);
            _writer.WriteGene(gffGene, requiredFields, internalGeneId);
        }

        private void WriteExon(ITranscriptRegion exon, IRequiredFields requiredFields, IGeneralAttributes attributes,
                               IInterval         codingRegion)
        {
            _writer.WriteExonicRegion(exon, requiredFields, attributes, exon.Id, "exon");
            WriteCds(codingRegion, exon, requiredFields, attributes);
            WriteUtr(codingRegion, exon, requiredFields, attributes);
        }

        private void WriteUtr(IInterval          codingRegion, ITranscriptRegion exon, IRequiredFields requiredFields,
                              IGeneralAttributes attributes)
        {
            if (!GffUtilities.HasUtr(codingRegion, exon)) return;
            if (exon.Start < codingRegion.Start) Write5PrimeUtr(codingRegion, exon, requiredFields, attributes);
            if (exon.End   > codingRegion.End) Write3PrimeUtr(codingRegion, exon, requiredFields, attributes);
        }

        private void Write5PrimeUtr(IInterval codingRegion, ITranscriptRegion exon, IRequiredFields requiredFields,
                                    IGeneralAttributes attributes)
        {
            int utrEnd                    = codingRegion.Start - 1;
            if (utrEnd > exon.End) utrEnd = exon.End;
            _writer.WriteExonicRegion(new Interval(exon.Start, utrEnd), requiredFields, attributes, exon.Id, "UTR");
        }

        private void Write3PrimeUtr(IInterval codingRegion, ITranscriptRegion exon, IRequiredFields requiredFields,
                                    IGeneralAttributes attributes)
        {
            int utrStart                        = codingRegion.End + 1;
            if (utrStart < exon.Start) utrStart = exon.Start;
            _writer.WriteExonicRegion(new Interval(utrStart, exon.End), requiredFields, attributes, exon.Id, "UTR");
        }

        private void WriteCds(IInterval          codingRegion, ITranscriptRegion exon, IRequiredFields requiredFields,
                              IGeneralAttributes attributes)
        {
            if (!GffUtilities.HasCds(codingRegion, exon)) return;
            var cds = GffUtilities.GetCdsCoordinates(codingRegion, exon);
            _writer.WriteExonicRegion(cds, requiredFields, attributes, exon.Id, "CDS");
        }

        private static IGffGene GetGene(IGene gene, string id) => new GffGene(gene.Start, gene.End, id,
            gene.EntrezGeneId.WithVersion, gene.EnsemblId.WithVersion, gene.Symbol);

        private static IRequiredFields GetRequiredFields(ITranscript transcript)
        {
            string source = transcript.Source.ToString();
            return new RequiredFields(transcript.Chromosome.UcscName, source, transcript.Gene.OnReverseStrand);
        }

        private IGeneralAttributes GetGeneralAttributes(ITranscript transcript)
        {
            string bioType        = GetBioType(transcript.BioType);
            int    internalGeneId = _geneToInternalId[transcript.Gene];
            string geneId = transcript.Source == Source.Ensembl
                ? transcript.Gene.EnsemblId.WithVersion
                : transcript.Gene.EntrezGeneId.WithVersion;

            return new GeneralAttributes(geneId, transcript.Gene.Symbol, transcript.Id.WithVersion,
                transcript.Translation?.ProteinId?.WithVersion, bioType, transcript.IsCanonical, internalGeneId);
        }

        private static string GetBioType(BioType bioType) => bioType == BioType.three_prime_overlapping_ncRNA
            ? "3prime_overlapping_ncRNA"
            : bioType.ToString();
    }
}