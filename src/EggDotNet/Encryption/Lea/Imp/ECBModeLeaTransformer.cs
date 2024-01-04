using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EggDotNet.Encryption.Lea.Imp
{
	internal class ECBModeLeaTransformer : LeaCryptoTransform
	{
		public ECBModeLeaTransformer(CryptoStreamMode mode, byte[] key)
			: base(mode, key)
		{
		}
	}
}
