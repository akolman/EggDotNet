namespace EggDotNet.Compression.AZO.Decoder
{
	internal class BlockCode
	{
		private readonly AlphaCode alphaProb;
		private readonly byte[] buf;
		private readonly uint bufsize;
		private readonly BoolState matchState;
		private readonly MatchCode matchCode;

		public BlockCode(byte[] buffer, int size)
		{
			buf = buffer;
			bufsize = (uint)size;
			matchState = new BoolState();
			matchCode = new MatchCode(buf, (uint)size);
			alphaProb = new AlphaCode();
		}

		public bool CopyBlock(uint pos, uint dist, uint len)
		{
			if (pos >= dist && pos+len <= bufsize)
			{
				for (var i = 0; i < len; ++i)
				{
					buf[pos + i] = buf[pos - dist + i];
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public uint GetCode(EntropyCode entropy, uint pos)
		{
			if (!matchState.Code(entropy))
			{
				buf[pos] = alphaProb.Code(entropy, pos, buf[pos-1]);

				return 1;
			}
			else
			{
				uint dist = 0;
				uint length = 0;

				matchCode.Code(entropy, pos, ref dist, ref length);

				if (CopyBlock(pos, dist, length))
					return length;
				return 0;
			}

		}

		public int Code(EntropyCode entropy)
		{
			entropy.Initialize();
			int ret = 0;
			buf[0] = alphaProb.Code(entropy, 0, 0);
			for (var i = 1; i < bufsize;)
			{
				uint retLen = GetCode(entropy, (uint)i);
				if (retLen == 0)
				{
					ret = -4;
					break;
				}

				i += (int)retLen;
			}

			return ret;
		}
	}
}
