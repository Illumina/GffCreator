using GffCreator.GFF;
using Xunit;

namespace UnitTests.GFF
{
    public class GffAttributeTests
    {
        [Fact]
        public void Fields_init()
        {
            const string entrezGeneId   = "7157";
            const string ensemblGeneId  = "ENSG00000141510";
            const string geneId         = entrezGeneId;
            const int    internalGeneId = 9;
            const string geneSymbol     = "TP53";

            var attribute = new GffAttribute(null, null, null, null, null, null, null, null, false, null);

            GffAttribute newAttribute = attribute with
            {
                GeneId = geneId,
                GeneSymbol = geneSymbol,
                EnsemblGeneId = ensemblGeneId,
                EntrezGeneId = entrezGeneId,
                InternalGeneId = internalGeneId
            };

            Assert.Equal(entrezGeneId,   newAttribute.EntrezGeneId);
            Assert.Equal(ensemblGeneId,  newAttribute.EnsemblGeneId);
            Assert.Equal(internalGeneId, newAttribute.InternalGeneId);
        }
    }
}