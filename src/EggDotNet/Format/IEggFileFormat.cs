using EggDotNet.Format.Egg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format
{
	internal interface IEggFileFormat : IDisposable
	{
		void ParseHeaders(Stream stream, bool ownStream, Func<Stream, IEnumerable<Stream>>? streamCallback);

		public Stream GetStreamForEntry(EggArchiveEntry entry);

		List<EggArchiveEntry> Scan();
	}
}
