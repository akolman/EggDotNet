using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA5379

namespace EggDotNet.Encryption.Aes
{
	[ExcludeFromCodeCoverage]
	internal sealed class EggAesCrypto
	{
		private const int Rfc2898KeygenIterations = 1000;

		internal byte[] _Salt;
		internal byte[] _providedPv;
		internal byte[] _generatedPv;
		internal int _KeyStrengthInBits;
		private byte[] _MacInitializationVector;
		private byte[] _keyBytes;
		private short PasswordVerificationStored;
		private short PasswordVerificationGenerated;
		private readonly string _Password;
		private bool _cryptoGenerated;

		private EggAesCrypto(string password, int keyStrengthInBits)
		{
			_Password = password;
			_KeyStrengthInBits = keyStrengthInBits;
		}

		public static EggAesCrypto ReadFromStream(string password, int keyStrengthInBits, byte[] salt, byte[] pwV)
		{
			var c = new EggAesCrypto(password, keyStrengthInBits)
			{
				_Salt = salt,
				_providedPv = pwV
			};

			c.PasswordVerificationStored = (short)(c._providedPv[0] + c._providedPv[1] * keyStrengthInBits);

			if (password != null)
			{
				c.PasswordVerificationGenerated = (short)(c.GeneratedPV[0] + c.GeneratedPV[1] * keyStrengthInBits);
			}

			return c;
		}

		public bool PasswordValid => PasswordVerificationGenerated == PasswordVerificationStored;

		public byte[] GeneratedPV
		{
			get
			{
				if (!_cryptoGenerated) _GenerateCryptoBytes();
				return _generatedPv;
			}
		}

		public byte[] Salt => _Salt;

		public byte[] KeyBytes
		{
			get
			{
				if (!_cryptoGenerated) _GenerateCryptoBytes();
				return _keyBytes;
			}
		}

		public byte[] MacIv
		{
			get
			{
				if (!_cryptoGenerated) _GenerateCryptoBytes();
				return _MacInitializationVector;
			}
		}

		public int SizeOfEncryptionMetadata => _KeyStrengthInBytes / 2 + 10 + 2;

		private int _KeyStrengthInBytes => _KeyStrengthInBits / 8;

		private void _GenerateCryptoBytes()
		{
			using (var rfc2898 = new System.Security.Cryptography.Rfc2898DeriveBytes(_Password, Salt, Rfc2898KeygenIterations))
			{
				_keyBytes = rfc2898.GetBytes(_KeyStrengthInBytes);
				_MacInitializationVector = rfc2898.GetBytes(_KeyStrengthInBytes);
				_generatedPv = rfc2898.GetBytes(2);
			}

			_cryptoGenerated = true;
		}
	}
}
