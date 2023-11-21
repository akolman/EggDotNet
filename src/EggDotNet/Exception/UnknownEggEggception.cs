namespace EggDotNet.Exception
{
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
