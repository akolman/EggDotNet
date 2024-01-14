using EggDotNet.InternalExtensions;
using System;
using System.IO;

#if NETSTANDARD2_0
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
#endif

namespace EggDotNet.Format.Egg
{
    internal sealed class EncryptHeader
	{
		public const int EGG_ENCRYPT_HEADER_MAGIC = 0x08D1470F;

		public EncryptionMethod EncryptionMethod { get; private set; }
		public short Size { get; private set; }

		public byte[] Param1 { get; private set; }

		public byte[] Param2 { get; private set; }

#if NETSTANDARD2_1_OR_GREATER
		public EncryptHeader(EncryptionMethod encryptionMethod, short size, Span<byte> aesHeader, Span<byte> aesFooter)
		{
			EncryptionMethod = encryptionMethod;
			Size = size;
			Param1 = aesHeader.ToArray();
			Param2 = aesFooter.ToArray();
		}
#else
		public EncryptHeader(EncryptionMethod encryptionMethod, short size, byte[] aesHeader, byte[] aesFooter)
		{
			EncryptionMethod = encryptionMethod;
			Size = size;
			Param1 = aesHeader;
			Param2 = aesFooter;
		}
#endif


		public static EncryptHeader Parse(Stream stream)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> encryptHeaderBuffer = stackalloc byte[3];
#else
			var encryptHeaderBuffer = new byte[3];
#endif

			if (stream.Read(encryptHeaderBuffer) != 3)
			{
				Console.Error.WriteLine("Could not read encrypt header size");
				return null;
			}

			var size = BitConverter.ToInt16(encryptHeaderBuffer.Slice(1, 2));

#if NETSTANDARD2_1_OR_GREATER
			Span<byte> encDataBuffer = stackalloc byte[size];
#else
			var encDataBuffer = new byte[size];
#endif
			if (stream.Read(encDataBuffer) != size)
			{
				Console.Error.WriteLine("Could not read encryption header");
				return null;
			}

			var encMethod = (EncryptionMethod)encDataBuffer[0];

			if (encMethod == EncryptionMethod.AES128)
			{
				var aesHeader = encDataBuffer.Slice(1, 10);
				var aesFooter = encDataBuffer.Slice(11, 10);
				return new EncryptHeader(encMethod, size, aesHeader, aesFooter);
			}
			else if(encMethod == EncryptionMethod.AES256)
			{
				var aesHeader = encDataBuffer.Slice(1, 18);
				var aesFooter = encDataBuffer.Slice(19, 10);
				return new EncryptHeader(encMethod, size, aesHeader, aesFooter);
			}
			else if(encMethod == EncryptionMethod.LEA128)
			{
				var leaHeader = encDataBuffer.Slice(1, 10);
				var leaFooter = encDataBuffer.Slice(11, 10);
				return new EncryptHeader(encMethod, size, leaHeader, leaFooter);
			}
			else if(encMethod == EncryptionMethod.LEA256)
			{
				var leaHeader = encDataBuffer.Slice(1, 18);
				var leaFooter = encDataBuffer.Slice(19, 10);
				return new EncryptHeader(encMethod, size, leaHeader, leaFooter);
			}
			else if (encMethod == EncryptionMethod.Standard)
			{
				var standardHeader = encDataBuffer.Slice(1, 12);
				var pwData = encDataBuffer.Slice(13, 4);
				return new EncryptHeader(encMethod, size, standardHeader, pwData);
			}
			else
			{
				throw new System.NotImplementedException("Not implemented");
			}
		}
	}
}
