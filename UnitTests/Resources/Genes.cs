using VariantAnnotation.AnnotatedPositions.Transcript;
using VariantAnnotation.Caches.DataStructures;

namespace UnitTests.Resources
{
    public static class Genes
    {
        public static readonly Gene KRAS = new Gene(Chromosomes.Chr12, 25204789, 25252093, true, "KRAS", 6407,
            CompactId.Convert("3845"), CompactId.Convert("ENSG00000133703"));

        public static readonly Gene NRAS = new Gene(Chromosomes.Chr1, 114704464, 114716894, true, "NRAS", 7989,
            CompactId.Convert("4893"), CompactId.Convert("ENSG00000213281"));

        public static readonly Gene TP53 = new Gene(Chromosomes.Chr17, 7661779, 7687550, true, "TP53", 11998,
            CompactId.Convert("7157"), CompactId.Convert("ENSG00000141510"));

        public static readonly Gene TP53b = new Gene(Chromosomes.Chr17, 7661779, 7687550, true, "TP53", 11998,
            CompactId.Convert("7157"), CompactId.Convert("ENSG00000141510"));
    }
}