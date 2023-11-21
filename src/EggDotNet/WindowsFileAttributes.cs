using System;
using System.Collections.Generic;
using System.Text;

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
