using EggDotNet.Exception;
using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Egg
{
	internal sealed class FilenameHeader //: ExtraField2
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
			var nameEncoder = Encoding.UTF8;

			var bitFlagByte = stream.ReadByte();
			if (bitFlagByte == -1)
			{
				throw new BadDataEggception("Filename header flag couldn't be read");
			}

			var bitFlag = (FilenameFlags)bitFlagByte;

			if (bitFlag.HasFlag(FilenameFlags.Encrypt))
			{
				throw new BadDataEggception("Encrypted filenames not supported");
			}

			if (!stream.ReadShort(out short filenameSize))
			{
				throw new BadDataEggception("Filename size couldn't be read");
			}

			if (bitFlag.HasFlag(FilenameFlags.UseAreaCode))
			{
				stream.ReadShort(out short locale);
				try
				{
					nameEncoder = Encoding.GetEncoding(locale);
				}
				catch(System.Exception ex)
				{
					throw new UnsupportedLocalEggception(locale, ex); 
				}
			}

			if (!stream.ReadN(filenameSize, out byte[] filenameBuffer))
			{
				throw new BadDataEggception("Filename header corrupt");
			}

			return new FilenameHeader(nameEncoder.GetString(filenameBuffer));
		}
	}
}
