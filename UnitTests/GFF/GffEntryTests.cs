using Genome;
using GffCreator.GFF;
using UnitTests.Resources;
using VariantAnnotation.Interface.AnnotatedPositions;
using Xunit;

namespace UnitTests.GFF
{
    public class GffEntryTests
    {
        [Fact]
        public void Fields_init()
        {
            IChromosome  chromosome = Chromosomes.Chr17;
            const Source source     = Source.RefSeq;

            var entry = new GffEntry(Chromosomes.Chr12, Source.None, GffFeature.UTR, -1, -1, false, null);

            GffEntry newEntry = entry with
            {
                Chromosome = chromosome,
                Source = source,
                OnReverseStrand = true
            };

            Assert.Equal(chromosome, newEntry.Chromosome);
            Assert.Equal(source,     newEntry.Source);
            Assert.True(newEntry.OnReverseStrand);
        }
    }
}