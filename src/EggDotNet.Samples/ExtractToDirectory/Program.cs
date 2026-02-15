using EggDotNet;
using System.Diagnostics;

namespace EggDotNet.Samples.ExtractToDirectory
{
	internal class Program
	{
		internal class ProgressReporter
		{
			private Stopwatch sw = new Stopwatch();

			internal void ReportStart(EggArchiveEntry entry, long written, long total)
			{
				sw.Restart();
				Console.WriteLine($"Starting extract of {entry.FullName} ({entry.UncompressedLength} bytes)");
			}

			internal void ReportEnd(EggArchiveEntry entry, long written, long total)
			{
				sw.Stop();
				double pct = (double)written * 100 / total;
				Console.WriteLine($"Finished extract of {entry.FullName} in {sw.ElapsedMilliseconds} ms : {pct.ToString("F2")}% complete");
			}
		}

		static void Main(string[] args)
		{
			var sampleRootPath = @"../../../../SampleFiles";
			var archiveName = "defaults.egg";
			var archivePath = Path.Combine(sampleRootPath, archiveName);
			var outputPath = @"../../../output";
			var reporter = new ProgressReporter();
			try
			{
				EggFile.ExtractToDirectory(archivePath, outputPath, reporter.ReportStart, reporter.ReportEnd);
			}
			catch(Exception e) 
			{
				Console.Error.WriteLine($"Encountered exception extracting archive {archiveName}: {e.ToString()}");
			}
		}
	}
}
