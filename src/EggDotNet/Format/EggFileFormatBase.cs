using System;
using System.Collections.Generic;
using System.IO;

namespace EggDotNet.Format
{
	/// <summary>
	/// Interface that represents a parser specific to an EGG format
	/// </summary>
	internal abstract class EggFileFormatBase : IDisposable
	{
		/// <summary>
		/// Parses the headers from the given Egg stream, and if a split archive, requests the additional volumes.
		/// Once provided, each volume will be scanned.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="ownStream"></param>
		public abstract void ParseHeaders(Stream stream, bool ownStream);

		/// <summary>
		/// Scans the current format and provides a list of EggArchiveEntries.  EggArchive instance is required so
		/// that it can be attached to each EggArchiveEntry.
		/// </summary>
		/// <param name="archive"></param>
		/// <returns></returns>
		public abstract List<EggArchiveEntry> Scan(EggArchive archive);

		/// <summary>
		/// Produces a stream for a given <see cref="EggArchiveEntry"/>.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		public abstract Stream GetStreamForEntry(EggArchiveEntry entry);

		/// <inheritdoc/>
		public abstract void Dispose();
	}
}
