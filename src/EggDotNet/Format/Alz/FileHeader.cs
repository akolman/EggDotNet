using EggDotNet.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;

#if NETSTANDARD2_0
using BitConverter = EggDotNet.Extensions.BitConverterWrapper;
#endif

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

#if NETSTANDARD2_1_OR_GREATER
			Span<byte> fileheaderBuffer = stackalloc byte[9];
#else
			var fileheaderBuffer = new byte[9];
#endif

			if (stream.Read(fileheaderBuffer) != 9)
			{
				throw new InvalidDataException("Failed reading Alz entry header");
			}

			var filenameLen = BitConverter.ToInt16(fileheaderBuffer.Slice(0, 2));
			var attributes = fileheaderBuffer[2];
			_ = attributes; //TODO
			var moddate = Utilities.FromAlzTime(BitConverter.ToUInt32(fileheaderBuffer.Slice(3, 4)));
			var bitFlags = BitConverter.ToInt16(fileheaderBuffer.Slice(7, 2));


			if (bitFlags != 0)
			{
				var rfs = GetReadFileSize(bitFlags);
				var fileinfoBufferLen = rfs * 2 + 6;
#if NETSTANDARD2_1_OR_GREATER
				Span<byte> fileInfoBuffer = stackalloc byte[fileinfoBufferLen];
#else
				var fileInfoBuffer = new byte[fileinfoBufferLen];
#endif
				if (stream.Read(fileInfoBuffer) != fileinfoBufferLen)
				{
					throw new InvalidDataException("Failed reading Alz entry info");
				}

				var compMethodVal = BitConverter.ToInt16(fileInfoBuffer.Slice(0, 2));
				header.CompressionMethod = compMethodVal == 2 ? CompressionMethod.Deflate : CompressionMethod.Store;
				header.Crc32 = BitConverter.ToUInt32(fileInfoBuffer.Slice(2, 4));
				header.CompressedSize = ReadSize(rfs, fileInfoBuffer.Slice(6, rfs));
				header.UncompressedSize = ReadSize(rfs, fileInfoBuffer.Slice(6 + rfs, rfs));
			}

#if NETSTANDARD2_1_OR_GREATER
			Span<byte> filenameBuffer = stackalloc byte[filenameLen];
#else
			var filenameBuffer = new byte[filenameLen];
#endif

			if (stream.Read(filenameBuffer) != filenameLen)
			{
				throw new InvalidDataException("Failed reading Alz entry filename");
			}

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

#if NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long ReadSize(short size, byte[] buf)
		{
			switch (size)
			{
				case 1:
					return buf[0];
				case 2:
					return System.BitConverter.ToInt16(buf, 0);
				case 4:
					return System.BitConverter.ToInt32(buf, 0);
				case 8:
					return System.BitConverter.ToInt64(buf, 0);
				default:
					throw new InvalidDataException($"Invalid file size descriptor ({size})");
			};
		}
#else
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long ReadSize(short size, Span<byte> buf)
		{
			switch (size)
			{
				case 1:
					return buf[0];
				case 2:
					return BitConverter.ToInt16(buf);
				case 4:
					return BitConverter.ToInt32(buf);
				case 8:
					return BitConverter.ToInt64(buf);
				default:
					throw new InvalidDataException($"Invalid file size descriptor ({size})");
			};
		}
#endif
	}
}
