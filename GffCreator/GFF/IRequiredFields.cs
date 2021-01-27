namespace GffCreator.GFF
{
    public interface IRequiredFields
    {
        string UcscName { get; }
        string Source { get; }
        bool OnReverseStrand { get; }
    }
}
