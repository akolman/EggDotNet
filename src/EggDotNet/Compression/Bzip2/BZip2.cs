using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Compression.Bzip2
{
	/// <summary>
	/// An example class to demonstrate compression and decompression of BZip2 streams.
	/// </summary>
	internal static class BZip2
	{
		/// <summary>
		/// Decompress the <paramref name="inStream">input</paramref> writing
		/// uncompressed data to the <paramref name="outStream">output stream</paramref>
		/// </summary>
		/// <param name="inStream">The readable stream containing data to decompress.</param>
		/// <param name="outStream">The output stream to receive the decompressed data.</param>
		/// <param name="isStreamOwner">Both streams are closed on completion if true.</param>
		public static void Decompress(Stream inStream, Stream outStream, bool isStreamOwner)
		{
			if (inStream == null)
				throw new ArgumentNullException(nameof(inStream));

			if (outStream == null)
				throw new ArgumentNullException(nameof(outStream));

			try
			{
				using (BZip2InputStream bzipInput = new BZip2InputStream(inStream))
				{
					bzipInput.IsStreamOwner = isStreamOwner;
					Copy(bzipInput, outStream, new byte[4096]);
				}
			}
			finally
			{
				if (isStreamOwner)
				{
					// inStream is closed by the BZip2InputStream if stream owner
					outStream.Dispose();
				}
			}
		}

		/// <summary>
		/// Compress the <paramref name="inStream">input stream</paramref> sending
		/// result data to <paramref name="outStream">output stream</paramref>
		/// </summary>
		/// <param name="inStream">The readable stream to compress.</param>
		/// <param name="outStream">The output stream to receive the compressed data.</param>
		/// <param name="isStreamOwner">Both streams are closed on completion if true.</param>
		/// <param name="level">Block size acts as compression level (1 to 9) with 1 giving
		/// the lowest compression and 9 the highest.</param>
		public static void Compress(Stream inStream, Stream outStream, bool isStreamOwner, int level)
		{
			if (inStream == null)
				throw new ArgumentNullException(nameof(inStream));

			if (outStream == null)
				throw new ArgumentNullException(nameof(outStream));

			try
			{
				using (BZip2OutputStream bzipOutput = new BZip2OutputStream(outStream, level))
				{
					bzipOutput.IsStreamOwner = isStreamOwner;
					Copy(inStream, bzipOutput, new byte[4096]);
				}
			}
			finally
			{
				if (isStreamOwner)
				{
					// outStream is closed by the BZip2OutputStream if stream owner
					inStream.Dispose();
				}
			}
		}

		public static void Copy(Stream source, Stream destination, byte[] buffer)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (destination == null)
			{
				throw new ArgumentNullException(nameof(destination));
			}

			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

			// Ensure a reasonable size of buffer is used without being prohibitive.
			if (buffer.Length < 128)
			{
				throw new ArgumentException("Buffer is too small", nameof(buffer));
			}

			bool copying = true;

			while (copying)
			{
				int bytesRead = source.Read(buffer, 0, buffer.Length);
				if (bytesRead > 0)
				{
					destination.Write(buffer, 0, bytesRead);
				}
				else
				{
					destination.Flush();
					copying = false;
				}
			}
		}
	}
}
