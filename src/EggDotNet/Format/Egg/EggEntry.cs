using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace EggDotNet.Format.Egg
{
	internal sealed class EggEntry : IEggFileEntry
	{
		public FileHeader FileHeader { get; private set; }
		public FilenameHeader FilenameHeader { get; private set; }

		public WinFileInfo WinFileInfo { get; private set; }

		public EncryptHeader EncryptHeader { get; private set; }

		public BlockHeader BlockHeader { get; private set; }

		public int Id => FileHeader.FileId;

		public string Name => FilenameHeader.FileNameFull;

		public long Position => BlockHeader.BlockDataPosition;

		public long UncompressedSize => FileHeader.FileLength;

		public long CompressedSize => BlockHeader.CompressedSize;

		public CompressionMethod CompressionMethod => BlockHeader.CompressionMethod;

		public uint Crc32 => BlockHeader.Crc32;

		public bool IsEncrypted => EncryptHeader != null;

		public long ExternalAttributes => GetExternalAttributes();

#if NETSTANDARD2_1_OR_GREATER
#nullable enable
		public DateTime? LastWriteTime => GetLastWriteTime();

		public string? Comment { get; private set; }
#else
		public DateTime LastWriteTime => GetLastWriteTime();

		public string Comment { get; private set; } = string.Empty;
#endif

		public static List<EggEntry> ParseEntries(Stream stream, EggArchive archive)
		{
			var entries = new List<EggEntry>();

			while (true)
			{
				var entry = new EggEntry();

				BuildHeaders(entry, archive, stream);

				if (stream.Position >= stream.Length) break; //sanity check

				if (entry.UncompressedSize > 0) //check for empty entries (e.g. directories)
				{
					BuildBlocks(entry, stream);
				}

				entries.Add(entry);
			}

			return entries;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void BuildHeaders(EggEntry entry, EggArchive archive, Stream stream)
		{
			var foundEnd = false;
			var insideFileheader = false;
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> buff = stackalloc byte[4];
			while (!foundEnd && stream.Read(buff) == 4)
			{
				var nextHeader = BitConverter.ToInt32(buff);
#else
			var buff = new byte[4];
			while (!foundEnd && stream.Read(buff, 0, 4) == 4)
			{
				var nextHeader = BitConverter.ToInt32(buff, 0);
#endif
				switch (nextHeader)
				{
					case FileHeader.FILE_HEADER_MAGIC:
						entry.FileHeader = FileHeader.Parse(stream);
						insideFileheader = true;
						break;
					case FilenameHeader.FILENAME_HEADER_MAGIC:
						entry.FilenameHeader = FilenameHeader.Parse(stream);
						break;
					case WinFileInfo.WIN_FILE_INFO_MAGIC_HEADER:
						entry.WinFileInfo = WinFileInfo.Parse(stream);
						break;
					case EncryptHeader.EGG_ENCRYPT_HEADER_MAGIC:
						entry.EncryptHeader = EncryptHeader.Parse(stream);
						break;
					case CommentHeader.COMMENT_HEADER_MAGIC:
						var comment = CommentHeader.Parse(stream); //TODO: should we save CommentHeader like other members?
						if (insideFileheader)
							entry.Comment = comment.CommentText;
						else
							archive.Comment = comment.CommentText;
						break;
					case FileHeader.FILE_END_HEADER:
						foundEnd = true;
						break;
					default:
						foundEnd = true;
						break;
				}
			}
		}

		//It may be incorrect to assume that an entry is contained in only one block per volume.  This method won't work if so because
		//it applies one block per entry.  It should be relatively easy to revise if needed (although it will complicate block retrieval).
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void BuildBlocks(EggEntry entry, Stream stream)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> buffer = stackalloc byte[4];
			while (stream.Read(buffer) == 4)
			{
				var nextHeader = BitConverter.ToInt32(buffer);
#else
			var buffer = new byte[4];
			while (stream.Read(buffer, 0, 4) == 4)
			{
				var nextHeader = BitConverter.ToInt32(buffer, 0);
#endif
				if (nextHeader == BlockHeader.BLOCK_HEADER_MAGIC)
				{
					entry.BlockHeader = BlockHeader.Parse(stream);
					stream.Seek(entry.CompressedSize, SeekOrigin.Current);
					break;
				}
			}
		}

#if NETSTANDARD2_1_OR_GREATER
		private DateTime? GetLastWriteTime()
		{
			if (WinFileInfo != null)
			{
				return WinFileInfo.LastModified;
			}

			return null;
		}
#else
		private DateTime GetLastWriteTime()
		{
			if (WinFileInfo != null)
			{
				return WinFileInfo.LastModified;
			}

			return DateTime.MinValue;
		}
#endif

		private long GetExternalAttributes()
		{
			if (WinFileInfo != null)
			{
				return WinFileInfo.WindowsFileAttributes;
			}
			else
			{
				return 0;
			}
		}
	}
}
