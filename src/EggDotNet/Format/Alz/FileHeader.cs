using EggDotNet.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace EggDotNet.Format.Alz
{
	internal sealed class FileHeader
	{
		public const int ALZ_FILE_HEADER_START_MAGIC = 0x015A4C42;

		public const int ALZ_FILE_HEADER_END_MAGIC = 0x025A4C43;

		public CompressionMethod CompressionMethod { get; private set; } = CompressionMethod.Store;

		public long CompressedSize { get; private set; }

		public long UncompressedSize { get; private set; }

		public uint Crc32 { get; private set; }

		public string Name { get; private set; }

		public long StartPosition { get; private set; }

		public DateTime LastWriteTime { get; private set; }

		private FileHeader()
		{

		}

		/*
		private FileHeader(CompressionMethod compressionMethod, long compressedSize, long uncompressedSize, uint crc32, string name, long startPos)
		{
			CompressionMethod = compressionMethod;
			CompressedSize = compressedSize;
			UncompressedSize = uncompressedSize;
			Crc32 = crc32;
			Name = name;
			StartPosition = startPos;
		}*/

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FileHeader Parse(Stream stream)
		{
			var header = new FileHeader();

			stream.ReadShort(out short filenameLen);
			stream.ReadByte(out byte attributes);
			_ = attributes; //TODO
			stream.ReadUInt(out uint moddate);

			header.LastWriteTime = Utilities.FromAlzTime(moddate);
			//header.La = Utilities.FromAlzTime(moddate);
			
			//stream.Seek(5, SeekOrigin.Current);

			stream.ReadShort(out short bitFlags);

			if (bitFlags != 0)
			{
				stream.ReadShort(out short compMethodVal);
				header.CompressionMethod = compMethodVal == 2 ? CompressionMethod.Deflate : CompressionMethod.Store;
				stream.ReadUInt(out uint crc);
				header.Crc32 = crc;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static short GetReadFileSize(short bitflags)
		{
			var a = (bitflags & 0xF0) >> 4;
			return (short)a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long ReadSize(short size, byte[] buf)
		{
			switch (size)
			{
				case 1:
					return buf[0];
				case 2:
					return BitConverter.ToInt16(buf, 0);
				case 4:
					return BitConverter.ToInt32(buf, 0);
				case 8:
					return BitConverter.ToInt64(buf, 0);
				default:
					throw new InvalidDataException($"Invalid file size descriptor ({size})");
			};
		}
	}
}
