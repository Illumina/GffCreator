// ReSharper disable InconsistentNaming
namespace GffCreator.GFF
{
    public enum GffFeature : byte
    {
        gene       = 0,
        transcript = 1,
        exon       = 2,
        CDS        = 3,
        UTR        = 4
    }
}