using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Egg
{
	internal class EncryptHeader
	{
		public static readonly int EGG_ENCRYPT_HEADER_MAGIC = 0x08D1470F;

		public EncryptionMethod EncryptionMethod { get; private set; }
		public short Size { get; private set; }

		public byte[] AesHeader { get; private set; }

		public byte[] AesFooter { get; private set; }

		public EncryptHeader(EncryptionMethod encryptionMethod, short size, byte[] aesHeader, byte[] aesFooter)
		{
			EncryptionMethod = encryptionMethod;
			Size = size;
			AesHeader = aesHeader;
			AesFooter = aesFooter;
		}

		public static EncryptHeader Parse(Stream stream)
		{
			if (stream.ReadByte() == -1)
			{

			}

			if (!stream.ReadShort(out short size))
			{

			}

			if (!stream.ReadByte(out var encMethodVal))
			{

			}

			var encMethod = (EncryptionMethod)encMethodVal;
			if (encMethod == EncryptionMethod.AES128)
			{
				throw new System.NotImplementedException("Only AES256");
			}
			else if(encMethod == EncryptionMethod.AES256)
			{
				stream.ReadN(18, out byte[] aesHeader);
				stream.ReadN(10, out byte[] aesFooter);
				return new EncryptHeader(encMethod, size, aesHeader, aesFooter);
			}
			else
			{
				throw new System.NotImplementedException("Only AES256");
			}

		}

	}
}
