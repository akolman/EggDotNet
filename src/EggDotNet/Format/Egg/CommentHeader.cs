using EggDotNet.Extensions;
using System;
using System.IO;
using System.Text;

#if NETSTANDARD2_0
using BitConverter = EggDotNet.Extensions.BitConverterWrapper;
#endif

namespace EggDotNet.Format.Egg
{
	internal sealed class CommentHeader
	{
		public const int COMMENT_HEADER_MAGIC = 0x04C63672;

		public string CommentText { get; private set; }

		private CommentHeader(string commentText)
		{
			CommentText = commentText;
		}

		public static CommentHeader Parse(Stream stream)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> commentHeaderBuffer = stackalloc byte[3];
#else
			var commentHeaderBuffer = new byte[3];
#endif
			if (stream.Read(commentHeaderBuffer) != 3)
			{
				throw new InvalidDataException("Failed reading comment header");
			}

			var attributes = commentHeaderBuffer[0];
			var commentSize = BitConverter.ToInt16(commentHeaderBuffer.Slice(1, 2));

#if NETSTANDARD2_1_OR_GREATER
			Span<byte> commentDataBuffer = (commentSize < 1024) ? stackalloc byte[commentSize] : new byte[commentSize];
#else
			var commentDataBuffer = new byte[commentSize];
#endif
			if (stream.Read(commentDataBuffer) != commentSize)
			{
				Console.Error.WriteLine("Failed to read all contents of comment");
			}

			return new CommentHeader(Encoding.UTF8.GetString(commentDataBuffer));
		}

	}
}
