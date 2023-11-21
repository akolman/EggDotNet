using EggDotNet.Format.Egg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format
{
	internal interface IEggFileFormat : IDisposable
	{
		void ParseHeaders(Stream stream, bool ownStream);

		public Stream GetStreamForEntry(EggArchiveEntry entry);

		/// <summary>
		/// Scans the current format and provides a list of EggArchiveEntries.  EggArchive instance is required so
		/// that it can be attached to each EggArchiveEntry.
		/// </summary>
		/// <param name="archive"></param>
		/// <returns></returns>
		List<EggArchiveEntry> Scan(EggArchive archive);
	}
}
