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
			using var eggArchive = new EggArchive(sourceStream, false);

			foreach(var archiveEntry in  eggArchive.Entries)
			{
				using var entryStream = archiveEntry.Open();
				var path = Path.Combine(destinationDirectory, archiveEntry.FullName);

				using var foStream = new FileStream(path, FileMode.Create, FileAccess.Write);
				entryStream.CopyTo(foStream);
				foStream.Flush();
				foStream.Close();
				if (archiveEntry.LastWriteTime.HasValue)
				{
					File.SetLastWriteTime(path, archiveEntry.LastWriteTime.Value);
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
			using var inputStream = new FileStream(sourceArchiveName, FileMode.Open, FileAccess.Read, FileShare.Read);
			ExtractToDirectory(inputStream, destinationDirectory);
		}
	}
}
