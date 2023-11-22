using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Alz
{
	internal sealed class FileHeader
	{
		public const int ALZ_FILE_HEADER_START_MAGIC = 0x015A4C42;

		public long CompressedSize { get; private set; }

		public long UncompressedSize { get; private set; }

		public string? Name { get; private set; }

		public long StartPosition { get; private set; }

		private static short GetReadFileSize(short bitflags)
		{
			var a = (bitflags & 0xF0) >> 4;
			return (short)a;
		}
			private static long ReadSize(short size, byte[] buf)
			{
				if (size == 2)
				{
					return BitConverter.ToInt16(buf, 0);
				}
				throw new NotImplementedException();
			}

		public static FileHeader Parse(Stream stream)
		{
			var header = new FileHeader();

			stream.ReadShort(out short filenameLen);
			stream.Seek(5, SeekOrigin.Current);

			
			stream.ReadShort(out short bitFlags);

			if (bitFlags != 0)
			{
#pragma warning disable IDE0059
				stream.ReadShort(out short compMethodVal);

				stream.ReadInt(out int crc);
				var rfs = GetReadFileSize(bitFlags);
				stream.ReadN(rfs, out var fsBuf);
				header.CompressedSize = ReadSize(rfs, fsBuf);
				stream.ReadN(rfs, out fsBuf);
				header.UncompressedSize = ReadSize(rfs, fsBuf);
			}

			stream.ReadN(filenameLen, out var filenameBuffer);

			header.Name = System.Text.Encoding.UTF8.GetString(filenameBuffer);
			header.StartPosition = stream.Position;

			return header;
		}
	}
}
