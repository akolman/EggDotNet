using EggDotNet.Exceptions;
using System;
using System.IO;

namespace EggDotNet.Format.Alz
{
	internal sealed class AlzVolume : IDisposable
	{
		private readonly Stream _stream;
		private readonly bool _ownStream;
		private bool disposedValue;

		public Stream GetStream() => _stream;

		public Header Header { get; private set; }

		public AlzVolume(Stream stream, bool ownStream, Header header)
		{
			_stream = stream;
			_ownStream = ownStream;
			Header = header;
		}

		public static AlzVolume Parse(Stream stream, bool ownStream)
		{
			var alzHeader = Header.Parse(stream);
			if (alzHeader.Version != 10)
			{
				throw new UnknownEggException(alzHeader.Version);
			}

			return new AlzVolume(stream, ownStream, alzHeader);
		}

		private void Dispose(bool disposing)
		{
			if (!disposedValue && disposing)
			{
				if (_ownStream)
				{
					_stream.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
