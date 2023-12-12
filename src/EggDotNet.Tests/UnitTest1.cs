using System.ComponentModel;
using EggDotNet.Exceptions;

namespace EggDotNet.Tests
{
	public class UnitTest1
	{
		[Fact]
		[Description("Validates an archive with 3 files, all in folders, compressed using deflate compression")]
		public void Test_Basic_Defalte_With_Folders()
		{
			using var archive = EggFile.Open("../../../test_files/basic_deflate_folders.egg");
			var firstEntry = archive.Entries.First();
			Assert.Equal("bliss.jpg", firstEntry.Name);

			using var lastEntryStream = archive.Entries.Last().Open();
			using var sr = new StreamReader(lastEntryStream);
			var text = sr.ReadToEnd();
			Assert.Equal(34446, text.Length);
			Assert.StartsWith("Lorem ipsum dolor sit amet", text);
			Assert.EndsWith("gravida.", text);
		}

		[Fact]
		public void Test_Lzma()
		{
			using var archive = EggFile.Open("../../../test_files/lzma_simple.egg");
			var loremEntry = archive.GetEntry("lorem_ipsum.txt");
			Assert.Equal(5723, loremEntry!.CompressedLength);
			Assert.Equal(34446, loremEntry.UncompressedLength);
			using var lstr = loremEntry.Open();
			using var reader = new StreamReader(lstr);
			var data = reader.ReadToEnd();
			Assert.Equal(34446, data.Length);
		}

		[Fact]
		public void Test_Global_Comment()
		{
			using var archive = EggFile.Open("../../../test_files/globalcomment.egg");
			Assert.Equal("Global comment", archive.Comment);
		}

		[Fact]
		public void Test_Basic_Deflate_Aes128()
		{
			using var fs = new FileStream("../../../test_files/test128.egg", FileMode.Open, FileAccess.Read);
			using var archive = new EggArchive(fs, false, null, () => "password12345!");
			var onlyEntry = archive.Entries.Single();
			Assert.Equal("test.txt", onlyEntry.Name);

			using var onlyEntryStream = onlyEntry.Open();
			using var sReader = new StreamReader(onlyEntryStream);
			var text = sReader.ReadToEnd();

			Assert.Equal("Hello there my name is Andrew.", text);

			Assert.True(onlyEntry.ChecksumValid());
		}

		[Fact]
		public void Test_Basic_Deflate_Aes256()
		{
			using var fs = new FileStream("../../../test_files/test256.egg", FileMode.Open, FileAccess.Read);
			using var archive = new EggArchive(fs, false, null, () => "password12345!");
			var onlyEntry = archive.Entries.Single();
			Assert.Equal("test.txt", onlyEntry.Name);

			using var onlyEntryStream = onlyEntry.Open();
			using var sReader = new StreamReader(onlyEntryStream);
			var text = sReader.ReadToEnd();

			Assert.Equal("Hello there my name is Andrew.", text);
		}

		[Fact]
		public void Test_Large()
		{
			using var archive = EggFile.Open("../../../test_files/zeros.egg");
			var singleEntry = archive.Entries.Single();
			Assert.Equal(338, singleEntry.CompressedLength);
			Assert.Equal(197_540_460, singleEntry.UncompressedLength);
		}

		[Fact]
		public void Test_Invalid_File_Throws()
		{
			var fileData = new byte[] { 1, 2, 3, 4, 5 };
			using var inputStream = new MemoryStream(fileData);

			Assert.Throws<UnknownEggException>(() =>
			{
				using var archive = new EggArchive(inputStream);
			});
		}
	}
}