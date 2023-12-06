namespace EggDotNet.Exception
{
	/// <summary>
	/// Represents an exception thrown when a local encoding can't be loaded.
	/// </summary>
	public class UnsupportedLocalEggception : Eggception
	{
		internal UnsupportedLocalEggception(int localeCode, System.Exception innerException)
			: base($"Could not load encoder for locale {localeCode}", innerException)
		{
		}
	}
}
