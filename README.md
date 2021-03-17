# GffCreator

GffCreator is a standalone tool that creates a GTF (GFF v2) output file from a Nirvana transcript cache file.

## Building

Remember to clone the git repo using the recursive option - this will automatically handle the Nirvana submodule:

```
git clone --recursive https://github.com/Illumina/GffCreator.git
```

After that, build the solution using the [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0):

```
cd GffCreator
dotnet build -c Release
```

## Running GffCreator

Using the paths to your Nirvana 3.2.3 cache and reference files, use the following command to create the GFF file:

```bash
USAGE: GffCreator.dll <transcript source> <transcript cache path> <reference path> <output GFF path>

dotnet bin/Release/net5.0/GffCreator.dll \
	RefSeq \
	Cache/26/GRCh37/RefSeq.transcripts.ndb \
	References/6/Homo_sapiens.GRCh37.Nirvana.dat \
	GRCh37_RefSeq_26.gff.gz
```

When this command is run, the following output is displayed:

```bash
- loading reference sequence... finished.
- loading cache... finished.
- filter by transcript source... 101,270 remaining.
- updating transcript & gene coordinates... 1 transcripts & 1 genes updated.
- writing GFF entries... finished.
```

## Caveats

The current version of GffCreator is using Nirvana 3.2.3 as a git submodule. As such, the GFF files produced by this tool will also contain the updated gene models that were [manually added to that version](https://github.com/Illumina/Nirvana/blob/v3.2.3/VariantAnnotation/Caches/DataStructures/Transcript.cs#L106-L1009). To generate a GFF file for a specific version of Nirvana, just update the Nirvana submodule accordingly.
