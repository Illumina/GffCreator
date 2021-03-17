using System.Collections.Generic;
using System.IO;
using GffCreator.Comparers;
using VariantAnnotation.Interface.AnnotatedPositions;

namespace GffCreator.Utilities
{
    public static class GeneUtilities
    {
        public static Dictionary<IGene, int> CreateDictionary(IGene[] genes)
        {
            var geneComparer     = new GeneComparer();
            var geneToInternalId = new Dictionary<IGene, int>(geneComparer);

            for (var geneIndex = 0; geneIndex < genes.Length; geneIndex++)
            {
                IGene gene = genes[geneIndex];

                if (geneToInternalId.TryGetValue(gene, out int oldGeneIndex))
                {
                    throw new InvalidDataException(
                        $"Found a duplicate gene in the dictionary: {genes[geneIndex]} ({geneIndex} vs {oldGeneIndex})");
                }

                geneToInternalId[gene] = geneIndex;
            }

            return geneToInternalId;
        }
    }
}