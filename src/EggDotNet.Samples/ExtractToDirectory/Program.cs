using EggDotNet;
using System.Diagnostics;

namespace EggDotNet.Samples.ExtractToDirectory
{
	internal class Program
	{
		internal class ProgressReporter
		{
			private readonly Stopwatch sw = new();

			internal void ReportStart(EggArchiveEntry entry, long written, long total)
			{
				Debug.Assert(total > 0);
				Debug.Assert(written >= 0);
				sw.Restart();
				Console.WriteLine($"Starting extract of {entry.FullName} ({entry.UncompressedLength} bytes)");
			}

			internal void ReportEnd(EggArchiveEntry entry, long written, long total)
			{
				sw.Stop();
				double pct = (double)written * 100 / total;
				Console.WriteLine($"Finished extract of {entry.FullName} in {sw.ElapsedMilliseconds} ms : {pct:F2}% complete");
			}
		}

		static void Main()
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
				Console.Error.WriteLine($"Encountered exception extracting archive {archiveName}: {e}");
			}
		}
	}
}
