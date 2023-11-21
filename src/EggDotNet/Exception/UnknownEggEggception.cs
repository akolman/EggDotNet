namespace EggDotNet.Exception
{
	/// <summary>
	/// Exception indicating that the provided data is not EGG data, or is an unknown version.
	/// </summary>
	public class UnknownEggEggception : Eggception
	{
		internal UnknownEggEggception() 
			: base("EGG format is unknown or unsupported") 
		{ 
		}

		internal UnknownEggEggception(int version)
			: base($"EGG version {version} is not supported" )
		{
		}
	}
}
