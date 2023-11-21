using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Exception
{
	public class UnsupportedLocalEggception : Eggception
	{
		public UnsupportedLocalEggception(int localeCode, System.Exception innerException)
			: base($"Could not load encoder for locale {localeCode}", innerException)
		{
		}
	}
}
