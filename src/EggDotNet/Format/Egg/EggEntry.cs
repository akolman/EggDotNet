using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace EggDotNet.Format.Egg
{
	internal sealed class EggEntry
	{
		public string Name { get; private set; }
		public long Position { get; private set; }

		public long UncompressedSize { get; private set; }

		public long CompressedSize { get; private set; }

		public CompressionMethod CompressionMethod { get; private set; }

		public DateTime? LastModifiedTime { get; private set; }

		public EncryptHeader EncryptHeader { get; private set; }

		public string? Comment { get; private set; }


		public static ICollection<EggEntry> Parse(Stream stream)
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
			}

			return entries;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void BuildHeaders(EggEntry entry, Stream stream)
		{
			while (stream.ReadInt(out int nextHeader))
			{
				if (nextHeader == FileHeader.FILE_HEADER_MAGIC)
				{
					var fileHeader = FileHeader.Parse(stream);
				}
				else if (nextHeader == FilenameHeader.FILENAME_HEADER_MAGIC)
				{
					var filename = FilenameHeader.Parse(stream);
					entry.Name = filename.FileName;
				}
				else if (nextHeader == WinFileInfo.WIN_FILE_INFO_MAGIC_HEADER)
				{
					var winFileInfo = WinFileInfo.Parse(stream);
					entry.LastModifiedTime = winFileInfo.LastModified;
				}
				else if (nextHeader == EncryptHeader.EGG_ENCRYPT_HEADER_MAGIC)
				{
					var encryptHeader = EncryptHeader.Parse(stream);
					entry.EncryptHeader = encryptHeader;
				}
				else if (nextHeader == CommentHeader.COMMENT_HEADER_MAGIC)
				{
					var comment = CommentHeader.Parse(stream);
					entry.Comment = comment.CommentText;
				}
				else if (nextHeader == FileHeader.FILE_END_HEADER)
				{
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
					stream.Seek(entry.CompressedSize, SeekOrigin.Current);
					break;
				}
			}
		}
	}
}
