namespace EggDotNet.Compression.AZO.Decoder
{
	internal class SymbolCode
	{
		private readonly HistoryList history;
		private readonly EntropyBitProb prob;

		public SymbolCode(uint n, uint historyN)
		{
			history = new HistoryList(historyN);
			prob = new EntropyBitProb(n);
		}

		public uint Code(EntropyCode entropy)
		{
			uint ret = 0;
			if (!history.Code(entropy, ref ret))
			{
				ret = prob.Code(entropy);
				history.Add(ref ret);
			}

			return ret;
		}
	}
}
