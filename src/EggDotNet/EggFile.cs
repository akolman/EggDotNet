using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet
{
	public enum CompressionMethod : byte
	{
		Store = 0,
		Deflate = 1,
		Bzip2 = 2,
		Azo = 3,
		Lzma = 4
	}

	public enum EncryptionMethod : byte
	{
		Standard = 0,
		AES128 = 1,
		AES256 = 2
	}

	public static class EggFile
	{
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

		public static void ExtractToDirectory(string sourceArchiveName, string destinationDirectory)
		{
			using var inputStream = new FileStream(sourceArchiveName, FileMode.Open, FileAccess.Read, FileShare.Read);
			ExtractToDirectory(inputStream, destinationDirectory);
		}
	}
}
