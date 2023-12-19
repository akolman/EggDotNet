using System;
using System.Collections.Generic;
using System.IO;

namespace EggDotNet
{
	internal static class DefaultStreamCallbacks
	{
		public static Func<Stream, IEnumerable<Stream>> DefaultFileStreamCallback = (st) =>
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

		public static Func<string> DefaultPasswordCallback = () =>
		{
			Console.WriteLine("Please enter archive password: ");

			var password = string.Empty;
			while (string.IsNullOrWhiteSpace(password))
			{
				password = Console.ReadLine();
			}
			return password;
		};

		public static Func<Stream, IEnumerable<Stream>> GetStreamCallback(Stream st)
		{
			if (st is FileStream)
			{
				return DefaultFileStreamCallback;
			}

			return null;
		}

		public static Func<string> GetPasswordCallback()
		{
			return DefaultPasswordCallback;
		}
	}
}
