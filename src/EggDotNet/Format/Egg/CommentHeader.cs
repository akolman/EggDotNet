using EggDotNet.InternalExtensions;
using System;
using System.IO;
using System.Text;

#if !USE_SPAN
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
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
#if USE_SPAN
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

			var commentDataBuffer = new byte[commentSize]; /*skip stackalloc because we don't know size and we can't let it be unbounded*/
			if (stream.Read(commentDataBuffer) != commentSize)
			{
				Console.Error.WriteLine("Failed to read all contents of comment");
			}

			return new CommentHeader(Encoding.UTF8.GetString(commentDataBuffer));
		}
	}
}
