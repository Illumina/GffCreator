using VariantAnnotation.Interface.AnnotatedPositions;

namespace GffCreator.Utilities
{
    public static class SourceUtilities
    {
        public static Source GetSource(string source)
        {
            source = source.ToLower();
            if (source.StartsWith("ensembl")) return Source.Ensembl;
            return source.StartsWith("refseq") ? Source.RefSeq : Source.None;
        }
    }
}