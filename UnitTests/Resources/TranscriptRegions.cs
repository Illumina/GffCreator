using VariantAnnotation.Caches.DataStructures;
using VariantAnnotation.Interface.AnnotatedPositions;

namespace UnitTests.Resources
{
    public static class TranscriptRegions
    {
        public static readonly TranscriptRegion NM_000546_0 = new(TranscriptRegionType.Exon, 11, 7668402, 7669690, 1303,
            2591);

        public static readonly TranscriptRegion NM_000546_1 = new(TranscriptRegionType.Intron, 10, 7669691, 7670608,
            1302, 1303);

        public static readonly TranscriptRegion NM_000546_18 = new(TranscriptRegionType.Exon, 2, 7579839, 7579940,
            175, 276);

        public static readonly TranscriptRegion NM_000546_20 = new(TranscriptRegionType.Exon, 1, 7590695, 7590868,
            1, 174);

        public static readonly TranscriptRegion ENST00000610292_0 =
            new(TranscriptRegionType.Exon, 10, 7668402, 7669690, 1351, 2639);

        public static readonly TranscriptRegion ENST00000610292_1 =
            new(TranscriptRegionType.Intron, 9, 7669691, 7670608, 1350, 1351);

        public static readonly TranscriptRegion NM_002524_0 = new(TranscriptRegionType.Exon, 7, 114704464, 114708050,
            868, 4454);

        public static readonly TranscriptRegion NM_002524_1 = new(TranscriptRegionType.Intron, 6, 114708051, 114708153,
            867, 868);

        public static readonly TranscriptRegion NM_033360_0 = new(TranscriptRegionType.Exon, 6, 25204789, 25209911, 767,
            5889);

        public static readonly TranscriptRegion NM_033360_1 = new(TranscriptRegionType.Intron, 5, 25209912, 25215436,
            766, 767);

        public static readonly ITranscriptRegion[] NM_000546 = {NM_000546_0, NM_000546_1, NM_000546_18, NM_000546_20};
        public static readonly ITranscriptRegion[] ENST00000610292 = {ENST00000610292_0, ENST00000610292_1};
        public static readonly ITranscriptRegion[] NM_002524 = {NM_002524_0, NM_002524_1};
        public static readonly ITranscriptRegion[] NM_033360 = {NM_033360_0, NM_033360_1};
    }
}