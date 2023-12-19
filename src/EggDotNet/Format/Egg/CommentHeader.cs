using EggDotNet.Exceptions;
using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
			_ = stream.ReadByte();

			if (!stream.ReadShort(out short size))
			{
				throw new InvalidDataException("Failed to read comment size");
			}

			if (!stream.ReadN(size, out byte[] commentData))
			{
				Console.Error.WriteLine("Failed to read all contents of comment");
			}

			return new CommentHeader(Encoding.UTF8.GetString(commentData));
		}

	}
}
