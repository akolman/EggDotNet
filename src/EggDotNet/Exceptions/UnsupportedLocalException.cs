using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an exception thrown when a local encoding can't be loaded.
	/// </summary>
	public sealed class UnsupportedLocalException : Exception
	{
		internal UnsupportedLocalException(int localeCode, System.Exception innerException)
			: base($"Could not load encoder for locale {localeCode}", innerException)
		{
		}
	}
}
