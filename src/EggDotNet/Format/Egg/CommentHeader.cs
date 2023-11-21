using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Egg
{
	internal class CommentHeader
	{
		public const int COMMENT_HEADER_MAGIC = 0x04C63672;

		public string CommentText { get; private set; }

		private CommentHeader(string commentText)
		{
			CommentText = commentText;
		}

		public static CommentHeader Parse(Stream stream)
		{
			var bitFlag = stream.ReadByte();

			if (!stream.ReadShort(out short size))
			{

			}

			if (!stream.ReadN(size, out byte[] commentData))
			{

			}

			var comment = System.Text.Encoding.UTF8.GetString(commentData);

			return new CommentHeader(comment);
		}

	}
}
