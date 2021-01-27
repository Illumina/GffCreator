using Genome;
using VariantAnnotation.Interface.AnnotatedPositions;
using VariantAnnotation.Interface.Providers;

namespace VariantAnnotation.Caches.Utilities
{
    public static class TranscriptValidator
    {
        public static void Validate(ISequenceProvider sequenceProvider, IChromosome chromosome, string transcriptId, bool geneOnReverseStrand, ITranscriptRegion[] transcriptRegions, IRnaEdit[] rnaEdits, ITranslation translation)
        {
        }
    }
}