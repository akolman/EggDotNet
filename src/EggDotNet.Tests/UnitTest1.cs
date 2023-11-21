using System.ComponentModel;

namespace EggDotNet.Tests
{
	public class UnitTest1
	{
		[Fact]
		[Description("Validates an archive with 3 files, all in folders, compressed using deflate compression")]
		public void Test_Basic_Defalte_With_Folders()
		{
			using var archive = new EggArchive("../../../test_files/basic_deflate_folders.egg");
			var firstEntry = archive.Entries.First();
			Assert.Equal("bliss.jpg", firstEntry.Name);

			using var lastEntryStream = archive.Entries.Last().Open();
			using var sr = new StreamReader(lastEntryStream);
			var text = sr.ReadToEnd();
			Assert.Equal(34446, text.Length);
			Assert.StartsWith("Lorem ipsum dolor sit amet", text);
			Assert.EndsWith("gravida.", text);
		}
	}
}