using Genome;
using IO;
using VariantAnnotation.Interface.AnnotatedPositions;

namespace GffCreator.Mutable
{
    public sealed class MutableGene : IGene
    {
        public int         Start           { get; set; }
        public int         End             { get; set; }
        public IChromosome Chromosome      { get; }
        public bool        OnReverseStrand { get; }
        public string      Symbol          { get; }
        public ICompactId  EntrezGeneId    { get; }
        public ICompactId  EnsemblId       { get; }
        public int         HgncId          { get; }

        public readonly int InternalGeneId;

        public MutableGene(IChromosome chromosome,   int        start, int end, bool onReverseStrand, string symbol,
                           ICompactId  entrezGeneId, ICompactId ensemblGeneId, int internalGeneId)
        {
            Chromosome      = chromosome;
            Start           = start;
            End             = end;
            OnReverseStrand = onReverseStrand;
            Symbol          = symbol;
            EntrezGeneId    = entrezGeneId;
            EnsemblId       = ensemblGeneId;
            InternalGeneId  = internalGeneId;
        }

        public void Write(IExtendedBinaryWriter writer) => throw new System.NotImplementedException();
    }
}