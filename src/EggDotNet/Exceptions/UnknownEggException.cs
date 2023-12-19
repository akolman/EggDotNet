using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Exception indicating that the provided data is not EGG data, or is an unknown version.
	/// </summary>
	public sealed class UnknownEggException : Exception
	{
		internal UnknownEggException() 
			: base("EGG format is unknown or unsupported") 
		{ 
		}

		internal UnknownEggException(int version)
			: base($"EGG version {version} is not supported" )
		{
		}
	}
}
