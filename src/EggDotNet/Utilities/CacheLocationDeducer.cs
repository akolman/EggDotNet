using EggDotNet.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Utilities
{
	internal static class CacheLocationDeducer
	{
		public static string GetCacheDirectory(string inputFilePath)
		{
			switch (SolidStreamConfiguration.CacheLocation)
			{
				case SolidStreamCacheLocation.InputDirectory:
					return GetInputDirectory(inputFilePath);
				case SolidStreamCacheLocation.WorkingDirectory:
					return GetWorkingDirectory();
				case SolidStreamCacheLocation.UserTemp:
					return GetUserTempDirectory();
				case SolidStreamCacheLocation.DefineDirectory:
					return GetUserDefinedDirectory();
			}
			return string.Empty;
		}

		private static string GetInputDirectory(string inputFilePath)
		{
			if (string.IsNullOrWhiteSpace(inputFilePath))
			{
				throw new InvalidOperationException("Input path must be set when using input directory for solid extraction");
			}
			return Path.GetDirectoryName(inputFilePath);
		}

		private static string GetWorkingDirectory() => AppContext.BaseDirectory;

		private static string GetUserTempDirectory() => System.IO.Path.GetTempPath();


		private static string GetUserDefinedDirectory()
		{
			if (string.IsNullOrWhiteSpace(SolidStreamConfiguration.UserDefinedSolidCacheDirectory))
			{
				throw new InvalidOperationException("Solid temp directory must be set");
			}

			return SolidStreamConfiguration.UserDefinedSolidCacheDirectory;
		}
	}
}
