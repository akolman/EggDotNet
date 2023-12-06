using System.IO;

namespace EggDotNet.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="EggArchiveEntry"/>.
	/// </summary>
	public static class EggArchiveEntryExtensions
	{
		/// <summary>
		/// Extracts an <see cref="EggArchiveEntry"/> to a directory.
		/// </summary>
		/// <param name="entry">The source entry.</param>
		/// <param name="destinationDirectory">The destination directory.</param>
		public static void ExtractToDirectory(this EggArchiveEntry entry, string destinationDirectory)
		{
			using var entryStream = entry.Open();
			var path = Path.Combine(destinationDirectory, entry.FullName);

			using var foStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
			entryStream.CopyTo(foStream);
			foStream.Flush();
			foStream.Close();
			if (entry.LastWriteTime.HasValue)
			{
				File.SetLastWriteTime(path, entry.LastWriteTime.Value);
			}
		}
	}
}
