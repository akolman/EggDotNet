using EggDotNet.Configuration;
using EggDotNet.SpecialStreams;
using System;
using System.IO;
using System.Linq;

namespace EggDotNet.Format.Egg
{
	internal sealed class SolidStreamManager : IDisposable
	{
		private readonly bool UseDisk;
		private readonly Stream tempStream;
		
		private bool disposedValue;

		public SolidStreamManager(Stream baseStream, long totalSize, string tempPath = "./")
		{
			var solidFilePath = Path.Combine(tempPath, Guid.NewGuid().ToString());
			UseDisk = totalSize >= SolidStreamConfiguration.SolidDiskBufferCutoff;
			if (UseDisk)
			{
				tempStream = new FileStream(solidFilePath, FileMode.Create, 
											FileAccess.ReadWrite, FileShare.None, 
											4096, FileOptions.DeleteOnClose);
			}
			else
			{
				tempStream = new MemoryStream(new byte[totalSize]);
			}

			baseStream.CopyTo(tempStream);
			baseStream.Flush();
		}

		~SolidStreamManager() 
		{
			Dispose();
		}

		public Stream GetEntryStream(EggArchiveEntry entry)
		{
			long startPointFound = entry.Archive.Entries.Take(entry.Archive.Entries.IndexOf(entry))
														.Sum(e => e.UncompressedLength);

			return new SubStream(tempStream, startPointFound, startPointFound + entry.UncompressedLength);
		}


		private void Dispose(bool disposing)
		{
			if (disposing && !disposedValue)
			{
				if (tempStream != null)
				{
					tempStream.Close();
					tempStream.Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
