
using System;
using System.IO;

#pragma warning disable CA2211 // Non-constant fields should not be visible
namespace EggDotNet.Configuration
{
	/// <summary>
	/// Defines configuration parameters used to decompress 'solid' streams.
	/// </summary>
	public static class SolidStreamConfiguration
	{
		/// <summary>
		/// Value representing the number of bytes after which point solid decompression will extract to disk.
		/// </summary>
		public static long SolidDiskBufferCutoff = 128_000_000;

		/// <summary>
		/// Configuration representing how this library will cache decompressed solid data.
		/// </summary>
		public static SolidStreamCacheLocation CacheLocation = SolidStreamCacheLocation.WorkingDirectory;

		/// <summary>
		/// Configuration representing a user-defined path used to decompress solid data.
		/// </summary>
		public static string UserDefinedSolidCacheDirectory = string.Empty;
	}
}
#pragma warning restore CA2211 // Non-constant fields should not be visible
