using EggDotNet.InternalExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#if NETSTANDARD2_0
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
#endif


namespace EggDotNet.Format.Egg
{
	internal sealed class PosixFileInfo
	{
		public const int POSIX_FILE_INFO_MAGIC_HEADER = 0x1EE922E5;

		public long LastModified { get; private set; }

		public int FileMode { get; private set; }

		public int Uid { get; private set; }

		public int Gid { get; set; }

		public static PosixFileInfo Parse(Stream stream)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> posixFileBuffer = stackalloc byte[23];
#else
			var posixFileBuffer = new byte[23];
#endif
			if (stream.Read(posixFileBuffer) != 23)
			{
				throw new InvalidDataException("Failed reading Posix file header");
			}

			var mode = BitConverter.ToInt32(posixFileBuffer.Slice(3, 4));
			var uid = BitConverter.ToInt32(posixFileBuffer.Slice(7, 4));
			var gid = BitConverter.ToInt32(posixFileBuffer.Slice(11, 4));
			var lastMod = BitConverter.ToInt64(posixFileBuffer.Slice(15, 8));
			return new PosixFileInfo() {FileMode = mode, Uid = uid, Gid = gid, LastModified = lastMod };
		}
	}
}
