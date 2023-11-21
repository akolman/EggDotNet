using EggDotNet.Extensions;
using System.IO;

namespace EggDotNet.Format.Egg
{
	internal sealed class SplitHeader
	{
		public const int SPLIT_HEADER_MAGIC = 0x24F5A262;

		public int PreviousFileId { get; private set; }

		public int NextFileId { get; private set; }

		private SplitHeader(int previousFileId, int nextFileId)
		{
			PreviousFileId = previousFileId;
			NextFileId = nextFileId;
		}

		public static SplitHeader Parse(Stream stream)
		{
			_ = stream.ReadByte();

			stream.ReadShort(out short _);

			stream.ReadInt(out int prevFileId);

			stream.ReadInt(out int nextFileId);

			return new SplitHeader(prevFileId, nextFileId);
		}
	}
}
