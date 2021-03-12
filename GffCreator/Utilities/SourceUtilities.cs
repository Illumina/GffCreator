using VariantAnnotation.Interface.AnnotatedPositions;

namespace GffCreator.Utilities
{
    public static class SourceUtilities
    {
        public static Source GetSource(string source)
        {
            source = source.ToLower();
            if (source.StartsWith("ensembl")) return Source.Ensembl;
            if (source.StartsWith("refseq")) return Source.RefSeq;
            return source.StartsWith("both") ? Source.BothRefSeqAndEnsembl : Source.None;
        }
    }
}