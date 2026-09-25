using EggDotNet.Exceptions;
using System;
using System.IO;
using System.Text;

#if !USE_SPAN
using EggDotNet.InternalExtensions;
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
			AbsolutePath = 16 /*Documentation seems to be wrong but implementation shows absolute when set*/
		}

#if DEBUG
		internal int codepage;
#endif
		public const int FILENAME_HEADER_MAGIC = 0x0A8591AC;

		public string FileNameFull { get; private set; }

		public FilenameHeader(string filename, Encoding encoder)
		{
			FileNameFull = filename;
#if DEBUG
			codepage = encoder.CodePage;
#endif
		}

		public static FilenameHeader Parse(Stream stream)
		{
			var nameEncoder = Encoding.UTF8;
#if USE_SPAN
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

			if (bitFlag.HasFlag(FilenameFlags.UseAreaCode))
			{
#if USE_SPAN
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

			if (bitFlag.HasFlag(FilenameFlags.AbsolutePath))
			{
				var parentIdBytes = new byte[4];
				_ = stream.Read(parentIdBytes); /*unsure how to implement this as I believe it's unused*/
			}

			var filenameBytes = new byte[filenameSize];
			if (stream.Read(filenameBytes) != filenameSize)
			{
				throw new InvalidDataException("Filename header corrupt");
			}

			return new FilenameHeader(nameEncoder.GetString(filenameBytes), nameEncoder);
		}
	}
}
