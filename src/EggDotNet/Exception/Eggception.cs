using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Exception
{
	public class Eggception : System.Exception
	{
		public Eggception(string message)
			: base(message)
		{

		}
	}
}
