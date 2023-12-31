using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace EggDotNet.Format.Egg
{
	internal sealed class EggEntry : IEggFileEntry
	{
		public int Id { get; private set; }
		public string Name { get; private set; }
		public long Position { get; private set; }

		public long UncompressedSize { get; private set; }

		public long CompressedSize { get; private set; }

		public CompressionMethod CompressionMethod { get; private set; }

		//public DateTime? LastModifiedTime { get; private set; }

		public WinFileInfo WinFileInfo { get; private set; }

		public EncryptHeader EncryptHeader { get; private set; }

		public string Comment { get; private set; }

		public uint Crc32 { get; private set; }

		public bool IsEncrypted => EncryptHeader != null;

		public long ExternalAttributes => GetExternalAttributes();

#if NETSTANDARD2_1_OR_GREATER
		public DateTime? LastWriteTime => GetLastWriteTime();
#else
		public DateTime LastWriteTime => GetLastWriteTime();
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
			while (!foundEnd && stream.ReadInt(out int nextHeader))
			{
				switch (nextHeader)
				{
					case FileHeader.FILE_HEADER_MAGIC:
						var fileHeader = FileHeader.Parse(stream);
						entry.Id = fileHeader.FileId;
						entry.UncompressedSize = fileHeader.FileLength;
						insideFileheader = true;
						break;
					case FilenameHeader.FILENAME_HEADER_MAGIC:
						var filename = FilenameHeader.Parse(stream);
						entry.Name = filename.FileNameFull;
						break;
					case WinFileInfo.WIN_FILE_INFO_MAGIC_HEADER:
						entry.WinFileInfo = WinFileInfo.Parse(stream);
						break;
					case EncryptHeader.EGG_ENCRYPT_HEADER_MAGIC:
						var encryptHeader = EncryptHeader.Parse(stream);
						entry.EncryptHeader = encryptHeader;
						break;
					case CommentHeader.COMMENT_HEADER_MAGIC:
						var comment = CommentHeader.Parse(stream);
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
			while (stream.ReadInt(out int nextHeader))
			{
				if (nextHeader == BlockHeader.BLOCK_HEADER_MAGIC)
				{
					var blockHeader = BlockHeader.Parse(stream);
					entry.Position = blockHeader.BlockDataPosition;
					entry.CompressedSize = blockHeader.CompressedSize;
					//entry.UncompressedSize = blockHeader.UncompressedSize; //set by file header
					entry.CompressionMethod = blockHeader.CompressionMethod;
					entry.Crc32 = blockHeader.Crc32;
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
