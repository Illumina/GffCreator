using System.Collections.Generic;
using System.IO;
using GffCreator;
using GffCreator.Utilities;
using UnitTests.Resources;
using VariantAnnotation.Interface.AnnotatedPositions;
using Xunit;

namespace UnitTests.Utilities
{
    public class GeneUtilitiesTests
    {
        [Fact]
        public void CreateDictionary_ExpectedResults()
        {
            IGene[]                genes            = {Genes.TP53, Genes.KRAS, Genes.NRAS};
            Dictionary<IGene, int> geneToInternalId = GeneUtilities.CreateDictionary(genes);

            Assert.Equal(3, geneToInternalId.Count);
            Assert.Equal(0, geneToInternalId[Genes.TP53]);
            Assert.Equal(1, geneToInternalId[Genes.KRAS]);
            Assert.Equal(2, geneToInternalId[Genes.NRAS]);
        }

        [Fact]
        public void CreateDictionary_DuplicateGene_ThrowInvalidDataException()
        {
            IGene[] genes = {Genes.TP53, Genes.KRAS, Genes.TP53};

            Assert.Throws<InvalidDataException>(delegate { GeneUtilities.CreateDictionary(genes); });
        }
    }
}