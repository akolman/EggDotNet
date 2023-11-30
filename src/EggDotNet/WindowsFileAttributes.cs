using System;
namespace EggDotNet
{
	/// <summary>
	/// Represents a set of Windows file attributes.
	/// </summary>
	[Flags]
	public enum WindowsFileAttributes : int
	{
		/// <summary>
		/// No attributes specified.
		/// </summary>
		None = 0,

		/// <summary>
		/// The entry is a read only only.
		/// </summary>
		ReadOnly = 1,

		/// <summary>
		/// The entry is a hidden file.
		/// </summary>
		Hidden = 2,

		/// <summary>
		/// The entry is a system file.
		/// </summary>
		SystemFile = 4,

		/// <summary>
		/// The entry is a link file.
		/// </summary>
		LinkFile = 8,

		/// <summary>
		/// The entry is a directory.
		/// </summary>
		Directory = 128
	}
}
