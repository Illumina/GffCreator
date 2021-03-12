using Genome;
using Intervals;
using VariantAnnotation.Interface.AnnotatedPositions;

namespace GffCreator.Mutable
{
    public sealed class MutableTranscript : IInterval
    {
        public int                 Start             { get; set; }
        public int                 End               { get; set; }
        public IChromosome         Chromosome        { get; }
        public ICompactId          Id                { get; }
        public BioType             BioType           { get; }
        public bool                IsCanonical       { get; }
        public Source              Source            { get; }
        public MutableGene         Gene              { get; }
        public ITranscriptRegion[] TranscriptRegions { get; }
        public ITranslation        Translation       { get; }

        public MutableTranscript(IChromosome chromosome, int start, int end, ICompactId id, ITranslation translation,
                                 BioType bioType, MutableGene gene, bool isCanonical,
                                 ITranscriptRegion[] transcriptRegions, Source source)
        {
            Chromosome        = chromosome;
            Start             = start;
            End               = end;
            Id                = id;
            Translation       = translation;
            BioType           = bioType;
            Gene              = gene;
            IsCanonical       = isCanonical;
            TranscriptRegions = transcriptRegions;
            Source            = source;
        }
    }
}