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
			using (var entryStream = entry.Open())
			{
				var path = Path.Combine(destinationDirectory, entry.FullName);

				using (var foStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					entryStream.CopyTo(foStream);
					foStream.Flush();
					foStream.Close();

#if NETSTANDARD2_1_OR_GREATER
					if (entry.LastWriteTime.HasValue)
					{
						File.SetLastWriteTime(path, entry.LastWriteTime.Value);
					}
#else
					if (entry.LastWriteTime != null)
					{
						File.SetLastWriteTime(path, entry.LastWriteTime);
					}
#endif
					if (entry.ExternalAttributes !=  (long)WindowsFileAttributes.None)
					{
						SetWindowsFileAttributes(path, (WindowsFileAttributes)entry.ExternalAttributes);
					}
				}
			}
		}

		private static void SetWindowsFileAttributes(string path, WindowsFileAttributes fileAttributes)
		{
			if (fileAttributes.HasFlag(WindowsFileAttributes.ReadOnly))
			{
				File.SetAttributes(path, FileAttributes.ReadOnly);
			}
			if (fileAttributes.HasFlag(WindowsFileAttributes.Hidden))
			{
				File.SetAttributes(path, FileAttributes.Hidden);
			}
			if (fileAttributes.HasFlag(WindowsFileAttributes.SystemFile))
			{
				File.SetAttributes(path, FileAttributes.System);
			}
		}
	}
}
