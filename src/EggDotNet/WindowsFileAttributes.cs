using System;
namespace EggDotNet
{
	[Flags]
	public enum WindowsFileAttributes
	{
		None = 0,
		ReadOnly = 1,
		Hidden = 2,
		SystemFile = 4,
		LinkFile = 8,
		Directory = 128
	}
}
