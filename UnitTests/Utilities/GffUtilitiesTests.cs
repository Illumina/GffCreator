using System.Collections.Generic;
using System.Linq;
using GffCreator;
using GffCreator.Utilities;
using Intervals;
using VariantAnnotation.Caches.DataStructures;
using VariantAnnotation.Interface.AnnotatedPositions;
using Xunit;

namespace UnitTests.Utilities
{
    public class GffUtilitiesTests
    {
        [Fact]
        public void HasCds_ExpectedResults()
        {
            ICodingRegion codingRegion = new CodingRegion(7669609, 7676594, 203, 1384, 1182);

            Assert.True(GffUtilities.HasCds(codingRegion,
                new TranscriptRegion(TranscriptRegionType.Exon, 0, 7669700, 7669800, -1, -1)));

            Assert.False(GffUtilities.HasCds(codingRegion,
                new TranscriptRegion(TranscriptRegionType.Exon, 0, 7669500, 7669600, -1, -1)));

            Assert.False(GffUtilities.HasCds(null,
                new TranscriptRegion(TranscriptRegionType.Exon, 0, 7669700, 7669800, -1, -1)));

            Assert.False(GffUtilities.HasCds(new CodingRegion(-1, 7676594, 203, 1384, 1182),
                new TranscriptRegion(TranscriptRegionType.Exon, 0, 7669700, 7669800, -1, -1)));

            Assert.False(GffUtilities.HasCds(new CodingRegion(7669609, -1, 203, 1384, 1182),
                new TranscriptRegion(TranscriptRegionType.Exon, 0, 7669700, 7669800, -1, -1)));
        }

        [Fact]
        public void GetCdsCoordinates_ExpectedResults()
        {
            IInterval codingRegion = new Interval(150, 2000);

            ITranscriptRegion firstExon  = new TranscriptRegion(TranscriptRegionType.Exon, 0, 100,  300,  -1, -1);
            ITranscriptRegion middleExon = new TranscriptRegion(TranscriptRegionType.Exon, 1, 500,  700,  -1, -1);
            ITranscriptRegion lastExon   = new TranscriptRegion(TranscriptRegionType.Exon, 2, 1900, 2100, -1, -1);

            IInterval firstCds  = GffUtilities.GetCdsCoordinates(codingRegion, firstExon);
            IInterval middleCds = GffUtilities.GetCdsCoordinates(codingRegion, middleExon);
            IInterval lastCds   = GffUtilities.GetCdsCoordinates(codingRegion, lastExon);

            Assert.Equal(new Interval(150,  300),  firstCds);
            Assert.Equal(new Interval(500,  700),  middleCds);
            Assert.Equal(new Interval(1900, 2000), lastCds);
        }

        [Fact]
        public void HasUtr_ExpectedResults()
        {
            ICodingRegion codingRegion = new CodingRegion(1000, 2000, -1, -1, -1);

            IInterval before            = new Interval(800,  900);
            IInterval fivePrimePartial  = new Interval(950,  1100);
            IInterval interior          = new Interval(1300, 1500);
            IInterval threePrimeNone    = new Interval(1900, 2000);
            IInterval threePrimePartial = new Interval(1900, 2100);
            IInterval after             = new Interval(2200, 2400);

            Assert.True(GffUtilities.HasUtr(codingRegion,  before));
            Assert.True(GffUtilities.HasUtr(codingRegion,  fivePrimePartial));
            Assert.False(GffUtilities.HasUtr(codingRegion, interior));
            Assert.False(GffUtilities.HasUtr(codingRegion, threePrimeNone));
            Assert.True(GffUtilities.HasUtr(codingRegion,  threePrimePartial));
            Assert.True(GffUtilities.HasUtr(codingRegion,  after));

            Assert.False(GffUtilities.HasUtr(null,                                     before));
            Assert.False(GffUtilities.HasUtr(new CodingRegion(-1,   2000, -1, -1, -1), before));
            Assert.False(GffUtilities.HasUtr(new CodingRegion(1000, -1,   -1, -1, -1), before));
        }

        [Fact]
        public void GetExons_Forward_ExpectedResults()
        {
            ITranscriptRegion[] transcriptRegions =
            {
                new TranscriptRegion(TranscriptRegionType.Exon,   0, 1001, 1200, 1,   200),
                new TranscriptRegion(TranscriptRegionType.Exon,   0, 1205, 1304, 201, 300),
                new TranscriptRegion(TranscriptRegionType.Intron, 0, 1305, 1403, 300, 301),
                new TranscriptRegion(TranscriptRegionType.Exon,   1, 1404, 1703, 301, 600),
                new TranscriptRegion(TranscriptRegionType.Intron, 1, 1704, 1766, 600, 601),
                new TranscriptRegion(TranscriptRegionType.Exon,   2, 1767, 2166, 601, 1000)
            };

            List<ITranscriptRegion> exons = transcriptRegions.GetExons().ToList();
            Assert.Equal(3, exons.Count);

            ITranscriptRegion firstExon = exons[0];
            Assert.Equal(1001, firstExon.Start);
            Assert.Equal(1304, firstExon.End);
            Assert.Equal(1,    firstExon.CdnaStart);
            Assert.Equal(300,  firstExon.CdnaEnd);
        }

        [Fact]
        public void GetExons_OneExon_ExpectedResults()
        {
            ITranscriptRegion[] transcriptRegions =
            {
                new TranscriptRegion(TranscriptRegionType.Exon, 0, 1001, 1200, 1, 200)
            };

            List<ITranscriptRegion> exons = transcriptRegions.GetExons().ToList();
            Assert.Single(exons);

            ITranscriptRegion firstExon = exons[0];
            Assert.Equal(1001, firstExon.Start);
            Assert.Equal(1200, firstExon.End);
            Assert.Equal(1,    firstExon.CdnaStart);
            Assert.Equal(200,  firstExon.CdnaEnd);
        }
    }
}