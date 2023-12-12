using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an exception thrown when the provided data is not EGG data, or is an unknown version.
	/// </summary>
	public class UnknownEggException : Exception
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
