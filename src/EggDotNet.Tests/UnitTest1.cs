using System.ComponentModel;
using System.Security.Cryptography;
using EggDotNet.Exceptions;

namespace EggDotNet.Tests
{
	public class TestFileInfo
	{
		public long UncompressedSize { get; }

		public byte[] Crc32 { get; }

		public byte[] Sha256 { get; }

		public TestFileInfo(long uncompressedSize, string crc32, string sha256)
		{
			Sha256 = StringToByteArray(sha256);
			Crc32 = StringToByteArray(crc32);
			UncompressedSize = uncompressedSize;
		}

		private static byte[] StringToByteArray(string hex)
		{
			if (hex.Length % 2 == 1)
				throw new Exception("The binary key cannot have an odd number of digits");

			byte[] arr = new byte[hex.Length >> 1];

			for (int i = 0; i < hex.Length >> 1; ++i)
			{
				arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
			}

			return arr;
		}

		private static int GetHexVal(char hex)
		{
			int val = (int)hex;
			//For uppercase A-F letters:
			//return val - (val < 58 ? 48 : 55);
			//For lowercase a-f letters:
			//return val - (val < 58 ? 48 : 87);
			//Or the two combined, but a bit slower:
			return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
		}
	}

	public class UnitTest1 : IDisposable
	{
		private static readonly string TEST_FILES_DIR = "../../../test_files/";

		public static readonly Dictionary<string, TestFileInfo> TestFileInfos = new()
		{
			{ "lorem_ipsum_short.txt" , new TestFileInfo(525, "3AAE00F7", "ACFE59AFCFFCBE68A4DEE454D11F5BF78DFDEE32C77A03C1A50CF384418FE38F") },
			{ "lorem_ipsum_long.pdf" , new TestFileInfo(66_431, "798BEF9F", "4B3E49290B2399C2AB63886B0585B819AE7C6D9FA947F1B3DB53BD9DA62A2B10")},
			{ "lorem_ipsum_long.tif", new TestFileInfo(1_195_080 , "90D70968", "162D8B99FD7E695BD3E8C790493C93B649A76996F4E129C818A366614977EC67") },
			{ "lorem_ipsum_medium.txt", new TestFileInfo(3_950, "AE4BAE1D", "5DEA92AB15A3A73AF46B0AFBF793C109B576E35C62CE9DB703E7F4088F2E8A07" ) },
			{ "lorem_ipsum_long.txt", new TestFileInfo(15_238, "EE3EDF61", "D63CFF6C64AB12F1C08A298F7B3A0C27F19A9822D2C6C7156D676D62265DB46E" ) },
			{ "lorem_ipsum.zip", new TestFileInfo(1_242_971, "32AE3E83", "F7E4474594F6B6C8EB4A38A86C5391761AC8C8B8F8CFE2D35101A1B43B467BCC") }
		};

		private readonly SHA256 sha;
		private bool disposedValue;

		public UnitTest1()
		{
			sha = SHA256.Create();
		}

		[Fact]
		[Description("Tests handling of a smple which includes all basic test files, using default EGG options in ALZip (Priority on compressing speed).")]
		public void Test_Default_File_Default_Options()
		{
			using var archive = OpenTestEgg("defaults.egg");
			Assert.Equal("Sample which includes all basic test files, using default EGG options in ALZip (Priority on compressing speed).", archive.Comment);
			Assert.Equal(6, archive.Entries.Count);
			Assert.Contains<CompressionMethod>(CompressionMethod.Store, archive.Entries.Select(e => e.CompressionMethod));
			Assert.Contains<CompressionMethod>(CompressionMethod.Deflate, archive.Entries.Select(e => e.CompressionMethod));
			ValidateAllEggEntries(archive);
		}

		[Fact]
		public void Test_Default_File_Priority_Compress_Ratio()
		{
			using var archive = OpenTestEgg("default_lzma_pri_compress_ratio.egg");
			Assert.Equal("Sample which includes all basic test files, with Priority on Compress Ratio.", archive.Comment);
			Assert.Equal(6, archive.Entries.Count);
			Assert.Contains<CompressionMethod>(CompressionMethod.Lzma, archive.Entries.Select(e => e.CompressionMethod));
			ValidateAllEggEntries(archive);
		}

		[Fact]
		public void Test_Defaults_Optimized()
		{
			using var archive = OpenTestEgg("defaults_bz_optimized.egg");
			Assert.Equal("Sample which includes all basic test files, with Optimized for Compression.", archive.Comment);
			Assert.Equal(6, archive.Entries.Count);
			Assert.Contains<CompressionMethod>(CompressionMethod.Bzip2, archive.Entries.Select(e => e.CompressionMethod));
			ValidateAllEggEntries(archive);
		}

