using EggDotNet.Format;
using EggDotNet.SpecialStreams;
using System;
using System.IO;

#if NETSTANDARD2_1_OR_GREATER
#nullable enable
#endif

namespace EggDotNet
{
	/// <summary>
	/// Represents an entry with an EGG archive.
	/// </summary>
	public sealed class EggArchiveEntry
	{
		private IEggFileFormat _format => Archive.format;

		internal readonly IEggFileEntry entry;
		internal long PositionInStream => entry.Position;
		
		/// <summary>
		/// Gets the parent <see cref="EggArchive"/> for this entry.
		/// </summary>
		public EggArchive Archive { get; private set; }

		/// <summary>
		/// Gets the ID of the egg entry.
		/// </summary>
		public int Id => entry.Id;

		/// <summary>
		/// Gets the name of the egg entry, not including any directory.
		/// </summary>
#if NETSTANDARD2_1_OR_GREATER
		public string? Name => Path.GetFileName(FullName);
#else
		public string Name => Path.GetFileName(FullName);
#endif

		/// <summary>
		/// Gets the name of the egg entry, including any directory.
		/// </summary>
#if NETSTANDARD2_1_OR_GREATER
		public string? FullName => entry.Name;
#else
		public string FullName => entry.Name;
#endif

		/// <summary>
		/// Gets the Crc32 checksum for this entry.
		/// </summary>
		public uint Crc32 => entry.Crc32;

		/// <summary>
		/// Gets a flag indicating whether the entry is encrypted.
		/// </summary>
		public bool IsEncrypted => entry.IsEncrypted;

		/// <summary>
		/// Gets the compressed length of the file.
		/// </summary>
		public long CompressedLength => entry.CompressedSize;

		/// <summary>
		/// Gets the uncompressed length of the file.
		/// </summary>
		public long UncompressedLength => entry.UncompressedSize;

		/// <summary>
		/// Gets the compression method used to compress this entry.
		/// </summary>
		public CompressionMethod CompressionMethod => entry.CompressionMethod;

		/// <summary>
		/// Gets the last write time of the file.
		/// </summary>
#if NETSTANDARD2_1_OR_GREATER
		public DateTime? LastWriteTime => entry.LastWriteTime;
#else
		public DateTime LastWriteTime => entry.LastWriteTime;
#endif

		/// <summary>
		/// Gets the external attributes for the entry.
		/// </summary>
		/// <remarks>See <see cref="WindowsFileAttributes"/>.
		/// For entries which contain Windows file attributes, the value will be the lowest 4 bytes of the total long value.
		/// </remarks>
		public long ExternalAttributes => entry.ExternalAttributes;

		/// <summary>
		/// Gets the comment of the file.
		/// </summary>
#if NETSTANDARD2_1_OR_GREATER
		public string? Comment => entry.Comment;
#else
		public string Comment => entry.Comment ?? string.Empty;
#endif

		internal EggArchiveEntry(IEggFileEntry entry, EggArchive archive)
		{
			Archive = archive;
			this.entry = entry;
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
		/// Verifies the checksum.
		/// </summary>
		/// <returns>True is checksum matches, false if not.</returns>
		public bool ChecksumValid()
		{
			using (var st = _format.GetStreamForEntry(this))
			{
				using (var crc = new Crc32Stream(st))
				{
					var data = new byte[8192];
					while (crc.Read(data, 0, data.Length) > 0) { };
					return crc.Crc == Crc32;
				}
			}
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(entry.Name)) 
			{
				return $"{nameof(EggArchiveEntry)} {entry.Id}";
			}
			return $"{nameof(EggArchiveEntry)} {entry.Id} - {entry.Name}";
		}
	}
}
