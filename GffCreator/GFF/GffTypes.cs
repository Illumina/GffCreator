using Intervals;

namespace GffCreator.GFF
{
    public record GffTranscript(string UcscName, string Source, int Start, int End, bool OnReverseStrand) : IInterval;
}