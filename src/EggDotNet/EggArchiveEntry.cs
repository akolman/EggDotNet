using EggDotNet.Format;
using EggDotNet.SpecialStreams;
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

		/// <summary>
		/// Gets the parent <see cref="EggArchive"/> for this entry.
		/// </summary>
		public EggArchive Archive { get; internal set; }

		/// <summary>
		/// Gets the name of the egg entry, not including any directory.
		/// </summary>
		public string? Name => Path.GetFileName(FullName);

		/// <summary>
		/// Gets the name of the egg entry, including any directory.
		/// </summary>
		public string? FullName { get; internal set; }

		/// <summary>
		/// Gets the Crc32 checksum for this entry.
		/// </summary>
		public uint Crc32 { get; internal set; }

		/// <summary>
		/// Gets a flag indicating whether the entry is encrypted.
		/// </summary>
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
		public DateTime? LastWriteTime { get; internal set; }

		public long ExternalAttributes {  get; internal set; }

		/// <summary>
		/// Gets the comment of the file.
		/// </summary>
		public string? Comment { get; internal set; }

		internal EggArchiveEntry(IEggFileFormat format, EggArchive archive)
		{
			_format = format;
			Archive = archive;
		}

		/// <summary>
		/// Produces a stream to the entry.
		/// </summary>
		/// <returns>A Stream to the entry.</returns>
		public Stream Open()
		{
			return _format.GetStreamForEntry(this);
		}

		/// <summary>
		/// Calculates the CRC32 checksum for this entry.
		/// </summary>
		/// <returns>The CRC32 checksum.</returns>
		public uint ComputeChecksum()
		{
			using var st = _format.GetStreamForEntry(this);
			using var crc = new Crc32Stream(st);
			var data = new byte[8192];
			while (crc.Read(data, 0, data.Length) > 0) { };
			return crc.Crc;
		}
	}
}
