using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet
{
	/// <summary>
	/// Represents Posix file descriptor flags.
	/// </summary>
	[Flags]
	public enum PosixFileAttributes : long
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		OthersExecute	= 0x0001,
		OthersWrite		= 0x0002,
		OthersRead		= 0x0004,
		GroupExecute	= 0x0008,
		GroupWrite		= 0x0010,
		GroupRead		= 0x0020,
		OwnerExecute	= 0x0040,
		OwnerWrite		= 0x0080,
		OwnerRead		= 0x0100,
		StickyBit		= 0x0200,
		SetGidBit		= 0x0400,
		SetUidBit 		= 0x0800,
		FifoBit			= 0x1000,
		CharDevice		= 0x2000,
		Directory		= 0x4000,
		BlockDevice		= 0x6000,
		RegularFile		= 0x8000,
		SymLink			= 0xA000,
		Socket			= 0xC000,
#pragma warning restore   
	}
}
