using EggDotNet.Format;
using System;
using System.IO;

namespace EggDotNet
{
	/// <summary>
	/// Represents an entry with an EGG archive.
	/// </summary>
	public sealed class EggArchiveEntry
	{
		private readonly IEggFileFormat _format;

		internal int Id { get; set; }
		internal long PositionInStream { get; set; }

		public EggArchive Archive { get; internal set; }

		/// <summary>
		/// Gets the name of the egg entry, not including any directory.
		/// </summary>
		public string? Name => Path.GetFileName(FullName);

		/// <summary>
		/// Gets the name of the egg entry, including any directory.
		/// </summary>
		public string? FullName { get; internal set; }

		public uint Crc32 { get; internal set; }

		public bool IsEncrypted { get; internal set; }

		/// <summary>
		/// Gets the compressed length of the file.
		/// </summary>
		public long CompressedLength { get; internal set; }

		/// <summary>
		/// Gets the uncompressed length of the file.
		/// </summary>
		public long UncompressedLength { get; internal set; }

		/// <summary>
		/// Gets the last write time of the file.
		/// </summary>
		public DateTime? LastWriteTime { get; internal set; } = null;

		/// <summary>
		/// Gets the comment of the file.
		/// </summary>
		public string? Comment { get; internal set; }

		internal EggArchiveEntry(IEggFileFormat format, EggArchive archive)
		{
			_format = format;
			Archive = archive;
		}

		public Stream Open()
		{
			return _format.GetStreamForEntry(this);
		}
	}
}
