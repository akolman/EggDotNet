using System;
using System.Linq;
using System.Security.Cryptography;

namespace EggDotNet.Encryption.Lea.Imp
{
	internal sealed class Lea : SymmetricAlgorithm
	{
		private byte[] _salt;
		private byte[] _keyBytes;
		private byte[] _MacInitializationVector;
		private byte[] _generatedPv;
		private byte[] _storedPv;

		public override int BlockSize { get; set; } = 128;

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

		public bool PasswordValid => Enumerable.SequenceEqual(_generatedPv, _storedPv);

		public override int KeySize { get; set; }

		public override CipherMode Mode { get; set; }

		public Lea(int keySizeBits, string password, byte[] salt)
		{
			_salt = salt.Take(16).ToArray();

#pragma warning disable CA5379 // Ensure Key Derivation Function algorithm is sufficiently strong
			var rfc2898 = new Rfc2898DeriveBytes(password, _salt, 1000);
#pragma warning restore CA5379 // Ensure Key Derivation Function algorithm is sufficiently strong

			_keyBytes = rfc2898.GetBytes(keySizeBits / 8); // 16 or 24 or 32 ???
			_MacInitializationVector = rfc2898.GetBytes(32).Take(16).ToArray();
			_generatedPv = rfc2898.GetBytes(2);
			_storedPv = salt.Skip(16).Take(2).ToArray();
			//_cryptoGenerated = true;
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new CTRModeLeaTransformer(CryptoStreamMode.Write, rgbKey, rgbIV);
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
