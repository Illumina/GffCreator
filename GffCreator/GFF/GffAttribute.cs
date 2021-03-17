using System.Text;

namespace GffCreator.GFF
{
    public record GffAttribute(string GeneId,         string EntrezGeneId, string EnsemblGeneId, string GeneSymbol,
                               int?   InternalGeneId, string TranscriptId, string ProteinId,     string BioType,
                               bool   IsCanonical,    int?   ExonNumber)
    {
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(GeneId)) sb.Append($"gene_id \"{GeneId}\"; ");
            if (!string.IsNullOrEmpty(EntrezGeneId)) sb.Append($"entrez_gene_id \"{EntrezGeneId}\"; ");
            if (!string.IsNullOrEmpty(EnsemblGeneId)) sb.Append($"ensembl_gene_id \"{EnsemblGeneId}\"; ");
            if (!string.IsNullOrEmpty(GeneSymbol)) sb.Append($"gene_name \"{GeneSymbol}\"; ");
            if (!string.IsNullOrEmpty(TranscriptId)) sb.Append($"transcript_id \"{TranscriptId}\"; ");

            if (!string.IsNullOrEmpty(BioType)) sb.Append($"transcript_type \"{BioType}\"; ");
            if (IsCanonical) sb.Append("tag \"canonical\"; ");

            if (!string.IsNullOrEmpty(ProteinId)) sb.Append($"protein_id \"{ProteinId}\"; ");
            if (ExonNumber     != null) sb.Append($"exon_number {ExonNumber}; ");
            if (InternalGeneId != null) sb.Append($"internal_gene_id \"{InternalGeneId}\"; ");

            return sb.ToString();
        }
    }
}