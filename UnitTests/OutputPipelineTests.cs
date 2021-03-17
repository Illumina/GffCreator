using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GffCreator;
using GffCreator.GFF;
using GffCreator.Mutable;
using UnitTests.Resources;
using VariantAnnotation.AnnotatedPositions.Transcript;
using VariantAnnotation.Caches.DataStructures;
using VariantAnnotation.Interface.AnnotatedPositions;
using Xunit;

namespace UnitTests
{
    public class OutputPipelineTests
    {
        private readonly List<string> _gffLines;

        public OutputPipelineTests()
        {
            IEnumerable<MutableTranscript> transcripts = GetRefSeqTranscripts();
            _gffLines = WriteGffAndGetOutput(transcripts);
        }
        
        private static IEnumerable<MutableTranscript> GetRefSeqTranscripts()
        {
            CompactId transcriptId  = CompactId.Convert("NM_123456", 7);
            CompactId entrezGeneId  = CompactId.Convert("1234");
            CompactId ensemblGeneId = CompactId.Convert("ENSG00000123456");
            CompactId proteinId     = CompactId.Convert("NP_123456", 7);

            var gene = new MutableGene(Chromosomes.Chr17, 1001, 2100, false, "ABC", entrezGeneId, ensemblGeneId, 9);

            var codingRegion = new CodingRegion(1200, 2100, -1, -1, -1);
            var translation  = new Translation(codingRegion, proteinId, null);

            ITranscriptRegion[] transcriptRegions =
            {
                new TranscriptRegion(TranscriptRegionType.Exon,   1, 1001, 1100, 1,    100), // 5' UTR
                new TranscriptRegion(TranscriptRegionType.Intron, 1, 1101, 1125, 100,  101),
                new TranscriptRegion(TranscriptRegionType.Exon,   2, 1126, 1275, 101,  250),
                new TranscriptRegion(TranscriptRegionType.Exon,   3, 1280, 1479, 251,  450),
                new TranscriptRegion(TranscriptRegionType.Intron, 3, 1480, 1554, 450,  451),
                new TranscriptRegion(TranscriptRegionType.Exon,   4, 1555, 1804, 451,  700),
                new TranscriptRegion(TranscriptRegionType.Intron, 4, 1805, 1929, 700,  701),
                new TranscriptRegion(TranscriptRegionType.Exon,   5, 1930, 2229, 701,  1000),
                new TranscriptRegion(TranscriptRegionType.Intron, 5, 2230, 2404, 1000, 1001),
                new TranscriptRegion(TranscriptRegionType.Exon,   6, 2405, 2804, 1001, 1400) // 3' UTR
            };

            var transcript = new MutableTranscript(gene.Chromosome, 1001, 2804, transcriptId, translation,
                BioType.protein_coding, gene, true, transcriptRegions, Source.RefSeq);

            return new[] {transcript};
        }

        [Fact]
        public void GFF_SeqName_ExpectedResults()
        {
            string[] actual = GetColumn(_gffLines, 0);
            foreach (string entry in actual) Assert.Equal("chr17", entry);
        }

        [Fact]
        public void GFF_Source_ExpectedResults()
        {
            string[] actual = GetColumn(_gffLines, 1);
            foreach (string entry in actual) Assert.Equal("RefSeq", entry);
        }

