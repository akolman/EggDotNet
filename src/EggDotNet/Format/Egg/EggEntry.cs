using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace EggDotNet.Format.Egg
{
	internal sealed class EggEntry
	{
		public int Id { get; private set; }
		public string? Name { get; private set; }
		public long Position { get; private set; }

		public long UncompressedSize { get; private set; }

		public long CompressedSize { get; private set; }

		public CompressionMethod CompressionMethod { get; private set; }

		public DateTime? LastModifiedTime { get; private set; }

		public EncryptHeader? EncryptHeader { get; private set; }

		public string? Comment { get; private set; }

		public uint Crc { get; private set; }


		public static List<EggEntry> Parse(Stream stream, EggArchive archive)
		{
			var entries = new List<EggEntry>();

			while(true)
			{
				var entry = new EggEntry();

				BuildHeaders(entry, stream);

				if (stream.Position >= stream.Length)
				{
					break;
				}

				BuildBlocks(entry, stream);

				entries.Add(entry);

				if (stream.Position >= stream.Length)
				{
					break;
				}

				BuildRemaining(stream, archive);
			}

			return entries;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void BuildHeaders(EggEntry entry, Stream stream)
		{
			var foundEnd = false;
			while (!foundEnd && stream.ReadInt(out int nextHeader))
			{
				switch(nextHeader)
				{
					case FileHeader.FILE_HEADER_MAGIC:
						var fileHeader = FileHeader.Parse(stream);
						entry.Id = fileHeader.FileId;
						break;
					case FilenameHeader.FILENAME_HEADER_MAGIC:
						var filename = FilenameHeader.Parse(stream);
						entry.Name = filename.FileNameFull;
						break;
					case WinFileInfo.WIN_FILE_INFO_MAGIC_HEADER:
						var winFileInfo = WinFileInfo.Parse(stream);
						entry.LastModifiedTime = winFileInfo.LastModified;
						break;
					case EncryptHeader.EGG_ENCRYPT_HEADER_MAGIC:
						var encryptHeader = EncryptHeader.Parse(stream);
						entry.EncryptHeader = encryptHeader;
						break;
					case CommentHeader.COMMENT_HEADER_MAGIC:
						var comment = CommentHeader.Parse(stream);
						entry.Comment = comment.CommentText;
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
					entry.UncompressedSize = blockHeader.UncompressedSize;
					entry.CompressionMethod = blockHeader.CompressionMethod;
					entry.Crc = blockHeader.Crc32;
					stream.Seek(entry.CompressedSize, SeekOrigin.Current);
					break;
				}
			}
		}

		private static void BuildRemaining(Stream stream, EggArchive archive)
		{
			while (stream.ReadInt(out int nextHeader))
			{
				if (nextHeader == CommentHeader.COMMENT_HEADER_MAGIC)
				{
					var comment = CommentHeader.Parse(stream);
					archive.Comment = comment.CommentText;
				}
				else if (nextHeader == FileHeader.FILE_END_HEADER)
				{
					break;
				}
			}
		}
	}
}
