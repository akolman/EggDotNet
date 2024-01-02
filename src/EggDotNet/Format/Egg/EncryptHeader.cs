using EggDotNet.Extensions;
using System;
using System.IO;
using System.Linq;

namespace EggDotNet.Format.Egg
{
	internal sealed class EncryptHeader
	{
		public const int EGG_ENCRYPT_HEADER_MAGIC = 0x08D1470F;

		public EncryptionMethod EncryptionMethod { get; private set; }
		public short Size { get; private set; }

		public byte[] Param1 { get; private set; }

		public byte[] Param2 { get; private set; }

		public EncryptHeader(EncryptionMethod encryptionMethod, short size, byte[] aesHeader, byte[] aesFooter)
		{
			EncryptionMethod = encryptionMethod;
			Size = size;
			Param1 = aesHeader;
			Param2 = aesFooter;
		}

		public static EncryptHeader Parse(Stream stream)
		{
			if (stream.ReadByte() == -1)
			{

			}

			if (!stream.ReadShort(out short size))
			{
				Console.Error.WriteLine("Could not read encrypt header size");
				return null;
			}

			var encDataBuffer = new byte[size];
			if (stream.Read(encDataBuffer, 0, size) != size)
			{
				Console.Error.WriteLine("Could not read encryption header");
				return null;
			}

			var encMethod = (EncryptionMethod)encDataBuffer[0];

			if (encMethod == EncryptionMethod.AES128)
			{
				var aesHeader = encDataBuffer.Skip(1).Take(10).ToArray();
				var aesFooter = encDataBuffer.Skip(11).Take(10).ToArray();
				return new EncryptHeader(encMethod, size, aesHeader, aesFooter);
			}
			else if(encMethod == EncryptionMethod.AES256)
			{
				var aesHeader = encDataBuffer.Skip(1).Take(18).ToArray();
				var aesFooter = encDataBuffer.Skip(19).Take(10).ToArray();
				return new EncryptHeader(encMethod, size, aesHeader, aesFooter);
			}
			else if (encMethod == EncryptionMethod.Standard)
			{
				var standardHeader = encDataBuffer.Skip(1).Take(12).ToArray();
				var pwData = encDataBuffer.Skip(13).Take(4).ToArray();
				return new EncryptHeader(encMethod, size, standardHeader, pwData);
			}
			else
			{
				throw new System.NotImplementedException("Not implemented");
			}
		}
	}
}
