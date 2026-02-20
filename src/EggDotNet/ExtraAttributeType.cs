namespace EggDotNet
{
	/// <summary>
	/// Attribute type specifier for use with <see cref="EggArchiveEntry.GetExtraAttributes(ExtraAttributeType)"/>
	/// </summary>
	public enum ExtraAttributeType : short
	{
		/// <summary>
		/// Attributes related to file descriptor.
		/// </summary>
		FileAttibutes = 0,

		/// <summary>
		/// User/group attributes (Posix only).
		/// </summary>
		UserGroupAttributes = 1,

		/// <summary>
		/// Raw date value.
		/// </summary>
		DateAttributes = 2,
	}
}
