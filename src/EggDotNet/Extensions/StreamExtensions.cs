﻿using System;
using System.IO;

namespace EggDotNet.Extensions
{
	internal static class StreamExtensions
	{
		public static bool ReadByte(this Stream stream, out byte value)
		{
			var valRead = stream.ReadByte();
			value = (byte)valRead;
			return valRead > -1;
		}

#if NETSTANDARD2_0
		public static int Read(this Stream stream, byte[] buffer)
		{
			return stream.Read(buffer, 0, buffer.Length);
		}
#endif

#if NETSTANDARD2_1_OR_GREATER
		public static bool ReadShort(this Stream stream, out short value)
		{
			Span<byte> buffer = stackalloc byte[2];
			var fullRead = stream.Read(buffer) == 2;
			value = BitConverter.ToInt16(buffer);
			return fullRead;
		}

		public static bool ReadUInt(this Stream stream, out uint value)
		{
			Span<byte> buffer = stackalloc byte[4];
			var fullRead = stream.Read(buffer) == 4;
			value = BitConverter.ToUInt32(buffer);
			return fullRead;
		}

		public static bool ReadInt(this Stream stream, out int value)
		{
			Span<byte> buffer = stackalloc byte[4];
			var fullRead = stream.Read(buffer) == 4;
			value = BitConverter.ToInt32(buffer);
			return fullRead;
		}

		public static bool ReadLong(this Stream stream, out long value)
		{
			Span<byte> buffer = stackalloc byte[8];
			var fullRead = stream.Read(buffer) == 8;
			value = BitConverter.ToInt64(buffer);
			return fullRead;
		}
#else
		public static bool ReadShort(this Stream stream, out short value)
		{
			var buffer = new byte[2];
			var fullRead = stream.Read(buffer, 0, 2) == 2;
			value = BitConverter.ToInt16(buffer, 0);
			return fullRead;
		}

		public static bool ReadUInt(this Stream stream, out uint value)
		{
			var buffer = new byte[4];
			var fullRead = stream.Read(buffer, 0, 4) == 4;
			value = BitConverter.ToUInt32(buffer, 0);
			return fullRead;
		}

		public static bool ReadInt(this Stream stream, out int value)
		{
			var buffer = new byte[4];
			var fullRead = stream.Read(buffer, 0, 4) == 4;
			value = BitConverter.ToInt32(buffer, 0);
			return fullRead;
		}

		public static bool ReadLong(this Stream stream, out long value)
		{
			var buffer = new byte[8];
			var fullRead = stream.Read(buffer, 0, 8) == 8;
			value = BitConverter.ToInt64(buffer, 0);
			return fullRead;
		}
#endif

		public static bool ReadN(this Stream stream, int chars, out byte[] buffer)
		{
			buffer = new byte[chars];
#if NETSTANDARD2_0
			return (stream.Read(buffer, 0, chars) == chars);
#elif NETSTANDARD2_1_OR_GREATER
			return (stream.Read(buffer) == chars);
#endif
		}
	}
}
