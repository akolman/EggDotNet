namespace EggDotNet
{
	/// <summary>
	/// Specifies what type of file info metadata (Windows/Posix) was found.
	/// </summary>
	public enum EntryInfoType
	{
		/// <summary>
		/// No file info was found.
		/// </summary>
		None	= 0,

		/// <summary>
		/// Windows file info was found.
		/// </summary>
		Windows = 1,

		/// <summary>
		/// Posix file info was found.
		/// </summary>
		Posix	= 2
	}
}
