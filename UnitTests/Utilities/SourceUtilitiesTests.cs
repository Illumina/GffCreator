using GffCreator.Utilities;
using VariantAnnotation.Interface.AnnotatedPositions;
using Xunit;

namespace UnitTests.Utilities
{
    public class SourceUtilitiesTests
    {
        [Theory]
        [InlineData("refseq",  Source.RefSeq)]
        [InlineData("Refseq",  Source.RefSeq)]
        [InlineData("refseQ",  Source.RefSeq)]
        [InlineData("ensembl", Source.Ensembl)]
        [InlineData("Ensembl", Source.Ensembl)]
        [InlineData("ensembL", Source.Ensembl)]
        [InlineData("bob",     Source.None)]
        public void GetSource_ExpectedResults(string s, Source expectedResult) =>
            Assert.Equal(expectedResult, SourceUtilities.GetSource(s));
    }
}