        [Fact]
        public void GFF_Feature_ExpectedResults()
        {
            string[] expected =
            {
                "gene", "transcript", "exon", "UTR", "exon", "CDS", "UTR", "exon", "CDS", "exon", "CDS", "exon", "CDS",
                "UTR", "exon", "UTR"
            };
            string[] actual = GetColumn(_gffLines, 2);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GFF_Start_ExpectedResults()
        {
            string[] expected =
            {
                "1001", "1001", "1001", "1001", "1126", "1200", "1126", "1280", "1280", "1555", "1555", "1930", "1930",
                "2101", "2405", "2405"
            };
            string[] actual = GetColumn(_gffLines, 3);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GFF_End_ExpectedResults()
        {
            string[] expected =
            {
                "2100", "2804", "1100", "1100", "1275", "1275", "1199", "1479", "1479", "1804", "1804", "2229", "2100",
                "2229", "2804", "2804"
            };
            string[] actual = GetColumn(_gffLines, 4);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GFF_Strand_ExpectedResults()
        {
            string[] actual = GetColumn(_gffLines, 6);
            foreach (string entry in actual) Assert.Equal("+", entry);
        }

        [Fact]
        public void Gene_Attributes_ExpectedResults()
        {
            Dictionary<string, string> attributes = GetAttributes(_gffLines[0]);

            Assert.Equal(5,                 attributes.Count);
            Assert.Equal("1234",            attributes["gene_id"]);
            Assert.Equal("1234",            attributes["entrez_gene_id"]);
            Assert.Equal("ENSG00000123456", attributes["ensembl_gene_id"]);
            Assert.Equal("ABC",            attributes["gene_name"]);
            Assert.Equal("9",               attributes["internal_gene_id"]);
        }

        [Fact]
        public void Transcript_Attributes_ExpectedResults()
        {
            Dictionary<string, string> attributes = GetAttributes(_gffLines[1]);

            Assert.Equal(7,                attributes.Count);
            Assert.Equal("1234",           attributes["gene_id"]);
            Assert.Equal("ABC",           attributes["gene_name"]);
            Assert.Equal("NM_123456.7",    attributes["transcript_id"]);
            Assert.Equal("protein_coding", attributes["transcript_type"]);
            Assert.Equal("canonical",      attributes["tag"]);
            Assert.Equal("NP_123456.7",    attributes["protein_id"]);
            Assert.Equal("9",              attributes["internal_gene_id"]);
        }

        [Fact]
        public void UTR_Attributes_ExpectedResults()
        {
            Dictionary<string, string> attributes = GetAttributes(_gffLines[3]);
            CheckExonicAttributes(attributes, "1");

            attributes = GetAttributes(_gffLines[6]);
            CheckExonicAttributes(attributes, "2");

            attributes = GetAttributes(_gffLines[13]);
            CheckExonicAttributes(attributes, "5");
            
            attributes = GetAttributes(_gffLines[15]);
            CheckExonicAttributes(attributes, "6");
        }

        private static void CheckExonicAttributes(Dictionary<string, string> attributes, string expectedExonNumber)
        {
            Assert.Equal(8,                  attributes.Count);
            Assert.Equal("1234",             attributes["gene_id"]);
            Assert.Equal("ABC",              attributes["gene_name"]);
            Assert.Equal("NM_123456.7",      attributes["transcript_id"]);
            Assert.Equal("protein_coding",   attributes["transcript_type"]);
            Assert.Equal("canonical",        attributes["tag"]);
            Assert.Equal("NP_123456.7",      attributes["protein_id"]);
            Assert.Equal(expectedExonNumber, attributes["exon_number"]);
            Assert.Equal("9",                attributes["internal_gene_id"]);
        }

        [Fact]
        public void CDS_Attributes_ExpectedResults()
        {
            Dictionary<string, string> attributes = GetAttributes(_gffLines[5]);
            CheckExonicAttributes(attributes, "2");

            attributes = GetAttributes(_gffLines[8]);
            CheckExonicAttributes(attributes, "3");
            
            attributes = GetAttributes(_gffLines[10]);
            CheckExonicAttributes(attributes, "4");
            
            attributes = GetAttributes(_gffLines[12]);
            CheckExonicAttributes(attributes, "5");
        }

        [Fact]
        public void Exon_Attributes_ExpectedResults()
        {
            Dictionary<string, string> attributes = GetAttributes(_gffLines[2]);
            CheckExonicAttributes(attributes, "1");

            attributes = GetAttributes(_gffLines[4]);
            CheckExonicAttributes(attributes, "2");

            attributes = GetAttributes(_gffLines[7]);
            CheckExonicAttributes(attributes, "3");

            attributes = GetAttributes(_gffLines[9]);
            CheckExonicAttributes(attributes, "4");

            attributes = GetAttributes(_gffLines[11]);
            CheckExonicAttributes(attributes, "5");

            attributes = GetAttributes(_gffLines[14]);
            CheckExonicAttributes(attributes, "6");
        }

        private static List<string> WriteGffAndGetOutput(IEnumerable<MutableTranscript> transcripts)
        {
            using var ms = new MemoryStream();
            using (var writer = new GffWriter(new StreamWriter(ms, Encoding.UTF8, 1024, true)))
            {
                var output = new OutputPipeline(writer);
                output.Create(transcripts);
            }

            ms.Position = 0;
            var gffLines = new List<string>();

            using (var reader = new StreamReader(ms))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    gffLines.Add(line);
                }
            }

            return gffLines;
        }

        private static string[] GetColumn(IEnumerable<string> gffLines, int colIndex) =>
            gffLines.Select(line => line.Split('\t')[colIndex]).ToArray();

        private static Dictionary<string, string> GetAttributes(string gffLine)
        {
            var      attribDictionary = new Dictionary<string, string>();
            string   attribCol        = gffLine.Split('\t')[8];
            string[] entries          = attribCol.Substring(0, attribCol.Length - 2).Split("; ");

            foreach (string entry in entries)
            {
                string[] cols  = entry.Split(' ');
                string   key   = cols[0];
                string   value = cols[1].Trim('"');
                attribDictionary[key] = value;
            }

            return attribDictionary;
        }
    }
}