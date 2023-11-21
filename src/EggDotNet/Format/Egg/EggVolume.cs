using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Egg
{
    internal class EggVolume : IDisposable
    {
        internal Stream _stream;
        private readonly bool _ownStream;
		private bool disposedValue;

		public Header Header { get; private set; }
		public bool IsSplit => Header.SplitHeader != null;

		private EggVolume(Stream stream, bool ownStream, Header header)
		{
			_stream = stream;
			_ownStream = ownStream;
			Header = header;
		}

		public static EggVolume Parse(Stream stream, bool ownStream)
        {
            return new EggVolume(stream, ownStream, Header.Parse(stream));
        }

		public Stream GetStream() => _stream;

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

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
