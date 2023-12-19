using EggDotNet.Exceptions;
using System;
using System.IO;

namespace EggDotNet.Format.Egg
{
#pragma warning disable CA1852

	/// <summary>
	/// Represents a single/individual Egg volume (file).
	/// EggVolume will be the final holder for a stream.
	/// </summary>
	internal class EggVolume : IDisposable
    {
		private bool disposedValue;

		private readonly Stream _stream;
        private readonly bool _ownStream;

		/// <summary>
		/// Gets the <see cref="Egg.Header"/> associated with this volume.
		/// </summary>
		public Header Header { get; private set; }

		/// <summary>
		/// Gets a flag indicating whether this volume is part of a split archive.
		/// </summary>
		public bool IsSplit => Header.SplitHeader != null;

		public bool IsSolid => Header.SolidHeader != null;

		/// <summary>
		/// Gets the stream held by this volume.
		/// </summary>
		/// <returns></returns>
		public Stream GetStream() => _stream;

		private EggVolume(Stream stream, bool ownStream, Header header)
		{
			_stream = stream;
			_ownStream = ownStream;
			Header = header;
		}

		public static EggVolume Parse(Stream stream, bool ownStream)
        {
			var eggHeader = Header.Parse(stream);
			if (eggHeader.Version != 256)
			{
				throw new UnknownEggException(eggHeader.Version);
			}

			return new EggVolume(stream, ownStream, eggHeader);
        }

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (_ownStream)
					{
						_stream.Dispose();
					}
				}

				disposedValue = true;
			}
		}
	}
}
