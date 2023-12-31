using System;

namespace EggDotNet.Format
{
	internal interface IEggFileEntry
	{
		int Id { get; }

		string Name { get; }

		uint Crc32 { get; }

		bool IsEncrypted { get; }

		long UncompressedSize { get; }

		long CompressedSize { get;  }

		CompressionMethod CompressionMethod { get; }

		long Position { get; }

		long ExternalAttributes { get; }

#if NETSTANDARD2_1_OR_GREATER
#nullable enable
		DateTime? LastWriteTime { get; }

		string? Comment { get; }
#else
		DateTime LastWriteTime { get; }

		string Comment { get; }
#endif

	}
}
