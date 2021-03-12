namespace GffCreator.GFF
{
    public record GeneralAttributes(string GeneId,  string GeneSymbol,  string TranscriptId, string ProteinId,
                                    string BioType, bool   IsCanonical, int    InternalGeneId);
}