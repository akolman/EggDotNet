using System;
using System.Linq;
using System.Security.Cryptography;

namespace EggDotNet.Encryption.Lea.Imp
{


	internal class Lea : SymmetricAlgorithm
	{

		public override int BlockSize { get; set; }

		public override byte[] IV
		{
			get
			{
				return _MacInitializationVector;
			}
			set
			{
				_MacInitializationVector = value;
			}
		}

		public override byte[] Key
		{
			get
			{
				return _keyBytes;
			}
			set
			{
				_keyBytes = value;
			}
		}

		public override int KeySize { get; set; }

		public override CipherMode Mode { get; set; }

		private byte[] _salt;
		private byte[] _keyBytes;
		private byte[] _MacInitializationVector;
		private byte[] _generatedPv;

		public Lea()
		{

		}

		public Lea(int keySizeBits, string password, byte[] salt)
		{
			_salt = salt;

			System.Security.Cryptography.Rfc2898DeriveBytes rfc2898 =
					new System.Security.Cryptography.Rfc2898DeriveBytes(password, _salt, 1000);

			_keyBytes = rfc2898.GetBytes(keySizeBits / 8); // 16 or 24 or 32 ???
			_MacInitializationVector = rfc2898.GetBytes(32).Take(16).ToArray();
			_generatedPv = rfc2898.GetBytes(2);

			//_cryptoGenerated = true;
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			if (Mode == CipherMode.ECB)
			{
				return new ECBModeLeaTransformer(CryptoStreamMode.Read, rgbKey);
			}
			throw new NotImplementedException();
		}

		public ICryptoTransform CreateDecryptor(string mode, byte[] rgbKey, byte[] rgbIV)
		{
			if (mode == "CTR")
			{
				return new CTRModeLeaTransformer(CryptoStreamMode.Write, rgbKey, rgbIV);
			}
			else if (mode == "ECB")
			{
				return new ECBModeLeaTransformer(CryptoStreamMode.Read, rgbKey);
			}

			throw new NotImplementedException();
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			throw new NotImplementedException();
		}

		public override void GenerateIV()
		{
			throw new NotImplementedException();
		}

		public override void GenerateKey()
		{
			throw new NotImplementedException();
		}
	}

}
