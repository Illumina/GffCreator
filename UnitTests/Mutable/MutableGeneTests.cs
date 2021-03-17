using System;
using System.IO;
using GffCreator.Mutable;
using IO;
using UnitTests.Resources;
using VariantAnnotation.Caches.DataStructures;
using Xunit;

namespace UnitTests.Mutable
{
    public class MutableGeneTests
    {
        [Fact]
        public void Write_ThrowNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(delegate
            {
                using var ms     = new MemoryStream();
                using var writer = new ExtendedBinaryWriter(ms);

                Gene kras = Genes.KRAS;
                var gene = new MutableGene(kras.Chromosome, kras.Start, kras.End, kras.OnReverseStrand, kras.Symbol,
                    kras.EntrezGeneId, kras.EnsemblId, 0);
                gene.Write(writer);
            });
        }
    }
}