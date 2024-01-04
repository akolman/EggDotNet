using System;

#pragma warning disable

namespace EggDotNet.Encryption.Aes
{
	internal class EggAesCrypto
	{
		internal byte[] _Salt;
		internal byte[] _providedPv;
		internal byte[] _generatedPv;
		internal int _KeyStrengthInBits;
		private byte[] _MacInitializationVector;
		private byte[] _StoredMac;
		private byte[] _keyBytes;
		private Int16 PasswordVerificationStored;
		private Int16 PasswordVerificationGenerated;
		private const int Rfc2898KeygenIterations = 1000;
		private string _Password;
		private bool _cryptoGenerated;

		private EggAesCrypto(string password, int KeyStrengthInBits)
		{
			_Password = password;
			_KeyStrengthInBits = KeyStrengthInBits;
		}

		public static EggAesCrypto ReadFromStream(string password, int KeyStrengthInBits, byte[] salt, byte[] pwV)
		{
			EggAesCrypto c = new EggAesCrypto(password, KeyStrengthInBits)
			{
				_Salt = salt,
				_providedPv = pwV
			};

			c.PasswordVerificationStored = (Int16)(c._providedPv[0] + c._providedPv[1] * KeyStrengthInBits);

			if (password != null)
			{
				c.PasswordVerificationGenerated = (Int16)(c.GeneratedPV[0] + c.GeneratedPV[1] * KeyStrengthInBits);
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

		public byte[] Salt
		{
			get
			{
				return _Salt;
			}
		}

		private int _KeyStrengthInBytes
		{
			get
			{
				return _KeyStrengthInBits / 8;

			}
		}

		public int SizeOfEncryptionMetadata
		{
			get
			{
				// 10 bytes after, (n-10) before the compressed data
				return _KeyStrengthInBytes / 2 + 10 + 2;
			}
		}

		public string Password
		{
			set
			{
				_Password = value;
				if (_Password != null)
				{
					PasswordVerificationGenerated = (Int16)(GeneratedPV[0] + GeneratedPV[1] * 256);
					if (PasswordVerificationGenerated != PasswordVerificationStored)
						throw new System.Exception();
				}
			}
			private get
			{
				return _Password;
			}
		}

		private void _GenerateCryptoBytes()
		{
			System.Security.Cryptography.Rfc2898DeriveBytes rfc2898 =
				new System.Security.Cryptography.Rfc2898DeriveBytes(_Password, Salt, Rfc2898KeygenIterations);

			_keyBytes = rfc2898.GetBytes(_KeyStrengthInBytes); // 16 or 24 or 32 ???
			_MacInitializationVector = rfc2898.GetBytes(_KeyStrengthInBytes);
			_generatedPv = rfc2898.GetBytes(2);

			_cryptoGenerated = true;
		}

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

		public byte[] CalculatedMac;

		public void ReadAndVerifyMac(System.IO.Stream s)
		{
			bool invalid = false;

			// read integrityCheckVector.
			// caller must ensure that the file pointer is in the right spot!
			_StoredMac = new byte[10];  // aka "authentication code"
			s.Read(_StoredMac, 0, _StoredMac.Length);

			if (_StoredMac.Length != CalculatedMac.Length)
				invalid = true;

			if (!invalid)
			{
				for (int i = 0; i < _StoredMac.Length; i++)
				{
					if (_StoredMac[i] != CalculatedMac[i])
						invalid = true;
				}
			}

			if (invalid)
				throw new System.Exception("The MAC does not match.");
		}

	}


}
