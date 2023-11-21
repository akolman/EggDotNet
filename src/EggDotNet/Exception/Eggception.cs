namespace EggDotNet.Exception
{
	/// <summary>
	/// Represents an <see cref="Exception"/> thrown from EggDotNet.
	/// </summary>
	public class Eggception : System.Exception
	{
		internal Eggception(string message)
			: base(message)
		{
		}
	}
}
