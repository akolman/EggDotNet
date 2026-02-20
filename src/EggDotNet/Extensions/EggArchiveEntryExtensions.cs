using System;
using System.IO;
using System.Linq;

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
		/// <param name="destinationDirectory">The destination directory to extract the entry into.</param>
		public static void ExtractToDirectory(this EggArchiveEntry entry, string destinationDirectory)
		{
			using (var entryStream = entry.Open())
			{
				var entryName = entry.FullName;
				var entryNameParts = entryName.Split('/');
				if (entryNameParts.Length > 1)
				{
					var entryDirectoryParts = entryNameParts.Take(entryNameParts.Length - 1);
					entryName = entryNameParts.Last();
					destinationDirectory = Path.Combine(destinationDirectory, Path.Combine(entryDirectoryParts.ToArray()));
				}

				Directory.CreateDirectory(destinationDirectory);

				var path = Path.Combine(destinationDirectory, entryName);

				using (var foStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					entry.ExtractToStream(foStream);
					foStream.Close();
				}

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
				HandleFileAttributes(entry, path);
			}
		}

		/// <summary>
		/// Extracts the EggArchiveEntry to the provided output Stream.  Caller should close Stream.
		/// </summary>
		/// <param name="entry">The EggArchiveEntry to extract.</param>
		/// <param name="outputStream">The Stream to extract to.</param>
		public static void ExtractToStream(this EggArchiveEntry entry, Stream outputStream)
		{
			using (var entryStream = entry.Open())
			{
				entryStream.CopyTo(outputStream);
				outputStream.Flush();
			}
		}

		private static void HandleFileAttributes(EggArchiveEntry entry, string path)
		{
			var fileAttrs = entry.GetExtraAttributes(ExtraAttributeType.FileAttibutes);

			if (entry.EntryInfoType == EntryInfoType.Windows)
			{
				SetWindowsFileAttributes(path, (WindowsFileAttributes)fileAttrs);
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
