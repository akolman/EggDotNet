using EggDotNet.Extensions;
using System.IO;

namespace EggDotNet
{
	/// <summary>
	/// Static class used to handle extraction of egg archives.
	/// </summary>
	public static class EggFile
	{
		/// <summary>
		/// Extracts an EGG archive from a source Stream to a destination directory.
		/// </summary>
		/// <param name="sourceStream">The source EGG stream.</param>
		/// <param name="destinationDirectory">The destination directory path to place files.</param>
		public static void ExtractToDirectory(Stream sourceStream, string destinationDirectory)
		{
			using (var eggArchive = new EggArchive(sourceStream, false))
			{
				foreach (var archiveEntry in eggArchive.Entries)
				{
					archiveEntry.ExtractToDirectory(destinationDirectory);
				}
			}
		}

		/// <summary>
		/// Extracts an EGG archive file specified by a source path to a destination directory.
		/// </summary>
		/// <param name="sourceArchiveName">The source EGG file path.</param>
		/// <param name="destinationDirectory">The desination directory path to place files.</param>
		public static void ExtractToDirectory(string sourceArchiveName, string destinationDirectory)
		{
			using (var inputStream = new FileStream(sourceArchiveName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				ExtractToDirectory(inputStream, destinationDirectory);
			}
		}

		/// <summary>
		/// Opens an EGG file from a specified path.
		/// </summary>
		/// <param name="eggArchivePath">The EGG file path.</param>
		/// <returns>A new EggArchive instance.</returns>
		public static EggArchive Open(string eggArchivePath)
		{
			var fs = new FileStream(eggArchivePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new EggArchive(fs, true);
		}
	}
}
