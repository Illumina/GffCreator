using Genome;
using VariantAnnotation.Interface.AnnotatedPositions;

namespace GffCreator.GFF
{
    public record GffEntry(IChromosome Chromosome,      Source       Source, GffFeature Feature, int Start, int End,
                           bool        OnReverseStrand, GffAttribute Attribute)
    {
        public override string ToString()
        {
            char strand = OnReverseStrand ? '-' : '+';
            return $"{Chromosome.UcscName}\t{Source}\t{Feature}\t{Start}\t{End}\t.\t{strand}\t.\t{Attribute}";
        }
    }
}