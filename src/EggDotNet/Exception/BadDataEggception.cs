namespace EggDotNet.Exception
{
	internal sealed class BadDataEggception : Eggception
	{
		public BadDataEggception(string message)
			: base(message, new System.IO.InvalidDataException())
		{
		}
	}
}
