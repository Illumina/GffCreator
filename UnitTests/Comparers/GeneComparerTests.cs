using GffCreator;
using GffCreator.Comparers;
using UnitTests.Resources;
using Xunit;

namespace UnitTests.Comparers
{
    public class GeneComparerTests
    {
        [Fact]
        public void Equals_Null_ReturnFalse()
        {
            var comparer = new GeneComparer();
            Assert.True(comparer.Equals(Genes.TP53,  Genes.TP53));
            Assert.False(comparer.Equals(Genes.TP53, null));
            Assert.False(comparer.Equals(null,       Genes.TP53));
            Assert.False(comparer.Equals(null,       null));
        }
    }
}