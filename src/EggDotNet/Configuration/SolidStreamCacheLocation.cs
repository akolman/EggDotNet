namespace EggDotNet.Configuration
{
	/// <summary>
	/// Represents the method used to determine temp directory for solid extraction caching
	/// </summary>
	public enum SolidStreamCacheLocation : short
	{
		/// <summary>
		/// Cache to the same directory used by the input file.
		/// </summary>
		InputDirectory = 0,

		/// <summary>
		/// Cache to the current working directory.
		/// </summary>
		WorkingDirectory = 1,

		/// <summary>
		/// Cache to the user's temp directory.
		/// </summary>
		UserTemp = 2,

		/// <summary>
		/// Cache to the user-defined directory (see <see cref="SolidStreamConfiguration.UserDefinedSolidCacheDirectory"/>)
		/// </summary>
		DefineDirectory = 3
	}
}
