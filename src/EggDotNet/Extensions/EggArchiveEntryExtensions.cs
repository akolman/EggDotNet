using System.IO;

namespace EggDotNet.Extensions
{
	public static class EggArchiveEntryExtensions
	{
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
