using System;
using System.Linq;
using System.Security.Cryptography;

namespace EggDotNet.Encryption.Lea.Imp
{
	internal sealed class Lea : SymmetricAlgorithm
	{
		private bool disposed;

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
			_salt = salt.Take(keySizeBits == 256 ? 16 : 8).ToArray();

#pragma warning disable CA5379
			var rfc2898 = new Rfc2898DeriveBytes(password, _salt, 1000);
#pragma warning restore CA5379

			_keyBytes = rfc2898.GetBytes(keySizeBits / 8); // 16 or 24 or 32 ???
			_MacInitializationVector = rfc2898.GetBytes(keySizeBits == 256 ? 32 : 16).Take(keySizeBits == 256 ? 16 : 8).ToArray();
			_generatedPv = rfc2898.GetBytes(2);
			_storedPv = salt.Skip(keySizeBits == 256 ? 16 : 8).Take(2).ToArray();
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

#pragma warning disable CA2215
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!disposed)
				{
					Array.Clear(_salt, 0, _salt.Length);
					Array.Clear(_MacInitializationVector,0 , _MacInitializationVector.Length);
					Array.Clear(_keyBytes, 0, _keyBytes.Length);
					Array.Clear(_generatedPv, 0, _generatedPv.Length);
					Array.Clear(_storedPv, 0, _storedPv.Length);

					_salt = null;
					_MacInitializationVector = null;
					_keyBytes = null;
					_generatedPv = null;
					_storedPv = null;
				}

				disposed = false;
			}
		}
	}
}
