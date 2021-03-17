using Genome;

namespace VariantAnnotation.Interface.AnnotatedPositions
{
    public interface IAnnotatedPosition
    {
        IAnnotatedVariant[] AnnotatedVariants { get; set; }
        string              CytogeneticBand   { get; set; }
        IPosition           Position          { get; set; }
    }

    public interface IPosition
    {
        IChromosome Chromosome { get; set; }
        int         Start      { get; set; }
        int         End        { get; set; }
    }
    
    public interface IAnnotatedVariant
    {
        string Variant       { get; set; }
        string HgvsgNotation { get; set; }
    }
}