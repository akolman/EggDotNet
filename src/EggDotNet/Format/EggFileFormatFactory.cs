using EggDotNet.Exception;
using EggDotNet.Extensions;
using EggDotNet.Format.Egg;
using System.IO;

namespace EggDotNet.Format
{
	internal static class EggFileFormatFactory
	{
		public static IEggFileFormat Create(Stream stream)
		{
			if (!stream.ReadInt(out int header))
			{
				throw new BadDataException("Could not read header from stream");
			}

			return header switch
			{
				var _ when header == Egg.Header.EGG_HEADER_MAGIC => new EggFormat(),
				_ => throw new UnknownEggException()
			};
		}
	}
}
