using Intervals;

namespace GffCreator.GFF
{
    public record GffFields(string UcscName, string Source, int Start, int End, bool OnReverseStrand) : IInterval;
}