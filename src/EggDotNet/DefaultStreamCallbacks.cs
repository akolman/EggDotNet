using System;
using System.Collections.Generic;
using System.IO;
using static EggDotNet.Callbacks;

namespace EggDotNet
{
	internal static class DefaultStreamCallbacks
	{
		public static SplitFileReceiverCallback DefaultFileStreamCallback = (st) =>
		{
			if (st is FileStream fst)
			{
				var sts = new List<Stream>();
				var dirname = Path.GetDirectoryName(fst.Name);
				var files = Directory.GetFiles(dirname);
				foreach (var file in files)
				{
					if (file != fst.Name)
					{
						sts.Add(new FileStream(file, FileMode.Open, FileAccess.Read));
					}
				}
				return sts;
			}

			throw new InvalidOperationException("DefaultFileStream can only be used with FileStream");
		};

		public static PasswordCallback DefaultPasswordCallback = (string filename) =>
		{
			Console.WriteLine($"Please enter password for {filename} (return to quit): ");

			return Console.ReadLine();
		};

		public static SplitFileReceiverCallback GetStreamCallback(Stream st)
		{
			if (st is FileStream)
			{
				return DefaultFileStreamCallback;
			}

			return null;
		}

		public static PasswordCallback GetPasswordCallback()
		{
			return DefaultPasswordCallback;
		}
	}
}
