using System.Diagnostics;
using System.IO.Compression;

namespace EggDotNet.Benchmarks
{
	internal class Program
	{
		private static long TestEgg()
		{
			PrimeDir();
			var s = new Stopwatch();
			s.Start();
			EggFile.ExtractToDirectory("../../../test_files/defaults.egg", "../../../test_out");
			s.Stop();
			return s.ElapsedMilliseconds;
		}

		private static long TestZip()
		{
			PrimeDir();
			var s = new Stopwatch();
			s.Start();
			ZipFile.ExtractToDirectory("../../../test_files/defaults.zip", "../../../test_out");
			s.Stop();
			return s.ElapsedMilliseconds;
		}

		private static void PrimeDir()
		{
			Directory.CreateDirectory("../../../test_out");
			var di = new DirectoryInfo("../../../test_out");
			foreach (var fi in di.GetFiles())
			{
				File.Delete(fi.FullName);
			}
		}

		static void Main(string[] args)
		{
			var iterations = int.Parse(args[0]);
			var eggTimes = new long[iterations];
			var zipTimes = new long[iterations];

			for(var i=0; i< iterations; i++)
			{
				eggTimes[i] = TestEgg();
				zipTimes[i] = TestZip();
			}

			var eggAvg = eggTimes.Average();
			var zipAvg = zipTimes.Average();

			Console.WriteLine($"Test count {iterations}");
			Console.WriteLine($"Egg Average: {eggAvg}ms");
			Console.WriteLine($"Zip Average: {zipAvg}ms");			
		}
	}
}
