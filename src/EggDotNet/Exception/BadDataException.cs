using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Exception
{
	internal class BadDataException : Eggception
	{
		public BadDataException(string message)
			: base(message)
		{

		}
	}
}
