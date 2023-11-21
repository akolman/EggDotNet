using EggDotNet.Format;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;

namespace EggDotNet
{
	/// <summary>
	/// Represents an Egg archive which holds a number of <see cref="EggArchiveEntry">entries</see>.
	/// </summary>
	public class EggArchive : IDisposable
	{
		private bool disposedValue;

		private readonly List<EggArchiveEntry> _entries;
		private readonly IEggFileFormat format;

		/// <summary>
		/// Gets a collection of all <see cref="EggArchiveEntry"/> entries in this EggArchive.
		/// </summary>
		public ReadOnlyCollection<EggArchiveEntry> Entries => _entries.AsReadOnly();

		/// <summary>
		/// Constructs a new EggArchive using a file path.
		/// </summary>
		/// <param name="eggFilePath">The path to an egg file.</param>
		public EggArchive(string eggFilePath)
			: this(new FileStream(eggFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), true, null)
		{
		}

		/// <summary>
		/// Constructs a new EggArchive using a source stream.
		/// Caller owns the stream.
		/// </summary>
		/// <param name="sourceStream">The input egg stream.</param>
		public EggArchive(Stream sourceStream)
			: this(sourceStream, false, null)
		{	
		}

		/// <summary>
		/// Constructs a new EggArchive using a source stream and option indicating who will own the stream.
		/// </summary>
		/// <param name="sourceStream">The input egg stream</param>
		/// <param name="ownStream">A flag indicating whether the caller owns the stream (false) or the EggArchive (true)</param>
		public EggArchive(Stream sourceStream, bool ownStream)
			: this(sourceStream, ownStream, null)
		{
		}

		/// <summary>
		/// Constructs a new EggArchive using a source stream and stream retrieve callback.
		/// </summary>
		/// <param name="stream">The input egg stream</param>
		/// <param name="streamCallback">A callback that will be called to retrieve volumes of a multi-part archive.</param>
		public EggArchive(Stream stream, Func<Stream, IEnumerable<Stream>> streamCallback)
			: this(stream, false, streamCallback)
		{
		}

		/// <summary>
		/// Constructs a new EggArchive using a source stream, a flag indicating who will own the stream, and optional stream retrieve callback.
		/// </summary>
		/// <param name="stream">The input egg stream.</param>
		/// <param name="ownStream">A flag indicating whether the caller owns the stream (false) or the EggArchive (true)</param>
		/// <param name="streamCallback">A callback that will be called to retrieve volumes of a multi-part archive.</param>
		public EggArchive(Stream stream, bool ownStream, Func<Stream, IEnumerable<Stream>>? streamCallback = null, Func<string>? passwordCallback = null)
		{
			streamCallback ??= DefaultStreamCallbacks.GetStreamCallback(stream);

			this.format = EggFileFormatFactory.Create(stream, streamCallback, passwordCallback);
			this.format.ParseHeaders(stream, ownStream);
			_entries = this.format.Scan(this);		
		}



		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc/>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					format.Dispose();
				}

				disposedValue = true;
			}
		}
	}
}
