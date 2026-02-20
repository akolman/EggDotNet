using EggDotNet;

var archivePath = Path.Combine(@"../../../../SampleFiles", "posix_small.egg");
using var archive = EggFile.Open(archivePath);
var entry = archive.Entries.Single();

if (entry.EntryInfoType == EntryInfoType.Posix)
{
	var uidgidVal = entry.GetExtraAttributes(ExtraAttributeType.UserGroupAttributes);
	var uid = (int)(uidgidVal >> 32);
	var gid = (int)(uidgidVal & 0xFFFFFFFF);

	var fileAttributes = (PosixFileAttributes)entry.GetExtraAttributes(ExtraAttributeType.FileAttibutes);
	var epochTimestamp = entry.GetExtraAttributes(ExtraAttributeType.DateAttributes);

	Console.WriteLine($"Entry \"{entry.Name}\" owned by {uid}:{gid} (uid/gid)");
	Console.WriteLine($"File Attributes: {fileAttributes}");
	Console.WriteLine($"Original Epoch Timestamp: {epochTimestamp}");
}

