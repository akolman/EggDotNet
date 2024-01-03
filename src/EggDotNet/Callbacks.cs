using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet
{
	public class Callbacks
	{
		public delegate IEnumerable<Stream> SplitFileReceiverCallback(Stream sourceStream);

		public delegate string PasswordCallback(string filename);
	}
}
