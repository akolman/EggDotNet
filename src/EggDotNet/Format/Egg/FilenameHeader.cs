using EggDotNet.Exception;
using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Egg
{
	internal class FilenameHeader //: ExtraField2
	{
		[Flags]
		public enum FilenameFlags
		{
			None = 0,
			Encrypt = 4,
			UseAreaCode = 8,
			RelativePath = 16
		}

		public const int FILENAME_HEADER_MAGIC = 0x0A8591AC;

		public string FileNameFull { get; private set; }

		public string FileName { get; private set; }

		public FilenameHeader(string filename)
		{
			FileNameFull = filename;
			FileName = Path.GetFileName(filename);
		}

		public static FilenameHeader Parse(Stream stream)
		{
			var bitFlagByte = stream.ReadByte();
			if (bitFlagByte == -1)
			{
				throw new BadDataException("Filename header flag couldn't be read");
			}

			var bitFlag = (FilenameFlags)bitFlagByte;

			if (bitFlag.HasFlag(FilenameFlags.Encrypt))
			{
				throw new BadDataException("Encrypted filenames not supported");
			}

			if (!stream.ReadShort(out short filenameSize))
			{
				throw new BadDataException("Filename size couldn't be read");
			}

			if (!stream.ReadN(filenameSize, out byte[] filenameBuffer))
			{
				throw new BadDataException("Filename header corrupt");
			}

			string? filename;
			if (bitFlag.HasFlag(FilenameFlags.UseAreaCode))
			{
				throw new NotImplementedException("Only UTF-8 entry name supported at this time");
			}
			else
			{
				filename = Encoding.UTF8.GetString(filenameBuffer);
			}

			return new FilenameHeader(filename);
		}
	}
}
