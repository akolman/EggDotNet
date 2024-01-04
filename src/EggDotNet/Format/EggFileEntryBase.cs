using System;

namespace EggDotNet.Format
{
	internal abstract class EggFileEntryBase
	{
		public abstract int Id { get; }

		public abstract string Name { get; }

		public abstract uint Crc32 { get; }

		public abstract bool IsEncrypted { get; }

		public abstract long UncompressedSize { get; }

		public abstract long CompressedSize { get;  }

		public abstract CompressionMethod CompressionMethod { get; }

		public abstract long Position { get; }

		public abstract long ExternalAttributes { get; }

#if NETSTANDARD2_1_OR_GREATER
#nullable enable
		public abstract DateTime? LastWriteTime { get; }

		public abstract string? Comment { get; }
#else
		public abstract DateTime LastWriteTime { get; }

		public abstract string Comment { get; }
#endif

	}
}
