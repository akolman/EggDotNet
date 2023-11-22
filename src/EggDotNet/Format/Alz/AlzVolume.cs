using EggDotNet.Exception;
using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Alz
{
	internal sealed class AlzVolume
	{
		private readonly Stream _stream;

		public Stream GetStream() => _stream;

		public Header Header { get; private set; }

		public AlzVolume(Stream stream, Header header)
		{
			Header = header;
			_stream = stream;
		}

		public static AlzVolume Parse(Stream stream)
		{
			var alzHeader = Header.Parse(stream);
			if (alzHeader.Version != 10)
			{
				throw new UnknownEggEggception(alzHeader.Version);
			}

			return new AlzVolume(stream, alzHeader);
		}
	}
}
