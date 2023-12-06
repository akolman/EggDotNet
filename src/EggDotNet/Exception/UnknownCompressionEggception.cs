namespace EggDotNet.Exception
{
	/// <summary>
	/// Represents an error thrown when an unknown compression method is used.
	/// </summary>
	public class UnknownCompressionEggception : Eggception
	{
		internal UnknownCompressionEggception()
			: base("Compression type is unknown")
		{
		}
	}
}
