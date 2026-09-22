using EggDotNet.Exceptions;
using EggDotNet.InternalExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#if LEGACY_DOTNET
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
#endif

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

		public FilenameHeader(string filename)
		{
			FileNameFull = filename;
		}

		public static FilenameHeader Parse(Stream stream)
		{
			var nameEncoder = Encoding.UTF8;
#if !LEGACY_DOTNET
			Span<byte> filenameHeaderBuffer = stackalloc byte[3];
#else
			var filenameHeaderBuffer = new byte[3];
#endif
			if (stream.Read(filenameHeaderBuffer) != 3)
			{
				throw new InvalidDataException("Failed reading filename header");
			}

			var bitFlag = (FilenameFlags)filenameHeaderBuffer[0];

			if (bitFlag.HasFlag(FilenameFlags.Encrypt))
			{
				throw new InvalidDataException("Encrypted filenames not supported");
			}

			var filenameSize = BitConverter.ToInt16(filenameHeaderBuffer.Slice(1, 2));
			if (filenameSize > 2 << 7) /*protect from stack blowout*/
			{
				throw new InvalidDataException("Invalid filename size");
			}

			if (bitFlag.HasFlag(FilenameFlags.UseAreaCode))
			{
#if !LEGACY_DOTNET
				Span<byte> localeBuffer = stackalloc byte[2];
#else
				var localeBuffer = new byte[2];
#endif
				if (stream.Read(localeBuffer) != 2)
				{
					throw new InvalidDataException("Failed reading filename locale");
				}

				var locale = BitConverter.ToInt16(localeBuffer);

				try
				{
					nameEncoder = Encoding.GetEncoding(locale);
				}
				catch(System.Exception ex)
				{
					throw new UnsupportedLocaleException(locale, ex); 
				}
			}

#if !LEGACY_DOTNET
			Span<byte> filenameBytes = stackalloc byte[filenameSize];
#else
			var filenameBytes = new byte[filenameSize];
#endif
			if (stream.Read(filenameBytes) != filenameSize)
			{
				throw new InvalidDataException("Filename header corrupt");
			}

			return new FilenameHeader(nameEncoder.GetString(filenameBytes));
		}
	}
}
