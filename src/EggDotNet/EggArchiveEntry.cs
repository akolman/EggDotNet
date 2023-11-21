using EggDotNet.Format;
using System;
using System.IO;

namespace EggDotNet
{
	public class EggArchiveEntry
	{
		private readonly IEggFileFormat _format;

		internal int Id { get; set; }

		internal long PositionInStream { get; set; }

		public string? Name => Path.GetFileName(FullName);

		public string? FullName { get; internal set; }

		public long CompressedLength { get; internal set; }

		public long UncompressedLength { get; internal set; }

		public DateTime? LastWriteTime { get; internal set; } = null;

		internal EggArchiveEntry(IEggFileFormat format)
		{
			_format = format;
		}

		public Stream Open()
		{
			return _format.GetStreamForEntry(this);
		}
	}
}
