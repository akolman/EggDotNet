namespace EggDotNet
{
	/// <summary>
	/// Represents the compression method used for an entry.
	/// </summary>
	public enum CompressionMethod : byte
	{
		/// <summary>
		/// Store compression (no compression).
		/// </summary>
		Store = 0,

		/// <summary>
		/// Deflate compression.
		/// </summary>
		Deflate = 1,

		/// <summary>
		/// BZip2 compression.
		/// </summary>
		Bzip2 = 2,

		/// <summary>
		/// AZO compression.
		/// </summary>
		Azo = 3,

		/// <summary>
		/// LZMA compression.
		/// </summary>
		Lzma = 4
	}
}
