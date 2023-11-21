using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Exception
{
	public class UnknownCompressionEggception : Eggception
	{
		internal UnknownCompressionEggception()
			: base("Compression type is unknown")
		{
		}
	}
}
