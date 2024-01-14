using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EggDotNet.Encryption.Lea.Imp
{
	internal abstract class LeaCryptoTransform : ICryptoTransform
	{
		public const int BLOCK_SIZE_BYTES = 16;
		private static uint[] delta = new uint[] { 0xc3efe9db, 0x44626b02, 0x79e27c8a, 0x78df30ec, 0x715ea49e, 0xc785da0a, 0xe04ef22a, 0xe5c40957 };

		private readonly uint[] _block = new uint[BLOCK_SIZE_BYTES / 4];
		private uint[,] _roundKeys;
		private int rounds;

		private bool disposedValue;

		public bool CanReuseTransform => true;

		public bool CanTransformMultipleBlocks => false;

		public int InputBlockSize => BLOCK_SIZE_BYTES;

		public int OutputBlockSize => BLOCK_SIZE_BYTES;

		private CryptoStreamMode _mode;

		public LeaCryptoTransform(CryptoStreamMode mode, byte[] key)
		{
			_mode = mode;
			GenerateRoundKeys(key);
		}


		public virtual int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			if (_mode == CryptoStreamMode.Read)
			{
				return DecryptBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);
			}
			else
			{
				return EncryptBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);
			}
		}

		public virtual byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			var output = new byte[BLOCK_SIZE_BYTES];
			if (_mode == CryptoStreamMode.Read)
			{
				DecryptBlock(inputBuffer, inputOffset, output, 0);
			}
			else
			{
				EncryptBlock(inputBuffer, inputOffset, output, 0);
			}
			return output;
		}

		protected static void XOR(byte[] lhs, int lhsOff, byte[] rhs, int rhsOff, int len)
		{
			for (int i = 0; i < len; ++i)
			{
				lhs[lhsOff + i] ^= rhs[rhsOff + i];
			}
		}

		protected static void XOR(byte[] outBuffer, int outOff, byte[] lhs, int lhs1Off, byte[] rhs, int rhsOff, int len)
		{
			for (int i = 0; i < len; ++i)
			{
				outBuffer[outOff + i] = (byte)(lhs[lhs1Off + i] ^ rhs[rhsOff + i]);
			}
		}

		private int DecryptBlock(byte[] inBuf, int inOff, byte[] outBuf, int outOff)
		{
			pack(inBuf, inOff, _block, 0, 16);

			for (int i = this.rounds - 1; i >= 0; --i)
			{
				_block[0] = (ROR(_block[0], 9) - (_block[3] ^ _roundKeys[i, 0])) ^ _roundKeys[i, 1];
				_block[1] = (ROL(_block[1], 5) - (_block[0] ^ _roundKeys[i, 2])) ^ _roundKeys[i, 3];
				_block[2] = (ROL(_block[2], 3) - (_block[1] ^ _roundKeys[i, 4])) ^ _roundKeys[i, 5];
				--i;

				_block[3] = (ROR(_block[3], 9) - (_block[2] ^ _roundKeys[i, 0])) ^ _roundKeys[i, 1];
				_block[0] = (ROL(_block[0], 5) - (_block[3] ^ _roundKeys[i, 2])) ^ _roundKeys[i, 3];
				_block[1] = (ROL(_block[1], 3) - (_block[0] ^ _roundKeys[i, 4])) ^ _roundKeys[i, 5];
				--i;

				_block[2] = (ROR(_block[2], 9) - (_block[1] ^ _roundKeys[i, 0])) ^ _roundKeys[i, 1];
				_block[3] = (ROL(_block[3], 5) - (_block[2] ^ _roundKeys[i, 2])) ^ _roundKeys[i, 3];
				_block[0] = (ROL(_block[0], 3) - (_block[3] ^ _roundKeys[i, 4])) ^ _roundKeys[i, 5];
				--i;

				_block[1] = (ROR(_block[1], 9) - (_block[0] ^ _roundKeys[i, 0])) ^ _roundKeys[i, 1];
				_block[2] = (ROL(_block[2], 5) - (_block[1] ^ _roundKeys[i, 2])) ^ _roundKeys[i, 3];
				_block[3] = (ROL(_block[3], 3) - (_block[2] ^ _roundKeys[i, 4])) ^ _roundKeys[i, 5];
			}

			unpack(_block, 0, outBuf, outOff, 4);

			return BLOCK_SIZE_BYTES;
		}

		private int EncryptBlock(byte[] inBuf, int inOff, byte[] outBuf, int outOff)
		{
			pack(inBuf, inOff, this._block, 0, 16);

			for (int i = 0; i < this.rounds; ++i)
			{
				_block[3] = ROR((_block[2] ^ _roundKeys[i, 4]) + (_block[3] ^ _roundKeys[i, 5]), 3);
				_block[2] = ROR((_block[1] ^ _roundKeys[i, 2]) + (_block[2] ^ _roundKeys[i, 3]), 5);
				_block[1] = ROL((_block[0] ^ _roundKeys[i, 0]) + (_block[1] ^ _roundKeys[i, 1]), 9);
				++i;

				_block[0] = ROR((_block[3] ^ _roundKeys[i, 4]) + (_block[0] ^ _roundKeys[i, 5]), 3);
				_block[3] = ROR((_block[2] ^ _roundKeys[i, 2]) + (_block[3] ^ _roundKeys[i, 3]), 5);
				_block[2] = ROL((_block[1] ^ _roundKeys[i, 0]) + (_block[2] ^ _roundKeys[i, 1]), 9);

				++i;
				_block[1] = ROR((_block[0] ^ _roundKeys[i, 4]) + (_block[1] ^ _roundKeys[i, 5]), 3);
				_block[0] = ROR((_block[3] ^ _roundKeys[i, 2]) + (_block[0] ^ _roundKeys[i, 3]), 5);
				_block[3] = ROL((_block[2] ^ _roundKeys[i, 0]) + (_block[3] ^ _roundKeys[i, 1]), 9);

				++i;
				_block[2] = ROR((_block[1] ^ _roundKeys[i, 4]) + (_block[2] ^ _roundKeys[i, 5]), 3);
				_block[1] = ROR((_block[0] ^ _roundKeys[i, 2]) + (_block[1] ^ _roundKeys[i, 3]), 5);
				_block[0] = ROL((_block[3] ^ _roundKeys[i, 0]) + (_block[0] ^ _roundKeys[i, 1]), 9);
			}

			unpack(_block, 0, outBuf, outOff, 4);

			return BLOCK_SIZE_BYTES;
		}

		private static uint ROL(uint state, uint num)
		{
			return ((state << (int)num) | (state >> (int)(32 - num)));
		}

		private static uint ROR(uint state, uint num)
		{
			return ((state >> (int)num) | (state << (int)(32 - num)));
		}

		private static void pack(byte[] inBuffer, int inOffset, uint[] outBuffer, int outOffset, int inLength)
		{
			int outIdx = outOffset;
			int endInIdx = inOffset + inLength;
			for (int inIdx = inOffset; inIdx < endInIdx; ++inIdx, ++outIdx)
			{
				outBuffer[outIdx] = (uint)(inBuffer[inIdx] & 0xff);
				outBuffer[outIdx] |= (uint)(inBuffer[++inIdx] & 0xff) << 8;
				outBuffer[outIdx] |= (uint)(inBuffer[++inIdx] & 0xff) << 16;
				outBuffer[outIdx] |= (uint)(inBuffer[++inIdx] & 0xff) << 24;
			}
		}

		private static void unpack(uint[] inBuffer, int inOffset, byte[] outBuffer, int outOffset, int inLength)
		{
			int outIdx = outOffset;
			int endInIdx = inOffset + inLength;
			for (int inIdx = inOffset; inIdx < endInIdx; ++inIdx, ++outIdx)
			{
				outBuffer[outIdx] = (byte)inBuffer[inIdx];
				outBuffer[++outIdx] = (byte)(inBuffer[inIdx] >> 8);
				outBuffer[++outIdx] = (byte)(inBuffer[inIdx] >> 16);
				outBuffer[++outIdx] = (byte)(inBuffer[inIdx] >> 24);
			}
		}

		private void GenerateRoundKeys(byte[] mk)
		{
			uint[] T = new uint[8];

			this.rounds = (mk.Length >> 1) + 16;
			this._roundKeys = new uint[this.rounds, 6];

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

					this._roundKeys[i, 0] = T[0] = ROL(T[0] + ROL(temp, 0), 1);
					this._roundKeys[i, 1] = this._roundKeys[i, 3] = this._roundKeys[i, 5] = T[1] = ROL(T[1] + ROL(temp, 1), 3);
					this._roundKeys[i, 2] = T[2] = ROL(T[2] + ROL(temp, 2), 6);
					this._roundKeys[i, 4] = T[3] = ROL(T[3] + ROL(temp, 3), 11);
				}

			}
			else if (mk.Length == 24)
			{
				for (int i = 0; i < 28; ++i)
				{
					uint temp = ROL(delta[i % 6], (uint)i);

					this._roundKeys[i, 0] = T[0] = ROL(T[0] + ROL(temp, 0), 1);
					this._roundKeys[i, 1] = T[1] = ROL(T[1] + ROL(temp, 1), 3);
					this._roundKeys[i, 2] = T[2] = ROL(T[2] + ROL(temp, 2), 6);
					this._roundKeys[i, 3] = T[3] = ROL(T[3] + ROL(temp, 3), 11);
					this._roundKeys[i, 4] = T[4] = ROL(T[4] + ROL(temp, 4), 13);
					this._roundKeys[i, 5] = T[5] = ROL(T[5] + ROL(temp, 5), 17);
				}

			}
			else
			{
				for (int i = 0; i < 32; ++i)
				{
					uint temp = ROL(delta[i & 7], (uint)i & 0x1f);

					this._roundKeys[i, 0] = T[(6 * i + 0) & 7] = ROL(T[(6 * i + 0) & 7] + temp, 1);
					this._roundKeys[i, 1] = T[(6 * i + 1) & 7] = ROL(T[(6 * i + 1) & 7] + ROL(temp, 1), 3);
					this._roundKeys[i, 2] = T[(6 * i + 2) & 7] = ROL(T[(6 * i + 2) & 7] + ROL(temp, 2), 6);
					this._roundKeys[i, 3] = T[(6 * i + 3) & 7] = ROL(T[(6 * i + 3) & 7] + ROL(temp, 3), 11);
					this._roundKeys[i, 4] = T[(6 * i + 4) & 7] = ROL(T[(6 * i + 4) & 7] + ROL(temp, 4), 13);
					this._roundKeys[i, 5] = T[(6 * i + 5) & 7] = ROL(T[(6 * i + 5) & 7] + ROL(temp, 5), 17);
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~LeaCryptoTransform()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
