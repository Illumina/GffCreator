using Intervals;

namespace GffCreator.GFF
{
    public interface IGffGene : IInterval
    {
        string Id { get; }
        string EntrezGeneId { get; }
        string EnsemblGeneId { get; }
        string Symbol { get; }
    }
}
