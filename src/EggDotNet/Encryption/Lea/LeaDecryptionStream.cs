using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EggDotNet.Encryption.Lea
{
	public class LeaDecryptionStream : Stream
	{
		private int rounds;
		internal byte[] _Salt;
		protected uint[,] roundKeys;
		private uint[] block;
		internal int _KeyStrengthInBits;
		private byte[] _MacInitializationVector;
		private byte[] _StoredMac;
		private byte[] _keyBytes;
		private const int Rfc2898KeygenIterations = 1000;
		internal byte[] _generatedPv;
		private bool _cryptoGenerated;
		private Stream _stream;
		private string _Password;
		private static uint[] delta = new uint[] { 0xc3efe9db, 0x44626b02, 0x79e27c8a, 0x78df30ec, 0x715ea49e, 0xc785da0a, 0xe04ef22a, 0xe5c40957 };

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => throw new NotImplementedException();

		public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public LeaDecryptionStream(string password, byte[] header, byte[] footer)
		{
			_Salt = header.Take(16).ToArray();
			block = new uint[16 / 4];
			_KeyStrengthInBits = 256;
			_Password = password;
			_GenerateCryptoBytes();

			var a = header[16] + header[17] * 256;
			var b = _generatedPv[0] + _generatedPv[1] * 256;

			GenerateRoundKeys(_keyBytes);

			Array.Copy(_MacInitializationVector, ctr, 16);
			Array.Copy(_MacInitializationVector, feedback, 16);

		}

		public void AttachStream(Stream stream)
		{
			_stream = stream;
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		private int _KeyStrengthInBytes
		{
			get
			{
				return _KeyStrengthInBits / 8;

			}
		}

		public byte[] Salt
		{
			get
			{
				return _Salt;
			}
		}

		private void _GenerateCryptoBytes()
		{
			System.Security.Cryptography.Rfc2898DeriveBytes rfc2898 =
				new System.Security.Cryptography.Rfc2898DeriveBytes(_Password, Salt, Rfc2898KeygenIterations);

			_keyBytes = rfc2898.GetBytes(_KeyStrengthInBytes); // 16 or 24 or 32 ???
			_MacInitializationVector = rfc2898.GetBytes(32).Take(16).ToArray();
			_generatedPv = rfc2898.GetBytes(2);

			_cryptoGenerated = true;
		}

		private byte[] ctr = new byte[16];
		private byte[] feedback = new byte[16];

		private void addCounter()
		{
			for (int i = ctr.Length - 1; i >= 0; --i)
			{
				if (++ctr[i] != 0)
				{
					break;
				}
			}
		}

		private static void XOR(byte[] lhs, int lhsOff, byte[] rhs, int rhsOff, int len)
		{
			for (int i = 0; i < len; ++i)
			{
				lhs[lhsOff + i] ^= rhs[rhsOff + i];
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var data = new byte[16];
			var readLen = _stream.Read(data, 0, 16);
	
			var outData = new byte[16];
			var block = new byte[16];
			DecryptBlock(data, 0, outData, 0);
			XOR(outData, 0, feedback, 0, 16);

			//var l = DecryptBlock(ctr, 0, block, 0);
			//addCounter();
			//XOR(buffer, 0, data, 0, block, 0, 16);

			return readLen;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		private static void XOR(byte[] outBuf, int outOff, byte[] lhs, int lhs1Off, byte[] rhs, int rhsOff, int len)
		{
			for (int i = 0; i < len; ++i)
			{
				outBuf[outOff + i] = (byte)(lhs[lhs1Off + i] ^ rhs[rhsOff + i]);
			}
		}

		private static uint ROL(uint state, uint num)
		{
			return ((state << (int)num) | (state >>> (int)(32 - num)));
		}

		private static uint ROR(uint state, uint num)
		{
			return ((state >>> (int)num) | (state << (int)(32 - num)));
		}

		private static void pack(byte[] inBuf, int inOff, uint[] outBuf, int outOff, int inlen)
		{
			int outIdx = outOff;
			int endInIdx = inOff + inlen;
			for (int inIdx = inOff; inIdx < endInIdx; ++inIdx, ++outIdx)
			{
				outBuf[outIdx] = (uint)(inBuf[inIdx] & 0xff);
				outBuf[outIdx] |= (uint)(inBuf[++inIdx] & 0xff) << 8;
				outBuf[outIdx] |= (uint)(inBuf[++inIdx] & 0xff) << 16;
				outBuf[outIdx] |= (uint)(inBuf[++inIdx] & 0xff) << 24;
			}
		}

		private static void unpack(uint[] inBuf, int inOff, byte[] outBuf, int outOff, int inlen)
		{
			int outIdx = outOff;
			int endInIdx = inOff + inlen;
			for (int inIdx = inOff; inIdx < endInIdx; ++inIdx, ++outIdx)
			{
				outBuf[outIdx] = (byte)inBuf[inIdx];
				outBuf[++outIdx] = (byte)(inBuf[inIdx] >> 8);
				outBuf[++outIdx] = (byte)(inBuf[inIdx] >> 16);
				outBuf[++outIdx] = (byte)(inBuf[inIdx] >> 24);
			}
		}
	

		private int DecryptBlock(byte[] inBuf, int inOff, byte[] outBuf, int outOff)
		{
			pack(inBuf, inOff, block, 0, 16);

			for (int i = this.rounds - 1; i >= 0; --i)
			{
				block[0] = (ROR(block[0], 9) - (block[3] ^ roundKeys[i,0])) ^ roundKeys[i,1];
				block[1] = (ROL(block[1], 5) - (block[0] ^ roundKeys[i,2])) ^ roundKeys[i,3];
				block[2] = (ROL(block[2], 3) - (block[1] ^ roundKeys[i,4])) ^ roundKeys[i,5];
				--i;

				block[3] = (ROR(block[3], 9) - (block[2] ^ roundKeys[i,0])) ^ roundKeys[i,1];
				block[0] = (ROL(block[0], 5) - (block[3] ^ roundKeys[i,2])) ^ roundKeys[i,3];
				block[1] = (ROL(block[1], 3) - (block[0] ^ roundKeys[i,4])) ^ roundKeys[i,5];
				--i;

				block[2] = (ROR(block[2], 9) - (block[1] ^ roundKeys[i,0])) ^ roundKeys[i,1];
				block[3] = (ROL(block[3], 5) - (block[2] ^ roundKeys[i,2])) ^ roundKeys[i,3];
				block[0] = (ROL(block[0], 3) - (block[3] ^ roundKeys[i,4])) ^ roundKeys[i,5];
				--i;

				block[1] = (ROR(block[1], 9) - (block[0] ^ roundKeys[i,0])) ^ roundKeys[i,1];
				block[2] = (ROL(block[2], 5) - (block[1] ^ roundKeys[i,2])) ^ roundKeys[i,3];
				block[3] = (ROL(block[3], 3) - (block[2] ^ roundKeys[i,4])) ^ roundKeys[i,5];
			}

			unpack(block, 0, outBuf, outOff, 4);

			return 16;
		}


		private void GenerateRoundKeys(byte[] mk)
		{
			uint[] T = new uint[8];

			this.rounds = (mk.Length >> 1) + 16;
			this.roundKeys = new uint[this.rounds, 6];

			pack(mk, 0, T, 0, 16);

			if (mk.Length > 16)
			{
				pack(mk, 16, T, 4, 8);
			}

			if (mk.Length > 24)
			{
				pack(mk, 24, T, 6, 8);
			}

			if (mk.Length == 16)
			{
				for (int i = 0; i < 24; ++i)
				{
					uint temp = ROL(delta[i & 3], (uint)i);

					this.roundKeys[i, 0] = T[0] = ROL(T[0] + ROL(temp, 0), 1);
					this.roundKeys[i, 1] = this.roundKeys[i, 3] = this.roundKeys[i, 5] = T[1] = ROL(T[1] + ROL(temp, 1), 3);
					this.roundKeys[i, 2] = T[2] = ROL(T[2] + ROL(temp, 2), 6);
					this.roundKeys[i, 4] = T[3] = ROL(T[3] + ROL(temp, 3), 11);
				}

			}
			else if (mk.Length == 24)
			{
				for (int i = 0; i < 28; ++i)
				{
					uint temp = ROL(delta[i % 6], (uint)i);

					this.roundKeys[i, 0] = T[0] = ROL(T[0] + ROL(temp, 0), 1);
					this.roundKeys[i, 1] = T[1] = ROL(T[1] + ROL(temp, 1), 3);
					this.roundKeys[i, 2] = T[2] = ROL(T[2] + ROL(temp, 2), 6);
					this.roundKeys[i, 3] = T[3] = ROL(T[3] + ROL(temp, 3), 11);
					this.roundKeys[i, 4] = T[4] = ROL(T[4] + ROL(temp, 4), 13);
					this.roundKeys[i, 5] = T[5] = ROL(T[5] + ROL(temp, 5), 17);
				}

			}
			else
			{
				for (int i = 0; i < 32; ++i)
				{
					uint temp = ROL(delta[i & 7],(uint)i & 0x1f);

					this.roundKeys[i, 0] = T[(6 * i + 0) & 7] = ROL(T[(6 * i + 0) & 7] + temp, 1);
					this.roundKeys[i, 1] = T[(6 * i + 1) & 7] = ROL(T[(6 * i + 1) & 7] + ROL(temp, 1), 3);
					this.roundKeys[i, 2] = T[(6 * i + 2) & 7] = ROL(T[(6 * i + 2) & 7] + ROL(temp, 2), 6);
					this.roundKeys[i, 3] = T[(6 * i + 3) & 7] = ROL(T[(6 * i + 3) & 7] + ROL(temp, 3), 11);
					this.roundKeys[i, 4] = T[(6 * i + 4) & 7] = ROL(T[(6 * i + 4) & 7] + ROL(temp, 4), 13);
					this.roundKeys[i, 5] = T[(6 * i + 5) & 7] = ROL(T[(6 * i + 5) & 7] + ROL(temp, 5), 17);
				}
			}
		}
	}
}
