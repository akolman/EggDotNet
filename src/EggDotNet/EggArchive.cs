using EggDotNet.Format;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if NETSTANDARD2_1_OR_GREATER
#nullable enable
#endif

namespace EggDotNet
{
	/// <summary>
	/// Represents an Egg archive which holds a number of <see cref="EggArchiveEntry">entries</see>.
	/// </summary>
	public class EggArchive : IDisposable
	{
		private bool disposedValue;

		private readonly List<EggArchiveEntry> _entries;
		
		internal readonly IEggFileFormat format;

		/// <summary>
		/// Gets the archive-level comment text.
		/// </summary>
#if NETSTANDARD2_1_OR_GREATER
		public string? Comment { get; internal set; }
#else
		public string Comment { get; internal set; } = string.Empty;
#endif

		/// <summary>
		/// Gets a collection of all <see cref="EggArchiveEntry"/> entries in this EggArchive.
		/// </summary>
		public ReadOnlyCollection<EggArchiveEntry> Entries => _entries.AsReadOnly();

		/// <summary>
		/// Constructs a new EggArchive using a source stream.
		/// Caller owns the stream.
		/// </summary>
		/// <param name="sourceStream">The input egg stream.</param>
		/// <exception cref="Exceptions.UnknownEggException"/>
		public EggArchive(Stream sourceStream)
			: this(sourceStream, false, null)
		{	
		}

		/// <summary>
		/// Constructs a new EggArchive using a source stream and option indicating who will own the stream.
		/// </summary>
		/// <param name="sourceStream">The input egg stream</param>
		/// <param name="ownStream">A flag indicating whether the caller owns the stream (false) or the EggArchive (true)</param>
		/// <exception cref="Exceptions.UnknownEggException"/>
		public EggArchive(Stream sourceStream, bool ownStream)
			: this(sourceStream, ownStream, null)
		{
		}

		/// <summary>
		/// Constructs a new EggArchive using a source stream and stream retrieve callback.
		/// </summary>
		/// <param name="stream">The input egg stream</param>
		/// <param name="streamCallback">A callback that will be called to retrieve volumes of a multi-part archive.</param>
		/// <exception cref="Exceptions.UnknownEggException"/>
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
		/// <param name="passwordCallback">A callback that will be called to retrieve a password used for decryption.</param>
		/// <exception cref="Exceptions.UnknownEggException"/>
#if NETSTANDARD2_1_OR_GREATER
		public EggArchive(Stream stream, bool ownStream = false, Func<Stream, IEnumerable<Stream>>? streamCallback = null, Func<string>? passwordCallback = null)
#else
		public EggArchive(Stream stream, bool ownStream = false, Func<Stream, IEnumerable<Stream>> streamCallback = null, Func<string> passwordCallback = null)
#endif
		{
			if (streamCallback == null) streamCallback = DefaultStreamCallbacks.GetStreamCallback(stream);

			this.format = EggFileFormatFactory.Create(stream, streamCallback, passwordCallback);
			this.format.ParseHeaders(stream, ownStream);
			_entries = this.format.Scan(this);		
		}

		/// <summary>
		/// Gets an <see cref="EggArchiveEntry"/> by name.
		/// </summary>
		/// <param name="entryName">The name of the entry.</param>
		/// <returns>The entry specified by Name, null if not found.</returns>
#if NETSTANDARD2_1_OR_GREATER
		public EggArchiveEntry? GetEntry(string entryName)
#else
		public EggArchiveEntry GetEntry(string entryName)
#endif
		{
			return _entries.SingleOrDefault(e => e.FullName != null && e.FullName.Equals(entryName, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Gets an <see cref="EggArchiveEntry"/> by ID.
		/// </summary>
		/// <param name="id">The ID of the entry.</param>
		/// <returns>The entry specifieid by ID, null if not found.</returns>
#if NETSTANDARD2_1_OR_GREATER
		public EggArchiveEntry? GetEntry(int id)
#else
		public EggArchiveEntry GetEntry(int id)
#endif
		{
			return _entries.SingleOrDefault(e => e.Id.Equals(id));
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
