using System;
using System.Security.Cryptography;

namespace EggDotNet.Encryption.Lea.Imp
{
	internal sealed class CTRModeLeaTransformer : LeaCryptoTransform
	{
		private readonly byte[] _iv;
		private readonly byte[] _ctr;
		private readonly byte[] _block;

		public CTRModeLeaTransformer(CryptoStreamMode mode, byte[] key, byte[] iv)
			: base(mode, key)
		{
			_iv = iv;
			_ctr = new byte[BLOCK_SIZE_BYTES];
			_block = new byte[BLOCK_SIZE_BYTES];
			Array.Copy(_iv, _ctr, BLOCK_SIZE_BYTES);
		}


		public override int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			var len = base.TransformBlock(_ctr, 0, inputCount, _block, 0);
			addCounter();
			XOR(outputBuffer, outputOffset, inputBuffer, inputOffset, _block, 0, len);
			return len;
		}

		public override byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			var output = base.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
			addCounter();
			XOR(output, 0, inputBuffer, inputOffset, _block, 0, output.Length);
			return output;
		}

		private void addCounter()
		{
			for (int i = _ctr.Length - 1; i >= 0; --i)
			{
				if (++_ctr[i] != 0)
				{
					break;
				}
			}
		}
	}
}
