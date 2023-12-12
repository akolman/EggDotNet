using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Compression.AZO.Implementation.Common
{
	internal static class AZOOption
	{
		public const uint AZO_VERSION = 1;
		public const uint COMPRESSION_REDUCE_MIN_SIZE = 8;
		public const uint ALPHA_SIZE = 256;
		public const uint ALPHA_HISTORY_SIZE = 1;

		public const uint MATCH_LEN_SGAP = 32;
		public const uint MATCH_LEN_GAP = 8;
		public const uint MATCH_DIST_SGAP = 16;
		public const uint MATCH_DIST_GAP = 4;

		public const uint MATCH_MIN_LEN = 2;
		public const uint MATCH_MIN_DISTANCE = 1;

		public const uint MATCH_LEN_CODE_SIZE = 128;
		public const uint MATCH_DIST_CODE_SIZE = 128;

		public const uint MATCH_HASH_LEVEL = 5;
		public const uint MATCH_HASH_BITSIZE = 22;

		public const uint DISTANCE_HISTORY_SIZE = 2;

		public const uint DICTIONARY_SIZE = 128;
		public const uint DICTIONARY_HISTORY_SIZE = 2;

		public const uint ALPHACODE_PREDICT_SHIFT = 5;
		public const uint LENGTHCODE_PREDICT_SHIFT = 4;
	}
}
