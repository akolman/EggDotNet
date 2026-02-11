using EggDotNet.Extensions;
using System.IO;
using System.Linq;

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
		/// 
		/// </summary>
		/// <param name="sourceStream">The source EGG stream.</param>
		/// <param name="destinationDirectory">The desination directory path to place files.</param>
		/// <param name="startCallback">Callback executed at the start of extraction for each entry.</param>
		/// <param name="endCallback">Callback executed at the end of extraction for each entry.</param>
		public static void ExtractToDirectory(Stream sourceStream, string destinationDirectory, Callbacks.EggFileEntryDecompressStart startCallback, Callbacks.EggFileEntryDecompressEnd endCallback = null)
		{
			using (var eggArchive = new EggArchive(sourceStream, false))
			{
				var totalToWrite = eggArchive.Entries.Select(e => e.UncompressedLength).Sum();
				var totalWritten = 0L;

				foreach (var archiveEntry in eggArchive.Entries)
				{
					startCallback.Invoke(archiveEntry, totalWritten, totalToWrite);
					archiveEntry.ExtractToDirectory(destinationDirectory);
					totalWritten += archiveEntry.UncompressedLength;
					if (endCallback != null)
					{
						endCallback.Invoke(archiveEntry, totalWritten, totalToWrite);
					}
				}
			}
		}

		/// <summary>
		/// Extracts an EGG archive file specified by a source path to a destination directory, calling the provided callbacks upon start and completion of each entry.
		/// </summary>
		/// <param name="sourceArchiveName">The source EGG file path.</param>
		/// <param name="destinationDirectory">The desination directory path to place files.</param>
		/// <param name="startCallback">Callback executed at the start of extraction for each entry.</param>
		/// <param name="endCallback">Callback executed at the end of extraction for each entry.</param>
		public static void ExtractToDirectory(string sourceArchiveName, string destinationDirectory, Callbacks.EggFileEntryDecompressStart startCallback, Callbacks.EggFileEntryDecompressEnd endCallback = null)
		{
			using (var inputStream = new FileStream(sourceArchiveName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				ExtractToDirectory(inputStream, destinationDirectory, startCallback, endCallback);
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
