using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Egg
{
	internal class SplitHeader
	{
		public const int SPLIT_HEADER_MAGIC = 0x24F5A262;

		public int NextFileId { get; private set; }

		public int PreviousFileId { get; private set; }

		private SplitHeader(int previousFileId, int nextFileId)
		{
			PreviousFileId = previousFileId;
			NextFileId = nextFileId;
		}

		public static SplitHeader Parse(Stream stream)
		{
			var bitFlag = stream.ReadByte();

			stream.ReadShort(out short size);

			stream.ReadInt(out int prevFileId);

			stream.ReadInt(out int nextFileId);

			return new SplitHeader(prevFileId, nextFileId);
		}
	}
}
