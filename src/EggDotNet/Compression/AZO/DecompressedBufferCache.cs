using System;
using System.Linq;

namespace EggDotNet.Compression.AZO
{
	internal class DecompressedBufferCache
	{
		private byte[] buffer;
		private int pos;

		public bool IsEmpty => pos == 0;

		public DecompressedBufferCache()
		{
			buffer = new byte[4096];
			pos = 0;
		}

		public void Push(byte[] data)
		{
			if (buffer.Length < buffer.Length + data.Length)
			{
				Array.Resize(ref buffer, buffer.Length + data.Length);
			}

			Array.Copy(data, 0, buffer, pos, data.Length);
			pos += data.Length;
		}

		public int Pop(int count, byte[] destBuffer)
		{
			var t = buffer.Take(count).ToArray();
			count = Math.Min(count, pos);
			Array.Copy(t, destBuffer, count);
			Array.Copy(buffer, count, buffer, 0, buffer.Length - count);
			pos -= count; 
			if (pos < 0) pos = 0;
			return count;
		}
	}
}
