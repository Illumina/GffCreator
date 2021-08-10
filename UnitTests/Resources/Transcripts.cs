using VariantAnnotation.AnnotatedPositions.Transcript;
using VariantAnnotation.Caches.DataStructures;
using VariantAnnotation.Interface.AnnotatedPositions;

// ReSharper disable InconsistentNaming
namespace UnitTests.Resources
{
    public static class Transcripts
    {
        // TP53
        public static readonly ITranscript NM_000546 = new Transcript(Genes.TP53.Chromosome, 7668402, 7687550,
            CompactId.Convert("NM_000546", 5), Translations.NM_000546, BioType.protein_coding, Genes.TP53, 2591, 0,
            true, TranscriptRegions.NM_000546, 11, null, 0, 0, Source.RefSeq, false, false, null);
        
        // ENST00000610292.4
        public static readonly ITranscript ENST00000610292 = new Transcript(Genes.TP53.Chromosome, 7668402, 7687481,
            CompactId.Convert("ENST00000610292", 4), Translations.ENST00000610292, BioType.protein_coding, Genes.TP53,
            2591, 0,
            true, TranscriptRegions.ENST00000610292, 11, null, 0, 0, Source.Ensembl, false, false, null);

        // NRAS
        public static readonly ITranscript NM_002524 = new Transcript(Genes.NRAS.Chromosome, 114704464, 114716894,
            CompactId.Convert("NM_002524", 4), Translations.NM_002524, BioType.protein_coding, Genes.NRAS, 4454, 0,
            true, TranscriptRegions.NM_002524, 7, null, 0, 0, Source.RefSeq, false, false, null);
        
        // KRAS
        public static readonly ITranscript NM_033360 = new Transcript(Genes.KRAS.Chromosome, 25204789, 25250931,
            CompactId.Convert("NM_033360", 3), Translations.NM_033360, BioType.protein_coding, Genes.KRAS, 5889, 0,
            true, TranscriptRegions.NM_033360, 6, null, 0, 0, Source.RefSeq, false, false, null);
    }
}