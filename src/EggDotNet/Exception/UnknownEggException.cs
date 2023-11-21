namespace EggDotNet.Exception
{
	internal class UnknownEggException : Eggception
	{
		public UnknownEggException() : base("EGG format is unknown or unsupported") { }
	}
}
