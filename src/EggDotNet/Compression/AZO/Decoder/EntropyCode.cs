namespace EggDotNet.Compression.AZO.Decoder
{
	internal class EntropyCode
	{
		private uint low_ = uint.MinValue;
		private uint up_ = uint.MaxValue;
		private uint tag_;
		private readonly uint MSB = int.MaxValue + 1u;
		private readonly uint sMSB;
		private readonly BitCode bit;
		
		public EntropyCode(byte[] buf, uint size)
		{
			bit = new BitCode(buf, size);
			sMSB = 1 << 30;
		}

		public void Initialize()
		{
			uint s = sizeof(uint) / sizeof(byte);
			for(var i=0; i<s; ++i)
			{
				uint b = 0;
				byte bb = (byte)b;
				bit.Code(ref bb, 8);
				tag_ |= (uint)( bb << (byte)((s - 1 - i) * 8));
			}
		}

		private void Rescale()
		{
			while ((low_ & MSB) == (up_ & MSB))
			{
				bool b = false;
				bit.Code(ref b);
				tag_ <<= 1;
				tag_ |= (b ? (uint)1 : 0);

				low_ <<= 1;
				up_ <<= 1;
				up_ |= 1;
			}
			while ((low_ & sMSB) != 0 && (up_ & sMSB) == 0)
			{
				bool b = false;
				bit.Code(ref b);
				tag_ <<= 1;
				tag_ |= (b ? (uint)1 : 0);
				tag_ ^= (uint)MSB;

				low_ <<= 1;
				low_ &= (uint)(MSB - 1);
				up_ <<= 1; 
				up_ |= 1;
				up_ |= (uint)MSB;
			}
		}

		public uint Code(uint totalBit)
		{
			uint t;
			if (low_ == uint.MinValue && up_ == uint.MaxValue)
			{
				t = (uint)(1 << (int)(32u - totalBit));
			}
			else
			{
				t = (up_ - low_ + 1) >> (int)totalBit;
			}

			uint v = (tag_ - low_) / t;

			up_ = low_ + t * (v + 1) - 1;
			low_ += t * v;

			Rescale();
			return v;
		}

		public bool Code(uint cumCount, int totalBit)
		{
			uint t;
			if (low_ == uint.MinValue && up_ == uint.MaxValue)
			{
				t = (uint)((int)1 << (int)(32 - totalBit));
			}
			else
			{
				t = (up_ - low_ + 1) >> (int)totalBit;
			}

			uint v = (tag_ - low_) / t;

			if (v >= cumCount)
			{
				low_ += t * cumCount;
			}
			else
			{
				up_ = low_ + t * cumCount - 1;

			}

			Rescale();
			return v >= cumCount;
		}
	}
}
