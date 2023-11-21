#pragma warning disable CA1710

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

		internal Eggception(string message, System.Exception innerException)
			: base(message, innerException)
		{

		}
	}
}