		[Fact]
		public void Test_Normal()
		{
			using var archive = OpenTestEgg("defaults_normal.egg");
			Assert.Equal("Sample which includes all basic test files, with Normal setting selected.", archive.Comment);
			Assert.Equal(6, archive.Entries.Count);
			ValidateAllEggEntries(archive);
		}

		[Fact]
		public void Test_Store()
		{
			using var archive = OpenTestEgg("defaults_nocompress.egg");
			var compMethod = archive.Entries.Select(e => e.CompressionMethod).Distinct().Single();
			Assert.Equal(CompressionMethod.Store, compMethod);
			ValidateAllEggEntries(archive);
		}

		[Fact]
		public void Test_Aes128()
		{
			using var fs = new FileStream(GetTestPath("lorem_long_aes128.egg"), FileMode.Open, FileAccess.Read);
			using var archive = new EggArchive(fs, false, null, () => "password12345!");
			Assert.Equal("Lorem long text encrypted with AES128", archive.Comment);
			ValidateAllEggEntries(archive);
			var aes256Entry = archive.GetEntry("lorem_ipsum_long.txt");
			Assert.Equal("This file is encrypted using AES128", aes256Entry!.Comment);
			using var entryStream = aes256Entry.Open();
			using var sr = new StreamReader(entryStream);
			var loremLongText = sr.ReadToEnd();
			Assert.Equal(15_238, loremLongText.Length);
			Assert.StartsWith("Lorem ipsum dolor sit amet", loremLongText);
			Assert.EndsWith("sed faucibus orci ligula eu nisi.", loremLongText);
		}

		[Fact]
		public void Test_Aes256()
		{
			using var fs = new FileStream(GetTestPath("lorem_long_aes256.egg"), FileMode.Open, FileAccess.Read);
			using var archive = new EggArchive(fs, false, null, () => "password12345!");
			Assert.Equal("Lorem long text encrypted with AES256", archive.Comment);
			ValidateAllEggEntries(archive);
			var aes256Entry = archive.GetEntry("lorem_ipsum_long.txt");
			Assert.Equal("This file is encrypted using AES256", aes256Entry!.Comment);
			using var entryStream = aes256Entry.Open();
			using var sr = new StreamReader(entryStream);
			var loremLongText = sr.ReadToEnd();
			Assert.Equal(15_238, loremLongText.Length);
			Assert.StartsWith("Lorem ipsum dolor sit amet", loremLongText);
			Assert.EndsWith("sed faucibus orci ligula eu nisi.", loremLongText);			
		}

		[Fact]
		public void Test_Alz_Defaults()
		{
			using var archive = OpenTestEgg("defaults.alz");
			ValidateAllEggEntries(archive);
		}

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

		private static byte[] CrcToBytes(uint crc) => BitConverter.GetBytes(crc);

		private static long GetDataSize(EggArchiveEntry entry)
		{
			using var stream = entry.Open();
			var read = 0;
			var buf = new byte[4096];
			while (true)
			{
				var readCount = stream.Read(buf, 0, buf.Length);
				read += readCount;
				if (readCount == 0)
					break;

			}
			return read;
		}

		private byte[] GetDataSha(EggArchiveEntry entry)
		{
			using var entryStream = entry.Open();
			return sha.ComputeHash(entryStream);
		}

		private static string GetTestPath(string eggName) => Path.Combine(TEST_FILES_DIR, eggName);

		private static EggArchive OpenTestEgg(string eggName) => EggFile.Open(GetTestPath(eggName));

		private void ValidateEggEntry(string entryName, EggArchive archive)
		{
			var entry = archive.GetEntry(entryName);
			var fileInfo = TestFileInfos[entryName];

			Assert.Equal(fileInfo.UncompressedSize, entry!.UncompressedLength);
			Assert.Equal(fileInfo.UncompressedSize, GetDataSize(entry));
			Assert.Equal<byte>(fileInfo.Crc32, CrcToBytes(entry.Crc32));
			Assert.True(entry.ChecksumValid());
			Assert.Equal<byte>(fileInfo.Sha256, GetDataSha(entry));
		}

		private void ValidateAllEggEntries(EggArchive archive)
		{
			foreach (var entry in archive.Entries)
			{
				ValidateEggEntry(entry.FullName!, archive);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					sha.Dispose();
				}

				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~UnitTest1()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}