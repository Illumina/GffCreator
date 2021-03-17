using Genome;

namespace UnitTests.Resources
{
    public static class Chromosomes
    {
        public const int NumRefSeqs = 192;

        public static readonly IChromosome Chr1  = new Chromosome("chr1",  "1",  "", "", 1, 0);
        public static readonly IChromosome Chr12 = new Chromosome("chr12", "12", "", "", 1, 11);
        public static readonly IChromosome Chr17 = new Chromosome("chr17", "17", "", "", 1, 16);
    }
}