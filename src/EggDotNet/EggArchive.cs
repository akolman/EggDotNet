using EggDotNet.Format;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;

namespace EggDotNet
{
	public class EggArchive : IDisposable
	{
		private bool disposedValue;
		private readonly List<EggArchiveEntry> _entries;
		private IEggFileFormat format;

		public ReadOnlyCollection<EggArchiveEntry> Entries => _entries.AsReadOnly();

		public EggArchive(Stream stream)
			: this(stream, false, null)
		{
			
		}

		internal EggArchive(Stream stream, bool ownStream)
			: this(stream, false, null)
		{
		}

		public EggArchive(Stream stream, bool ownStream, Func<Stream, IEnumerable<Stream>>? streamCallback)
		{
			streamCallback ??= DefaultStreamCallbacks.GetStreamCallback(stream);

			this.format = EggFileFormatFactory.Create(stream);
			this.format.ParseHeaders(stream, ownStream, streamCallback);
			_entries = this.format.Scan();		
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{

				}

				disposedValue = true;
			}
		}


		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
